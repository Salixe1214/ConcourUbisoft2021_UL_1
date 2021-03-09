using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Informations
{ 
    [RequireComponent(typeof(VerticalLayoutGroup)), RequireComponent(typeof(ContentSizeFitter)), RequireComponent(typeof(ToggleGroup))]
    public class Accordion : MonoBehaviour
    {
        private readonly struct InformationItem
        {
            public readonly AccordionElement Element;
            public readonly ImageLayout Image;
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

        private Sprite _elementSprite;
        private readonly List<InformationItem> _items;

        public Accordion()
        {
            _items = new List<InformationItem>();
        }

        private void Awake()
        {
            GetComponent<ToggleGroup>().allowSwitchOff = true;
            gameObject.name = "Accordion";
        }

        #region GameObject Relative
        
        public void Setup(Sprite accordionSprite, Sprite elementSprite)
        {
            _elementSprite = elementSprite;
            AddVerticalLayoutGroup(gameObject);
            AddImage(gameObject, accordionSprite);
            gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
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

            //imageLayout.CreateLayout(images);
            imageLayout.GetComponent<RectTransform>().SetParent(parent);
            return imageLayout;
        }

        private void AddVerticalLayoutGroup(GameObject go)
        {
            VerticalLayoutGroup verticalLayoutGroup = go.GetComponent<VerticalLayoutGroup>();

            if (verticalLayoutGroup == null)
                verticalLayoutGroup = go.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childScaleHeight = false;
            verticalLayoutGroup.childForceExpandWidth = true;
            verticalLayoutGroup.spacing = 2;
            verticalLayoutGroup.padding = new RectOffset(2, 2, 2, 2);
        }

        private void AddImage(GameObject go, Sprite sprite)
        {
            Image image = go.AddComponent<Image>();
            
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
            image.fillCenter = true;
            image.pixelsPerUnitMultiplier = 1;
        }

        private AccordionElement InstantiateNewItem(Transform parent)
        {
            AccordionElement accordionElement = new GameObject().AddComponent<AccordionElement>();
            
            AddImage(accordionElement.gameObject, _elementSprite);
            AddVerticalLayoutGroup(accordionElement.gameObject);
            accordionElement.GetComponent<RectTransform>()?.SetParent(parent);
            return accordionElement;
        }

        #endregion

        #region Informations List

        public void AddItem(InformationsSystem.InformationItem item)
        {
            AccordionElement element = InstantiateNewItem(gameObject.transform);
            
            _items.Add(new InformationItem(
                element,
                InstantiateHeader(element.transform, item.title),
                InstantiateImageLayout(element.transform, item.images),
                InstantiateText(element.transform, item.content)));
        }
        
        public void UpdateItem(int at, InformationsSystem.InformationItem item)
        {
            if (at >= _items.Count)
                return;
            _items[at].Title.text = item.title;
            _items[at].Content.text = item.content;
        }

        public void DeleteItem(int at)
        {
            if (at >= _items.Count)
                return;
            Destroy(_items[at].Image);
            Destroy(_items[at].Title);
            Destroy(_items[at].Content);
            Destroy(_items[at].Element);
            _items.RemoveAt(at);
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

        #endregion

    }
}
