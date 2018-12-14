using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class playerInfo : NetworkBehaviour {

	public GameObject vidasText;
	public GameObject player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		vidasText.GetComponent<TextMeshProUGUI>().text =this.GetComponent<Atributos>().vidas.ToString()
											+" // "+this.GetComponent<Atributos>().balas.ToString()
											+" // "+this.GetComponent<Atributos>().espCargas.ToString()
											+"\n"+this.name;
		
	}
}
