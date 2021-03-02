using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Informations
{
    public class ImageLayout : HorizontalLayoutGroup
    {
        private List<Image> _images;

        public ImageLayout()
        {
            _images = new List<Image>();
        }

        protected override void Awake()
        {
            base.Awake();
            childAlignment = TextAnchor.MiddleCenter;
            gameObject.SetActive(true);
        }
                
        private Image CreateImageObject()
        {
            Image image = (new GameObject()).AddComponent<Image>();

            image.preserveAspect = true;
            image.GetComponent<RectTransform>()?.SetParent(gameObject.transform);
            return image;
        }

        public void UpdateSprite(int at, Sprite sprite)
        {
            if (at >= _images.Count)
                return;
            _images[at].sprite = sprite;
        }

        public void AddSprite(Sprite sprite)
        {
            Image image = CreateImageObject();

            image.sprite = sprite;
            image.gameObject.SetActive(true);
            _images.Add(image);
        }

        public void Clean()
        {
            foreach (Image image in _images)
            {
                Destroy(image.gameObject);
            }
        }

        public void CreateLayout(IEnumerable<Sprite> images)
        {
            foreach (Sprite image in images)
            {
                AddSprite(image);
            }
        }

        public void UpdateLayout(IEnumerable<Sprite> images)
        {
            int i = 0;

            foreach (Sprite image in images)
            {
                if (i < _images.Count)
                    UpdateSprite(i, image);
                else
                    AddSprite(image);
                i++;
            }
        }

        public void SetParent(Transform parent)
        {
            rectTransform.SetParent(parent);
        }
    }
}
