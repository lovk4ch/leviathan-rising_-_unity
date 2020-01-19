using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    private class WheelData
    {
        public Transform wheelTransform;
        public WheelCollider col;
        public Vector3 wheelStartPos;
        public float rotation = 0.0f;
    }

    private WheelData[] wheels;

    public WheelCollider[] WColForward;
    public WheelCollider[] WColBack;

    public Transform[] wheelsF;
    public Transform[] wheelsB;

    public float wheelOffset = 0.1f;
    public float wheelRadius = 0.13f;

    public float maxSteer = 30;
    public float maxAccel = 25;
    public float maxBrake = 50;

    public Transform centerOfMass;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
        wheels = new WheelData[WColForward.Length + WColBack.Length];

        for (int i = 0; i < WColForward.Length; i++)
        {
            wheels[i] = SetupWheels(wheelsF[i], WColForward[i]);
        }

        for (int i = 0; i < WColBack.Length; i++)
        {
            wheels[i + WColForward.Length] = SetupWheels(wheelsB[i], WColBack[i]);
        }
    }

    private WheelData SetupWheels(Transform wheel, WheelCollider col)
    {
        WheelData result = new WheelData
        {
            wheelTransform = wheel,
            col = col,
            wheelStartPos = wheel.transform.localPosition
        };

        return result;
    }

    private void FixedUpdate()
    {
        Move(InputManager.Instance.GetAxis(InputManager.VERTICAL_AXIS),
            InputManager.Instance.GetAxis(InputManager.HORIZONTAL_AXIS));
        UpdateWheels();
    }

    private void UpdateWheels()
    {
        float delta = Time.fixedDeltaTime;

        foreach (WheelData w in wheels)
        {
            w.rotation = Mathf.Repeat(w.rotation + delta * w.col.rpm * 360.0f / 60.0f, 360.0f);
            w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, w.col.steerAngle, 90.0f);
        }
    }

    public void Move(float accel, float steer)
    {
        foreach (WheelCollider col in WColForward)
        {
            col.steerAngle = steer * maxSteer;
        }

        if (accel == 0)
        {
            foreach (WheelCollider col in WColBack)
            {
                col.brakeTorque = maxBrake;
            }
        }
        else
        {
            foreach (WheelCollider col in WColBack)
            {
                col.brakeTorque = 0;
                col.motorTorque = accel * maxAccel;
            }
        }
    }
}