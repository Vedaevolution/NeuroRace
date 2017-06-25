using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {


	public GameObject Car;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var dif = Vector3.Lerp(gameObject.transform.position, Car.transform.position, 1 * Time.deltaTime);
        dif.y = gameObject.transform.position.y;
        gameObject.transform.position = dif;
	}
		
}
