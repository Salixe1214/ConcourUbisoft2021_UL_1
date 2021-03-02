using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Informations
{ 
    [RequireComponent(typeof(VerticalLayoutGroup)), RequireComponent(typeof(ContentSizeFitter)), RequireComponent(typeof(ToggleGroup))]
    public class Accordion : MonoBehaviour
    {
        private struct InformationItem
        {
            public AccordionElement Element;
            public ImageLayout Image;
            public readonly Text Title;
            public readonly Text Content;

            public InformationItem(AccordionElement element, Text title, ImageLayout image, Text content)
            {
                Element = element;
                Content = content;
                Image = image;
                Title = title;
            }
        }

        private Sprite front = null;
        private List<InformationItem> _items;

        public Accordion()
        {
            _items = new List<InformationItem>();
        }

        private void Awake()
        {
            GetComponent<ToggleGroup>().allowSwitchOff = true;
        }

        public void Setup(Sprite fond, Sprite devant)
        {
            Image image = gameObject.AddComponent<Image>();

            front = devant;
            image.sprite = fond;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            image.fillCenter = true;
            image.pixelsPerUnitMultiplier = 1;
            VerticalLayoutGroup test = gameObject.GetComponent<VerticalLayoutGroup>();
            gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            if (test != null)
            {
                test.childScaleHeight = false;
                test.childForceExpandWidth = true;
                test.childControlHeight = true;
                test.childControlWidth = true;
                test.childAlignment = TextAnchor.UpperLeft;
                test.childScaleHeight = false;
                test.childScaleWidth = false;
                test.spacing = 2;
                test.padding = new RectOffset(2, 2, 2, 2);
            }
        }

        private Text CreateTextObject()
        {
            return new GameObject().AddComponent<Text>();
        }
        
        private Text InstantiateText(Transform parent, string content)
        {
            Text text = CreateTextObject();

            text.text = content;
            text.fontSize = 14;
            text.color = Color.black;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.GetComponent<RectTransform>()?.SetParent(parent);
            text.gameObject.SetActive(true);
            return text;
        }

        private Text InstantiateHeader(Transform parent, string content)
        {
            Text text = InstantiateText(parent, content);

            text.fontSize = 16;
            text.gameObject.AddComponent<LayoutElement>().minHeight = 18;
            return text;
        }

        private ImageLayout InstantiateImageLayout(Transform parent, IEnumerable<Sprite> images)
        {
            ImageLayout imageLayout = new GameObject().AddComponent<ImageLayout>();

            imageLayout.CreateLayout(images);
            imageLayout.GetComponent<RectTransform>().SetParent(gameObject.transform);
            return imageLayout;
        }

        private AccordionElement InstantiateNewItem()
        {
            AccordionElement accordionElement = new GameObject().AddComponent<AccordionElement>();
            Image image = accordionElement.gameObject.AddComponent<Image>();

            image.sprite = front;
            image.type = Image.Type.Sliced;
            image.fillCenter = true;
            image.pixelsPerUnitMultiplier = 1;
//            image.color = Color.white;
            accordionElement.minHeight = 18;
            VerticalLayoutGroup test = accordionElement.gameObject.AddComponent<VerticalLayoutGroup>();
            if (test != null)
            {
                test.childScaleHeight = false;
                test.childForceExpandWidth = true;
                test.childControlHeight = true;
                test.childControlWidth = true;
                test.childAlignment = TextAnchor.UpperLeft;
                test.childScaleHeight = false;
                test.childScaleWidth = false;
                test.spacing = 2;
                test.padding = new RectOffset(2, 2, 2, 2);
            }
            accordionElement.GetComponent<RectTransform>()?.SetParent(gameObject.transform);
            return accordionElement;
        }

        public void AddItem(InformationsSystem.InformationItem item)
        {
            AccordionElement element = InstantiateNewItem();
            
            _items.Add(new InformationItem(
                element,
                InstantiateHeader(element.transform, item.title),
                InstantiateImageLayout(element.transform, item.images),
                InstantiateText(element.transform, item.content)));
        }

        // TODO
        public void UpdateItem()
        {
            
        }

        // TODO:
        public void DeleteItem()
        {
            
        }

        public void Clean()
        {
            foreach (InformationItem item in _items)
            {
                Destroy(item.Title.gameObject);
                Destroy(item.Content.gameObject);
                Destroy(item.Image.gameObject);
                Destroy(item.Element);
            }
            _items.Clear();
        }

        public void CreateAccordion(IEnumerable<InformationsSystem.InformationItem> items)
        {
            foreach (InformationsSystem.InformationItem item in items)
            {
                AddItem(item);
            }
        }
    }
}
