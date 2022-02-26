using System;
using DG.Tweening;
using PlasticPipe.Certificates;
using UnityEngine;

public class Elements : MonoBehaviour
{
    public static float ScalingTime => scalingTime;
    [SerializeField] private Vector2 minimalScale = Vector2.zero;
    [SerializeField] private Vector2 normalScale = Vector2.one;
    private static float scalingTime = 1f;
    [Space]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _board;
    
    public string Key;
    public event Action<Elements> OnClicked;

    private Element _config;
    private Vector2 _gridPosition;
    private bool _isActive;
    private bool _isInitialized;

    public Vector2 GridPosition => _gridPosition;
    public bool IsActive => _isActive;

    public bool IsInitialized => _isInitialized;

    public void Initialize(Element element, Vector2 gridPosition, Vector2 position)
    {
        SetConfig(element);
        SetLocalPosition(gridPosition, position);
        Enable();
    }

    public void Enable()
    {
        _isInitialized = true;
        gameObject.SetActive(true);
        transform.localScale = minimalScale;
        transform.DOScale(normalScale, scalingTime).OnComplete(() =>
        {
            _isActive = true;
        });
    }

    public void Disable()
    {
        transform.DOScale(minimalScale, scalingTime).OnComplete(() =>
        {
            _isActive = false;
        });
    }

    public void SetLocalPosition(Vector2 gridPosition, Vector2 position)
    {
        _gridPosition = gridPosition;
        transform.localPosition = position;
    }

    public void SetConfig(Element element)
    {
        _config = element;
        Key = element.Key;
        _spriteRenderer.sprite = element.Sprite;
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
        _board.SetActive(isOn);
    }
    
    /// <summary>
    /// Only for tests
    /// </summary>
    public void SetTestConfig(Element config)
    {
        SetConfig(config);
    }
}
