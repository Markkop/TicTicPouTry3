using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Linq;

public class Atributos : NetworkBehaviour {

	//SyncVar parece nao estar funcionando, pelo menos para newName.
	//Esta sendo necessario chamar RpcChangePlayerName() para que os
	//outros clients saibam da mudanca de newName no server.

	//[SyncVar (hook="OnNameChange")] public string newName = "";
	public string newName = "";

	[SyncVar] public bool vaiAtirar = false;
	[SyncVar] public bool vaiDefender = false;
	[SyncVar] public bool estaDefendendo;
	[SyncVar] public bool estaContraAtacando;
	[SyncVar] public bool vaiRecarregar = false;
	[SyncVar] public bool vaiRecarrEsp = false;
	[SyncVar] public bool vaiUsarEsp = false;
	[SyncVar] public bool ready = false;
	[SyncVar] public bool levouDano = false;
	[SyncVar] public bool levouKadabra = false;
	public bool estaComVidaExtra = false;

	[SyncVar] public int vidas = 2;
	[SyncVar] public int balas = 0;
	[SyncVar] public int espCargas = 0;
	[SyncVar] public int maxEspCargas = 1;
	public int classe = 1; //1 = Mago para tester, mudar para 0 como padrão
	public int maxBalas = 1;	

	//public GameObject[] playersArray;
	public List<GameObject> playersArray = new List<GameObject>();

	public GameObject alvo;
	public GameObject segundoAlvo;
	public GameObject mortoPor; 
	
	public GameObject _Manager;
	public GameObject[] cameras;
	public GameObject playerCamera;
	public GameObject playerCanvas;
	public GameObject acoesCanvasIn;
	public GameObject alvosPanel;
	public GameObject alvosPanel2;
	public GameObject toggleGroup;
	public TMP_Dropdown ClasseDropdown;
	public Toggle atiraButton;
	public Animator anim;

	public bool playerVencedor = false;
	public bool playerMorto = false;
	private bool firstSpawn = false;

	// Use this for initialization
	void Start () {

		if(this.GetComponent<botIA>() == null)
		{
			//Importa as configurações do MainMenu
			vidas = Settings.startingVidas;
			balas = Settings.startingBalas;
			newName = Settings.playerName;
		}
		
		cameras = GameObject.FindGameObjectsWithTag("MainCamera");

		//Caso o objeto seja spawnado sem autoridade. Usar para debugging, mas de preferencia nao
		//usar para evitar conflito.
		if(!hasAuthority)
		{
			//CmdAttachAuthority();
		}

		//Ativa a camera e o canvas apenas para o jogador local
		//Caso contrario, o mesmo painel aparece para todos de forma sobreposta
		if (isLocalPlayer)
		{
			//cameras[0].SetActive(false);
			playerCamera.SetActive(true);
			playerCanvas.SetActive(true);
		}
		else
		{	//Se um bot tenta isso, aparece mensagem de erro pela falta de referencia;
			//entao soh roda para quem nao tiver o script botIA (ou seja; players)
			if(this.GetComponent<botIA>() == null)
			{
				playerCamera.SetActive(false);
				playerCanvas.SetActive(false);
			}
		}

		//Pega o Manager da cena
		_Manager = GameObject.FindWithTag("Manager");
		_Manager.GetComponent<_Manager>().playersArray.Add(gameObject);


		// if(isServer && this.GetComponent<botIA>() == null)
		// {
		// 	if(_Manager.GetComponent<_Manager>().addBotPanel != null)
		// 	{
		// 		_Manager.GetComponent<_Manager>().addBotPanel.SetActive(true);	
		// 	}
			
		// }
	}
	
	// Update is called once per frame
	void Update () { 

		//Mantem a lista de player atualizada do _Manager
		if(playersArray != _Manager.GetComponent<_Manager>().playersArray)
		{
			playersArray = _Manager.GetComponent<_Manager>().playersArray;	
		}

		//Tirar do Update e botar em alguma parada de mudar de classe (se houver)
		if(this.GetComponent<botIA>() == null)
		{
			MudaMenuAcoes();
			switch(classe)
			{
				case 0: //Nada
					maxBalas = 1;
				break;
				case 1: //Mago
					maxBalas = 1;
					maxEspCargas = 1;
				break;
				case 2: //Samurai
					maxBalas = 1;
					maxEspCargas = 1;
				break;
				case 3: //Padre
					maxBalas = 1;
					maxEspCargas = 2;
				break;
				case 4: //Cangaceiro
					maxBalas = 2;
				break;
			}

			switch(ClasseDropdown.value)
			{
				case 0: //Nada
					classe = 0;
				break;
				case 1: //Mago
					classe = 1;
				break;
				case 2: //Samurai
					classe = 2;
				break;
				case 3: //Padre
					classe = 3;
				break;
				case 4: //Cangaceiro
					classe = 4;
				break;
			}

			//Deixa o texto do botão de ação transparente caso esteja desabilitado
			foreach(Transform child in acoesCanvasIn.transform)
			{
				if(child.GetComponent<Toggle>().interactable == false)
				{
					var cor = child.gameObject.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().color ;
					cor.a = 0.2f;
					child.gameObject.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().color = cor;
				}
				else
				{
					var cor = child.gameObject.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().color ;
					cor.a = 1f;
					child.gameObject.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().color = cor;
				}
			}
	
		}


	}

	//Funcao para Debug
	[Command]
    void CmdAttachAuthority()
    {
		//Da a autoridade sob o objeto para quem conectou com este client
		this.GetComponent<NetworkIdentity>().AssignClientAuthority ( connectionToClient );
		Debug.Log(this.name+" recebeu autoridade de "+connectionToClient);
    }

    [Command]
	public void CmdQuerDefender2(bool boleano)
	{
		if (boleano == true)
		{
			vaiDefender = true;
			vaiAtirar = false;
			vaiRecarregar = false;	
		}
		else
		{
			vaiDefender = false;
		}
		
		//Debug.Log(this.name +" defende?: "+boleano);
	}

	[Command]
	public void CmdQuerAtirar2(bool boleano)
	{
		if (boleano == true)
		{
			vaiDefender = false;
			vaiAtirar = true;
			vaiRecarregar = false;
			//this.alvosPanel.gameObject.SetActive (true);
		}
		else
		{
			vaiAtirar = false;
		}
		//Debug.Log(this.name +" atira?: "+boleano);
	}

    [Command]
	public void CmdQuerRecarregar2(bool boleano)
	{
		if (boleano == true)
		{
			vaiDefender = false;
			vaiAtirar = false;
			vaiRecarregar = true;	
		}
		else
		{
			vaiRecarregar = false;
		}
		//Debug.Log(this.name +" recarrega?: "+boleano);
	}

	[Command]
	public void CmdQuerRecarrEsp(bool boleano)
	{
		if (boleano == true)
		{
			vaiDefender = false;
			vaiAtirar = false;
			vaiRecarregar = false;
			vaiRecarrEsp = true;
			vaiUsarEsp = false;	
		}
		else
		{
			vaiRecarrEsp = false;
		}
	}

	[Command]
	public void CmdQuerUsarEsp(bool boleano)
	{
		if (boleano == true)
		{
			vaiDefender = false;
			vaiAtirar = false;
			vaiRecarregar = false;
			vaiRecarrEsp = false;
			vaiUsarEsp = true;	
		}
		else
		{
			vaiUsarEsp = false;
		}
	}


	[ClientRpc]
	void RpcMudaDefesa(bool boleano)
	{
		Debug.Log("Server: pedido de mudanca de DEFESA para "+boleano);
		this.vaiDefender = boleano;
	}

	[Command]
	public void CmdIsReady(bool boleano)
	{
		ready = boleano;
	}

	public void EscolheAlvo(int player)
	{

		for(int i = 0; i < playersArray.Count; i++) 
		{	//Se o play
			if(player == i)
			{
				alvo = playersArray[i]; //Da o gameObject indexado de acordo com o botao clicado
			}
		} 
		
	}

	public void MudaMenuAcoes()
	{
		GameObject defendeButton = acoesCanvasIn.GetComponent<Transform>().Find("defendeButton").gameObject;
		GameObject atiraButton = acoesCanvasIn.GetComponent<Transform>().Find("atiraButton").gameObject;
		GameObject RecarregaButton = acoesCanvasIn.GetComponent<Transform>().Find("RecarregaButton").gameObject;
		GameObject recarrEspButton = acoesCanvasIn.GetComponent<Transform>().Find("recarrEspButton").gameObject;
		GameObject usaEspButton = acoesCanvasIn.GetComponent<Transform>().Find("usaEspButton").gameObject;
		switch(classe)
		{
			case 0: //Nada
				atiraButton.SetActive(true);
				RecarregaButton.SetActive(true);
				recarrEspButton.SetActive(false);
				usaEspButton.SetActive(false);
				alvosPanel2.SetActive(false);
			break;
			case 1: // Mago
				atiraButton.SetActive(true);
				RecarregaButton.SetActive(true);
				recarrEspButton.SetActive(true);
				alvosPanel2.SetActive(false);
				recarrEspButton.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Carrega Kadabra";
				usaEspButton.SetActive(true);
				usaEspButton.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Usa Kadabra";
				if(espCargas > 0)
				{
					recarrEspButton.GetComponent<Toggle>().interactable = false;
					usaEspButton.GetComponent<Toggle>().interactable = true;
				}
				else
				{
					recarrEspButton.GetComponent<Toggle>().interactable = true;
					usaEspButton.GetComponent<Toggle>().interactable = false;
				}
			break;
			case 2: // Samurai
				atiraButton.SetActive(true);
				RecarregaButton.SetActive(true);
				recarrEspButton.SetActive(true);
				alvosPanel2.SetActive(false);
				recarrEspButton.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Carrega Katchin";
				usaEspButton.SetActive(true);
				usaEspButton.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Usa Katchin";
				if(espCargas > 0)
				{
					recarrEspButton.GetComponent<Toggle>().interactable = false;
					usaEspButton.GetComponent<Toggle>().interactable = true;
				}
				else
				{
					recarrEspButton.GetComponent<Toggle>().interactable = true;
					usaEspButton.GetComponent<Toggle>().interactable = false;
				}
			break;
			case 3: // Padre
				atiraButton.SetActive(true);
				RecarregaButton.SetActive(true);
				recarrEspButton.SetActive(true);
				alvosPanel2.SetActive(false);
				recarrEspButton.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Reza ("+espCargas+"/"+maxEspCargas+")";
				usaEspButton.SetActive(false);
				if(estaComVidaExtra == true)
				{
					recarrEspButton.GetComponent<Toggle>().interactable = false;
				}
				else
				{
					recarrEspButton.GetComponent<Toggle>().interactable = true;
				}
			break;
			case 4: // Cangaceiro
				atiraButton.SetActive(true);
				RecarregaButton.SetActive(true);
				recarrEspButton.SetActive(false);
				usaEspButton.SetActive(false);
				if(balas > 1)
				{
					alvosPanel2.SetActive(true)	;
				}
				else
				{
					alvosPanel2.SetActive(false);
				}
			
			break;
			case 5: // Assasino
				atiraButton.SetActive(false);
				RecarregaButton.SetActive(false);
				recarrEspButton.SetActive(true);
				alvosPanel2.SetActive(false);
				recarrEspButton.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Afia a adaga";
				usaEspButton.SetActive(true);
				usaEspButton.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Apunhala o alvo";
			break;
		}
		if(balas > 0)
		{
			atiraButton.GetComponent<Toggle>().interactable = true;
			alvosPanel.SetActive(true);
		}
		else
		{
			atiraButton.GetComponent<Toggle>().interactable = false;	
			alvosPanel.SetActive(false);
		}
		if(balas >= maxBalas)
		{
			RecarregaButton.GetComponent<Toggle>().interactable = false;
		}
		else
		{
			RecarregaButton.GetComponent<Toggle>().interactable = true;
		}
		RecarregaButton.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Recarregar ("+balas+"/"+maxBalas+")";

	}

}
