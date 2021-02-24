using System;
using System.Collections.Generic;
using TMPro;
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
            public List<Sprite> images;
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
            if (item.images.Count > 0)
            {
                CreateImageLayout(go.transform, item.images);
            }
            Instantiate(textPrefab, go.transform).GetComponent<Text>().text = item.content;
        }

        private void CreateImage(Transform parent, Sprite sprite)
        {
            GameObject go = new GameObject();
            Image image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            go.GetComponent<RectTransform>().SetParent(parent);
            go.SetActive(true);
        }

        private void CreateImageLayout(Transform parent, List<Sprite> images)
        {
            GameObject go = new GameObject();
            HorizontalLayoutGroup layout = go.AddComponent<HorizontalLayoutGroup>();

            layout.childAlignment = TextAnchor.MiddleCenter;
            images.ForEach(image => CreateImage(go.transform, image));
            go.GetComponent<RectTransform>().SetParent(parent);
            go.SetActive(true);
        }

        private void CreateParagraph(Transform parent, string content)
        {
            GameObject go = new GameObject();
            Text text = go.AddComponent<Text>();
            text.text = content;
            text.fontSize = 14;
            text.color = Color.black;
            go.GetComponent<RectTransform>().SetParent(parent);
            go.SetActive(true);
        }
    }
}
