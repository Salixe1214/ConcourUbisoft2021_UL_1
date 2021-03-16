using System.Collections.Generic;
using System.Linq;
using TechSupport.Informations.Items;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Informations
{ 
    [RequireComponent(typeof(VerticalLayoutGroup)), RequireComponent(typeof(ContentSizeFitter)), RequireComponent(typeof(ToggleGroup))]
    public class Accordion : MonoBehaviour
    {

        private Sprite _elementSprite;
        private List<InformationItem> _items;

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
        
        #endregion

        #region Informations List

        public void AddItem(InformationItem item)
        {
            item.Instantiate(gameObject.transform, _elementSprite);
        }
        
        public void UpdateItem(int at, InformationItem item)
        {
            if (at >= _items.Count)
                return;
            _items[at].UpdateItem(item);
        }

        public void DeleteItem(int at)
        {
            if (at >= _items.Count)
                return;
            _items[at].Delete();
            _items.RemoveAt(at);
        }

        public void Clean()
        {
            foreach (InformationItem item in _items)
            {
                item.Delete();
            }
            _items.Clear();
        }

        public void CreateAccordion(IEnumerable<InformationItem> items)
        {
            _items = items.ToList();
            _items.ForEach(AddItem);
        }

        #endregion

    }
}
