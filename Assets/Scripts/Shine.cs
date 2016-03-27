using UnityEngine;
using System.Collections;

public class Shine : MonoBehaviour {
    public bool Loop;
    public Sprite[] ShineAnim;

    private bool isPlaying;

    public void Play() {
        if (!isPlaying) {
            StartCoroutine(PlayCoroutine(0.5f));
        }
    }

    private IEnumerator PlayCoroutine(float duration) {
        isPlaying = true;
        var s = GetComponent<SpriteRenderer>();
        for (int i = 0; i < ShineAnim.Length; i++) {
            s.sprite = ShineAnim[i];
            yield return new WaitForSeconds(duration/ShineAnim.Length);
        }
        isPlaying = false;
        s.sprite = null;
        if (Loop) {
            StartCoroutine(PlayCoroutine(0.5f));
        }
    }
}
