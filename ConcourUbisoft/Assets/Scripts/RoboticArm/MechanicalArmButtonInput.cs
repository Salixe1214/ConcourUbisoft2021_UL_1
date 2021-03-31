using UnityEngine;

namespace Arm
{
    public class MechanicalArmButtonInput : Controllable
    {
        [SerializeField] private ArmController _armController;
        [SerializeField] private MagnetController _magnetController;

        public void MoveRight()
        {
            if (IsControlled)
                _armController.Translate(Vector3.right);
        }

        public void MoveLeft()
        {
            if (IsControlled)
                _armController.Translate(Vector3.left);
        }

        public void MoveForward()
        {
            if (IsControlled)
                _armController.Translate(Vector3.forward);
        }

        public void MoveBack()
        {
            if (IsControlled)
                _armController.Translate(Vector3.back);
        }

        public void ToggleMagnet()
        {
            if (IsControlled)
                _magnetController.MagnetActive = !_magnetController.MagnetActive;
        }

        public void ActiveMagnet()
        {
            if (IsControlled)
                _magnetController.MagnetActive = true;
        }

        public void DeactiveMagnet()
        {
            if (IsControlled)
                _magnetController.MagnetActive = false;
        }
    }
}