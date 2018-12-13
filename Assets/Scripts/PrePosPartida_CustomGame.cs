using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrePosPartida_CustomGame : MonoBehaviour {

	public GameObject _Manager;

	// Use this for initialization
	void Start () {
		
		//Nada acontece antes ou depois da partida no CustomGame;
		_Manager.GetComponent<_Manager>().prePartida = true;
		_Manager.GetComponent<_Manager>().posPartida = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
