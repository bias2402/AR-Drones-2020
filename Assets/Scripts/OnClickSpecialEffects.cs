using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickSpecialEffects : MonoBehaviour {
    [SerializeField] private RectTransform flashPanel = null;

    public void TriggerFlash() {
        StartCoroutine(Flash());
    }

    IEnumerator Flash() {
        flashPanel.gameObject.SetActive(true);
        flashPanel.sizeDelta = new Vector2(50, 50);
        for (int i = 0; i < 10; i++) {
            flashPanel.sizeDelta = new Vector2(50 - i * 5, 50 - i * 5);
            yield return new WaitForSeconds(0.05f);
        }
        flashPanel.gameObject.SetActive(false);
    }
}