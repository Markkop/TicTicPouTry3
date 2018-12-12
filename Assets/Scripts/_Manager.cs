using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

public class _Manager : NetworkBehaviour {

	//public GameObject[] playersArray;
	//public List<GameObject> gameObjectList = new List<GameObject>();
	//public List<int> playersList = new List<int>();
	public List<GameObject> playersArray = new List<GameObject>();
	public GameObject[] goMorto;

	public bool allReadyManager = false;
	public GameObject[] toggles;

	static public bool haveSpawnedManager = false;

	public GameObject ManagerPrefab;

	public bool fimDeJogo = false;
	public GameObject winner;
	public GameObject overviewCamera;

	public bool primeiroTurno = false;
	public bool fimDeTurno = false;
	public bool startReadyManager = false;
	public int rodada = 0;


	int nomesIndex = 0;
	float timeLeft = 5;
	float timeRitmo = 5;
	float timeLeft2 = 2;


	void Start () {


		//playersArray = GameObject.FindGameObjectsWithTag("Player");
	}
	
	void Update () {

		//Atualiza o playersArray e manda para todos os players 
		//Ficar ligado que isso pode causar lag
		//playersArray = GameObject.FindGameObjectsWithTag("Player");


		foreach(GameObject player in playersArray)
		{
			//player.GetComponent<Atributos>().playersArray = playersArray;
		}



		//Soh roda no servidor
		if(!isServer)
		{
			return;
		}

		//Debug de coisas
		if( Input.GetKeyDown("2") )
		{
			foreach(GameObject go in playersArray)
			{
				go.GetComponent<Atributos>().newName = "trxao";
				//go.name = "troxa";
			}
		}
		
		//Caso tenha acabado o jogo
		if(fimDeJogo)
		{
			foreach(GameObject player in playersArray)
			{
				if(player.GetComponent<botIA>() == null)
				{
					player.GetComponent<Atributos>().playerCamera.SetActive(false);
				}
			}

			//Timer de 5 segundos
			timeLeft -= Time.deltaTime;
			if(timeLeft < 0)
			{
				Debug.Log("Desconectando e resetando scene...");
				SceneManager.LoadScene(0);
				NetworkManager.singleton.StopClient();
				NetworkManager.singleton.StopHost();
				NetworkManager.singleton.StopServer();
				return;
				
			}
			else
			{
				//Afastar camera
				overviewCamera.GetComponent<Transform>().position = overviewCamera.GetComponent<Transform>().position + new Vector3(-Time.deltaTime,Time.deltaTime,-Time.deltaTime);
				Debug.Log("Fim de jogo. "+Mathf.Round(timeLeft)+" segundos para encerrar...");
				return;
			}
		}

		CmdMudaNome2();	

		if(!startReadyManager) // Verifica se todos estao prontos
		{
			// Caso a partida ainda nao tenha começado, posiciona os jogadores
			if(playersArray.Count != 0 && !primeiroTurno)
			{
				RpcPosicionaPlayers();
				CheckReady();	
			}
			
			return; //Se nao estiverem prontos, nao continua a rotina.
		}
		else
		{	
			//Quando todos os jogadores estiverem prontos pela primeira vez, ativa o primeiro Turno
			//para que eles não fiquem sendo posicionados toda vez.
			if(!primeiroTurno)
			{
				primeiroTurno = true;	
			}
		}

		//Contador para definir o Ritmo do jogo, exibindo as bolinhas de timer pros jogadores
		//e iniciando as resolucoes na bolinha maior (bola3)
		if(!allReadyManager)
		{
			timeRitmo -= Time.deltaTime;
			RpcRitmo();
			return;
		}

		//Para que o jogo aguarde 2 segundos antes de recomeçar a próxima rodada, adicionou-se o seguinte procedimento
		if(!fimDeTurno)
		{
			Debug.Log("Iniciando rodada "+rodada);
			ResolvePhase1(); //Resolve defesa e recarregamento
			ResolvePhase2(); //Verifica se ha municao para atirar
			ResolvePhase3(); //Resolve ataque x defesa
			ResolvePhase4(); //Verifica vida e remove player
		}
		else
		{
			timeLeft2 -= Time.deltaTime;
			if (timeLeft2 < 0)
			{
				//Desativas as bolinhas de timer
				foreach (GameObject go in playersArray)
				{
					if(go.GetComponent<NetworkIdentity>().isLocalPlayer)
					{
						go.GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola1").gameObject.SetActive(false);
						go.GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola2").gameObject.SetActive(false);
						go.GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola3").gameObject.SetActive(false);
					}
				}

				//Reseta os contadores
				timeRitmo = 5;
				allReadyManager = false;
				timeLeft2 = 2;
				fimDeTurno = false;
				rodada = rodada +1;
			}
		}
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
		//playersArray = GameObject.FindGameObjectsWithTag("Player");

		//Para cada jogador
		foreach(GameObject go in playersArray)
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
				foreach (GameObject go2 in playersArray)
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
		//playersArray = GameObject.FindGameObjectsWithTag("Player");

		//Para cada jogador
		foreach(GameObject go in playersArray)
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
				foreach (GameObject go2 in playersArray)
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
		if(playersArray.Count <= 1)
		{	
			Debug.Log("Aguardando mais jogadores...");
			return;
		}

		foreach(GameObject player in playersArray) //Para cada jogador 
		{
			//Debug.Log("Verificando se "+player.name+" esta pronto...");
			if(player.GetComponent<Atributos>().ready == false) //Se o jogador nao esta pronto
			{
				Debug.Log("Aguardando todos prontos para comecar a partida...");
				startReadyManager = false; //retorna falso e sai da funcao
				return;
			}
		}
		Debug.Log("Todos prontos...");
		startReadyManager = true; //anuncia todos prontos
		return;
	}

	void ResolvePhase1() // Resolve defesas e carregamentos
	{
		//Debug.Log("Resolvendo Phase 1");
		foreach (GameObject go in playersArray) //Para cada jogador declarado
		{	
			go.GetComponent<Atributos>().allReady = true; // (aproveita e confirma a todos que todos estao prontos)
			if(go.GetComponent<Atributos>().vaiDefender == true) //Se optou por defender 
				{
					if(go.GetComponent<NetworkIdentity>().isLocalPlayer)
						go.GetComponent<Animator>().SetTrigger("Defende");

					go.GetComponent<Atributos>().estaDefendendo = true; //Entao esta defendendo
				}
			if(go.GetComponent<Atributos>().vaiRecarregar == true) //Se optou por recarregar
				{
					if(go.GetComponent<NetworkIdentity>().isLocalPlayer)
						go.GetComponent<Animator>().SetTrigger("Recarrega");

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
		foreach (GameObject go in playersArray) //Para cada jogador...
		{
			if(go.GetComponent<Atributos>().vaiAtirar == true) //Se for atirar...
			{
				if(go.GetComponent<NetworkIdentity>().isLocalPlayer)
				go.GetComponent<Animator>().SetTrigger("Atira");

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
		foreach (GameObject go in playersArray) // Para cada jogador
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
							if(alvo1.GetComponent<NetworkIdentity>().isLocalPlayer) alvo1.GetComponent<Animator>().SetTrigger("gotShot2");

							if(alvo1.GetComponent<Atributos>().vidas == 0)
							{
								alvo1.GetComponent<Atributos>().mortoPor = go;
							}
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
		foreach (GameObject go in playersArray) // Para cada jogador
		{
			go.GetComponent<Atributos>().levouTiro = false;
			if(go.GetComponent<Atributos>().vidas == 0 && go.name != "Morto") // se nao tiver vida 
			{
				Debug.Log("O ["+go.name+"] morreu");
				go.GetComponent<Atributos>().newName = "Morto";
				//go.GetComponent<BoxCollider>().enabled = true;
				//go.GetComponent<Transform>().position = new Vector3(0,0,0);
				//go.GetComponent<Rigidbody>().AddRelativeForce(transform.forward * -4000);
				//go.name = "Morto";
				//go.tag = "Untagged"; //Esta crashando quando remove a tag
			}

		//Reseta as acoes
		//go.GetComponent<Atributos>().ready = false;
		go.GetComponent<Atributos>().vaiAtirar = false;
		go.GetComponent<Atributos>().vaiRecarregar = false;
		go.GetComponent<Atributos>().vaiDefender = false;
		go.GetComponent<Atributos>().estaDefendendo = false;
		go.GetComponent<Atributos>().alvo = null;

		fimDeTurno = true;
		//Debug.Log("Fim do Resolve4");

		}

		int playersMortos = 0;
		foreach (GameObject go in playersArray)
		{	
			
			if(go.name == "Morto")
			{
				playersMortos++;
			}

		}

		//Se houver apenas um jogador vivo
		if (playersMortos == playersArray.Count - 1)
		{
			
			foreach(GameObject player in playersArray)
			{
				if(player.GetComponent<Atributos>().vidas > 0)
				{
					//Atribui vitoria ao player vencedor
					player.GetComponent<Atributos>().playerVencedor = true;
					winner = player;
					if(winner.GetComponent<NetworkIdentity>().isLocalPlayer) winner.GetComponent<Animator>().SetTrigger("winner");

				}
			}
			//NetworkManager.StopClient
			Debug.Log("||FIM DE JOGO|| . Vencedor: "+winner.name);

			fimDeJogo = true;
		}

		if (playersMortos == playersArray.Count )
		{
			Debug.Log("||FIM DE JOGO|| . Ninguem ganhou");

			fimDeJogo = true;			
		}



	}

	//Posiciona os jogadores em espaçamentos iguais em um circulo
	[ClientRpc]
	void RpcPosicionaPlayers()
	{
		float angulo = 360/playersArray.Count;
		//Debug.Log("Angulo:"+angulo);

		for(int i = 0; i < playersArray.Count; i++)
		{
			float altura = playersArray[i].GetComponent<Collider>().bounds.max[1];
			float y = altura/2;

			if(playersArray[i].GetComponent<botIA>() == null)
			{
				//Gambiarra pra arrumar depois que tiver modelos pra player e players definidos
				y = 0;
			}

			float x = 8*Mathf.Cos(i*angulo*3.14f/180f);
			float z = 8*Mathf.Sin(i*angulo*3.14f/180f);
			playersArray[i].GetComponent<Transform>().position = new Vector3(x,y,z);

			playersArray[i].GetComponent<Transform>().LookAt(new Vector3(0f,playersArray[i].GetComponent<Transform>().position[1],0f));

			//Debug.Log("Posicao do "+playersArray[i]+"em x: "+x+" e em z: "+z);
		}
	}

	//Mesmo com Rpc, essa funcao nao esta ativando as bolas do Ritmo nos Clients, apenas no server
	//O Ritmo ainda funciona, só nao aparece visualmente
	[ClientRpc]
	void RpcRitmo()
	{
		//Debug.Log("Entrando na funcao ritmo");
		if(timeRitmo < 4)
		{
			//Debug.Log("Ritmo = 4");
			foreach (GameObject player in playersArray)
			{
				if(player.GetComponent<botIA>() == null)
				{
					player.GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola1").gameObject.SetActive(true);
				}
			}			
		}
		if(timeRitmo < 3)
		{
			//Debug.Log("Ritmo = 3");
			foreach (GameObject player in playersArray)
			{
				if(player.GetComponent<NetworkIdentity>().isLocalPlayer)
				{
					GameObject bola2 = player.GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola2").gameObject;
					bola2.SetActive(true);
				}
			}			
		}
		if(timeRitmo < 2)
		{
			//Debug.Log("Ritmo = 2");
			foreach (GameObject player in playersArray)
			{
				if(player.GetComponent<NetworkIdentity>().isLocalPlayer)
				{
					GameObject bola3 = player.GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola3").gameObject;
					bola3.SetActive(true);
				}
			}
			allReadyManager = true;			
		}
		
	}



}
