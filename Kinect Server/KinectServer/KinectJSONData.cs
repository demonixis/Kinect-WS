namespace Demonixis.Kinect.Server
{
    public struct JsonBody
    {
        public byte Version { get; set; }
        public string Id { get; set; }
        public bool IsTracked { get; set; }
        public JsonJoint[] Joints { get; set; }
        public Vector4 RootOrientation { get; set; }
        public byte HandLeftState { get; set; }
        public byte HandRightState { get; set; }
        public byte HandLeftConfidence { get; set; }
        public byte HandRightConfidence { get; set; }
    }

    public struct JsonJoint
    {
        public Vector3 Position { get; set; }
#if SEND_ROTATIONS
        public Vector4 Rotation { get; set; }
#endif
    }

    public struct Vector4
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }

    public struct Vector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
