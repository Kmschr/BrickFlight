using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSun : MonoBehaviour
{
    private Transform sun;

    // Start is called before the first frame update
    void Start()
    {
        sun = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Keypad9))
        {
            sun.Rotate(Vector3.up, 0.2f);
        }
        if (Input.GetKey(KeyCode.Keypad7))
        {
            sun.Rotate(Vector3.up, -0.2f);
        }
    }
}
