using UnityEngine;
using System.Collections;

public class AimLine : MonoBehaviour {
    private Vector2 currentAngle;

    public Vector2 CurrentAngle {
        get { return currentAngle; }
    }

    public void StartSweep() {
        var len = GetComponentInChildren<Camera>().targetTexture.height;
        StartCoroutine(Sweep(160, Random.Range(10, 170), len, 1));
    }

    private IEnumerator Sweep(int range, int initialAngle, float length, float duration) {
        var lr = GetComponent<LineRenderer>();
        lr.enabled = true;
        var deg = initialAngle;
        while (Input.GetMouseButton(0)) {
            deg = ++deg % (range * 2);
            //Debug.Log(deg);
            var deg2 = deg - (deg % range) * (deg / range) * 2 + (90 - range / 2);
            //print(string.Format("{0}%({1}*2) - ({0}%{1})*({0}/{1})*2 = {2}",deg,range,deg2));
            Vector3 startPos = new Vector3(0, 2);
            Vector3 endPos = ((Quaternion.AngleAxis(deg2, Vector3.forward) * Vector3.right) * length) + startPos;
            lr.SetPositions(new[] { startPos, endPos });
            DrawPixelized();
            currentAngle = (endPos - startPos).normalized;
            yield return new WaitForSeconds(duration / range);
        }
        lr.enabled = false;
        DrawPixelized();
    }

    private void DrawPixelized() {
        var rtCam = GetComponentInChildren<Camera>();
        RenderTexture rt = rtCam.targetTexture;
        Texture2D t = new Texture2D(rt.width, rt.height) { filterMode = FilterMode.Point };
        rtCam.Render();
        RenderTexture.active = rt;
        t.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        t.Apply();
        RenderTexture.active = null;

        Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0), 1);
        GetComponentInChildren<SpriteRenderer>().sprite = s;
    }
}
