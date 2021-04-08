using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Other
{
    public class Level3Controller : MonoBehaviour
    {
        [SerializeField] private GameObject RightRobot;
        [SerializeField] private GameObject LeftRobot;
        [SerializeField] private float xRobotRotationSpeed =20f;
        [SerializeField] private float yRobotRotationSpeed = 50f;
        [SerializeField] private CameraEffectDisabled CameraEffect;
        [SerializeField] private GameObject TargetText;
        [SerializeField] private float DelayBeforeEndMenu = 3;
        [SerializeField] private GameObject InfoText;
        [SerializeField] private GameObject ProgressText;

        private DialogSystem _dialogSystem;
        private bool _endButtonPressed;
        private bool _loadingDone;
        private float XRotationAccumulator = 20;
        private float YRotationAccumulator = 0;
        private Text _progressTextField;
        private GameController _gameController;

        private PhotonView _photonView = null;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _dialogSystem = GameObject.FindGameObjectWithTag("DialogSystem").GetComponent<DialogSystem>();
            _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        }

        private void Start()
        {
            _endButtonPressed = false;
            _loadingDone = false;
        }

        private void Update()
        {

        }

        private void OnEnable()
        {
            _dialogSystem.OnFinalDialog += WakeRobots;
            _dialogSystem.OnFinalDialogMusicStart += UpdateVisuals;
        }

        private void OnDisable()
        {
            _dialogSystem.OnFinalDialog -= WakeRobots;
            _dialogSystem.OnFinalDialogMusicStart -= UpdateVisuals;
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
                //_photonView.RPC("EnableCameraEffect", RpcTarget.All);
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

        [PunRPC]
        private void EnableCameraEffect()
        {
            CameraEffect.Enable();
        }

        [PunRPC]
        private void DisableCameraEffect()
        {
            CameraEffect.Disable();
            TargetText.SetActive(true);
        }

        private void UpdateVisuals()
        {
            TargetText.SetActive(true);
            InfoText.SetActive(true);
            _progressTextField =ProgressText.GetComponent<Text>();
            _progressTextField.text = "";
            ProgressText.SetActive(true);
            StartCoroutine(UpdateProgressText());
        }

        private void WakeRobots()
        {
            StartCoroutine(RotateBots());
        }

        private void LightUpBots()
        {
            LeftRobot.GetComponentInChildren<RobotLight>().LightItUp();
            RightRobot.GetComponentInChildren<RobotLight>().LightItUp();
            _loadingDone = true;
            StartCoroutine(WaitBeforeShowingEndMenu());
        }

        IEnumerator RotateBots()
        {
            
            while (XRotationAccumulator >0f || YRotationAccumulator < 190f)
            {
                if (XRotationAccumulator > 0f)
                {
                    float _rotation = xRobotRotationSpeed*Time.deltaTime  *-1;
                    RightRobot.transform.Rotate(Vector3.right,_rotation);
                    LeftRobot.transform.Rotate(Vector3.right,_rotation);
                    XRotationAccumulator += _rotation;
                }
                else if (YRotationAccumulator < 190f)
                {
                    float _rotation = yRobotRotationSpeed *Time.deltaTime;
                    RightRobot.transform.Rotate(Vector3.up,_rotation);
                    LeftRobot.transform.Rotate(Vector3.up,-_rotation);
                    YRotationAccumulator += _rotation;
                }
                yield return null;
            }
            LightUpBots();
        }

        IEnumerator WaitBeforeShowingEndMenu()
        {
            yield return new WaitForSeconds(DelayBeforeEndMenu);
            _gameController.CloseInGameMenu();
            GameObject.FindWithTag("EndMenu").GetComponent<EndGameMenuController>().ShowEndGameMenu();
            
        }

        IEnumerator UpdateProgressText()
        {
            int progress = 0;
            while (!_loadingDone)
            {
                if (progress < 100)
                {
                    progress += 1;
                }
                _progressTextField.text = progress + " %";
                yield return new WaitForSeconds(0.4f);
            }
            InfoText.SetActive(false);
            ProgressText.SetActive(false);
        }
    }
}