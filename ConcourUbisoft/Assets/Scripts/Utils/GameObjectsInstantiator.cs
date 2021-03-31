using System;
using System.Collections.Generic;
using System.Linq;
using Buttons;
using TechSupport.Informations;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public static class GameObjectsInstantiator
    { 
        public static Text CreateTextObject()
        {
            return new GameObject()?.AddComponent<Text>();
        }

        public static HorizontalLayoutGroup CreateHorizontalLayoutGroup()
        {
            return new GameObject()?.AddComponent<HorizontalLayoutGroup>();
        }

        public static OutlineButton CreateButton()
        {
            return new GameObject()?.AddComponent<OutlineButton>();
        }
        
        public static Text InstantiateText(Transform parent, string content)
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

        public static Text InstantiateHeader(Transform parent, string content)
        {
            Text text = InstantiateText(parent, content);

            text.fontSize = 16;
            text.gameObject.AddComponent<LayoutElement>().minHeight = 18;
            return text;
        }

        public static ImageLayout InstantiateImageLayout(Transform parent, IEnumerable<Sprite> images, Color[] colors)
        {
            if (images.Count() != colors.Length)
                throw new ArgumentException("Images number has to be the same as the color array length");
            ImageLayout imageLayout = new GameObject().AddComponent<ImageLayout>();

            imageLayout.CreateLayout(images, colors);
            imageLayout.GetComponent<RectTransform>().SetParent(parent);
            return imageLayout;
        }

        public static void AddVerticalLayoutGroup(GameObject go)
        {
            VerticalLayoutGroup verticalLayoutGroup = go.GetComponent<VerticalLayoutGroup>();

            if (verticalLayoutGroup == null)
                verticalLayoutGroup = go.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childScaleHeight = false;
            verticalLayoutGroup.childForceExpandWidth = true;
            verticalLayoutGroup.spacing = 0;
            verticalLayoutGroup.padding = new RectOffset(2, 2, 2, 2);
        }
        
        public static Image AddImage(GameObject go, Sprite sprite, Color color, Image.Type type)
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
        

        public static Image InstantiateImage(Transform parent, Sprite sprite, Color color, Image.Type type)
        {
            GameObject go = new GameObject();
            
            go.transform.SetParent(parent);
            Image i = AddImage(go, sprite, color, type);
            return i;
        }

        public static Image InstantiateImage(Transform parent, Sprite sprite)
        {
            GameObject go = new GameObject();
            
            go.transform.SetParent(parent);
            return AddImage(go, sprite, Color.clear, Image.Type.Simple);
        }

        public static OutlineButton InstantiateOutlineButton(Transform parent)
        {
            OutlineButton button = CreateButton();

            button.transform.SetParent(parent);
            if (!button)
                Debug.Log("No Button created or return");
            return button;
        }

        public static AccordionElement InstantiateNewItem(Transform parent, Sprite backgroundSprite)
        {
            AccordionElement accordionElement = new GameObject().AddComponent<AccordionElement>();
            
            AddImage(accordionElement.gameObject, backgroundSprite, Color.clear, Image.Type.Sliced);
            AddVerticalLayoutGroup(accordionElement.gameObject);
            accordionElement.GetComponent<RectTransform>()?.SetParent(parent);
            return accordionElement;
        }
    }
}