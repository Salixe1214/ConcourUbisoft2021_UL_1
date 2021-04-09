using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Informations
{
    public class ImageLayout : HorizontalLayoutGroup
    {
        private static readonly int Checked = Animator.StringToHash("Checked");
        private IEnumerable<Sprite> _sprites;
        private readonly List<Image> _images;
        private GameObject _checkAnimation = null;
        private Material _blurMaterial = null;
        private Material _fontMaterial = null;
        private Sprite _question = null;
        public Font Font { get; set; }
        public float TextOffset { get; set; } = -40.0f;

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
            name = "Image Layout";
        }

        #region GameObject Relative
        
        private Image CreateImageObject()
        {
            Image image = (new GameObject()).AddComponent<Image>();

            Text text = (new GameObject()).AddComponent<Text>();
            text.text = "";
            text.font = Font;
            text.alignment = TextAnchor.UpperCenter;
            text.GetComponent<RectTransform>().SetParent(image.transform);
            text.enabled = false;

            RectTransform textTransform = text.GetComponent<RectTransform>();
            textTransform.sizeDelta = new Vector2(100,20);
            text.transform.Translate(new Vector3(0, TextOffset, 0));

            image.preserveAspect = true;
            image.GetComponent<RectTransform>()?.SetParent(gameObject.transform);
            
            RectTransform imageTransform = image.GetComponent<RectTransform>();
            imageTransform.anchorMax = new Vector2(0, 0);
            imageTransform.anchorMin = new Vector2(0, 0);
            imageTransform.pivot = new Vector2(0.5f, 0.5f);
            imageTransform.localScale = new Vector3(1, 1, 1);
            imageTransform.localPosition = new Vector3(0, 0, 0);
            imageTransform.localRotation = Quaternion.identity;
            imageTransform.anchoredPosition = new Vector2(0, 0);

            return image;
        }

        public void SetParent(Transform parent)
        {
            rectTransform.SetParent(parent);
        }
        
        #endregion

        public void SetCheckImage(GameObject prefabs)
        {
            _checkAnimation = prefabs;
        }

        public void SetBlurMaterial(Material material)
        {
            _blurMaterial = material;
        }

        public void SetFontMaterial(Material material)
        {
            _fontMaterial = material;
        }

        public void SetQuestion(Sprite question)
        {
            _question = question;
        }

        public void CheckSprite(int index)
        {
            if (!_checkAnimation)
                return;
            Debug.Log("Sprite checked for: " + index);
            Animator animator = Instantiate(_checkAnimation, _images[index].transform).GetComponent<Animator>();
            animator.SetBool(Checked, true);
        }

        public void BlurImage(int index, bool blur)
        {
            if (!_blurMaterial)
                return;
            _images[index].material = blur ? _blurMaterial : null;
            _images[index].gameObject.GetComponentInChildren<Text>().material = blur ? _fontMaterial : null;
        }

        public void SelectItem(int index)
        {
            if (index > 0)
            {
                CheckSprite(index - 1);
                // BlurImage(index - 1, true);
            }
            if (index >= _images.Count)
                return;
            _images[index].sprite = _sprites.ElementAt(index);
            _images[index].gameObject.GetComponentInChildren<Text>().enabled = true;
            // BlurImage(index, false);
        }

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

        public void UpdateSpriteColor(int index, Color color)
        {
            if (index >= _images.Count)
                return;
            _images[index].color = color;
        }

        public void AddSprite(Color color)
        {
            Image image = CreateImageObject();
            image.color = color;
            image.sprite = _question;
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
            _sprites = images;
            for (int i = 0; i < images.Count(); i++)
            {
                AddSprite(colors[i]);
            }
            SelectItem(0);
        }

        public void UpdateLayout(IEnumerable<Sprite> images)
        {
            int i = 0;

            _sprites = images;
            foreach (Sprite image in images)
            {
                if (i < _images.Count)
                    UpdateSprite(i, image);
                else
                    AddSprite( Color.white);
                i++;
            }
        }

    }
}
