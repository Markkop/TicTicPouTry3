using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class ResetButtonScript : NetworkBehaviour {

	GameObject MyNetworkManager;
	GameObject _Manager;

	void Start()
	{
		MyNetworkManager = GameObject.Find("_NetworkManager");
		_Manager = GameObject.Find("_Manager");
	}

	void Update()
	{
	
	}

	public void ExitLevel()
	{
		//MyNetworkManager.GetComponent<Transform>().Find("canvasAll").gameObject.SetActive(true);
		//MyNetworkManager.GetComponent<MyNetworkManager>()._started = false;
	
		MyNetworkManager.GetComponent<MyNetworkManager>().offlineScene = "MainMenu";
		//MyNetworkManager.GetComponent<MyNetworkManager>().onlineScene = null;

		//SceneManager.LoadScene(0);
		NetworkManager.singleton.StopClient();
		NetworkManager.singleton.StopHost();
		NetworkServer.Reset();
		//NetworkManager.singleton.StopServer();

		//Debug.Log("Desconectando e resetando scene...");
	}

	public void ResetLevel()
	{		
		//int thisScene = SceneManager.GetActiveScene().buildIndex;

		//MyNetworkManager.GetComponent<Transform>().Find("canvasAll").gameObject.SetActive(true);
		//MyNetworkManager.GetComponent<MyNetworkManager>()._started = false;

		MyNetworkManager.GetComponent<MyNetworkManager>().offlineScene = null;
		MyNetworkManager.GetComponent<MyNetworkManager>().onlineScene = SceneManager.GetActiveScene().name;

		NetworkManager.singleton.StopClient();
		NetworkManager.singleton.StopHost();
		NetworkServer.Reset();
		_Manager.GetComponent<_Manager>().ResetaTudo();
		//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		NetworkManager.singleton.StartHost();

		//Limpa o playersArray do _Manager (ou aparece um monte de erro por MissingObject)
		if(_Manager.GetComponent<_Manager>().playersArray != null)
		{
			_Manager.GetComponent<_Manager>().playersArray.Clear();
	
		}


	}
}
