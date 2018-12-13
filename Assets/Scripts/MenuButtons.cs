using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;


public class MenuButtons : MonoBehaviour {

public bool _started;

	public void Start()
	{
		_started = false;
	}

	public void Update()
	{
		this.GetComponent<Transform>().Find("OptionsPanel/RitmoCanvas/RitmoText").GetComponent<TextMeshProUGUI>().text = "Velocidade: "+Settings.newRitmo;
		this.GetComponent<Transform>().Find("OptionsPanel/VidasCanvas/VidasText").GetComponent<TextMeshProUGUI>().text = "Vidas Iniciais: "+Settings.startingVidas;
		this.GetComponent<Transform>().Find("OptionsPanel/BalasCanvas/BalasText").GetComponent<TextMeshProUGUI>().text = "Balas Iniciais: "+Settings.startingBalas;
	}

	public void Host()
	{
		if(!_started)
		{
			NetworkManager.singleton.StartHost();
			_started = true;
		}
	}

	public void Client()
	{
		if(_started)
		{
			NetworkManager.singleton.StartClient();
		}
	}

	public void ChangeRitmo(float tempoMax)
	{
		Settings.newRitmo = tempoMax;
	}

	public void ChangeVidas(float startVida)
	{
		Settings.startingVidas = (int)Mathf.Round(startVida);
	}

	public void ChangeBalas(float startBala)
	{
		Settings.startingBalas = (int)Mathf.Round(startBala);
	}

}

public static class Settings{

	public static float newRitmo = 4;
	public static int startingVidas = 2;
	public static int startingBalas = 2;
}

