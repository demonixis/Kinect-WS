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
        private static Skeleton[] _skeletons;

        public static string SerializeToArray(this Skeleton body)
        {
            if (_skeletons == null)
                _skeletons = new Skeleton[1];

            _skeletons[0] = body;

            return _skeletons.Serialize();
        }

        public static string Serialize(this Skeleton skeleton)
        {
            return WebSocketServer.Serialize(skeleton.ToJsonBody());
        }

        public static string Serialize(this Skeleton[] skeletons)
        {
            var size = skeletons.Length;
            var array = new JsonBody[size];

            for (var i = 0; i < size; i++)
                array[i] = skeletons[i].ToJsonBody();

            return WebSocketServer.Serialize(array);
        }

        public static string Serialize(this List<Skeleton> skeletons)
        {
            var size = skeletons.Count;
            var array = new JsonBody[size];

            for (var i = 0; i < size; i++)
                array[i] = skeletons[i].ToJsonBody();

            return WebSocketServer.Serialize(array);
        }

        public static JsonBody ToJsonBody(this Skeleton skeleton)
        {
            var jsonBody = new JsonBody
            {
                Version = 1,
                Id = skeleton.TrackingId.ToString(),
                Joints = new JsonJoint[skeleton.Joints.Count],
                IsTracked = skeleton.TrackingState != SkeletonTrackingState.NotTracked,
                RootOrientation = FromValues(skeleton.BoneOrientations[JointType.HipCenter].AbsoluteRotation.Quaternion)
            };

            KinectVector4 orientation;

            foreach (Joint joint in skeleton.Joints)
            {
                orientation = skeleton.BoneOrientations[joint.JointType].AbsoluteRotation.Quaternion;

                jsonBody.Joints[(byte)joint.JointType] = new JsonJoint()
                {
                    Position = FromValues(joint.Position),
#if SEND_ROTATIONS
                    Rotation = FromValues(orientation)
#endif
                };
            }

            return jsonBody;
        }

        private static Vector3 FromValues(SkeletonPoint point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }

        private static Vector4 FromValues(KinectVector4 vector)
        {
            return new Vector4(vector.X, vector.Y, vector.Z, vector.W);
        }
    }
}
