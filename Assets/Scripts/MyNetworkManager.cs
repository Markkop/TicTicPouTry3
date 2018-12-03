using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager {

public GameObject hosta;
public GameObject clienta;
public GameObject canvas;


	public void Host()
	{
		base.StartHost();

	}

	public void Client()
	{
		base.StartClient();
	}
}
