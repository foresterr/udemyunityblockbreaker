using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    private bool alreadyCalled;
    public bool Paused;
    public GameObject PauseScreen;

    private void Start() {
        Ball.BallsOnScreen = 0;
        Block.BlockCount = 0;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        if (PauseScreen && Input.GetKeyDown(KeyCode.Space)) {
            Time.timeScale = Paused ? 1 : 0;
            Paused = !Paused;
            PauseScreen.SetActive(Paused);
        }
    }

    public void NextLevel() {
        if (!alreadyCalled) {
            alreadyCalled = true;
            StartCoroutine(NextLevelDelay());
        }
    }

    private IEnumerator NextLevelDelay() {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartGame() {
        SceneManager.LoadScene("Level_test");
    }

    public void RequestQuit() {
        Application.Quit();
    }

    public void Continue() {
        SceneManager.LoadScene("Screen_start");
        Paddle.ExtraLives = 2;
    }

    public void Victory() {
        SceneManager.LoadScene("Screen_win");
    }

    public void Defeat() {
        SceneManager.LoadScene("Screen_lose");
    }

    public void Credits() {
        SceneManager.LoadScene("Screen_credits");
    }

    public void Instructions() {
        SceneManager.LoadScene("Screen_help");
    }
}
