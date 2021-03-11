using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct Item
        {
            public TKey key;
            public TValue value;

            public Item(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }
        }
        [SerializeField]
        private List<Item> items = new List<Item>();
        
        public SerializableDictionary(IDictionary<TKey, TValue> items)
        {
            foreach (KeyValuePair<TKey,TValue> keyValuePair in items)
            {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }
        
        public void OnBeforeSerialize()
        {
            items.Clear();
            foreach(KeyValuePair<TKey, TValue> pair in this)
            {
                items.Add(new Item(pair.Key, pair.Value));
            }
        }
     
        public void OnAfterDeserialize()
        {
            items.ForEach(item => this[item.key] = item.value);
        }
    }
}
