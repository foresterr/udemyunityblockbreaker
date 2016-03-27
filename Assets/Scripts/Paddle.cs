using UnityEngine;
using System.Collections;

public class Paddle : MonoBehaviour {
    public static int ExtraLives = 2;
    public GameObject BallPrefab;
    public GameObject GuardBlockPrefab;
    public GameObject BulletPrefab;
    public Sprite[] StickyAnim;
    public Sprite[] LaserAnim;
    public Sprite[] WideAnim;

    private enum PaddleType {
        Normal,
        Laser,
        Sticky,
        Launcher,
        Wide
    }

    private PaddleType currentPaddle = PaddleType.Normal;
    private bool isAnimPlaying;
    private GameObject gluedBall;
    private Camera mainCam;
    private Sprite[] currentAnim;
    private GameObject[] guardBlocks = new GameObject[13];
    private SfxManager player;
    private LevelManager levelMgr;

    public bool HasBall {
        get { return gluedBall; }
    }

    private void ApplyBonus(string bonusName) {
        player.Transform();
        switch (bonusName) {
            case "sticky":
                if (currentPaddle != PaddleType.Sticky) {
                    currentPaddle = PaddleType.Sticky;
                    GetComponent<SpriteRenderer>().color = Color.white;
                    ChangeAnim(StickyAnim);
                    StartCoroutine(BonusTimeLimit(30, currentPaddle));
                }
                break;
            case "split":
                var ballsToSplit = GameObject.FindGameObjectsWithTag("ball");
                foreach (var b in ballsToSplit) {
                    b.GetComponent<Ball>().Split();
                }
                break;
            case "life":
                ++ExtraLives;
                break;
            case "extend":
                if (currentPaddle != PaddleType.Wide) {
                    currentPaddle = PaddleType.Wide;
                    GetComponent<SpriteRenderer>().color = Color.white;
                    ChangeAnim(WideAnim);
                    StartCoroutine(BonusTimeLimit(60, currentPaddle));
                }
                break;
            case "eraser":
                var ballsToEraser = GameObject.FindGameObjectsWithTag("ball");
                foreach (var b in ballsToEraser) {
                    //Debug.Log(b.bonusName);
                    b.GetComponent<Ball>().EnableEraser();
                }
                break;
            case "laser":
                if (currentPaddle != PaddleType.Laser) {
                    currentPaddle = PaddleType.Laser;
                    GetComponent<SpriteRenderer>().color = Color.white;
                    ChangeAnim(LaserAnim);
                    StartCoroutine(BonusTimeLimit(15, currentPaddle));
                }
                break;
            case "launcher":
                if (currentPaddle != PaddleType.Launcher) {
                    if (currentPaddle != PaddleType.Normal) {
                        PlayQueue(currentAnim, true);
                        currentAnim = null;
                    }
                    currentPaddle = PaddleType.Launcher;
                    GetComponent<SpriteRenderer>().color = Color.yellow;
                    StartCoroutine(BonusTimeLimit(3, currentPaddle));
                }
                break;
            case "shield":
                StartCoroutine(RebuildShield());
                break;
        }
    }

    private IEnumerator RebuildShield() {
        for (int i = 0; i < guardBlocks.Length; i++) {
            if (guardBlocks[i] == null) {
                var width = GuardBlockPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
                var pos = new Vector3(4 + width / 2 + i * width, 4);
                guardBlocks[i] = Instantiate(GuardBlockPrefab, pos, Quaternion.identity) as GameObject;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator BonusTimeLimit(float duration, PaddleType bonusToEnd) {
        yield return new WaitForSeconds(duration);
        GetComponent<SpriteRenderer>().color = Color.white;
        if (currentPaddle == bonusToEnd) {
            currentPaddle = PaddleType.Normal;
            if (currentAnim != null) {
                PlayQueue(currentAnim, true);
                currentAnim = null;
            }
        }
    }

    private void ChangeAnim(Sprite[] anim) {
        if (currentAnim != null) {
            PlayQueue(currentAnim, true);
        }
        currentAnim = anim;
        PlayQueue(currentAnim, false);
    }

    public void PlayQueue(Sprite[] anim, bool reverse) {
        StartCoroutine(PlayAnimCoroutine(anim, 0.3f, reverse));
    }

    private IEnumerator PlayAnimCoroutine(Sprite[] anim, float duration, bool reverse) {
        while (isAnimPlaying) {
            yield return null;
        }
        isAnimPlaying = true;
        var s = GetComponent<SpriteRenderer>();
        for (int i = 0; i < anim.Length; i++) {
            s.sprite = anim[reverse ? (anim.Length - 1 - i) : i];
            GetComponent<BoxCollider2D>().size = s.sprite.bounds.size;
            yield return new WaitForSeconds(duration / anim.Length);
        }
        isAnimPlaying = false;
    }

    private void Start() {
        player = FindObjectOfType<SfxManager>();
        levelMgr = FindObjectOfType<LevelManager>();
        mainCam = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        //ApplyBonus("Laser");
    }

    private void SpawnBall() {
        gluedBall = Instantiate(BallPrefab, transform.position, Quaternion.identity) as GameObject;
        if (gluedBall != null) {
            gluedBall.transform.SetParent(transform);
            gluedBall.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }

    private void Update() {
        if (levelMgr.Paused) return;
        PositionOnMouseX(4);
        if (!HasBall) {
            if (Ball.BallsOnScreen == 0) {
                if (currentAnim != null) {
                    PlayQueue(currentAnim, true);
                    currentAnim = null;
                    currentPaddle = PaddleType.Normal;
                }
                SpawnBall();
            } else if (currentPaddle == PaddleType.Launcher && Ball.BallsOnScreen < 16) {
                SpawnBall();
            }
        }
        var aimLine = GetComponentInChildren<AimLine>();
        if (Input.GetMouseButtonDown(0) && currentPaddle != PaddleType.Launcher && HasBall) {
            aimLine.StartSweep();
        }
        if (Input.GetMouseButtonUp(0) && HasBall) {
            Vector2 angle;
            if (currentPaddle != PaddleType.Launcher) {
                angle = aimLine.CurrentAngle;
            } else {
                int angle2 = Random.Range(-45, 46);
                angle = Quaternion.AngleAxis(angle2, Vector3.forward) * Vector2.up;
            }
            gluedBall.GetComponent<Ball>().Launch(angle * 100);
            gluedBall = null;
        }
        if (Input.GetMouseButtonDown(0) && currentPaddle == PaddleType.Laser) {
            player.Shoot();
            var p = transform.position;
            for (int i = -1; i <= 1; i += 2) {
                GameObject bullet =
                    Instantiate(BulletPrefab, new Vector3(p.x + i * 10, p.y + 1), Quaternion.identity) as GameObject;
                if (bullet != null) bullet.GetComponent<Rigidbody2D>().velocity = Vector2.up * 200;
            }
        }
    }

    private void PositionOnMouseX(float margin) {
        var mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        var worldMaxPos = mainCam.ScreenToWorldPoint(new Vector3(mainCam.pixelWidth, mainCam.pixelHeight, 0));
        var xLimit = margin + GetComponent<SpriteRenderer>().sprite.bounds.extents.x;
        var mouseToWorldX = Mathf.Clamp(mouseWorldPos.x, xLimit, worldMaxPos.x - xLimit);
        transform.position = new Vector3(mouseToWorldX, transform.position.y, transform.position.z);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Ball>()) {
            if (!gluedBall && currentPaddle == PaddleType.Sticky) {
                Capture(collision);
            } else {
                Deflect(collision);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        var capsule = collision.gameObject.GetComponent<Capsule>();
        if (capsule) {
            //Debug.Log(capsule.CapsuleBonus.Name);
            ApplyBonus(capsule.CapsuleBonus.Name);
            Destroy(collision.gameObject);
        }
    }

    private void Deflect(Collision2D coll) {
        var vm = coll.rigidbody.velocity.magnitude;
        var maxDist = 0.99f * (GetComponent<Collider2D>().bounds.extents.x);
        var xDist = transform.position.x - coll.transform.position.x;
        var yDist = transform.position.y - coll.transform.position.y;
        if (Mathf.Abs(xDist) <= maxDist && yDist < 2) {
            var angle = Mathf.Clamp(xDist / maxDist * 90 + 90, 10, 170);
            //always deflect at at least 10 deg, lower angles are boring
            var newV = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            newV *= vm;
            coll.rigidbody.velocity = newV;
        }
    }

    private void Capture(Collision2D coll) {
        coll.collider.isTrigger = true;
        coll.rigidbody.velocity = Vector2.zero;
        coll.gameObject.transform.position = transform.position;
        coll.gameObject.transform.SetParent(transform);
        gluedBall = coll.gameObject;
    }
}
