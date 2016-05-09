using System;
using UnityEngine;

namespace Demonixis.Kinect
{
    #region Enums

    public enum JointType
    {
        SpineBase = 0,
        SpineMid = 1,
        Neck = 2,
        Head = 3,
        ShoulderLeft = 4,
        ElbowLeft = 5,
        WristLeft = 6,
        HandLeft = 7,
        ShoulderRight = 8,
        ElbowRight = 9,
        WristRight = 10,
        HandRight = 11,
        HipLeft = 12,
        KneeLeft = 13,
        AnkleLeft = 14,
        FootLeft = 15,
        HipRight = 16,
        KneeRight = 17,
        AnkleRight = 18,
        FootRight = 19,
        SpineShoulder = 20,
        HandTipLeft = 21,
        ThumbLeft = 22,
        HandTipRight = 23,
        ThumbRight = 24
    }

    public enum HandState
    {
        Unknown = 0,
        NotTracked = 1,
        Open = 2,
        Closed = 3,
        Lasso = 4
    }

    public enum TrackingConfidence
    {
        Low = 0,
        High = 1
    }

    #endregion

    #region Data

    [Serializable]
    public class Body
    {
        public int Version { get; set; }
        public string Id { get; set; }
        public bool IsTracked { get; set; }
        public Joint[] Joints { get; set; }
        public Quaternion RootOrientation { get; set; }
        public HandState HandLeftState { get; set; }
        public HandState HandRightState { get; set; }
        public TrackingConfidence HandLeftConfidence { get; set; }
        public TrackingConfidence HandRightConfidence { get; set; }
    }

    [Serializable]
    public class Joint
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public void UpdateRotation(Joint parent)
        {
            if (parent != null)
                Rotation = Quaternion.Inverse(Quaternion.LookRotation(parent.Position - Position));
            else
                Rotation = Quaternion.identity;
        }
    }

    #endregion
}