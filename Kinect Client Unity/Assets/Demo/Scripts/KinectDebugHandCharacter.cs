using UnityEngine;

namespace Demonixis.Kinect.Demo
{
    public class KinectDebugHandCharacter : KinectDebugCharacter
    {
        [SerializeField]
        private GameObject _weapon = null;

        private JointType weaponParentLeft = JointType.HandLeft;
        private JointType weaponParentRight = JointType.HandRight;

        protected override void CreateDebugCharacter(int version)
        {
            base.CreateDebugCharacter(version);

            ForEachJoints(joint =>
            {
                if (joint.JointType == weaponParentLeft || joint.JointType == weaponParentRight)
                {
                    joint.UpdateRotation = true;
                    Destroy(joint.GetComponent<Rigidbody>());
                    Destroy(joint.GetComponent<KinectHand>());
                    AddWeaponToHand(joint.transform, joint.JointType == weaponParentLeft);
                }
            });

            GameVRSettings.Recenter();
        }

        public void AddWeaponToHand(Transform hand, bool isLeftHand = true)
        {
            var weapon = (GameObject)Instantiate(_weapon, Vector3.zero, Quaternion.identity);
            weapon.transform.parent = hand;
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);

            var shooter = weapon.GetComponentInChildren<KinectShootWithHand>();
            shooter.isLeftHand = isLeftHand;
            shooter.UserIndex = m_userIndex;
        }
    }
}