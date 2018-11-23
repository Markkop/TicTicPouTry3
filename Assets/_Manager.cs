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
		gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
		alvosPanel.gameObject.SetActive (false);
	}
	
	void Update () {

		listaPlayers();
				//Ordem de execucao
		//Verifica se todos estao ready
		Debug.Log(gameObjectArray[0]);
		if(gameObjectArray[0].GetComponent<Atributos>().ready == false)
		{
			Debug.Log("O primeiro player nao esta ready");
			return;
		}
		//Phase 1: ve quem defende, carrega
		ResolvePhase1();
		ResolvePhase2();
		ResolvePhase3();
		ResolvePhase4();
		gameObjectArray[0].GetComponent<Atributos>().ready = false;
		//Phase 2: ve quem atira e escolhe alvo
		//Phase 3: resolve acoes
		//Phase 4: verifica vidas e remove players e desfaz ready


		
	}

	void listaPlayers()
	{
		//Popula o array com GameObjects de players
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

	void ResolvePhase1() // Verifica defesas e carregamentos
	{
		Debug.Log("Resolvendo Phase 1");
		foreach (GameObject go in gameObjectArray)
		{
			if(go.GetComponent<Atributos>().vaiDefender == true)
				{
					go.GetComponent<Atributos>().estaDefendendo = true;
				}
			if(go.GetComponent<Atributos>().vaiRecarregar == true)
				{
					if(go.GetComponent<Atributos>().balas != go.GetComponent<Atributos>().maxBalas) 
					{
						go.GetComponent<Atributos>().balas += 1;
					}
					else
					{
			
					}		
				}
		}
	}

	void ResolvePhase2() //Verifica balas
	{
		Debug.Log("Resolvendo Phase 2");
		foreach (GameObject go in gameObjectArray)
		{
			if(go.GetComponent<Atributos>().vaiAtirar == true && go.GetComponent<Atributos>().balas > 0)
			{
				//Se for atirar e tiver balas
				//ok
			}
			else
			{
				//Senao cancela o tiro
				go.GetComponent<Atributos>().vaiAtirar = false;
			}
		}

	}

	void ResolvePhase3() //Verifica tiros e defesas e resolve
	{
		Debug.Log("Resolvendo Phase 3");
		foreach (GameObject go in gameObjectArray)
		{
			if(go.GetComponent<Atributos>().vaiAtirar == true) // Atira no player0
				{
					GameObject alvo1 = go.GetComponent<Atributos>().alvo;
					if(alvo1.GetComponent<Atributos>().estaDefendendo == false)
					{
						//Se o alvo nao estiver defendendo, perde uma vida
						alvo1.GetComponent<Atributos>().vidas -= 1;
					}
				//Perde 1 bala
				go.GetComponent<Atributos>().balas -= 1;
				}
		}
	}

	void ResolvePhase4() //Verifica vidas
	{
		foreach (GameObject go in gameObjectArray)
		{
			if(go.GetComponent<Atributos>().vidas == 0)
			{
				Debug.Log("O ["+go+"] morreu");
				Destroy(go);
			}
		}

	}
}
