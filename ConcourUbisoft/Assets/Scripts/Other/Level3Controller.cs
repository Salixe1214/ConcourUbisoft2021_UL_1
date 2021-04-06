using Photon.Pun;
using System;
using UnityEngine;

namespace Other
{
    public class Level3Controller : MonoBehaviour
    {
        [SerializeField] private GameObject RightRobot;
        [SerializeField] private GameObject LeftRobot;

        private DialogSystem _dialogSystem;
        private bool _endButtonPressed;

        private PhotonView _photonView = null;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            _dialogSystem = GameObject.FindGameObjectWithTag("DialogSystem").GetComponent<DialogSystem>();
            _endButtonPressed = false;
        }

        private void OnEnable()
        {
            _dialogSystem.OnFinalDialog += WakeRobots;
        }

        private void OnDisable()
        {
            _dialogSystem.OnFinalDialog -= WakeRobots;
        }

        public void StartLevel()
        {
            //_dialogSystem.StartDialog("Area03_start");
            //_photonView.RPC("StartFirstDialog", RpcTarget.All);
            StartFirstDialog();
        }

        public void OnButtonPressed()
        {
            if (!_endButtonPressed)
            {
                //_dialogSystem.StartDialog("Area03_end");
                _photonView.RPC("StartEndDialog", RpcTarget.All);
                _endButtonPressed = true;
            }
        }
        
        private void StartFirstDialog()
        {
            _dialogSystem.StartDialog("Area03_start","green");
        }

        [PunRPC]
        private void StartEndDialog()
        {
            _dialogSystem.StartEndDialogue("Area03_end");
        }

        private void WakeRobots()
        {
            RightRobot.transform.rotation = Quaternion.Slerp(RightRobot.transform.rotation,new Quaternion(0,90,0,0), 0.5f);
            LeftRobot.transform.rotation = Quaternion.Slerp(RightRobot.transform.rotation,new Quaternion(0,90,0,0), 0.5f);
        }

    }
}