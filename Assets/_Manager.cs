using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _Manager : MonoBehaviour {

	public GameObject[] gameObjectArray;
	public List<GameObject> gameObjectList = new List<GameObject>();
	public List<int> playersList = new List<int>();

	public GameObject player1Text;
	public GameObject player2Text;
	public GameObject player3Text;
	public GameObject alvosPanel;

	public bool allReady = false;

	void Start () {
		alvosPanel.gameObject.SetActive (false);
		gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
	}
	
	void Update () {

		listaPlayers(); //Lista os players atuais

		if(!CheckReady()) // Verifica se todos estao prontos
		{
			return; //Se retornar falso, nao faz nada
		}

		ResolvePhase1(); //Resolve defesa e recarregamento
		ResolvePhase2(); //Verifica se ha municao para atirar
		ResolvePhase3(); //Resolve ataque x defesa
		ResolvePhase4(); //Verifica vida e remove player		
	}

	void listaPlayers()
	{
		gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
		// foreach (GameObject go in gameObjectArray) // Para cada objeto no array
		// {
		// 	//Exibe os jogadores na lista de players
		// 	if(gameObjectArray[0] != null)
		// 	{
		// 		player1Text.GetComponent<Text>().text = gameObjectArray[0].ToString();
		// 	}
		// 	else
		// 	{
		// 		player1Text.GetComponent<Text>().text = "morto";
		// 	}
		// 	player2Text.GetComponent<Text>().text = gameObjectArray[1].ToString();
		// 	player3Text.GetComponent<Text>().text = gameObjectArray[2].ToString();

			// for(int i = 0; i < gameObjectArray.Length; i++)
	  //       {
   //      		go.GetComponent<Atributos>().playerId = i; // assigna o playerId (nao utilizado)
   //      		if(!playersList.Contains(i))
   //      		{
   //      			playersList.Add(i); //Bota na playerList	
   //      		}
        		
   //  		}
		}	
	

	bool CheckReady() // Verifica se todos estao prontos
	{
		for(int i = 0; i < gameObjectArray.Length; i++) //Para cada jogador i
		{
			if(gameObjectArray[i].GetComponent<Atributos>().ready == false) //Se o jogador i nao esta pronto
			{
				return false; //retorna falso e sai da funcao
			}
		}
		allReady = true; //anuncia todos prontos
		return true; //mas se nao sair, retorna true
	}

	void ResolvePhase1() // Resolve defesas e carregamentos
	{
		//Debug.Log("Resolvendo Phase 1");
		foreach (GameObject go in gameObjectArray) //Para cada jogador declarado
		{
			if(go.GetComponent<Atributos>().vaiDefender == true) //Se optou por defender 
				{
					go.GetComponent<Atributos>().estaDefendendo = true; //Entao esta defendendo
				}
			if(go.GetComponent<Atributos>().vaiRecarregar == true) //Se optou por recarregar
				{
					if(go.GetComponent<Atributos>().balas != go.GetComponent<Atributos>().maxBalas)  //e nao tiver com max de bala
					{
						Debug.Log(go.name+" carrega uma bala.");
						go.GetComponent<Atributos>().balas += 1; // ganha uma bala
					}
					else
					{
						//caso contrario, nao ganha bala.
						//Obs: o player vai gastar a propria acao
					}		
				}
		}
	}

	void ResolvePhase2() //Verifica se ha balas
	{
		//Debug.Log("Resolvendo Phase 2");
		foreach (GameObject go in gameObjectArray) //Para cada jogador
		{
			if(go.GetComponent<Atributos>().vaiAtirar == true) //Se for atirar & tiver bala
			{
				if(go.GetComponent<Atributos>().balas > 0)
				{
					//segue o baile
				}
				else
				{
					Debug.Log(go.name+" tentou atirar, mas esta sem bala");
					go.GetComponent<Atributos>().vaiAtirar = false;
				}
			}
		}

	}

	void ResolvePhase3() //Resolve ataque e defesa
	{
		//Debug.Log("Resolvendo Phase 3");
		foreach (GameObject go in gameObjectArray) // Para cada jogador
		{
			if(go.GetComponent<Atributos>().vaiAtirar == true) // que estiver atirando
				{
					GameObject alvo1 = go.GetComponent<Atributos>().alvo; //pega alvo do jogador
					if(alvo1.GetComponent<Atributos>().estaDefendendo == false) //se o alvo NAO estiver defendno
					{
						alvo1.GetComponent<Atributos>().vidas -= 1; //perde uma vida
						Debug.Log(go.name+" atira em "+alvo1.name+" que perde uma vida");
					}
					else
					{
						Debug.Log(go.name+" atira em "+alvo1.name+" que se defende");
					}
				go.GetComponent<Atributos>().balas -= 1; // remove uma bala (se o alvo defender ou nao)				}
				}
		}	
	}

	void ResolvePhase4() //Verifica vidas
	{
		//Debug.Log("Resolvendo Phase 4");

		foreach (GameObject go in gameObjectArray) // Para cada jogador
		{
			if(go.GetComponent<Atributos>().vidas == 0) // se nao tiver vida 
			{
				Debug.Log("O ["+go.name+"] morreu");
				Destroy(go); //destroy o gameObject do jogador
			}
		//Reseta as acoes
		go.GetComponent<Atributos>().ready = false;
		go.GetComponent<Atributos>().vaiAtirar = false;
		go.GetComponent<Atributos>().vaiRecarregar = false;
		go.GetComponent<Atributos>().vaiDefender = false;
		go.GetComponent<Atributos>().estaDefendendo = false;
		go.GetComponent<Atributos>().alvo = null;

		if (gameObjectArray.Length == 1)
		{
			//NetworkManager.StopClient
			Debug.Log("FIM DE JOGO");
		}

		}

	}
}
