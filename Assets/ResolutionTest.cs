using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResolutionTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    GetComponent<Text>().text = "W: " + Screen.width + " H: " + Screen.height;
	}
}
