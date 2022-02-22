using System;
using DG.Tweening;
using UnityEngine;

namespace DefaultNamespace
{
    public class Elements : MonoBehaviour
    {
        [SerializeField] private Vector2 minScale = Vector2.one;
        [SerializeField] private Vector2 normalScale = Vector2.one;
        [SerializeField] private float scalingTime = 1f;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private GameObject _board;

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
            IsInitialized = true;
        }

        private void SetConfig(Element config)
        {
            _config = config;
            Key = config.Key;
            _spriteRenderer.sprite = config.Sprite;
        }

        private void SetLocalPosition(Vector2 gridPosition, Vector2 position)
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

        public void SelSelected(bool isOn)
        {
            _board.SetActive(isOn);
        }
    }
}