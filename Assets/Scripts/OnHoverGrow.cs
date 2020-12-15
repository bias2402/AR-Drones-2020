using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OnHoverGrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Button btn = null;
    private RectTransform rectTransform = null;
    private bool isGrowing = false;
    private Vector2 baseSize = Vector2.zero;

    void Start() {
        if (btn == null) btn = GetComponent<Button>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        baseSize = rectTransform.sizeDelta;
    }

    void Update() => rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, isGrowing ? baseSize * 1.2f : baseSize, Time.deltaTime * 10);

    public void OnPointerEnter(PointerEventData eventData) => isGrowing = true;

    public void OnPointerExit(PointerEventData eventData) => isGrowing = false;

}