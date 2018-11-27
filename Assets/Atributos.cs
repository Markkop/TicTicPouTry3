using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Atributos : MonoBehaviour {

	public int playerId;
	public int classe; //0 = normal, 1 = noviço, 2 = mago, 3 = padre...
	public bool vaiAtirar = false;
	public bool vaiDefender = false;
	public bool estaDefendendo;
	public bool vaiRecarregar = false;
	public GameObject alvo;

	public int vidas = 1;
	public int balas = 1;

	public bool ready = false;

	public int maxBalas = 1;

	public GameObject alvosPanel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void QuerDefender()
	{
		if(vaiDefender == false)
		{
			vaiDefender = true;
			vaiRecarregar = false;
			vaiAtirar = false;
			//Debug.Log("Player ["+playerId+"] vai defender? ["+vaiDefender+"]");
		}
		else
		{
			vaiDefender = false;
			//Debug.Log("Player ["+playerId+"] vai defender? ["+vaiDefender+"]");
		}
	}

	public void QuerRecarregar()
	{
		if(vaiRecarregar == false)
		{
			vaiRecarregar = true;
			vaiDefender = false;
			vaiAtirar = false;
			//Debug.Log("Player ["+playerId+"] vai recarregar? ["+vaiRecarregar+"]");
		}
		else
		{
			vaiRecarregar = false;
			//Debug.Log("Player ["+playerId+"] vai recarregar? ["+vaiRecarregar+"]");
		}
	}

	public void QuerAtirar()
	{
		if(vaiAtirar == false)
		{
			vaiAtirar = true;
			vaiRecarregar = false;
			vaiDefender = false;
			//Debug.Log("Player ["+playerId+"] vai atirar? ["+vaiAtirar+"]");
			alvosPanel.gameObject.SetActive (true);

		}
		else
		{
			vaiAtirar = false;
			//Debug.Log("Player ["+playerId+"] vai atirar? ["+vaiAtirar+"]");
		}
	}

	public void IsReady()
	{
		if(ready == false)
		{
			alvosPanel.gameObject.SetActive (false);
			ready = true;
			//Debug.Log("Player ["+playerId+"] esta Ready");
		}
		else
		{
			ready = false;
			//Debug.Log("Player ["+playerId+"] nao esta Ready");
		}
	}

	public void EscolheAlvo(int player)
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); //Botas os players num array

		for(int i = 0; i < players.Length; i++) 
		{	//Se o play
			if(player == i)
			{
				alvo = players[i]; //Da o gameObject indexado de acordo com o botao clicado
			}
		} 
		
	}
}
