using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class MyNetworkManager : NetworkManager {

public GameObject hosta;
public GameObject clienta;
public GameObject canvas;
public GameObject CenarioObject;
public bool _started;

	public void Start()
	{
		_started = false;
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
}
