using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
}
