using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// TODO:
namespace TechSupport.Informations
{
    public class InformationsSystem : MonoBehaviour
    {
        [Serializable]
        public struct InformationItem
        {
            public string title;
            [TextArea]
            public string content;
            public Sprite image;
        }
        [Header("Information")]
        [SerializeField]
        private List<InformationItem> items;

        [Header("Transform")]
        [SerializeField] private Vector3 position;
        [SerializeField] private Quaternion rotation;

        [Header("System prefabs")] 
        [SerializeField] private GameObject accordionPrefab;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private GameObject headerPrefab;
        [SerializeField] private GameObject imagePrefab;
        [SerializeField] private GameObject textPrefab;

        private GameObject _accordion;
        private void Awake()
        {
            _accordion = Instantiate(accordionPrefab, position, rotation, gameObject.transform);
            items.ForEach(InstantiateItems);
        }

        private void InstantiateItems(InformationItem item)
        {
            GameObject go = Instantiate(itemPrefab, _accordion.transform);
            
            Instantiate(headerPrefab, go.transform).GetComponent<Text>().text = item.title;
            if (item.image)
            {
                Instantiate(imagePrefab, go.transform).GetComponent<Image>().sprite = item.image;
            }
            Instantiate(textPrefab, go.transform).GetComponent<Text>().text = item.content;
        }
    }
}
