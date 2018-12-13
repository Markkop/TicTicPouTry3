using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class ResetButtonScript : NetworkBehaviour {

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
