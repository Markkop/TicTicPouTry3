using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraButton : MonoBehaviour {

	public bool OnOff = false;
	public GameObject playerCamera;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () { }

	public void ChangeCamera()
	{
		if(OnOff == false)
		{
			playerCamera.SetActive(true);
			OnOff = true;
		}
		else
		{
			playerCamera.SetActive(false);
			OnOff = false;
		}	
	}
	


}
