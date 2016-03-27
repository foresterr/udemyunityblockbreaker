using UnityEngine;
using JetBrains.Annotations;

public class Block : MonoBehaviour {
    public bool IsBreakable;
    public bool IsBonus;
    public GameObject CapsulePrefab;
    public GameObject ParticlePrefab;
    public Sprite[] BreakProgress;

    public static int BlockCount;

    private int colorH;
    private int hitCount;

    private SpriteRenderer brickSprite;
    private SfxManager player;
    private LevelManager levelManager;

    private void Start() {
        brickSprite = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<SfxManager>();
        levelManager = FindObjectOfType<LevelManager>();
        if (IsBonus) {
            InvokeRepeating("RecolorSprite", 0, 0.2f);
        }
        if (IsBreakable && tag != "shieldBrick") {
            ++BlockCount;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "bullet") {
            //Debug.Log("Brick hit by a bullet");
            HandleHit(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag == "ball") {
            HandleHit(collision);
        }
    }

    private void HandleHit(Collision2D collision) {
        var go = collision.gameObject;
        bool isEraserBall = go.tag == "ball" && go.GetComponent<Ball>().IsEraser;
        if (isEraserBall || IsBreakable) {
            if (isEraserBall || ++hitCount >= BreakProgress.Length) {
                player.Pop();
                GameObject ps = Instantiate(ParticlePrefab, transform.position + new Vector3(0,4), Quaternion.AngleAxis(180,Vector3.up)) as GameObject;
                if (ps != null) {
                    ps.GetComponent<ParticleSystem>().startColor = GetComponent<SpriteRenderer>().color;
                    Destroy(ps,0.5f);
                }
                Destroy(gameObject);
                if (tag == "shieldBrick") {
                    return;
                }
                if (IsBonus || Random.Range(0, 10) == 0) {
                    SpawnCapsule();
                }
                if (IsBreakable && --BlockCount <= 0) {
                    levelManager.NextLevel();
                }
                if (isEraserBall) {
                    collision.gameObject.GetComponent<Ball>().ResetSpeedAndPos();
                }
                return;
            }
            brickSprite.sprite = BreakProgress[hitCount];
        }
        GetComponentInChildren<Shine>().Play();
        player.Clink();
    }

    private void SpawnCapsule() {
        if (CapsulePrefab) {
            GameObject capsule = Instantiate(CapsulePrefab, transform.position, Quaternion.identity) as GameObject;
            if (capsule != null) capsule.GetComponent<Capsule>().Initialize(rare: IsBonus);
        }
    }

    [UsedImplicitly]
    private void RecolorSprite() {
        brickSprite.color = Color.HSVToRGB(colorH/360f, 1, 1);
        colorH = (colorH + 30)%360;
    }
}
