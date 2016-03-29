using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
    public static int BallsOnScreen;
    public bool IsEraser;
    private Vector3 lastPosition;
    private Vector2 lastVelocity;
    private SfxManager player;

    private void Start() {
        player = FindObjectOfType<SfxManager>();
        ++BallsOnScreen;
    }

    private void FixedUpdate() {
        lastPosition = transform.position;
        lastVelocity = GetComponent<Rigidbody2D>().velocity;
    }

    public void ResetSpeedAndPos() {
        transform.position = lastPosition;
        GetComponent<Rigidbody2D>().velocity = lastVelocity;
    }

    private void OutOfBoundsCheck() {
        var p = transform.position;
        if (p.x > 320 || p.x < 0 || p.y > 240 || p.y < -10) {
            --BallsOnScreen;
            Destroy(gameObject);
        }
    }

    private void Update() {
        //if(Input.GetKeyDown("space")) Split();
        ResetLowSpeedAndAngle(100, 5);
        OutOfBoundsCheck();
    }

    private void ResetLowSpeedAndAngle(float speedMag, float angle) {
        var v = GetComponent<Rigidbody2D>().velocity;
        if (v.magnitude < speedMag) {
            GetComponent<Rigidbody2D>().velocity = v.normalized * speedMag;
        }
        float angleCheck = Vector2.Angle(Vector2.right, new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y)));
        if (Mathf.Abs(angleCheck) < angle) {
            var rot = Quaternion.AngleAxis(Mathf.Sign(angleCheck) * (angle), Vector3.forward);
            GetComponent<Rigidbody2D>().velocity = rot * v;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (!collision.gameObject.GetComponent<Block>()) {
            player.Boing();
        }
        var v = GetComponent<Rigidbody2D>().velocity;
        GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(v.normalized * (v.magnitude + 0.75f), 250); //linear speed increase and limit
    }

    public void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Ball>() || collision.gameObject.GetComponent<Paddle>()) {
            GetComponent<Collider2D>().isTrigger = false;
        }
    }

    public void Launch(Vector2 speed) {
        transform.SetParent(null, true);
        GetComponent<Collider2D>().enabled = true;
        GetComponent<Rigidbody2D>().velocity = speed;
        player.Boing();
    }

    public void Split() {
        if (BallsOnScreen < 13 && gameObject.GetComponent<Collider2D>().enabled) {
            for (int i = -1; i < 2; i += 2) {
                GameObject ball = Instantiate(gameObject, transform.position, Quaternion.identity) as GameObject;
                if (ball != null) {
                    ball.GetComponent<Collider2D>().isTrigger = true;
                    Vector2 velocity = Quaternion.AngleAxis(i * 10, Vector3.forward) * GetComponent<Rigidbody2D>().velocity;
                    ball.GetComponent<Rigidbody2D>().velocity = velocity;
                    if (ball.GetComponent<Ball>().IsEraser) {
                        ball.GetComponent<Ball>().EnableEraser();
                    }
                }
            }
        }
    }

    public void EnableEraser() {
        IsEraser = true;
        GetComponent<SpriteRenderer>().color = Color.red;
        StartCoroutine(EraserTimeLimit());
    }

    private IEnumerator EraserTimeLimit() {
        yield return new WaitForSeconds(8);
        GetComponent<SpriteRenderer>().color = Color.white;
        IsEraser = false;
    }
}