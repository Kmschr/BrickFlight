using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (BRS))]
public class BRSEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BRS brs = (BRS)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Load"))
        {
            BRS.Instance.LoadBRS();
            Repaint();
        }

        if (GUILayout.Button("Clear Bricks"))
        {
            BRS.ClearBricks();
        }

        if (brs.GetPreview() != null)
        {
            GUILayoutOption[] options = new GUILayoutOption[]
            {
                GUILayout.MaxWidth(300),
                GUILayout.MaxHeight(200)
            };
            var style = new GUIStyle("RL Background")
            {
                stretchWidth = false,
                stretchHeight = false
            };
            GUILayout.Box(brs.GetPreview(), style, options);
        }
    }
}
