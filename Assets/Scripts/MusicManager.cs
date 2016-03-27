using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {
    private static MusicManager instance;
    private int track;
    public AudioClip[] Playlist;
    public AudioClip VictoryMusic;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update() {
        var audioSource = gameObject.GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().name == "Screen_win") {
            audioSource.Stop();
            audioSource.clip = VictoryMusic;
            audioSource.Play();
        }
        if (!audioSource.isPlaying) {
            track = ++track%Playlist.Length;
            audioSource.clip = Playlist[track];
            audioSource.Play();
        }
    }
}
