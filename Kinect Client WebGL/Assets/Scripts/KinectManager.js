const KINECT_XBOX360_JOINT_COUNT = 20;
const KINECT_XBOXONE_JOINT_COUNT = 25;

class UserPosition {
    get position() {
        return this._position;
    }

    get rotation() {
        return this._rotation;
    }

    set position(value) {
        this._position.x = value.x;
        this._position.y = value.y;
        this._position.z = value.z;
    }

    set rotation(value) {
        this._rotation.x = value.x;
        this._rotation.y = value.y;
        this._rotation.z = value.z;
        this._rotation.w = value.w;
    }

    constructor() {
        this._position = new BABYLON.Vector3(0, 0, 0);
        this._rotation = new BABYLON.Quaternion(0, 0, 0, 1);
    }
}

class KinectManager {
    get initialized() {
        return this._initialized;
    }

    get bodies() {
        return this._bodies;
    }

    get kinectVersion() {
        if (this._bodyCount > 0) {
            return this._bodies[0].Version;
        }

        return -1;
    }

    get isKienctV1() {
        if (this._bodyCount > 0) {
            return this._bodies[0].Version == 1;
        }

        return false;
    }

    get isKinectV2() {
        if (this._bodyCount > 0) {
            return this._bodies[0].Version == 2;
        }

        return false;
    }

    get isConnected() {
        return this._bodyCount > 0;
    }

    constructor(ip, port) {
        this._bodies = null;
        this._bodyCount = 0;
        this._usersTransform = [];
        this._spinBaseJoint = null;

        this._webSocket = new WebSocket("ws://" + ip + ":" + port);
        this._webSocket.onopen = (e) => console.log("[KinectManager] Connected to the Kinect Server.");
        this._webSocket.onerror = (e) => console.log("[KinectManager] An error was detected.");
        this._webSocket.onmessage = (e) => this._onMessage(e);
    }

    dispose() {
        this._webSocket.close();
    }

    getJoint(type, userIndex) {
        if (this._bodyCount > 0) {
            if (this.isKinectV2 || type < KINECT_XBOX360_JOINT_COUNT) {
                return this._bodies[userIndex].Joints[type];
            }
        }

        return new KinectJoint();
    }

    getBody(userIndex) {
        if (this._bodyCount > userIndex) {
            return this._bodies[userIndex];
        }

        return null;
    }

    getJointPosition(type, userIndex) {
        return this.getJoint(type, userIndex).Position;
    }

    getJointOrientation(type, userIndex) {
        return this.getJoint(type, userIndex).Rotation;
    }

    getUserPosition(userIndex) {
        if (this._bodyCount > 0) {
            return this._usersTransform[userIndex].position;
        }

        return BABYLON.Vector3.Zero;
    }

    getUserRotation(userIndex) {
        if (this._bodyCount > 0) {
            return this._usersTransform[userIndex].rotation;
        }

        return new BABYLON.Quaternion(0, 0, 0, 1);
    }

    getHandState(handLeft, userIndex) {
        if (this._bodyCount > 0) {
            return handLeft ? this._bodies[userIndex].HandLeftState : this._bodies[userIndex].HandRightState;
        }

        return 0;
    }

    getHandConfidence(handLeft, userIndex) {
        if (this._bodyCount > 0) {
            return handLeft ? this._bodies[userIndex].HandLeftConfidence : this._bodies[userIndex].HandRightConfidence;
        }

        return 0;
    }

    getParentJoint(type, userIndex) {
        var parent = null;

        switch (type) {
            case JointType.SpineMid:
            case JointType.HipLeft:
            case JointType.HipRight:
            case JointType.KneeLeft:
            case JointType.KneeRight:
            case JointType.AnkleLeft:
            case JointType.AnkleRight:
            case JointType.FootLeft:
            case JointType.FootRight:
                parent = this.getJoint(JointType.SpineBase, userIndex);
                break;

            case JointType.SpineShoulder:
                parent = this.getJoint(JointType.SpineMid, userIndex);
                break;

            case JointType.Head:
            case JointType.Neck:
            case JointType.ShoulderLeft:
            case JointType.ShoulderRight:
            case JointType.ElbowLeft:
            case JointType.ElbowRight:
            case JointType.WristLeft:
            case JointType.WristRight:
                parent = this.getJoint(JointType.SpineShoulder, userIndex);
                break;

            case JointType.HandRight:
            case JointType.ThumbRight:
                parent = this.getJoint(JointType.WristRight, userIndex);
                break;

            case JointType.HandTipRight:
                parent = this.getJoint(JointType.HandRight, userIndex);
                break;

            case JointType.HandLeft:
            case JointType.ThumbLeft:
                parent = this.getJoint(JointType.WristLeft, userIndex);
                break;

            case JointType.HandTipLeft:
                parent = this.getJoint(JointType.HandLeft, userIndex);
                break;
        }

        return parent;
    }

    _onMessage(event) {
        this._bodies = JSON.parse(event.Data);

        if (this._bodies !== null) {
            this._bodyCount = this._bodies.length;

            for (var i = 0; i < this._bodyCount; i++) {
                this._spinBaseJoint = this.getJoint(JointType.SpineBase, i);
                this._usersTransform[i].position = this._spinBaseJoint.Position;
                this._usersTransform[i].rotation = this._spinBaseJoint.Rotation;
            }

            if (!this._initialized) {
                this._initialized = true;
            }
        } else {
            this._bodyCount = 0;
        }
    }
}