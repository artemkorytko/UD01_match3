using System;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class Elements : MonoBehaviour
    {
        public static float ScalingTime => scalingTime;

        [SerializeField] private Vector2 minScale = Vector2.zero;
        [SerializeField] private Vector2 normalScale = Vector2.one;
        private static float scalingTime = 1f;


        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private GameObject _border;

        private Element _config;
        public event Action<Elements> OnClicked;
        public string Key { get; private set; }
        public Vector2 GridPosition { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Only for tests
        /// </summary>
        public void SetTestConfig(Element config)
        {
            SetConfig(config);
        }

        public void Initialize(Element config, Vector2 gridPosition, Vector2 position)
        {
            SetConfig(config);
            SetLocalPosition(gridPosition, position);
            Enable();
            SetSelected(false);
            IsInitialized = true;
        }

        public void SetConfig(Element config)
        {
            _config = config;
            Key = config.Key;
            _spriteRenderer.sprite = config.Sprite;
        }

        public void SetLocalPosition(Vector2 gridPosition, Vector2 position)
        {
            GridPosition = gridPosition;
            transform.localPosition = position;
        }


        public void Enable()
        {
            gameObject.SetActive(true);
            transform.localScale = minScale;
            transform.DOScale(normalScale, scalingTime).OnComplete(() => { IsActive = true; });
        }

        public void Disable()
        {
            transform.DOScale(minScale, scalingTime).OnComplete(() =>
            {
                IsActive = false;
                gameObject.SetActive(false);
            });
        }

        private void OnMouseUpAsButton()
        {
            if (IsActive)
            {
                OnClicked?.Invoke(this);
            }
        }

        public void SetSelected(bool isOn)
        {
            _border.SetActive(isOn);
        }
    }
}