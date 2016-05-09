class KinectObject {
    constructor(name, scene, userIndex, kinectManager) {
        super(name, scene);
        this.userIndex = userIndex;
        this.kinectManager = kinectManager;
    }

    _setPosition(position) {
        this.position.copyFrom(position);
    }

    _setRotation(rotation) {
        rotation.toEulerAnglesToRef(this.rotation);
    }

    _getPosition() {
        return this.position;
    }

    _getRotation() {
        return BABYLON.Quaternion.RotationYawPitchRoll(this.rotation.y, this.rotation.x, this.rotation.z);
    }
}