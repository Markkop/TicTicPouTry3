using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeAnimation : MonoBehaviour {

	public Animator anim;

	// Use this for initialization
	void Start () {

		//anim = GetComponent<Animator>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
        //Press the space key to play the "Jump" state
        if (Input.GetKey(KeyCode.Q))
        {
        	Debug.Log("AAA");
            anim.Play("WIN00");
        }

	}
}
