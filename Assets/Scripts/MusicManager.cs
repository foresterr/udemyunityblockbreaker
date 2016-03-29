using UnityEngine;

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
        if (!audioSource.isPlaying) {
            track = ++track % Playlist.Length;
            audioSource.clip = Playlist[track];
            audioSource.Play();
        }
    }

    public void OnLevelWasLoaded(int level) {
        var audioSource = gameObject.GetComponent<AudioSource>();
        if (level == 1) {
            if (audioSource.clip.name == "08 Ascending") {
                audioSource.Stop();
                audioSource.clip = Playlist[1];
                audioSource.Play();
            }
        }
        if (level == 12) {
            audioSource.Stop();
            audioSource.clip = VictoryMusic;
            audioSource.Play();
        }
    }
}
