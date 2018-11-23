using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Manager : MonoBehaviour {

	public GameObject[] gameObjectArray;
	public List<GameObject> gameObjectList = new List<GameObject>();
	public List<int> playersList = new List<int>();

	public GameObject player1Text;
	public GameObject player2Text;
	public GameObject player3Text;
	public GameObject alvosPanel;

	void Start () {
		
	}
	
	void Update () {

		listaPlayers();
		alvosPanel.gameObject.SetActive (false);

		//Ordem de execucao
		//Verifica se todos estao ready
		//Phase 1: ve quem defende, carrega
		ResolvePhase1();
		//Phase 2: ve quem atira e escolhe alvo
		//Phase 3: resolve acoes
		//Phase 4: verifica vidas e remove players e desfaz ready


		
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
        		if(!playersList.Contains(i))
        		{
        			playersList.Add(i); //Bota na playerList	
        		}
        		
    		}
		}	
	}

	void ResolvePhase1()
	{

	}
}
