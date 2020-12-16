using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OnHoverEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private Button btn = null;
    private Image img = null;
    private RectTransform rectTransform = null;
    private bool isBeingHovered = false;
    private Vector2 baseSize = Vector2.zero;
    private Color baseColor = Color.white;

    void Start() {
        if (btn == null) btn = GetComponent<Button>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        baseSize = rectTransform.sizeDelta;
        if (img == null) img = GetComponent<Image>();
        baseColor = img.color;
    }

    void Update() {
        if (btn.interactable) {
            rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, isBeingHovered ? baseSize * 1.2f : baseSize, Time.deltaTime * 10);
            img.color = Color.Lerp(img.color, isBeingHovered ? Color.yellow : baseColor, Time.deltaTime * (isBeingHovered ? 3 : 10));
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => isBeingHovered = true;

    public void OnPointerExit(PointerEventData eventData) => isBeingHovered = false;

}