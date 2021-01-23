using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class KeyBinds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            BRS.Instance.LoadBRS();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            string picsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            ScreenCapture.CaptureScreenshot(picsFolder + "\\City.png", 4);
        }
    }
}
