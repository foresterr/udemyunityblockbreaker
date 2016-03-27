using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    public void OnCollisionEnter2D(Collision2D collision) {
        Destroy(gameObject);
    }
}
