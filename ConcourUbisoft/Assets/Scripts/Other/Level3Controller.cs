using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

namespace Other
{
    public class Level3Controller : MonoBehaviour
    {
        [SerializeField] private GameObject RightRobot;
        [SerializeField] private GameObject LeftRobot;
        [SerializeField] private float xRobotRotationSpeed =10f;
        [SerializeField] private float yRobotRotationSpeed = 10f;

        private DialogSystem _dialogSystem;
        private bool _endButtonPressed;

        private PhotonView _photonView = null;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _dialogSystem = GameObject.FindGameObjectWithTag("DialogSystem").GetComponent<DialogSystem>();
        }

        private void Start()
        {
           // _dialogSystem = GameObject.FindGameObjectWithTag("DialogSystem").GetComponent<DialogSystem>();
            _endButtonPressed = false;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.V))
            {
                StartCoroutine(RotateBots());
            }
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
            StartCoroutine(RotateBots());
        }

        private void LightUpBots()
        {
            
        }

        IEnumerator RotateBots()
        {
            float XRotationAccumulator = 20;
            float YRotationAccumulator = 0;
            while (XRotationAccumulator >=0.1f && YRotationAccumulator <= 179f)
            {
                if (XRotationAccumulator >= 0.1f)
                {
                    float _rotation = -xRobotRotationSpeed * Time.deltaTime;
                    RightRobot.transform.Rotate(Vector3.right,_rotation);
                    LeftRobot.transform.Rotate(Vector3.right,_rotation);
                    XRotationAccumulator -= _rotation;
                }

                if (YRotationAccumulator <= 179f)
                {
                    float _rotation = yRobotRotationSpeed * Time.deltaTime;
                    RightRobot.transform.Rotate(Vector3.up,_rotation);
                    LeftRobot.transform.Rotate(Vector3.up,_rotation);
                    YRotationAccumulator += _rotation;
                }
                yield return null;
            }
        }
    }
}