using UnityEngine;
using System.Linq;
namespace DefaultNamespace
{
    public class RepeaterButton:SimpleButton
    {
        protected override bool GetInput()
        {
            string[] joysticks = Input.GetJoystickNames();
            return Input.GetMouseButton(0) ||
                   (joysticks.Contains("Controller (Xbox One For Windows)") && Input.GetButton("ConfirmXBO")) ||
                   (joysticks.Contains("Wireless Controller") && Input.GetButton("ConfirmPS"));
        }
    }
}