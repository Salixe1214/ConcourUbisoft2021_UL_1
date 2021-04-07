using System;
using UnityEngine;
namespace Menu
{
    public class EndGameMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject EndGameMenu;
        
        private void Start()
        {
            
        }

        private void ShowEndGameMenu()
        {
            EndGameMenu.SetActive(true);
        }

        private void OnReturnMenuClicked()
        {
            
        }
    }
}