using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager {

public GameObject hosta;
public GameObject clienta;
public GameObject canvas;
public GameObject CenarioObject;


	public void Host()
	{
		base.StartHost();

	}

	public void Client()
	{
		base.StartClient();
	}

	//Nao esta sendo utilizado
	public void Cenario()
	{
		if(CenarioObject.activeSelf == false)
		{
			CenarioObject.SetActive(true);
		}
		else
		{
			CenarioObject.SetActive(false);	
		}
	}

	public void Reset()
	{
		GameObject MyNetworkManager = GameObject.Find("_NetworkManager");
		MyNetworkManager.GetComponent<Transform>().Find("canvasAll").gameObject.SetActive(true);
		Debug.Log("Desconectando e resetando scene...");
		SceneManager.LoadScene(0);
		NetworkManager.singleton.StopClient();
		NetworkManager.singleton.StopHost();
		NetworkManager.singleton.StopServer();
	}
}
