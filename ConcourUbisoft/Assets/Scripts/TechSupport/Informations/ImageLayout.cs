using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Informations
{
    public class ImageLayout : HorizontalLayoutGroup
    {
        private readonly List<Image> _images;
        public Font Font { get; set; }

        private GameController _gameController = null;

        public ImageLayout()
        {
            _images = new List<Image>();
        }

        protected override void Awake()
        {
            _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

            base.Awake();
            childAlignment = TextAnchor.MiddleCenter;
            gameObject.SetActive(true);
            gameObject.name = "Image Layout";
        }

        #region GameObject Relative
        
        private Image CreateImageObject()
        {
            Image image = (new GameObject()).AddComponent<Image>();

            Text text = (new GameObject()).AddComponent<Text>();
            text.text = "";
            text.font = Font;
            text.alignment = TextAnchor.UpperCenter;
            text.transform.Translate(new Vector3(0, -75.0f, 0));
            text.GetComponent<RectTransform>().SetParent(image.transform);
            
            image.preserveAspect = true;
            image.GetComponent<RectTransform>()?.SetParent(gameObject.transform);
            return image;
        }
        
        public void SetParent(Transform parent)
        {
            rectTransform.SetParent(parent);
        }
        
        #endregion

        public void DeleteSprite(int at)
        {
            if (at >= _images.Count)
                return;
            Destroy(_images[at]);
            _images.RemoveAt(at);
        }

        public void UpdateSprite(int at, Sprite sprite)
        {
            if (at >= _images.Count)
                return;
            _images[at].sprite = sprite;
        }

        public void HighlightSprite(int index)
        {
            if (index >= _images.Count)
                return;
            //_images[index].;
        }

        public void UpdateSpriteColor(int index, Color color)
        {
            if (index >= _images.Count)
                return;
            _images[index].color = color;
        }

        public void AddSprite(Sprite sprite, Color color)
        {
            Image image = CreateImageObject();
            image.color = color;
            image.sprite = sprite;
            image.gameObject.SetActive(true);
            image.GetComponentInChildren<Text>().text = _gameController.GetColorName(color);
            _images.Add(image);
        }

        public void Clean()
        {
            foreach (Image image in _images)
            {
                Destroy(image.gameObject);
            }
            _images.Clear();
        }

        public void CreateLayout(IEnumerable<Sprite> images, Color[] colors)
        {
            for (int i = 0; i < images.Count(); i++)
            {
                AddSprite(images.ElementAt(i),colors[i]);
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
                    AddSprite(image, Color.white);
                i++;
            }
        }
    }
}
