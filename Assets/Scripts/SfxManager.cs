using UnityEngine;
using System.Collections;

public class SfxManager : MonoBehaviour {

    public AudioClip PopClip;
    public AudioClip ClinkClip;
    public AudioClip BoingClip;
    public AudioClip ShootClip;
    public AudioClip TransformClip;

    private AudioSource player;

    private void Start() {
        player = GetComponent<AudioSource>();
    }

    public void Transform() {
        player.PlayOneShot(TransformClip);
    }

    public void Shoot() {
        player.PlayOneShot(ShootClip);
    }

    public void Pop() {
        player.PlayOneShot(PopClip);
    }

    public void Clink() {
        player.PlayOneShot(ClinkClip);
    }

    public void Boing() {
        player.PlayOneShot(BoingClip);
    }

}
