using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;


public class MenuButtons : MonoBehaviour {

public GameObject MyNetworkManager;
public bool _started;
public GameObject HostClientPanel;
public GameObject LevelSelectPanel;
public TMP_Dropdown LevelSelectDropdown;
public int FaseSelecionada;
public string textRitmo = "Normal";

	public void Start()
	{
		MyNetworkManager = GameObject.Find("_NetworkManager");
		_started = false;
	}

	public void Update()
	{
		//SceneManager.LoadSceneAsync(2);
		//Debug.Log(SceneManager.GetSceneByBuildIndex(2).name);

		this.GetComponent<Transform>().Find("OptionsPanel/RitmoPanel/RitmoText").GetComponent<TextMeshProUGUI>().text = "Velocidade: "+textRitmo;
		this.GetComponent<Transform>().Find("OptionsPanel/VidasPanel/VidasText").GetComponent<TextMeshProUGUI>().text = "Vidas Iniciais: "+Settings.startingVidas;
		this.GetComponent<Transform>().Find("OptionsPanel/BalasPanel/BalasText").GetComponent<TextMeshProUGUI>().text = "Balas Iniciais: "+Settings.startingBalas;
	}

	public void Host(string SceneName)
	{
		if(!_started)
		{
			MyNetworkManager.GetComponent<MyNetworkManager>().onlineScene = SceneName;
			NetworkManager.singleton.StartHost();
			//SceneManager.LoadScene(FaseSelecionada);
			_started = true;
		}
	}

	public void Client()
	{
		if(_started)
		{
			//MyNetworkManager.GetComponent<MyNetworkManager>().onlineScene = "CustomGame";
			NetworkManager.singleton.StartClient();
		}
	}

	public void ChangeRitmo(float valor)
	{
		//Foi-se pensado em fazer apenas com valores inteiros para ficar: (Padrão: 1s entre tempos; 4 tempos)
		//newRitmo = 1, velocidade normal, 		1s entre cada tempo 	(1/1)
		//newRitmo = 2, dobro da velocidade, 	0.5s entre cada tempo 	(1/2)
		//newRitmo = 3, triplo da velocidade, 	0.33s entre cada tempo 	(1/3)

		//Porém, para ter uma faixa maior de opções, incluindo uma opção mais lenta, dividiu-se o valor por 2, ficando:
		//newRitmo = 2, velocidade normal, 		1s entre cada tempo 	(1/2 * 2)
		//newRitmo = 4, dobro da velocidade,	0.5s entre cada tempo 	(1/4 * 2)
		//newRitmo = 1, metade da velocidade, 	2s entre cada tempo 	(1/1 * 2)
		Settings.newRitmo = valor/2;

		//Todos valores
		//newRitmo = 1, metade (+ lento), 		2s 		entre cada tempo 	(1/1 * 2)
		//newRitmo = 2, padrão, 				1s 		entre cada tempo 	(1/2 * 2)
		//newRitmo = 3, um pouco + rapido	 	0.66s 	entre cada tempo 	(1/3 * 2)
		//newRitmo = 4, dobro, 					0.5s	entre cada tempo 	(1/4 * 2)
		//newRitmo = 5, mais rápido, 			0.4s 	entre cada tempo 	(1/5 * 2)
		//Obs: se newRitmo = 0, os turnos acontecem apenas quando todos os jogadores clicarem em Ready

		switch((int)valor)
		{
			case 0:
				textRitmo = "Sem tempo";
			break;
			case 1:
				textRitmo = "Lento";
			break;
			case 2:
				textRitmo = "Normal";
			break;
			case 3:
				textRitmo = "Rápido";
			break;
			case 4:
				textRitmo = "Mais rápido";
			break;
			case 5:
				textRitmo = "SonicMode";
			break;
		}

	}

	public void ChangeVidas(float startVida)
	{
		Settings.startingVidas = (int)Mathf.Round(startVida);
	}

	public void ChangeBalas(float startBala)
	{
		Settings.startingBalas = (int)Mathf.Round(startBala);
	}

	public void ChangeScene(int scene)
	{
		//Host();
		SceneManager.LoadScene(scene);
		//Se for pro Tutorial

	}

	public void MostraHostClient()
	{
		if(HostClientPanel.activeSelf == true)
		{
			HostClientPanel.SetActive(false);
			return;
		}
		if(HostClientPanel.activeSelf == false)
		{
			HostClientPanel.SetActive(true);
			return;
		}
	}

	public void MostraLevelSelector()
	{
		if(LevelSelectPanel.activeSelf == true)
		{
			LevelSelectPanel.SetActive(false);
			return;
		}
		if(LevelSelectPanel.activeSelf == false)
		{
			LevelSelectPanel.SetActive(true);
			return;
		}
	}


	public void IrParaFase()
	{
		string fase = "";
		switch(LevelSelectDropdown.value)
		{
			case 0: //Tutorial (Scene 2)
				FaseSelecionada = 2;
				fase = "Tutorial"; //Nome da Scene
			break;
			case 1: //Fase 1 (Boss Teste, Scene 3)
				FaseSelecionada = 3;
				fase = "Boss1";
			break;
		}
		Host(fase);
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void ChangePlayerName(string nome)
	{
		Settings.playerName = nome;
	}



}

public static class Settings{

	public static float newRitmo = 0;
	public static int startingVidas = 2;
	public static int startingBalas = 2;
	public static string playerName = "";
}

