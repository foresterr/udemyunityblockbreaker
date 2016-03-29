using System.Collections;
using UnityEngine;

public class Paddle : MonoBehaviour {
    public static int ExtraLives = 2;
    public GameObject BallPrefab;
    public GameObject BulletPrefab;
    public GameObject GuardBlockPrefab;
    public Sprite[] LaserAnim;
    public Sprite[] StickyAnim;
    public Sprite[] WideAnim;

    private LevelManager levelMgr;
    private Camera mainCam;
    private SfxManager sfxPlayer;
    private GameObject[] guardBlocks = new GameObject[13];
    private GameObject gluedBall;

    private PaddleType currentPaddle = PaddleType.Normal;
    private IEnumerator scheduledApplyPaddleType;
    private bool paddleBonusActive;
    private bool bonusTimerRunning;
    private bool interruptBonusTimer;

    public bool HasBall {
        get { return gluedBall; }
    }

    private enum PaddleType {
        Normal,
        Laser,
        Sticky,
        Launcher,
        Wide
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        var capsule = collision.gameObject.GetComponent<Capsule>();
        if (capsule) {
            ApplyBonus(capsule.CapsuleBonus.Name);
            Destroy(collision.gameObject);
        }
    }

    private void ApplyBonus(string bonusName) {
        sfxPlayer.Transform();
        switch (bonusName) {
            case "sticky":
                scheduledApplyPaddleType = ApplyPaddleType(PaddleType.Sticky, 30, Color.white, StickyAnim);
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
                scheduledApplyPaddleType = ApplyPaddleType(PaddleType.Wide, 45, Color.white, WideAnim);
                break;
            case "eraser":
                var ballsToEraser = GameObject.FindGameObjectsWithTag("ball");
                foreach (var b in ballsToEraser) {
                    b.GetComponent<Ball>().EnableEraser();
                }
                break;
            case "laser":
                scheduledApplyPaddleType = ApplyPaddleType(PaddleType.Laser, 20, Color.white, LaserAnim);
                break;
            case "launcher":
                scheduledApplyPaddleType = ApplyPaddleType(PaddleType.Launcher, 3, Color.yellow, null);
                break;
            case "shield":
                StartCoroutine(RebuildShield());
                break;
        }
    }

    private IEnumerator ApplyPaddleType(PaddleType paddle, float duration, Color color, Sprite[] anim) {
        scheduledApplyPaddleType = null;
        paddleBonusActive = true;
        while (currentPaddle != PaddleType.Normal) {
            yield return null;
        }
        currentPaddle = paddle;
        if (anim != null) {
            yield return StartCoroutine(PlayBonusAnim(anim, 0.3f, reverse: false));
        }
        GetComponent<SpriteRenderer>().color = color;
        yield return StartCoroutine(StartBonusClock(duration));
        if (anim != null) {
            yield return StartCoroutine(PlayBonusAnim(anim, 0.3f, reverse: true));
        }
        GetComponent<SpriteRenderer>().color = Color.white;
        currentPaddle = PaddleType.Normal;
        //print("scheduled next bonus: "+scheduledApplyPaddleType);
        if (scheduledApplyPaddleType != null) {
            //print("Daisy chaining next bonus");
            StartCoroutine(scheduledApplyPaddleType); //daisy chain last picked (during the current bonus)
        } else {
            paddleBonusActive = false;
        }
    }

    private IEnumerator StartBonusClock(float duration) {
        bonusTimerRunning = true;
        //print("Starting bonus timer with duration "+duration);
        while (duration >= 0) {
            if (interruptBonusTimer) {
                //print("Bonus interrupted");
                interruptBonusTimer = false;
                bonusTimerRunning = false;
                yield break;
            }
            duration -= Time.deltaTime;
            yield return null;
        }
        bonusTimerRunning = false;
    }

    public void InterruptBonus() {
        scheduledApplyPaddleType = null;
        interruptBonusTimer = true;
    }

    private IEnumerator PlayBonusAnim(Sprite[] anim, float duration, bool reverse) {
        var s = GetComponent<SpriteRenderer>();
        for (int i = 0; i < anim.Length; i++) {
            s.sprite = anim[reverse ? anim.Length - 1 - i : i];
            GetComponent<BoxCollider2D>().size = s.sprite.bounds.size;
            yield return new WaitForSeconds(duration/anim.Length);
        }
    }

    private IEnumerator RebuildShield() {
        for (int i = 0; i < guardBlocks.Length; i++) {
            if (guardBlocks[i] == null) {
                var width = GuardBlockPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
                var pos = new Vector3(4 + width/2 + i*width, 4);
                guardBlocks[i] = Instantiate(GuardBlockPrefab, pos, Quaternion.identity) as GameObject;
            }
            yield return new WaitForSeconds(0.05f);
        }
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

    private void Capture(Collision2D coll) {
        coll.collider.isTrigger = true;
        coll.collider.enabled = false;
        coll.rigidbody.velocity = Vector2.zero;
        coll.gameObject.transform.position = transform.position;
        coll.gameObject.transform.SetParent(transform);
        gluedBall = coll.gameObject;
    }

    private void Deflect(Collision2D coll) {
        var vm = coll.rigidbody.velocity.magnitude;
        var maxDist = 0.99f*GetComponent<Collider2D>().bounds.extents.x;
        var xDist = transform.position.x - coll.transform.position.x;
        var yDist = transform.position.y - coll.transform.position.y;
        if (Mathf.Abs(xDist) <= maxDist && yDist < 2) {
            var angle = Mathf.Clamp(xDist/maxDist*90 + 90, 10, 170);
            //always deflect at at least 10 deg, lower angles are boring
            var newV = new Vector2(Mathf.Cos(angle*Mathf.Deg2Rad), Mathf.Sin(angle*Mathf.Deg2Rad));
            newV *= vm;
            coll.rigidbody.velocity = newV;
        }
    }

    private void Start() {
        sfxPlayer = FindObjectOfType<SfxManager>();
        levelMgr = FindObjectOfType<LevelManager>();
        mainCam = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update() {
        if (levelMgr.Paused) return;
        PositionOnMouseX(4);
        if (scheduledApplyPaddleType != null) {
            if (bonusTimerRunning) {
                //print("Asking current bonus to end");
                interruptBonusTimer = true;
            } else if (!paddleBonusActive){
                //print("Starting a completely new bonus");
                StartCoroutine(scheduledApplyPaddleType);
            }
        }
        if (!HasBall && (Ball.BallsOnScreen == 0 || currentPaddle == PaddleType.Launcher && Ball.BallsOnScreen < 13)) {
            SpawnBall();
        }
        var aimLine = GetComponentInChildren<AimLine>();
        if (Input.GetMouseButton(0) && currentPaddle != PaddleType.Launcher && HasBall) {
            aimLine.StartSweep();
        }
        if (Input.GetMouseButtonUp(0) && HasBall) {
            Vector2 angle;
            if (currentPaddle != PaddleType.Launcher) {
                angle = aimLine.CurrentAngle;
            } else {
                int angle2 = Random.Range(-45, 46);
                angle = Quaternion.AngleAxis(angle2, Vector3.forward)*Vector2.up;
            }
            gluedBall.GetComponent<Ball>().Launch(angle*100);
            gluedBall = null;
        }
        if (Input.GetMouseButtonDown(0) && currentPaddle == PaddleType.Laser) {
            sfxPlayer.Shoot();
            var p = transform.position;
            for (int i = -1; i <= 1; i += 2) {
                GameObject bullet =
                    Instantiate(BulletPrefab, new Vector3(p.x + i*10, p.y + 1), Quaternion.identity) as GameObject;
                if (bullet != null) bullet.GetComponent<Rigidbody2D>().velocity = Vector2.up*200;
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

    private void SpawnBall() {
        gluedBall = Instantiate(BallPrefab, transform.position, Quaternion.identity) as GameObject;
        if (gluedBall != null) {
            gluedBall.transform.SetParent(transform);
            gluedBall.GetComponent<Collider2D>().enabled = false;
            gluedBall.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }
}
