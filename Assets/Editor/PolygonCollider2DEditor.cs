using UnityEditor;
using UnityEngine;
using System;

public class PolygonCollider2DEditor : EditorWindow {

    [MenuItem("Window/PolyCollider2D Snap")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(PolygonCollider2DEditor));
    }

    PolygonCollider2D poly;
    Vector2[] vertices = new Vector2[0];

    void OnGUI() {
        GUILayout.Label("PolyCollider2D point editor", EditorStyles.boldLabel);
        poly = (PolygonCollider2D)EditorGUILayout.ObjectField("PolygonCollider2D to edit", poly, typeof(PolygonCollider2D), true);
        if (vertices.Length != 0) {
            for (int i = 0; i < vertices.Length; ++i) {
                vertices[i] = (Vector2)EditorGUILayout.Vector2Field("Element " + i, vertices[i]);
            }
        }

        if (GUILayout.Button("Retrieve")) {
            vertices = poly.points;
        }

        if (GUILayout.Button("Set")) {
            poly.points = vertices;
        }

        if (GUILayout.Button("Align to nearest unit")) {
            for (int i = 0; i < vertices.Length; ++i) {
                vertices[i] = new Vector2(Mathf.Round(vertices[i].x),Mathf.Round(vertices[i].y));
            }
        }
    }

    /*
        void OnSelectionChange() {
            if (Selection.gameObjects.Length == 1) {
                EdgeCollider2D aux = Selection.gameObjects[0].GetComponent<EdgeCollider2D>();

                if (aux) {
                    poly = aux;
                    vertices = poly.points;
                }
            }
        }
    */
}