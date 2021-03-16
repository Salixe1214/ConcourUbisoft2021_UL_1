using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Informations.Items
{
    public abstract class InformationItem
    {
        protected Text CreateTextObject()
        {
            return new GameObject()?.AddComponent<Text>();
        }

        protected HorizontalLayoutGroup CreateHorizontalLayoutGroup()
        {
            return new GameObject()?.AddComponent<HorizontalLayoutGroup>();
        }
        
        protected Text InstantiateText(Transform parent, string content)
        {
            Text text = CreateTextObject();

            text.text = content;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 14;
            text.color = Color.black;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.GetComponent<RectTransform>()?.SetParent(parent);
            return text;
        }

        protected Text InstantiateHeader(Transform parent, string content)
        {
            Text text = InstantiateText(parent, content);

            text.fontSize = 16;
            text.gameObject.AddComponent<LayoutElement>().minHeight = 18;
            return text;
        }

        protected ImageLayout InstantiateImageLayout(Transform parent, IEnumerable<Sprite> images, Color[] colors)
        {
            if (images.Count() != colors.Length)
                throw new ArgumentException("Images number has to be the same as the color array length");
            ImageLayout imageLayout = new GameObject().AddComponent<ImageLayout>();

            imageLayout.CreateLayout(images, colors);
            imageLayout.GetComponent<RectTransform>().SetParent(parent);
            return imageLayout;
        }

        protected void AddVerticalLayoutGroup(GameObject go)
        {
            VerticalLayoutGroup verticalLayoutGroup = go.GetComponent<VerticalLayoutGroup>();

            if (verticalLayoutGroup == null)
                verticalLayoutGroup = go.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childScaleHeight = false;
            verticalLayoutGroup.childForceExpandWidth = true;
            verticalLayoutGroup.spacing = 2;
            verticalLayoutGroup.padding = new RectOffset(2, 2, 2, 2);
        }
        
        protected Image AddImage(GameObject go, Sprite sprite, Color color, Image.Type type)
        {
            Image image = go.AddComponent<Image>();
            
            image.sprite = sprite;
            image.type = type;
            if (type == Image.Type.Simple || type == Image.Type.Filled)
                image.preserveAspect = true;
            else if (type == Image.Type.Sliced)
                image.fillCenter = true;
            image.color = color;
            image.pixelsPerUnitMultiplier = 1;
            return image;
        }
        

        protected Image InstantiateImage(Transform parent, Sprite sprite, Color color, Image.Type type)
        {
            GameObject go = new GameObject();
            
            go.transform.SetParent(parent);
            Image i = AddImage(go, sprite, color, type);
            return i;
        }

        protected Image InstantiateImage(Transform parent, Sprite sprite)
        {
            GameObject go = new GameObject();
            
            go.transform.SetParent(parent);
            return AddImage(go, sprite, Color.clear, Image.Type.Simple);
        }
        protected AccordionElement InstantiateNewItem(Transform parent, Sprite backgroundSprite)
        {
            AccordionElement accordionElement = new GameObject().AddComponent<AccordionElement>();
            
            AddImage(accordionElement.gameObject, backgroundSprite, Color.clear, Image.Type.Sliced);
            AddVerticalLayoutGroup(accordionElement.gameObject);
            accordionElement.GetComponent<RectTransform>()?.SetParent(parent);
            return accordionElement;
        }

        public abstract void Instantiate(Transform parent, Sprite backgroundSprite);
        public abstract void UpdateItem(InformationItem item);
        public abstract void Delete();
    }
}
