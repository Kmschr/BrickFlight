using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Plane : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private FlightController controller = null;
    [SerializeField] private Transform rudder = null;
    [SerializeField] private Transform prop = null;
    [SerializeField] private Transform elevators = null;
    [SerializeField] private Transform aileronLeft = null;
    [SerializeField] private Transform aileronRight = null;

    [Header("Physics")]
    [Tooltip("Force to push plane forwards with")] public float thrust = 100f;
    [Tooltip("Pitch, Yaw, Roll")] public Vector3 turnTorque = new Vector3(90f, 100f, 45f);
    [Tooltip("Multiplier for all forces")] public float forceMult = 1000f;

    [Header("Autopilot")]
    [Tooltip("Sensitivity for autopilot flight.")] public float sensitivity = 5f;
    [Tooltip("Angle at which airplane banks fully into target.")] public float aggressiveTurnAngle = 10f;

    [Header("Input")]
    [SerializeField] [Range(-1f, 1f)] private float pitch = 0f;
    [SerializeField] [Range(-1f, 1f)] private float yaw = 0f;
    [SerializeField] [Range(-1f, 1f)] private float roll = 0f;
    [SerializeField] private float forwardVelocity = 0;

    public float Pitch { set { pitch = Mathf.Clamp(value, -1f, 1f); } get { return pitch; } }
    public float Yaw { set { yaw = Mathf.Clamp(value, -1f, 1f); } get { return yaw; } }
    public float Roll { set { roll = Mathf.Clamp(value, -1f, 1f); } get { return roll; } }

    private Rigidbody rigid;

    private bool isPaused = false;
    private float thrustBeforePause = 0f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        if (controller == null)
            Debug.LogError(name + ": Plane - Missing reference to MouseFlightController!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                thrustBeforePause = thrust;
                thrust = 0;
                rigid.velocity = Vector3.zero;
                rigid.useGravity = false;
            } else
            {
                thrust = thrustBeforePause;
                rigid.useGravity = true;
            }
        }

        roll = Input.GetAxis("Horizontal");
        pitch = Input.GetAxis("Vertical");
        yaw = Input.GetAxisRaw("Yaw");

        if (Input.GetKey(KeyCode.LeftControl))
            thrust -= 0.1f;
        if (Input.GetKey(KeyCode.LeftShift))
            thrust += 0.1f;
        thrust = Mathf.Clamp(thrust, 0f, 100f);

        if (!isPaused)
        {
            rudder.localRotation = Quaternion.Euler(0, -yaw * 25, 0);
            elevators.localRotation = Quaternion.Euler(-pitch * 55, 0, 0);
            aileronLeft.localRotation = Quaternion.Euler(-roll * 45, 0, 0);
            aileronRight.localRotation = Quaternion.Euler(roll * 45, 0, 0);
            prop.Rotate(new Vector3(0, 0, thrust));
        }
    }

    private void FixedUpdate()
    {
        forwardVelocity = Vector3.Dot(rigid.velocity, rigid.transform.forward);

        float turningForce = (-5 * Mathf.Pow((forwardVelocity / 150f) - 5f, 2) + 125) / 25;

        if (isPaused)
            turningForce = 1;

        rigid.AddRelativeForce(Vector3.forward * thrust * forceMult, ForceMode.Force);
        rigid.AddRelativeTorque(new Vector3(turnTorque.x * pitch,
                                            turnTorque.y * yaw,
                                            -turnTorque.z * roll) * forceMult * turningForce,
                                ForceMode.Force);
    }
}
