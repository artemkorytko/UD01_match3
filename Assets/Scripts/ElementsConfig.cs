using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "ElementsConfig", menuName = "Configs/ElementsConfig", order = 0)]
public class ElementsConfig : ScriptableObject
{
    [SerializeField] private Element[] elements;

    public Element[] Elements => elements;

    public Element GetRandomElement()
    {
        return elements[Random.Range(0, elements.Length)];
    }

    public Element GetByKey(string key)
    {
        foreach(var el in elements)
        {
            if(el.Key == key)
            {
                return el;
            }
        }
        return null;
    }
}

[Serializable]
public class Element
{
    public string Key;
    public Sprite Sprite;
}


