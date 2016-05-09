class KinectJoint extends KinectObject {
    constructor(name, scene, userIndex, kinectManager) {
        super(name, scene, userIndex, kinectManager)
        this.jointType = 0;
        this.damping = 10;
        this.jointScale = 1;
        this.updatePosition = true;
        this.updateRotation = true;
        this.mirroredMovement = false;
        this._targetPosition = BABYLON.Vector3.Zero;
        this._offsetX = 0;
        this._offsetY = 0;
        this._offsetZ = 0;
    }

    update() {
        this._joint = this._kinectManager.getJoint(this.jointType, this.userIndex);
        this._centerPosition = this._kinectManager.getUserPosition(this.userIndex);

        this._offsetX = this._centerPosition.x * this.jointScale;
        this._offsetY = this._centerPosition.y * this.jointScale;
        this._offsetZ = (!this.mirroredMovement ? -this._centerPosition.z : this._centerPosition.z) * this.jointScale;

        if (this.updatePosition) {
            this._targetPosition.x = (this._joint.Position.x * this.jointScale - this._offsetX);
            this._targetPosition.y = (this._joint.Position.y * this.jointScale - this._offsetY);
            this._targetPosition.z = (!this.mirroredMovement ? (-this._joint.Position.z * this.jointScale - this._offsetZ) : (this._joint.Position.z * this.jointScale - this._offsetZ));
            this.position.copyFrom(this._targetPosition);
        }
    }
}