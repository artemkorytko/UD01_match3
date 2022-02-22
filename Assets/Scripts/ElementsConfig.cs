using UnityEngine;
using System.Linq;

namespace DefaultNamespace
{
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
            return elements.FirstOrDefault(element => element.Key == key);
        }
    }

    [System.Serializable]
    public class Element
    {
        public string Key;
        public Sprite Sprite;
    }
}