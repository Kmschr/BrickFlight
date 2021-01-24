using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField]
    public List<UIBinder> UIBinds = new List<UIBinder>();

    [Tooltip("The canvas the UI will be placed on.")]
    public GameObject UICanvas;

    [Tooltip("If enabled, this will lock the cursor when no windows that require cursor are active.")]
    public bool AutoLockCursor;

    [Tooltip("The keycode to close the newest UI")]
    public KeyCode CloseNewestUI = KeyCode.Escape;

    private void Start()
    {
        if (UICanvas == null)
            UICanvas = GameObject.Find("Canvas");
    }

    private void Update()
    {
        bool CursorLock = true;

        //If trying to close the newest UI
        if (Input.anyKey && Input.GetKeyDown(CloseNewestUI))
        {
            if (UIBinds.Count > 0)
                if (UIBinds[UIBinds.Count - 1].Instantiated != null)
                    Destroy(UIBinds[UIBinds.Count - 1].Instantiated);
        }

        //Check all UIs
        for (int i = 0; i < UIBinds.Count; i++)
        {
            //Get if it's open or not
            bool IsOpen = UIBinds[i].Instantiated;

            //Only check if any keys are pressed
            if (Input.anyKey)
            {
                //Check all keybinds for this UI
                for (int j = 0; j < UIBinds[i].ToggleKeybinds.Length; j++)
                {
                    if (Input.GetKeyDown(UIBinds[i].ToggleKeybinds[j]))
                    {
                        //Open/close
                        if (!IsOpen)
                        {
                            Open(UIBinds[i]);
                        }
                        else
                        {
                            //If it's open, close it
                            Close(UIBinds[i]);
                        }
                    }
                }
            }

            //Cursor visibility
            if (IsOpen && UIBinds[i].KeepCursorVisible)
            {
                CursorLock = false;
            }
        }

        if (AutoLockCursor && CursorLock)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    public void OpenByIndex(int idx)
    {
        //If it's not open, open it
        UIBinds[idx].Instantiated = Instantiate(UIBinds[idx].UIPrefab, UICanvas.transform);

        //If this UI closes others out
        if (UIBinds[idx].CloseOtherUI)
        {
            //Close all UIs but this one
            for (int k = 0; k < UIBinds.Count; k++)
            {
                if (UIBinds[idx] != UIBinds[k])
                    Close(UIBinds[k]);
            }
        }
    }

    public void Open(UIBinder UI)
    {
        //If it's not open, open it
        UI.Instantiated = Instantiate(UI.UIPrefab, UICanvas.transform);

        //If this UI closes others out
        if (UI.CloseOtherUI)
        {
            //Close all UIs but this one
            for (int k = 0; k < UIBinds.Count; k++)
            {
                if (UI != UIBinds[k])
                    Close(UIBinds[k]);
            }
        }
    }

    public void Close(UIBinder UI, bool BypassReturn = false)
    {
        if (UI.Instantiated != null)
            Destroy(UI.Instantiated);

        if (UI.ReturnIndex > -1 && !BypassReturn)
        {
            if (UIBinds.Count-1 < UI.ReturnIndex)
            {
                Debug.LogWarning("Tried to open UIBinds index: " + UI.ReturnIndex + "(+1)" + ", but there are only " + UIBinds.Count + " total binds! Halting operation to prevent errors.");
                return;
            }

            Open(UIBinds[UI.ReturnIndex]);
        }
    }

    public void CloseAll()
    {
        for (int k = 0; k < UIBinds.Count; k++)
        {
                Close(UIBinds[k], true);
        }
    }
}

[System.Serializable]
public class UIBinder
{
    [Tooltip("The UI object to instantiate.")]
    public GameObject UIPrefab;
    [Tooltip("The UI index that appears when this one is closed. -1 disables this")]
    public int ReturnIndex = -1;
    [SerializeField]
    [Tooltip("The keybinds that open and close this UI")]
    public KeyCode[] ToggleKeybinds = new KeyCode[0];
    [Tooltip("Should the cursor be kept unlocked when this UI is open?")]
    public bool KeepCursorVisible;
    [Tooltip("Should we close all other UIs when this one opens?")]
    public bool CloseOtherUI;

    [HideInInspector]
    public GameObject Instantiated;
}