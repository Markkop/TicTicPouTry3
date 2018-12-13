using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class Boss1Script : NetworkBehaviour {

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
			playersArray[0].GetComponent<Transform>().Find("playerCanvas/acoesCanvas").gameObject.SetActive(false);
			playersArray[0].GetComponent<Transform>().Find("playerCanvas/readyButton").gameObject.SetActive(false);
		}

		if(playersArray[0].GetComponent<Atributos>().vidas == 0)
		{
			_Manager.GetComponent<_Manager>().fimDeJogo = true;
		}


		//Quando acabar a partida
		if(_Manager.GetComponent<_Manager>().fimDeJogo == true)
		{
			if(_Manager.GetComponent<_Manager>().winner == playersArray[0])
			{
				TutorialText.text = "Parabens, você derrotou o chefe!!";
				TutorialCanvas.SetActive(true);
			}
			else
			{
				TutorialText.text = "Você perdeu.\n"+
									"Dica: o Chefe não segue a mesma lógica que os outros bots.\n"+
									"Seja paciente, é difícil mesmo.";
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
				TutorialText.text = "<color=#ff0000ff>Capanga:</color> Ei, quem é você? Chefe! Venha cá!";
				playersArray[0].GetComponent<Transform>().Find("playerCanvas/acoesCanvas").gameObject.SetActive(true);
				playersArray[0].GetComponent<Transform>().Find("playerCanvas/readyButton").gameObject.SetActive(true);

				GameObject bot1 = (GameObject)Instantiate(botPrefab);
				NetworkServer.Spawn(bot1);
				bot1.GetComponent<botIA>().botSanguinario = true;
				bot1.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;

				playersArray[0].GetComponent<Atributos>().vidas = 1;
				playersArray[0].GetComponent<Atributos>().balas = 0;
			break;

			case 2:
				TutorialText.text = "<color=#808080ff>Chefe:</color> Então você me encontrou! Prepara-se para morrer!";
				GameObject bot2 = (GameObject)Instantiate(botPrefab);
				NetworkServer.Spawn(bot2);
				bot2.GetComponent<botIA>().botRDRA = true;
				bot2.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.grey;
				bot2.GetComponent<Atributos>().vidas = 3;
				bot2.GetComponent<Atributos>().balas = 0;
			break;

			case 3:
				TutorialText.text = "<color=#ff0000ff>Outro campanga:</color> Toda essa lore pq nao deu pra adicionar\n"+ 
									"os 3 bots ao mesmo tempo...\n"+
									"(Obs: você começa com 1 vida e 0 balas,\n"+
									"mas pode mudar a velocidade no Menu Principal)";
				GameObject bot3 = (GameObject)Instantiate(botPrefab);
				NetworkServer.Spawn(bot3);
				bot3.GetComponent<botIA>().botSanguinario = true;
				bot3.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;				
			break;

			case 4:
				TutorialCanvas.SetActive(false);
				_Manager.GetComponent<_Manager>().prePartida = true;
			break;

			case 5:
				_Manager.GetComponent<_Manager>().posPartida = true;
			break;

		}
	}
}
