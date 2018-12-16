using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class Tutorial : NetworkBehaviour {

	public GameObject TutorialCanvas;
	public TextMeshProUGUI TutorialText;
	public GameObject _Manager;
	public GameObject botPrefab;
	public GameObject playerPrefab;
	public List<GameObject> playersArray = new List<GameObject>();
	public int tutorialEtapas = 0;
	public bool liberaInfo = false;


	// Use this for initialization
	void Start () {

		TutorialCanvas.SetActive(true);
		//playersArray[0].GetComponent<Transform>().Find("playerCanvas/addBotPanel").gameObject.SetActive(false);

		
	}
	
	// Update is called once per frame
	void Update () {
		
		playersArray = _Manager.GetComponent<_Manager>().playersArray;

		if(playersArray.Count == 0)
		{
			//Só faz algo depois que tiver pelo menos o proprio player na scene
			return;
		}

		if(tutorialEtapas == 0)
		{
			//Ver de manter esses objetos desativos e só ativar após o prePartida do _Manager
			playersArray[0].GetComponent<Transform>().Find("playerCanvas/acoesPanel").gameObject.SetActive(false);
			playersArray[0].GetComponent<Transform>().Find("playerCanvas/readyButton").gameObject.SetActive(false);
		}

		if(!liberaInfo)
		{
			foreach(GameObject player in playersArray)
			{
				player.GetComponent<Transform>().Find("playerInfoCanvas").gameObject.SetActive(false);
			}	
		}

		//Quando acabar a partida
		if(_Manager.GetComponent<_Manager>().fimDeJogo == true)
		{
			if(_Manager.GetComponent<_Manager>().winner == playersArray[0])
			{
				TutorialText.text = "Parabens, você ganhou e concluiu o tutorial!";
				TutorialCanvas.SetActive(true);
			}
			else
			{
				TutorialText.text = "Você perdeu. Clique em reset para recomeçar";
				TutorialCanvas.SetActive(true);	
			}
		}


		

	}

	public void BotaoTutorial()
	{
		tutorialEtapas ++;
		TutorialEtapas();
		//TutorialCanvas.SetActive(false);
		//GameObject bot = (GameObject)Instantiate(botPrefab);
		//NetworkServer.Spawn(bot);
		
	}

	public void TutorialEtapas()
	{
		switch(tutorialEtapas)
		{
			case 1:
				TutorialText.text = "Você pode acompanhar o andamento da rodada de acordo com as bolinhas acima.\n"+
									"Você pode escolher a sua ação enquanto as <color=#ff0000ff>bolinhas vermelhas</color> estiverem visíveis.\n"+
									"Quando a <color=#00ff00ff>bolinha verde</color> aparecer, todas as ações são executadas ao mesmo tempo.";

				playersArray[0].GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola1").gameObject.SetActive(true);
				playersArray[0].GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola2").gameObject.SetActive(true);
				playersArray[0].GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola3").gameObject.SetActive(true);
			break;

			case 2:
				playersArray[0].GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola1").gameObject.SetActive(false);
				playersArray[0].GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola2").gameObject.SetActive(false);
				playersArray[0].GetComponent<Transform>().Find("playerCanvas/ritmoCanvas/bola3").gameObject.SetActive(false);
				TutorialText.text = "Você pode escolher uma das 3 ações seguintes:\n"+
									"<color=#00ff00ff>Defender:</color> protege de qualquer ataque recebido.\n"+
									"<color=#00ff00ff>Recarregar:</color> recarrega uma bala.\n"+
									"<color=#00ff00ff>Atirar:</color> atira no alvo escolhido.\n";
				playersArray[0].GetComponent<Transform>().Find("playerCanvas/acoesPanel").gameObject.SetActive(true);
			break;

			case 3:
				TutorialText.text = "Estou adicionando um bot para ver como você se sai.\n"+ 
									"Esse é o Bot Sanguinario. Ele apenas carrega e atira";
				GameObject bot = (GameObject)Instantiate(botPrefab);
				NetworkServer.Spawn(bot);
				bot.GetComponent<botIA>().botSanguinario = true;
				bot.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
			break;

			case 4:
				TutorialText.text = "Para ficar mais fácil de acompanhar o jogo, vou ativar\n"+ 
									"um painel extra com informações dos jogadores.\n"+
									"Ainda não decidi se esse painel será permanente,\n"+ 
									"mas penso em tornar uma feature desbloqueável";
				liberaInfo = true;
				foreach(GameObject player in playersArray)
				{
					player.GetComponent<Transform>().Find("playerInfoCanvas").gameObject.SetActive(true);
				}	
			break;

			case 5:
				TutorialText.text = "Você irá começar com 2 vidas e 0 balas.\n"+
									"Normalmente, os jogadores começam com apenas 1 vida.\n"+
									"Mas ainda não defini os valores iniciais padrões do jogo.\n"+
									"Vale notar que há um limite de apenas uma bala, por enquanto..";
				playersArray[0].GetComponent<Atributos>().vidas = 2;
				playersArray[0].GetComponent<Atributos>().balas = 0;
			break;

			case 6:
				TutorialText.text = "Quando estiver prontus, clique em Ready.\n"+
									"Bom jogo!\n";
				playersArray[0].GetComponent<Transform>().Find("playerCanvas/readyButton").gameObject.SetActive(true);
			break;

			case 7:
				TutorialCanvas.SetActive(false);
				_Manager.GetComponent<_Manager>().prePartida = true;
			break;

			case 8:
				_Manager.GetComponent<_Manager>().posPartida = true;
			break;

		}
	}
}
