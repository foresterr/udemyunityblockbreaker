using UnityEngine;
using UnityEngine.UI;

public class Lives : MonoBehaviour {
    
	void Update () {
	    if (Paddle.ExtraLives >= 0) {
	        GetComponent<Text>().text = Paddle.ExtraLives.ToString();
	    }
	}
}
