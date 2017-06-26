using UnityEngine;
using System.Collections;

public class RearWheelDrive : MonoBehaviour {

	private WheelCollider[] wheels;

	public float maxAngle = 30;
	public float maxTorque = 300;
    public float maxSpeed = 10f;
	public GameObject wheelShape;
    public float WheelScale = 1;
    public float Tourque = 0;
    public float SteeringAngle = 0;
    public float Speed = 0;
    public bool InputControl = true;
	// here we find all the WheelColliders down in the hierarchy
	public void Start()
	{
		wheels = GetComponentsInChildren<WheelCollider>();
        wheelShape.transform.localScale = new Vector3(WheelScale, WheelScale, WheelScale);

        for (int i = 0; i < wheels.Length; ++i) 
		{
			var wheel = wheels [i];

			// create wheel shapes only when needed
			if (wheelShape != null)
			{
				var ws = GameObject.Instantiate (wheelShape);
				ws.transform.parent = wheel.transform;
			}
		}
	}

	// this is a really simple approach to updating wheels
	// here we simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero
	// this helps us to figure our which wheels are front ones and which are rear
	public void Update()
	{
        var rigid = gameObject.GetComponent<Rigidbody>();
        Speed = rigid.velocity.magnitude;

        if (Speed > maxSpeed) {
            rigid.velocity = rigid.velocity.normalized * maxSpeed;
        }

        if (InputControl)
        {
            Tourque = Input.GetAxis("Vertical");
            SteeringAngle = Input.GetAxis("Horizontal");
        }

        float angle = maxAngle * SteeringAngle; //Input.GetAxis("Horizontal");
		float torque = maxTorque * Tourque;//Input.GetAxis("Vertical");

		foreach (WheelCollider wheel in wheels)
		{
			// a simple car where front wheels steer while rear ones drive
			if (wheel.transform.localPosition.z > 0)
				wheel.steerAngle = angle;

			if (wheel.transform.localPosition.z < 0)
				wheel.motorTorque = torque;

			// update visual wheels if any
			if (wheelShape) 
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose (out p, out q);

				// assume that the only child of the wheelcollider is the wheel shape
				Transform shapeTransform = wheel.transform.GetChild (0);
				shapeTransform.position = p;
				shapeTransform.rotation = q;
			}

		}
	}
}
