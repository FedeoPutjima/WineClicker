using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(PowerUp))]
public class AssetPreviewer : Editor
{
    PowerUp powerUp;
    Texture2D tex1;
    Texture2D tex2;

    public override void OnInspectorGUI()
    {
        powerUp = (PowerUp)target;

        GUILayout.BeginHorizontal();

        tex1 = AssetPreview.GetAssetPreview(powerUp.powerUpImage);
        GUILayout.Label(tex1); 

        tex2 = AssetPreview.GetAssetPreview(powerUp.unknownPowerUpImage);
        GUILayout.Label(tex2);

        GUILayout.EndHorizontal();
        DrawDefaultInspector();
    }
}
