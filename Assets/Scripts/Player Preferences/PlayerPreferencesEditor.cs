using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerPreferencesManager))]
public class PlayerPreferencesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerPreferencesManager playerPreferencesManager = (PlayerPreferencesManager)target;

        if (GUILayout.Button("Reset player preferences"))
        {
            playerPreferencesManager.ResetPlayerPreferences();
        }
    }

}
