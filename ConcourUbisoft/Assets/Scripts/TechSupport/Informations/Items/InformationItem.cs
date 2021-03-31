using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TechSupport.Informations.Items
{
    public abstract class InformationItem
    {
        public abstract void Instantiate(Transform parent, Sprite backgroundSprite);
        public abstract void UpdateItem(InformationItem item);
        public abstract void Delete();
    }
}
