using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atributos : MonoBehaviour {

	public int playerId;
	public int classe; //0 = normal, 1 = noviço, 2 = mago, 3 = padre...
	public bool vaiAtacar = false;
	public bool vaiDefende = false;
	public bool vaiRecarr = false;
	public int alvo;

	public int vidas = 2;
	public int balas = 0;

	public bool ready = false;

	public int maxBalas = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Defende()
	{
		if(defende == false)
		{
			defende = true;
			Debug.Log("Player ["+playerId+"] setou defesa como ["+defende+"]");
		}
		else
		{
			defende = false;
			Debug.Log("Player ["+playerId+"] setou defesa como ["+defende+"]");
		}
	}

	public void Recarrega()
	{
		if(balas != maxBalas)
		{
			balas += 1;
			Debug.Log("Player ["+playerId+"] recarregou e tem ["+balas+"]");
		}
		else
		{
			Debug.Log("Player ["+playerId+"] tem maxBalas");
		}
	}

	public void Ready()
	{
		if(ready == false)
		{
			ready = true;
			Debug.Log("Player ["+playerId+"] esta Ready");
		}
		else
		{
			defende = false;
			Debug.Log("Player ["+playerId+"] nao esta Ready");
		}
	}
}
