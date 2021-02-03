using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] private GameObject RoomListElementPrefab = null;
    [SerializeField] private GameObject ContentPanel = null;

    public void AddElement(string name, UnityAction action) {
        GameObject roomListElement = Instantiate(RoomListElementPrefab, ContentPanel.transform);
        roomListElement.transform.Find("RoomName").GetComponent<Text>().text = name;
        roomListElement.transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(action);
    }

    public void ClearElements()
    {
        foreach (Transform child in ContentPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
