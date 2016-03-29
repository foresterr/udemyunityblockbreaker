using UnityEngine;

public class BottomLoseZone : MonoBehaviour {
    private LevelManager levelManager;
    private Paddle paddle;

    private void Start() {
        levelManager = FindObjectOfType<LevelManager>();
        paddle = FindObjectOfType<Paddle>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Ball>()) {
            if (--Ball.BallsOnScreen <= 0) {
                paddle.EndCurrentBonus();
                if (--Paddle.ExtraLives < 0) {
                    Cursor.visible = true;
                    levelManager.Defeat();
                }
            }
        }
        Destroy(collision.gameObject);
    }
}
