using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Arm
{
    public class MechanicalArmWASDControl : Controllable
    {
        [SerializeField] private ArmController _armController = null;
        [SerializeField] private MagnetController _magnetController = null;
        [SerializeField] private GameController.Role _owner = GameController.Role.SecurityGuard;

        private NetworkController _networkController = null;


        private void Awake()
        {
            _networkController = GameObject.FindGameObjectWithTag("NetworkController")?.GetComponent<NetworkController>();
        }

        private void Update()
        {
            if (IsControlled && (_owner == GameController.Role.None || _owner == _networkController.GetLocalRole())) {
                Vector3 translation = new Vector3(-Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
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
