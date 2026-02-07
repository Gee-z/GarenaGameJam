using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class UIHoverScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private float _scalePercentage = 5f;
    [SerializeField] private float _duration = 0.2f;
    [SerializeField] private GameObject scaledObject;
    public bool BlockUi = false;

    private float _clickDuration = 0.1f;
    private RectTransform _rectTransform;
    private Vector2 _originalSize;
    private Vector2 _hoverSize;

    private void Awake()
    {
        if (scaledObject != null)
            _rectTransform = scaledObject.GetComponent<RectTransform>();
        else
            _rectTransform = GetComponent<RectTransform>();

        _originalSize = _rectTransform.sizeDelta;
        _hoverSize = _originalSize * (1f + _scalePercentage / 100f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(BlockUi) return;
        Debug.Log("Pointer Entered");

        DOTween.Kill(_rectTransform);
        _rectTransform.DOSizeDelta(_hoverSize, _duration).SetTarget(_rectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(BlockUi) return;

        DOTween.Kill(_rectTransform);
        _rectTransform.DOSizeDelta(_originalSize, _duration).SetTarget(_rectTransform);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(BlockUi) return;
        DOTween.Kill(_rectTransform);

        _rectTransform
            .DOSizeDelta(_originalSize, _clickDuration)
            .OnComplete(() =>
            {
                _rectTransform.DOSizeDelta(_hoverSize, _clickDuration).SetTarget(_rectTransform);
            })
            .SetTarget(_rectTransform);
    }
}
