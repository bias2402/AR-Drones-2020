using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OnHoverEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Handler handler = null;
    [SerializeField] private RectTransform[] childrenUsingEffects = null;
    [SerializeField] private bool isNoninteractableAtStart = false;
    private Button btn = null;
    private Image img = null;
    private RectTransform rectTransform = null;
    private bool isBeingHovered = false;
    private Vector2 baseSize = Vector2.zero;
    private Color baseColor = Color.white;

    //On Start, ensure the necessary references are found and saved
    void Start() {
        if (btn == null) btn = GetComponent<Button>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        baseSize = rectTransform.sizeDelta;
        if (img == null) img = GetComponent<Image>();
        baseColor = img.color;
        if (isNoninteractableAtStart) btn.interactable = false;
    }

    //Each Update, check if the button is interactable, so the different effects can be played. These effects will play both ways depending on
    //buttons isBeingHovered state
    void Update() {
        if (btn.interactable) {
            //Lerp the size of the button to make it easier to gaze and click
            rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, isBeingHovered ? baseSize * 1.2f : baseSize, Time.deltaTime * 10);

            //Lerp the color from base to yellow, where the transition speed changes while the button is hovered (slow to yellow, fast to base)
            img.color = Color.Lerp(img.color, isBeingHovered ? Color.yellow : baseColor, Time.deltaTime * (isBeingHovered ? 3 : 10));

            //If there are any children that should be affected as well, go through and lerp their sizes as well
            if (childrenUsingEffects.Length > 0) {
                foreach (RectTransform rect in childrenUsingEffects) {
                    rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, isBeingHovered ? baseSize * 1.2f : baseSize, Time.deltaTime * 10);
                }
            }
        } else {
            //Ensure the button is turned on at the correct point of time (when a drone actually exists)
            if (isNoninteractableAtStart && handler.IsDroneSpawned()) btn.interactable = true;
        }
    }

    //Interfaces to handle hovering of UI elements. They're used to determine when to trigger the effects.
    public void OnPointerEnter(PointerEventData eventData) => isBeingHovered = true;

    public void OnPointerExit(PointerEventData eventData) => isBeingHovered = false;

}