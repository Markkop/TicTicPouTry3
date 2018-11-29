using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class playerInfo : NetworkBehaviour {

	public GameObject vidasText;
	public GameObject player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		vidasText.GetComponent<Text>().text =""+this.GetComponent<Atributos>().vidas.ToString()
											+" // "+this.GetComponent<Atributos>().balas.ToString()
											+"\n"+this.name;
		
	}
}
