using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    UIHandler UIManager;

    private void Start()
    {
        if (UIManager == null)
            UIManager = GameObject.Find("Game Scripts").GetComponent<UIHandler>();
    }

    public void Resume()
    {
        UIManager.CloseAll();
    }

    public void Quit()
    {
        StartCoroutine(QuitEnum());
    }

    public void LoadBricks()
    {
        UIManager.OpenByIndex(0);
    }

    private IEnumerator QuitEnum()
    {
        yield return new WaitForEndOfFrame();
        Application.Quit();
    }
}
