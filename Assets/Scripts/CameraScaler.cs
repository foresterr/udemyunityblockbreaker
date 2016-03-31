using UnityEngine;

public class CameraScaler : MonoBehaviour {
    public int pixWidth;
    public int pixHeigth;
    private int w;
    private int h;

	// Use this for initialization
	void Start () {
        ScaleCamera(pixWidth, pixHeigth);
        w = Screen.width;
        h = Screen.height;
	}
	
	// Update is called once per frame
	void Update () {
	    if (w != Screen.width || h != Screen.height) {
            ScaleCamera(pixWidth, pixHeigth);
            w = Screen.width;
            h = Screen.height;
        }
    }

    private void ScaleCamera(int pixW, int pixH) {
        float scale = Mathf.Min(Screen.width/pixW, Screen.height/pixH);
        print(Screen.width);
        float camH = Screen.height/(scale*2);
        GetComponent<Camera>().orthographicSize = camH;
        transform.parent.GetComponentInChildren<Canvas>().scaleFactor = scale;
    }
}
