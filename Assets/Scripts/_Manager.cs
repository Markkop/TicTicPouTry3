using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class _Manager : NetworkBehaviour {

	public GameObject[] gameObjectArray;
	public List<GameObject> gameObjectList = new List<GameObject>();
	public List<int> playersList = new List<int>();
	public GameObject[] goMorto;

	public bool allReadyManager = false;
	public GameObject[] toggles;

	static public bool haveSpawnedManager = false;

	public GameObject ManagerPrefab;

	public bool fimDeJogo = false;

	int nomesIndex = 0;

	void Start () {

		gameObjectArray = GameObject.FindGameObjectsWithTag("Player");
	}
	
	void Update () {

		//Soh roda no servidor
		if(!isServer)
		{
			return;
		}

		//Debug de coisas
		if( Input.GetKeyDown("2") )
		{
			foreach(GameObject go in gameObjectArray)
			{
				go.GetComponent<Atributos>().newName = "trxao";
				//go.name = "troxa";
			}
		}
		
		if(fimDeJogo)
		{
			return;
		}

		CmdMudaNome2();
		CheckReady();

		if(!allReadyManager) // Verifica se todos estao prontos
		{
			return; //Se retornar falso, nao faz nada
		}

		ResolvePhase1(); //Resolve defesa e recarregamento
		ResolvePhase2(); //Verifica se ha municao para atirar
		ResolvePhase3(); //Resolve ataque x defesa
		ResolvePhase4(); //Verifica vida e remove player		
	}


	//O unico comando era de manter o gameObjectArray atualizado com os players para caso 
	//entre novos players ou bots durante a partida (pelo menos para debug), mas houve um
	//problema que os Clients nao tinham seu nome atualizado (mas newName sim), possivelmente
	//por conta de alguma ordem de execuçao, tipo entra o host+client, seu nome eh atualizado
	//e avisado aos outros clients (nenhum) pelo Hook, entao um segundo player (client) se conecta,
	//mas ele nao foi avisado da mudanca de nome do primeiro player.
	[Command]
	void CmdlistaPlayers() //Nao esta sendo utilizada
	{
		gameObjectArray = GameObject.FindGameObjectsWithTag("Player");

		//Para cada jogador
		foreach(GameObject go in gameObjectArray)
		{
			string oldGoName = go.name;
			string newGoName = "Jogador "+nomesIndex;

			//Verifica se esta sem nome
			if(go.GetComponent<Atributos>().newName == "")
			{
				Debug.Log("Mudando o nome de "+oldGoName+" para "+newGoName+" ");

				go.GetComponent<Atributos>().newName = newGoName;
				go.name = go.GetComponent<Atributos>().newName;

				//Verifica se algum jogador ativo (exceto ele mesmo) ja possui este nome
				foreach (GameObject go2 in gameObjectArray)
				{
					if(go.name == go2.name && go2 != go) 
					{
						nomesIndex++; 
						newGoName = "Jogador "+nomesIndex;

						Debug.Log(go.name+" na verdade muda nome para "+newGoName);
						go.GetComponent<Atributos>().newName = newGoName; 
						go.name = go.GetComponent<Atributos>().newName;

						RpcChangePlayerName(go, go.GetComponent<Atributos>().newName);
					}
				}
			}


		}
	}


	//Esta eh uma versao levemente modificada da de cima que ao invez de renomear quem tentou pegar
	//um nome repetido, renomeia quem ja tinha o nome definido. Assim o hook de newName em Atributos
	//eh acionado toda vez que alguem muda de nome (pois todos mudam). (baita workaround)
	[Command]
	void CmdMudaNome2()
	{
		gameObjectArray = GameObject.FindGameObjectsWithTag("Player");

		//Para cada jogador
		foreach(GameObject go in gameObjectArray)
		{
			string oldGoName = go.name;
			string newGoName = "Jogador "+nomesIndex;

			//Verifica se esta sem nome
			if(go.GetComponent<Atributos>().newName == "")
			{
				Debug.Log("Mudando o nome de "+oldGoName+" para "+newGoName+" [NOVO]");

				go.GetComponent<Atributos>().newName = newGoName;
				go.name = go.GetComponent<Atributos>().newName;

				//Verifica se algum jogador ativo (exceto ele mesmo) ja possui este nome
				foreach (GameObject go2 in gameObjectArray)
				{
					if(go.name == go2.name && go2 != go) 
					{
						nomesIndex++; 
						newGoName = "Jogador "+nomesIndex;

						Debug.Log("Antigo" +go2.name+" na verdade muda nome para "+newGoName);
						go2.GetComponent<Atributos>().newName = newGoName; 
						go2.name = go2.GetComponent<Atributos>().newName;

						//RpcChangePlayerName(go, go.GetComponent<Atributos>().newName);
					}
				}
			}


		}
	}

	//Por algum motivo esse (e outros) ClientRpc nao estao funcionando corretamente.
	//No lugar de chamar essa funcao, utilizei hook em newName de Atributos.
	[ClientRpc]
    void RpcChangePlayerName(GameObject go, string n)
    {
		Debug.Log("Avisando todos os clientes da mudanca de nome do "+go.name+" para "+n);
		go.GetComponent<Atributos>().newName = n;
		go.name = n;
    }


		 	
	

	void CheckReady() // Verifica se todos estao prontos
	{
		for(int i = 0; i < gameObjectArray.Length; i++) //Para cada jogador i
		{
			if(gameObjectArray[i].GetComponent<Atributos>().ready == false) //Se o jogador i nao esta pronto
			{
				Debug.Log("Aguardando todos prontos...");
				allReadyManager = false; //retorna falso e sai da funcao
				return;
			}
		}
		Debug.Log("Todos prontos...");
		allReadyManager = true; //anuncia todos prontos
		return; //mas se nao sair, retorna true
	}

	void ResolvePhase1() // Resolve defesas e carregamentos
	{
		//Debug.Log("Resolvendo Phase 1");
		foreach (GameObject go in gameObjectArray) //Para cada jogador declarado
		{	
			go.GetComponent<Atributos>().allReady = true; // (aproveita e confirma a todos que todos estao prontos)
			if(go.GetComponent<Atributos>().vaiDefender == true) //Se optou por defender 
				{
					go.GetComponent<Atributos>().estaDefendendo = true; //Entao esta defendendo
				}
			if(go.GetComponent<Atributos>().vaiRecarregar == true) //Se optou por recarregar
				{
					if(go.GetComponent<Atributos>().balas != go.GetComponent<Atributos>().maxBalas)  //e nao tiver com max de bala
					{
						Debug.Log(go.name+" carrega uma bala...");
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
		foreach (GameObject go in gameObjectArray) //Para cada jogador...
		{
			if(go.GetComponent<Atributos>().vaiAtirar == true) //Se for atirar...
			{
				if(go.GetComponent<Atributos>().alvo != null) //e tiver escolhido um alvo..
				{
					if(go.GetComponent<Atributos>().balas > 0)//e tiver bala
					{
						//segue o baile
					}
					else
					{
						Debug.Log(go.name+" tentou atirar, mas esta sem bala");
						go.GetComponent<Atributos>().vaiAtirar = false;
					}
				}
				else
				{
					Debug.Log(go.name+" tentou atirar, mas nao escolheu um alvo");
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
						if(!alvo1.GetComponent<Atributos>().levouTiro) // Para levar apenas um tiro no max
						{
							alvo1.GetComponent<Atributos>().levouTiro = true;
							alvo1.GetComponent<Atributos>().vidas -= 1; //perde uma vida
						}

						Debug.Log(go.name+" atira em "+alvo1.name+" que perde uma vida...");
					}
					else
					{
						Debug.Log(go.name+" atira em "+alvo1.name+" que se defende...");
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
			go.GetComponent<Atributos>().levouTiro = false;
			if(go.GetComponent<Atributos>().vidas == 0) // se nao tiver vida 
			{
				Debug.Log("O ["+go.name+"] morreu");
				//go.GetComponent<BoxCollider>().enabled = true;
				//go.GetComponent<Transform>().position = new Vector3(0,0,0);
				//go.GetComponent<Rigidbody>().AddRelativeForce(transform.forward * -4000);
				go.name = "Morto";
				//go.tag = "Untagged"; //Esta crashando quando remove a tag
			}

		//Reseta as acoes
		go.GetComponent<Atributos>().ready = false;
		go.GetComponent<Atributos>().vaiAtirar = false;
		go.GetComponent<Atributos>().vaiRecarregar = false;
		go.GetComponent<Atributos>().vaiDefender = false;
		go.GetComponent<Atributos>().estaDefendendo = false;
		go.GetComponent<Atributos>().alvo = null;

		if(go.GetComponent<botIA>() == null) //Se nao for um bot
			go.GetComponent<ButtonCreator>().Destroi(); //Destroi os botoes de alvos

		//if(isLocalPlayer)
			//go.GetComponent<Atributos>().alvosPanel.gameObject.SetActive (false);

		//Debug.Log("Fim do Resolve4");

		}

		int a = 0;
		foreach (GameObject go in gameObjectArray)
		{	
			
			if(go.name == "Morto")
			{
				a++;
			}

		}
		if (a == gameObjectArray.Length - 1)
		{
			//NetworkManager.StopClient
			Debug.Log("FIM DE JOGO");
			fimDeJogo = true;
		}



	}
}
