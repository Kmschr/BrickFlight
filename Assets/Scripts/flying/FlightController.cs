using UnityEngine;

public class FlightController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] [Tooltip("Transform of the aircraft the rig follows and references")]
    private Transform aircraft = null;
    [SerializeField] [Tooltip("Transform of the object on the rig which the camera is attached to")]
    private Transform cameraRig = null;
    [SerializeField] [Tooltip("Transform of the camera itself")]
    private Transform cam = null;

    [Header("Options")]
    [SerializeField] [Tooltip("Follow aircraft using fixed update loop")]
    private bool useFixed = true;

    [SerializeField] [Tooltip("How quickly the camera tracks the aim point.")]
    private float camSmoothSpeed = 5f;

    private void Awake()
    {
        if (aircraft == null)
            Debug.LogError(name + "MouseFlightController - No aircraft transform assigned!");
        if (cameraRig == null)
            Debug.LogError(name + "MouseFlightController - No camera rig transform assigned!");
        if (cam == null)
            Debug.LogError(name + "MouseFlightController - No camera transform assigned!");

        transform.parent = null;
    }

    private void Update()
    {
        if (useFixed == false)
            UpdateCameraPos();
        RotateRig();
    }

    private void FixedUpdate()
    {
        if (useFixed == true)
            UpdateCameraPos();
    }

    private void RotateRig()
    {
        if (cam == null || cameraRig == null)
            return;

        cameraRig.rotation = Damp(cameraRig.rotation,
                                    Quaternion.LookRotation(aircraft.transform.forward, aircraft.transform.up),
                                    camSmoothSpeed,
                                    Time.deltaTime);
    }

    private void UpdateCameraPos()
    {
        if (aircraft != null)
            transform.position = aircraft.position;
    }

    private Quaternion Damp(Quaternion a, Quaternion b, float lambda, float dt)
    {
        return Quaternion.Slerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }

}