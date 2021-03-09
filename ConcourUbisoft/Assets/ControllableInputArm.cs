using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Arm
{
    public class ControllableInputArm : Controllable
    {
        [SerializeField] private ArmController _armController = null;
        [SerializeField] private MagnetController _magnetController = null;

        private void Update()
        {
            if (IsControlled) {
                Vector3 translation = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
                if(translation != Vector3.zero)
                {
                    _armController.Translate(translation);
                }

                _magnetController.MagnetActive = (Input.GetButton("Grab") ||
                                    Input.GetButton("GrabControllerXBO") ||
                                    Input.GetButton("GrabControllerPS"));
            }
        }
    }
}
