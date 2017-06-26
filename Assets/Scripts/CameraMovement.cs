using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {


	public GameObject Car;
    private Vector3 _Distance;
	// Use this for initialization
	void Start () {
        _Distance = transform.position - Car.transform.position;
	}
	
    private void LateUpdate()
    {
        var dif = Car.transform.position + _Distance;
        //dif.y = gameObject.transform.position.y;
        gameObject.transform.position = dif;
    }

}
