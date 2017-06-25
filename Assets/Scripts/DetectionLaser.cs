using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionLaser : MonoBehaviour {

    public float MaxDistance = 20;
    public float Distance = 0;
    private LineRenderer _LR;

	// Use this for initialization
	void Start () {
        _LR = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        _LR.SetPosition(0, transform.position);
  
        if (Physics.Raycast(transform.position, transform.forward.normalized, out hit, MaxDistance))
        {
            _LR.SetPosition(1, hit.point);
            Distance = Vector3.Distance(gameObject.transform.position, hit.point);
        }
        else
        {
            _LR.SetPosition(1, transform.position + transform.forward.normalized * MaxDistance);
        }
	}
}
