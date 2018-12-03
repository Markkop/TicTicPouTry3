using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MenuButtons : NetworkBehaviour {

public GameObject nman;
public GameObject hosta;
public GameObject clienta;
public GameObject canvas;

  	void Awake () {
         DontDestroyOnLoad(hosta);
         DontDestroyOnLoad(clienta);
         DontDestroyOnLoad(canvas);
     }

	// Use this for initialization
	void Start () {

		hosta.SetActive(true);
		clienta.SetActive(true);
		
	}
	
	// Update is called once per frame
	void Update () {

		hosta.SetActive(true);
		clienta.SetActive(true);
		canvas.SetActive(true);
			
	}

	public void Host()
	{
		nman.GetComponent<NetworkManager>().StartHost();

	}

	public void Client()
	{
		nman.GetComponent<NetworkManager>().StartClient();
	}
}
