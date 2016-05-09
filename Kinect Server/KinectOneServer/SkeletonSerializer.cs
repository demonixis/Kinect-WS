using Microsoft.Kinect;
using System.Collections.Generic;

namespace Demonixis.Kinect.Server
{
    using KinectVector4 = Microsoft.Kinect.Vector4;

    /// <summary>
    /// Serializes a Kinect skeleton to JSON fromat.
    /// </summary>
    public static class SkeletonSerializer
    {
        private static Body[] _bodies;

        public static string SerializeToArray(this Body body)
        {
            if (_bodies == null)
                _bodies = new Body[1];

            _bodies[0] = body;

            return _bodies.Serialize();
        }

        public static string Serialize(this Body body)
        {
            return WebSocketServer.Serialize(body.ToJsonBody());
        }

        public static string Serialize(this Body[] bodies)
        {
            var size = bodies.Length;
            var array = new JsonBody[size];

            for (var i = 0; i < size; i++)
                array[i] = bodies[i].ToJsonBody();

            return WebSocketServer.Serialize(array);
        }

        public static string Serialize(this List<Body> bodies)
        {
            var size = bodies.Count;
            var array = new JsonBody[size];

            for (var i = 0; i < size; i++)
                array[i] = bodies[i].ToJsonBody();

            return WebSocketServer.Serialize(array);
        }

        public static JsonBody ToJsonBody(this Body body)
        {
            var jsonBody = new JsonBody
            {
                Version = 2,
                Id = body.TrackingId.ToString(),
                Joints = new JsonJoint[body.Joints.Count],
                IsTracked = body.IsTracked,
                HandLeftState = (byte)body.HandLeftState,
                HandRightState = (byte)body.HandRightState,
                HandLeftConfidence = (byte)body.HandLeftConfidence,
                HandRightConfidence = (byte)body.HandRightConfidence,
                RootOrientation = FromValues(body.JointOrientations[JointType.SpineMid].Orientation)
            };

            KinectVector4 orientation;

            foreach (var joint in body.Joints)
            {
                orientation = body.JointOrientations[joint.Key].Orientation;
                jsonBody.Joints[(byte)joint.Key] = new JsonJoint()
                {
                    Position = FromValues(joint.Value.Position),
#if SEND_ROTATIONS
                    Rotation = FromValues(orientation)
#endif
                };
            }

            return jsonBody;
        }

        private static Vector3 FromValues(CameraSpacePoint point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }

        private static Vector4 FromValues(KinectVector4 vector)
        {
            return new Vector4(vector.X, vector.Y, vector.Z, vector.W);
        }
    }
}
