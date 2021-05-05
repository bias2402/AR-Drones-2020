using System.Collections;
using UnityEngine;

public class OnClickSpecialEffects : MonoBehaviour {
    [SerializeField] private RectTransform flashPanel = null;
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioClip[] audioClips = null;

    //Set the clip to 'Click' effect (placed first in array)
    public void PlayClickSound() {
        if (audioClips.Length == 0) return;
        audioSource.clip = audioClips[0];
        PlayAudio();
    }

    //If the audio source got a clip, play it
    void PlayAudio() {
        if (audioSource.clip == null) return;
        audioSource.Play();
    }

    public void TriggerFlash() {
        StartCoroutine(Flash());
    }

    //Activate the flash object, increase its size, and scale it down over time
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