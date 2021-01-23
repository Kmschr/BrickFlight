using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    private bool visible = false;
    public FPSController player;
    public GUIStyle customButton;

    private void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            visible = !visible;
            if (visible)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                player.canLook = false;
            } else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                player.canLook = true;
            }
        }

        if (visible)
        {
            int w = Screen.width, h = Screen.height;
            if (GUI.Button(new Rect(w / 2 - 25, h / 2 - 15 - 50, 50, 30), "Quit", customButton))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }

}
