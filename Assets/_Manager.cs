using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Manager : MonoBehaviour {

	public GameObject[] gameObjectArray;
	public List<GameObject> gameObjectList = new List<GameObject>();
	public List<int> intList = new List<int>();

	public GameObject player1Text;
	public GameObject player2Text;
	public GameObject player3Text;

	void Start () {

		listaPlayers();
		
	}
	
	void Update () {

		listaPlayers();
		
	}

	void listaPlayers()
	{
		gameObjectArray = GameObject.FindGameObjectsWithTag("Player"); //Popula o array com GameObjects de players
		//gameObjectList = gameObjectArray.ToList(); // Transforma o array em list

		foreach (GameObject go in gameObjectArray) // Para cada objeto no array
		{
			Debug.Log("Tamanho do array gameObjectArray: "+gameObjectArray.Length);
			for(int i = 0; i < gameObjectArray.Length; i++)
	        {
        		go.GetComponent<Atributos>().playerId = i; // assigna o playerId
    		}
	}	}
}
