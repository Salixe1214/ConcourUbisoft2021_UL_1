using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private List<Sprite> imagesList; // it a temporary things ^^
        [SerializeField] private List<InformationItem> items;

        [Header("Accordion")]
        [SerializeField] private Vector3 position;
        [SerializeField] private Quaternion rotation;
        [SerializeField] private Sprite background;
        [SerializeField] private Sprite front;

        [Header("System prefabs")] 
        [SerializeField] private GameObject accordionPrefab;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private GameObject headerPrefab;

        private GameObject _accordion;
        private void Awake()
        {
            _accordion = Instantiate(accordionPrefab, position, rotation, gameObject.transform);
            items.ForEach(InstantiateItems);

            ImageLayout go = new GameObject().AddComponent<ImageLayout>();
            GameObject o;
            (o = go.gameObject).GetComponent<RectTransform>().SetParent(gameObject.transform);
            o.transform.position = new Vector3 (Screen.width * 0.5f, Screen.height * 0.9f, 0);
            go.CreateLayout(imagesList);
            /*Accordion accordion = new GameObject().AddComponent<Accordion>();
            accordion.GetComponent<RectTransform>().SetParent(gameObject.transform);
            accordion.Setup(background, front);
            accordion.gameObject.transform.position = position;
            accordion.CreateAccordion(items);*/
        }

        private void InstantiateItems(InformationItem item)
        {
            GameObject go = Instantiate(itemPrefab, _accordion.transform);
            
            Instantiate(headerPrefab, go.transform).GetComponent<Text>().text = item.title;
            if (item.images.Count > 0)
            {
                ImageLayout imageLayout = new GameObject().AddComponent<ImageLayout>();
                imageLayout.CreateLayout(item.images);
                imageLayout.SetParent(go.transform);
            }
            //Instantiate(textPrefab, go.transform).GetComponent<Text>().text = item.content;
            CreateParagraph(go.transform, item.content);
        }

        private void CreateParagraph(Transform parent, string content)
        {
            GameObject go = new GameObject();
            Text text = go.AddComponent<Text>();
            text.text = content;
            text.fontSize = 14;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = Color.black;
            go.GetComponent<RectTransform>()?.SetParent(parent);
            go.SetActive(true);
        }
    }
}
