using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arenaCameraMoving : MonoBehaviour {

	public float angleY = 46;

	// Use this for initialization
	void Start () {

		this.GetComponent<Transform>().LookAt(new Vector3(0f,1.5f,0f));
	}
	
	// Update is called once per frame
	void Update () {


		if(angleY > 45)
		{
			angleY = angleY - Time.deltaTime*5;
		}
		if(angleY < 45)
		{
			angleY = angleY + Time.deltaTime*5;
		}
		this.GetComponent<Transform>().rotation = Quaternion.Euler(51f , angleY , -8f);
		
	}
}
