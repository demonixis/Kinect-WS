class KinectDebugCharacter extends KinectObject {
    constructor(name, scene, userIndex, kinectManager) {
        super(name, scene, userIndex, kinectManager);

        this.moveScale = 1;
        this.updatePosition = true;
        this.dampingPosition = 10;
        this.mirrorMovement = false;
        this.lockYPosition = false;
        this.cameraNode = null;
    }

    update() {

    }

    createDebugCharacter(version) {

    }

    destroyBones() {

    }

    updateDebugCharacter() {

    }

    isJointExclued(joint) {

    }

    getJoint(type) {

    }

    forEachJoints(callback) {
        
    }
}