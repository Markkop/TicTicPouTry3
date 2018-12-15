using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

public class _Manager : NetworkBehaviour {

	public GameObject MyNetworkManager;
	public GameObject addBotPanel;

	//public GameObject[] playersArray;
	//public List<GameObject> gameObjectList = new List<GameObject>();
	//public List<int> playersList = new List<int>();

	//O playerArray (que é na vdd é uma lista) é populado toda vez
	//que um jogador entra no jogo.
	public List<GameObject> playersArray = new List<GameObject>();
	public int tempCountPlayers = 0;

	public List<GameObject> magosKadabra = new List<GameObject>();

	public bool fimDeJogo = false;
	public GameObject winner;
	public GameObject overviewCamera;

	public bool primeiroTurno = false;
	public bool fimDeTurno = false;
	public bool comecaRodada = false;
	public bool startReadyManager = false;
	public int rodada = 0;

	public bool prePartida = false;
	public bool posPartida = false;

	public float tempoRodada = 4; //Verifique se no Editor tambem foi alterado
	float timeLeft = 5;
	float timeRitmo;
	float timeLeft2;


	void Start () {

	//Gambiarra temporaria para sumir e aparecer os botoes Host e Client do NetworkManager
	MyNetworkManager = GameObject.Find("_NetworkManager");

	tempoRodada = Settings.newRitmo;
	timeRitmo = tempoRodada;
	timeLeft2 = 2*tempoRodada/5;
	}
	
	void Update () {

		//Soh roda no servidor
		if(!isServer)
		{
			return;
		}
		
		//Caso tenha acabado o jogo
		if(fimDeJogo)
		{
			FimDeJogo();
			return;
		}

		if(!startReadyManager) // Verifica se todos estao prontos
		{
			// Caso a partida ainda nao tenha começado, posiciona os jogadores
			if(playersArray.Count != 0 && !primeiroTurno)
			{
				if(tempCountPlayers != playersArray.Count)
				{
					NomeiaPlayers();
					PosicionaPlayers();
					tempCountPlayers = playersArray.Count;	
				}
				CheckReady();
			}
			return; //Se nao estiverem prontos, nao continua a rotina.
		}
		else
		{	
			//Quando todos os jogadores estiverem prontos pela primeira vez, ativa o primeiro Turno
			//para que eles não fiquem sendo posicionados toda vez. (na real talvez nao precise por conta do startReadyManager)
			if(!primeiroTurno)
			{	
				primeiroTurno = true;	
			}
		}

		//Caso os eventos prePartida ainda nao tenham ocorrido
		if(!prePartida)
		{
			return;
		}

		//Contador para definir o Ritmo do jogo, exibindo as bolinhas de timer pros jogadores
		//e iniciando as resolucoes na bolinha maior (bola3)
		if(!comecaRodada)
		{

			timeRitmo -= Time.deltaTime;
			Ritmo();
			return;
		}

		//Para que o jogo aguarde 2 segundos antes de recomeçar a próxima rodada, adicionou-se "fimDeTurno"
		if(!fimDeTurno)
		{
			Debug.Log("Iniciando rodada "+rodada+"...");
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
				foreach (GameObject player in playersArray)
				{
					if(player.GetComponent<botIA>() == null)
					{
						RpcToggleBola(1, player, false);
						RpcToggleBola(2, player, false);
						RpcToggleBola(3, player, false);
					}
				}
				//Reseta os contadores
				timeRitmo = tempoRodada;
				timeLeft2 = 2*tempoRodada/5;
				fimDeTurno = false;
				comecaRodada = false;
				rodada = rodada +1;
			}
		}
	}
	
	void NomeiaPlayers()
	{
		for(int i = 0; i < playersArray.Count; i++)
		{
			playersArray[i].GetComponent<Atributos>().newName = "Jogador "+i;
			playersArray[i].name = "Jogador "+i;
			RpcChangePlayerName(playersArray[i], "Jogador "+i);
		}
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

		//Desativa os botoes de ready uma vez que todos estao prontos
		foreach(GameObject player in playersArray)
		{
			if(player.GetComponent<botIA>() == null)
			{
				RpcDesativaReadyButton(player);	
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
			//go.GetComponent<Atributos>().allReady = true; // (aproveita e confirma a todos que todos estao prontos)
			if(go.GetComponent<Atributos>().vaiDefender == true) //Se optou por defender 
			{
				RpcAnimTrigger(go, "Defende");
				go.GetComponent<Atributos>().estaDefendendo = true; //Entao esta defendendo
			}
			if(go.GetComponent<Atributos>().vaiRecarregar == true) //Se optou por recarregar
			{
				if(go.GetComponent<Atributos>().balas != go.GetComponent<Atributos>().maxBalas)  //e nao tiver com max de bala
				{
					RpcAnimTrigger(go, "Recarrega");
					Debug.Log(go.name+" carrega uma bala...");
					go.GetComponent<Atributos>().balas += 1; // ganha uma bala
				}
				else
				{
					Debug.Log(go.name+" tentou carregar uma bala, mas ja tem o max.");
					//caso contrario, nao ganha bala.
					//Obs: o player vai gastar a propria acao
				}		
			}
			if(go.GetComponent<Atributos>().vaiRecarrEsp == true)
			{
				switch(go.GetComponent<Atributos>().classe)
				{
					case 1: //Mago
						if(go.GetComponent<Atributos>().espCargas != go.GetComponent<Atributos>().maxEspCargas)
						{
							Debug.Log(go.name+" carrega um ABRA");
							RpcAnimTrigger(go, "CarregaKadabra");
							go.GetComponent<Atributos>().espCargas += 1;	
						}
					break;
					case 2: //Samurai
						if(go.GetComponent<Atributos>().espCargas != go.GetComponent<Atributos>().maxEspCargas)
						{
							Debug.Log(go.name+" carrega um CONTRA-ATAQUE");
							RpcAnimTrigger(go, "CarregaKatchin");
							go.GetComponent<Atributos>().espCargas += 1;	
						}
					break;
					case 3: //Padre
						go.GetComponent<Atributos>().espCargas += 1;
						Debug.Log(go.name+" carrega uma REZA ("+go.GetComponent<Atributos>().espCargas+"/"+go.GetComponent<Atributos>().maxEspCargas+")");
						RpcAnimTrigger(go, "Reza");	
					break;
					case 4: //Samurai
						//sem especiail
					break;
				}	
			}
		}
	}

	void ResolvePhase2() //Verifica se ha balas
	{
		magosKadabra.Clear();
		//Debug.Log("Resolvendo Phase 2");
		foreach (GameObject go in playersArray) //Para cada jogador...
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
			if(go.GetComponent<Atributos>().vaiUsarEsp == true)
			{
				if(go.GetComponent<Atributos>().espCargas > 0)
				{
					switch(go.GetComponent<Atributos>().classe)
					{
						case 1: //Mago
							//magosKadabra.Add(go); //Magos usando Kadabra
						break;
						case 2: //Samurai
							go.GetComponent<Atributos>().espCargas -= 1;
							go.GetComponent<Atributos>().estaContraAtacando = true;
							RpcAnimTrigger(go, "UsaKatchin");
						break;

					}
				}
				else
				{
					Debug.Log("Nao carregou sua ação especial...");
					go.GetComponent<Atributos>().vaiUsarEsp = false;
				}
			}
		}
	}

	void ResolvePhase3() //Resolve ataque e defesa
	{

		//Resolve tiros
		foreach (GameObject go in playersArray) // Para cada jogador
		{
			if(go.GetComponent<Atributos>().vaiAtirar == true) // que estiver atirando
			{
				AtiraAlvo(go, go.GetComponent<Atributos>().alvo);
			}
			if(go.GetComponent<Atributos>().segundoAlvo != null && go.GetComponent<Atributos>().classe == 4)
			{
				AtiraAlvo(go, go.GetComponent<Atributos>().segundoAlvo);
			}
		}

		CancelaEspSeMorto();

		//Resolve Kadabras
		foreach (GameObject caster in playersArray)
		{
			if(caster.GetComponent<Atributos>().vaiUsarEsp == true)	//Se alguem usar especial
			{
				if(caster.GetComponent<Atributos>().classe == 1) //Magos 
				{
					foreach(GameObject atingido in playersArray)
					{
						if(atingido.GetComponent<Atributos>().estaContraAtacando == true)
						{
							RpcAnimTrigger(caster, "UsaKadabra");
							RecebeDano(atingido, caster, 1); //Mago toma dano
						}
					}
				}
			}
		}

		CancelaEspSeMorto();

		//Finalmente, demais jogadores levam dano do Kadabra
		foreach(GameObject player in playersArray)
		{
			if(player.GetComponent<Atributos>().classe == 1 && player.GetComponent<Atributos>().vaiUsarEsp == true)
			{
				explodeKadabra(player);
			}
		}
	}



	void ResolvePhase4() //Verifica vidas
	{
		//Debug.Log("Resolvendo Phase 4");
		foreach (GameObject go in playersArray) // Para cada jogador
		{
			if(go.GetComponent<Atributos>().classe == 3 && go.GetComponent<Atributos>().vidas > 0) //Padres
			{
				if(go.GetComponent<Atributos>().espCargas == go.GetComponent<Atributos>().maxEspCargas)
				{
					go.GetComponent<Atributos>().vidas += 1;
					go.GetComponent<Atributos>().estaComVidaExtra = true;
				}
			}


			if(go.GetComponent<Atributos>().vidas == 0 && go.GetComponent<Atributos>().playerMorto == false) // se nao tiver vida 
			{
				Debug.Log("O ["+go.name+"] morreu");
				go.GetComponent<Atributos>().newName = "Morto";

				RpcChangePlayerName(go, "Morto");
				RpcMorte(go, go.GetComponent<Atributos>().mortoPor);
			}

		//Reseta as acoes
		//go.GetComponent<Atributos>().ready = false;
		go.GetComponent<Atributos>().vaiAtirar = false;
		go.GetComponent<Atributos>().vaiRecarregar = false;
		go.GetComponent<Atributos>().vaiDefender = false;
		go.GetComponent<Atributos>().estaDefendendo = false;
		go.GetComponent<Atributos>().vaiUsarEsp = false;
		go.GetComponent<Atributos>().vaiRecarrEsp = false;
		go.GetComponent<Atributos>().alvo = null;
		go.GetComponent<Atributos>().levouDano = false;
		go.GetComponent<Atributos>().estaContraAtacando = false;
		RpcDesativaToggles(go);


		fimDeTurno = true;
		//Debug.Log("Fim do Resolve4");
		}

		//Contabiliza players mortos
		int playersMortos = 0;
		foreach (GameObject go in playersArray)
		{		
			if(go.GetComponent<Atributos>().vidas <= 0)
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
					//if(winner.GetComponent<NetworkIdentity>().isLocalPlayer) winner.GetComponent<Animator>().SetTrigger("winner");
				}
			}
			Debug.Log("||FIM DE JOGO|| . Vencedor: "+winner.name);
			fimDeJogo = true;
		}
		if (playersMortos == playersArray.Count )
		{
			Debug.Log("||FIM DE JOGO|| . Ninguem ganhou");
			fimDeJogo = true;			
		}
	}

	void AtiraAlvo(GameObject atacante, GameObject alvo1)
	{
		RpcAnimTrigger(atacante, "Atira");
		RpcFaceTo(atacante, alvo1);

		if(alvo1.GetComponent<Atributos>().estaDefendendo == false) //se o alvo NAO estiver defendno
		{
			if(alvo1.GetComponent<Atributos>().estaContraAtacando)
			{
				RecebeDano(alvo1, atacante, 1);
				Debug.Log(alvo1.name+" contra ataca "+atacante.name+" que perde uma vida...");

			}
			else
			{
				RecebeDano(atacante, alvo1, 1);
				Debug.Log(atacante.name+" atira em "+alvo1.name+" que perde uma vida...");
			}
			
		}
		else
		{
			alvo1.GetComponent<PlaySound>().BlockSound();
			Debug.Log(atacante.name+" atira em "+alvo1.name+" que se defende...");
		}
		atacante.GetComponent<Atributos>().balas -= 1; // remove uma bala (se o alvo defender ou nao)				
	}	

	void RecebeDano(GameObject atacante, GameObject atacado, int dano)
	{
		//Se ainda nao levou Dano (LogicaOriginal)
		if(atacado.GetComponent<Atributos>().levouDano == false)
		{
			atacado.GetComponent<Atributos>().vidas -= dano;
			atacado.GetComponent<PlaySound>().PainSound();
			atacado.GetComponent<Atributos>().levouDano = true;

			if(atacado.GetComponent<Atributos>().vidas == 0) //Se morreu
			{
				RpcAnimTrigger(atacado, "Morto");
				atacado.GetComponent<Atributos>().mortoPor = atacante; //memoriza quem o matou
			}
			if(atacado.GetComponent<Atributos>().classe == 3)
			{
				if(atacado.GetComponent<Atributos>().estaComVidaExtra == true)
				{
					atacado.GetComponent<Atributos>().estaComVidaExtra = false;
					atacado.GetComponent<Atributos>().espCargas = 0;
				}
			}
		}		
	}

	void CancelaEspSeMorto()
	{
		//Cancela especial se tiver morrido
		foreach(GameObject player in playersArray) //Verificada cada player
		{
			if(player.GetComponent<Atributos>().classe == 1) //Se for mago
			{
				if(player.GetComponent<Atributos>().vidas < 1) //E tiver 0 vidas
				{
					player.GetComponent<Atributos>().vaiUsarEsp = false; // Cancela o especial
				}
			}
		}	
	}

	void explodeKadabra(GameObject mago)
	{
		foreach(GameObject player in playersArray) //Ataca todo mundo
		{
			if(player != mago) //Menos ele mesmo
			{
				if(player.GetComponent<Atributos>().estaContraAtacando == false && player.GetComponent<Atributos>().estaDefendendo == false) //Menos quem defender
				{
					RecebeDano(mago, player, 1);		
				}
				
			}
		}
		RpcAnimTrigger(mago, "UsaKadabra");
		mago.GetComponent<Atributos>().espCargas -= 1;
	}

	//Posiciona os jogadores em espaçamentos iguais em um circulo
	void PosicionaPlayers()
	{
		float angulo = 360/playersArray.Count;
		for(int i = 0; i < playersArray.Count; i++)
		{
			//float altura = playersArray[i].GetComponent<Collider>().bounds.max[1];
			//float y = altura/2;
			float y = 0.5f ;

			if(playersArray[i].GetComponent<botIA>() == null)
			{
				//Gambiarra pra arrumar depois que tiver modelos pra player e players definidos
				//Nota-se que é preciso fazer isso pois os players são Spawnados pelo NetworkManager e
				//provavelmente spawnando usando a base do player, enquanto que os Bots são spawnados
				//pelo jogador (host) e usando o centro do bot.
				y = 0.5f;
			}
			float x = 8*Mathf.Cos(i*angulo*3.14f/180f);
			float z = 8*Mathf.Sin(i*angulo*3.14f/180f);
			RpcPosiciona(playersArray[i], new Vector3(x,y,z));
			//Debug.Log("Posicao do "+playersArray[i]+"em x: "+x+" e em z: "+z);
		}
	}

	// Temporizador para definir o ritmo das acoes dos jogadores.
	// Deve ter alguma forma de deixar isso mais limpo.	
	public void Ritmo()
	{
		//Debug.Log("Entrando na funcao ritmo");
		if(timeRitmo < 4*tempoRodada/5)
		{
			//Debug.Log("Ritmo = 4");
			foreach (GameObject player in playersArray)
			{
				if(player.GetComponent<botIA>() == null)
				{
					//player.GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola1").gameObject.SetActive(true);
					RpcFaceToReset(player);
					RpcToggleBola(1, player, true);
				}
			}			
		}
		if(timeRitmo < 3*tempoRodada/5)
		{
			//Debug.Log("Ritmo = 3");
			foreach (GameObject player in playersArray)
			{
				if(player.GetComponent<botIA>() == null)
				{
					RpcToggleBola(2, player, true);
				}
			}			
		}
		if(timeRitmo < 2*tempoRodada/5)
		{
			//Debug.Log("Ritmo = 2");
			foreach (GameObject player in playersArray)
			{
				if(player.GetComponent<botIA>() == null)
				{
					RpcToggleBola(3, player, true);
				}
			}
			comecaRodada = true;			
		}
		
	}

	void FimDeJogo()
	{
		foreach(GameObject player in playersArray)
		{
			//Desativa a camera e afasta a camera OverView
			RpcFimDeJogo(player);
		}

		//Timer de 5 segundos
		timeLeft -= Time.deltaTime;
		if(timeLeft < 0)
		{
			if(posPartida == false)
			{
				return;
			}

			Debug.Log("Desconectando e resetando scene...");
			SceneManager.LoadScene(0);
			NetworkManager.singleton.StopClient();
			NetworkManager.singleton.StopHost();
			NetworkManager.singleton.StopServer();

			//Gambiarra temporaria para voltar com os botoes Host e Client
			MyNetworkManager.GetComponent<Transform>().Find("canvasAll").gameObject.SetActive(true);
			return;	
		}
		else
		{	
			Debug.Log("Fim de jogo. "+Mathf.Round(timeLeft)+" segundos para encerrar...");
			return;
		}
	}

	//========== RPC CALLS =============== //
	//Os seguintes comandos são chamados pelo server e executados em todos os Clients.


	[ClientRpc]
    void RpcChangePlayerName(GameObject go, string n)
    {
		//Debug.Log("Avisando todos os clientes da mudanca de nome do "+go.name+" para "+n);
		go.GetComponent<Atributos>().newName = n;
		go.name = n;
    }

	[ClientRpc]
	void RpcToggleBola(int bola, GameObject player, bool bools)
	{
		player.GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola"+bola).gameObject.SetActive(bools);
	}

	[ClientRpc]
	void RpcAnimTrigger(GameObject player, string anim)
	{
		player.GetComponent<Animator>().SetTrigger(anim);
	}

	[ClientRpc]
	void RpcMorte(GameObject playerQueMorreu, GameObject playerQueMatou)
	{
		playerQueMorreu.GetComponent<Transform>().LookAt(playerQueMatou.GetComponent<Transform>().position);
		playerQueMorreu.GetComponent<Rigidbody>().AddRelativeForce(transform.forward * -8000);
		playerQueMorreu.GetComponent<Atributos>().playerMorto = true;

		if(playerQueMorreu.GetComponent<botIA>() == null)
		{
			playerQueMorreu.GetComponent<Atributos>().playerCamera.SetActive(false);
			playerQueMorreu.GetComponent<Atributos>().cameras[0].active = true;
			playerQueMorreu.GetComponent<Atributos>().playerCanvas.SetActive(false);
		}
	}

	[ClientRpc]
	void RpcPosiciona(GameObject player, Vector3 pos)
	{
		player.GetComponent<Transform>().position = pos;
		player.GetComponent<Transform>().LookAt(new Vector3(0f,player.GetComponent<Transform>().position[1],0f));
	}

	[ClientRpc]
	void RpcFaceTo(GameObject player, GameObject alvo)
	{
		//Gambiarra fudida para que apenas o jogador vire no LookAt, com a camera permanecendo igual
		Quaternion originalCameraRot = new Quaternion(0f,0f,0f,1f);
		Vector3 originalCameraPos = Vector3.zero;

		//Essa verificação para player/bot é necessaria para evitar erros, uma vez que bot nao tem camera
		if(player.GetComponent<botIA>() == null)
		{
			//Armazena a posicao e rotacao original da camera
			originalCameraRot = player.GetComponent<Atributos>().playerCamera.GetComponent<Transform>().rotation;
			originalCameraPos = player.GetComponent<Atributos>().playerCamera.GetComponent<Transform>().position;
		}

		//A funcao poderia ser só essa linha... sigh..
		player.GetComponent<Transform>().LookAt(alvo.GetComponent<Transform>());

		if(player.GetComponent<botIA>() == null)
		{
			//Retorna a posicao e rotacao original da camera
			player.GetComponent<Atributos>().playerCamera.GetComponent<Transform>().rotation = originalCameraRot;
			player.GetComponent<Atributos>().playerCamera.GetComponent<Transform>().position = originalCameraPos;
		}
		
	}

	[ClientRpc]
	void RpcFaceToReset(GameObject player)
	{
		Quaternion originalCameraRot = new Quaternion(0f,0f,0f,1f);
		Vector3 originalCameraPos = Vector3.zero;
		
		if(player.GetComponent<botIA>() == null)
		{
			originalCameraRot = player.GetComponent<Atributos>().playerCamera.GetComponent<Transform>().rotation;
			originalCameraPos = player.GetComponent<Atributos>().playerCamera.GetComponent<Transform>().position;
		}

		//Olhar para o centro hardcoded
		player.GetComponent<Transform>().LookAt(new Vector3(0f,player.GetComponent<Transform>().position[1],0f));

		if(player.GetComponent<botIA>() == null)
		{
			player.GetComponent<Atributos>().playerCamera.GetComponent<Transform>().rotation = originalCameraRot;
			player.GetComponent<Atributos>().playerCamera.GetComponent<Transform>().position = originalCameraPos;
		}

	}

	[ClientRpc]
	void RpcDesativaReadyButton(GameObject player)
	{
		player.GetComponent<Transform>().Find("playerCanvas/readyButton").gameObject.SetActive(false);

		//Aproveita e desativa o painel de bots
		if(addBotPanel != null)
			addBotPanel.SetActive(false);
		
		//Gambiarra temporaria para sumir com os botoes Host e Client
		MyNetworkManager.GetComponent<Transform>().Find("canvasAll").gameObject.SetActive(false);
	}

	[ClientRpc]
	void RpcDesativaToggles(GameObject player)
	{
		GameObject[] toggles = GameObject.FindGameObjectsWithTag("PlayerToggle");
		foreach (GameObject tog in toggles)
		{
			tog.GetComponent<Toggle>().isOn = false;
		}
	}

	[ClientRpc]
	void RpcFimDeJogo(GameObject player)
	{
		if(player.GetComponent<botIA>() == null)
		{
			player.GetComponent<Atributos>().playerCamera.SetActive(false);
			player.GetComponent<Atributos>().playerCanvas.SetActive(false);
		}
		overviewCamera.GetComponent<Transform>().position = overviewCamera.GetComponent<Transform>().position + new Vector3(-Time.deltaTime,Time.deltaTime,-Time.deltaTime);
	
	}

}
