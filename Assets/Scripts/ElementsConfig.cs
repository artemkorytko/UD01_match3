using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementsConfig", menuName = "Elements Config")]
public class ElementsConfig : ScriptableObject
{
    [System.Serializable]
    public class Element
    {
        public string Key = string.Empty;
        public Sprite Sprite = null;
    }

    public List<Element> Elements = new List<Element>();

    public Element GetByKey(string key)
    {
        foreach(var el in Elements)
        {
            if(el.Key == key)
            {
                return el;
            }
        }

        return null;
    }
}
