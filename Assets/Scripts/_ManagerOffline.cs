using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class _ManagerOffline : MonoBehaviour {

	//public GameObject[] playersArray;
	//public List<GameObject> gameObjectList = new List<GameObject>();
	//public List<int> playersList = new List<int>();

	//O playerArray (que é na vdd é uma lista) é populado toda vez
	//que um jogador entra no jogo.
	public List<GameObject> playersArray = new List<GameObject>();
	public int tempCountPlayers = 0;
	

	public bool fimDeJogo = false;
	public GameObject winner;
	public GameObject overviewCamera;

	public bool primeiroTurno = false;
	public bool fimDeTurno = false;
	public bool comecaRodada = false;
	public bool startReadyManager = false;
	public int rodada = 0;

	public float tempoRodada = 4; //Verifique se no Editor tambem foi alterado
	float timeLeft = 5;
	float timeRitmo;
	float timeLeft2;


	void Start () {

	//Gambiarra temporaria para sumir e aparecer os botoes Host e Client do NetworkManager

	tempoRodada = Settings.newRitmo;
	timeRitmo = tempoRodada;
	timeLeft2 = 2*tempoRodada/5;
	}
	
	void Update () {

		//Soh roda no servidor
		
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
						ToggleBola(1, player, false);
						ToggleBola(2, player, false);
						ToggleBola(3, player, false);
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
			ChangePlayerName(playersArray[i], "Jogador "+i);
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
				DesativaReadyButton(player);	
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
				AnimTrigger(go, "Defende");
				go.GetComponent<Atributos>().estaDefendendo = true; //Entao esta defendendo
			}
			if(go.GetComponent<Atributos>().vaiRecarregar == true) //Se optou por recarregar
			{
				AnimTrigger(go, "Recarrega");
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
					AnimTrigger(go, "Atira");
					FaceTo(go, alvo1);

					if(alvo1.GetComponent<Atributos>().estaDefendendo == false) //se o alvo NAO estiver defendno
					{
						if(!alvo1.GetComponent<Atributos>().levouTiro) // Para levar apenas um tiro no max
						{
							alvo1.GetComponent<Atributos>().levouTiro = true;
							alvo1.GetComponent<Atributos>().vidas -= 1; //perde uma vida
							if(alvo1.GetComponent<Atributos>().vidas == 0) //Se estiver morto
							{
								alvo1.GetComponent<Atributos>().mortoPor = go; //memoriza quem o matou
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
			if(go.GetComponent<Atributos>().vidas == 0 && go.GetComponent<Atributos>().playerMorto == false) // se nao tiver vida 
			{
				Debug.Log("O ["+go.name+"] morreu");
				go.GetComponent<Atributos>().newName = "Morto";

				ChangePlayerName(go, "Morto");
				Morte(go, go.GetComponent<Atributos>().mortoPor);
			}

		//Reseta as acoes
		//go.GetComponent<Atributos>().ready = false;
		go.GetComponent<Atributos>().vaiAtirar = false;
		go.GetComponent<Atributos>().vaiRecarregar = false;
		go.GetComponent<Atributos>().vaiDefender = false;
		go.GetComponent<Atributos>().estaDefendendo = false;
		go.GetComponent<Atributos>().alvo = null;
		DesativaToggles(go);


		fimDeTurno = true;
		//Debug.Log("Fim do Resolve4");
		}

		//Contabiliza players mortos
		int playersMortos = 0;
		foreach (GameObject go in playersArray)
		{		
			if(go.GetComponent<Atributos>().vidas == 0)
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
			Posiciona(playersArray[i], new Vector3(x,y,z));
			//Debug.Log("Posicao do "+playersArray[i]+"em x: "+x+" e em z: "+z);
		}
	}

	// Temporizador para definir o ritmo das acoes dos jogadores.
	// Deve ter alguma forma de deixar isso mais limpo.	
	void Ritmo()
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
					FaceToReset(player);
					ToggleBola(1, player, true);
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
					ToggleBola(2, player, true);
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
					ToggleBola(3, player, true);
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
			FimDeJogo(player);
		}

		//Timer de 5 segundos
		timeLeft -= Time.deltaTime;
		if(timeLeft < 0)
		{
			Debug.Log("Desconectando e resetando scene...");
			SceneManager.LoadScene(0);

			//Gambiarra temporaria para voltar com os botoes Host e Client
			return;	
		}
		else
		{	
			Debug.Log("Fim de jogo. "+Mathf.Round(timeLeft)+" segundos para encerrar...");
			return;
		}
	}

	//==========  CALLS =============== //
	//Os seguintes comandos são chamados pelo server e executados em todos os Clients.


	
    void ChangePlayerName(GameObject go, string n)
    {
		//Debug.Log("Avisando todos os clientes da mudanca de nome do "+go.name+" para "+n);
		go.GetComponent<Atributos>().newName = n;
		go.name = n;
    }

	
	void ToggleBola(int bola, GameObject player, bool bools)
	{
		player.GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola"+bola).gameObject.SetActive(bools);
	}

	
	void AnimTrigger(GameObject player, string anim)
	{
		player.GetComponent<Animator>().SetTrigger(anim);
	}

	
	void Morte(GameObject playerQueMorreu, GameObject playerQueMatou)
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

	
	void Posiciona(GameObject player, Vector3 pos)
	{
		player.GetComponent<Transform>().position = pos;
		player.GetComponent<Transform>().LookAt(new Vector3(0f,player.GetComponent<Transform>().position[1],0f));
	}

	
	void FaceTo(GameObject player, GameObject alvo)
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

	
	void FaceToReset(GameObject player)
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

	
	void DesativaReadyButton(GameObject player)
	{
		player.GetComponent<Transform>().Find("playerCanvas/readyButton").gameObject.SetActive(false);

		//Aproveita e desativa o painel de bots
		player.GetComponent<Transform>().Find("playerCanvas/addBotPanel").gameObject.SetActive(false);

	}

	
	void DesativaToggles(GameObject player)
	{
		GameObject[] toggles = GameObject.FindGameObjectsWithTag("PlayerToggle");
		foreach (GameObject tog in toggles)
		{
			tog.GetComponent<Toggle>().isOn = false;
		}
	}

	
	void FimDeJogo(GameObject player)
	{
		if(player.GetComponent<botIA>() == null)
		{
			player.GetComponent<Atributos>().playerCamera.SetActive(false);
			player.GetComponent<Atributos>().playerCanvas.SetActive(false);
		}
		overviewCamera.GetComponent<Transform>().position = overviewCamera.GetComponent<Transform>().position + new Vector3(-Time.deltaTime,Time.deltaTime,-Time.deltaTime);
	
	}

}