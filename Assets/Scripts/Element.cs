using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Element : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 _minScale = Vector3.zero;
    [SerializeField] private Vector3 _normalScale = Vector3.one;
    [SerializeField] private Vector3 _maxScale = Vector3.one;
    [SerializeField] private float _scalingTime = 1f;

    [Header("References")]
    [SerializeField] private SpriteRenderer _bgSpriteRenderer = null;
    [SerializeField] private SpriteRenderer _iconSpriteRenderer = null;

    private string _key = string.Empty;
    private Vector2 _gridPosition = Vector2.zero;

    public bool IsActive { get; private set; }
    public float EffectDuration => _scalingTime;
    public string Key => _key;
    public Vector2 GridPosition => _gridPosition;
    public System.Action<Element> OnClicked = null;
    public bool IsInitialized { get; private set; }
    public ElementsConfig.Element ElementConfig { get; private set; }

    public void Initialize(ElementsConfig.Element element, Vector3 localPosition, Vector2 gridPostion)
    {
        SetConfig(element);
        SetLocalPosition(localPosition, gridPostion);
        Enable();
        IsInitialized = true;
    }

    public void SetConfig(ElementsConfig.Element element)
    {
        ElementConfig = element;
        _iconSpriteRenderer.sprite = element.Sprite;
        _key = element.Key;
    }

    public void SetLocalPosition(Vector3 newLocalPosition, Vector2 gridPostion)
    {
        transform.localPosition = newLocalPosition;
        _gridPosition = gridPostion;
    }

    private void OnMouseUpAsButton()
    {
        if (IsActive) OnClicked?.Invoke(this);
    }

    public void SetSelected(bool state)
    {
        Color newColor = _bgSpriteRenderer.color;
        newColor.a = state ? 1f : 0f;
        _bgSpriteRenderer.color = newColor;
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        transform.localScale = _minScale;
        transform.DOScale(_normalScale, _scalingTime).OnComplete(() =>
        {
            IsActive = true;
        });
    }

    public void Disable()
    {
        transform.DOScale(_minScale, _scalingTime).OnComplete(() =>
        {
            IsActive = false;
            gameObject.SetActive(false);
        });
    }
}
