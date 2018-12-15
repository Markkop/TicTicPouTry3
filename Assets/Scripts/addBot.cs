using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;


public class addBot : NetworkBehaviour {

	public GameObject botPrefab;
	public GameObject botInfoPrefab;
	public GameObject parentCanvas;
	public Toggle botInfoToggle;

	public GameObject infoPanel;


	// Use this for initialization
	void Start () { }
	
	// Update is called once per frame
	void Update () { 

	}

	// Lista de bots
	// 1 - Defensor
	// 2 - Sanguinario
	// 3 - Oraculo
	// 4 - AlvoEsperto


	[Command]
	public void CmdSpawnBot(int bot)
	{
		if(!isServer)
		{
			Debug.Log(this.name+" tentou invocar um bot, mas nao eh server");
			return;
		}

		GameObject go = (GameObject)Instantiate(botPrefab);
		NetworkServer.Spawn(go);
		
		switch(bot){
			case 1:
			go.GetComponent<botIA>().botDefensor = true;
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;
			RpcSpawnBot(go, Color.blue); //Avisa outros clients dessa mudança
			break;

			case 2:
			go.GetComponent<botIA>().botSanguinario = true;
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
			RpcSpawnBot(go, Color.red);
			break;

			case 3:
			go.GetComponent<botIA>().botOraculo = true;			
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
			RpcSpawnBot(go, Color.white);
			break;

			case 4:
			go.GetComponent<botIA>().botAlvoEsperto = true;			
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.green;
			RpcSpawnBot(go, Color.green);
			break;

			case 5:
			go.GetComponent<botIA>().botRDRA = true;			
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.grey;
			RpcSpawnBot(go, Color.grey);
			break;

			case 6:
			go.GetComponent<botIA>().botMagoSanguinario = true;			
			go.GetComponent<Atributos>().classe = 1;
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.magenta;
			RpcSpawnBot(go, Color.magenta);
			break;

			case 7:
			go.GetComponent<botIA>().botSamuraiMedroso = true;			
			go.GetComponent<Atributos>().classe = 2;
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
			RpcSpawnBot(go, Color.white);
			break;
		}
		

	}

	[ClientRpc]
	public void RpcSpawnBot(GameObject bot, Color cor)
	{
		bot.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = cor;
	}

	public void BotInfo(bool onoff)
	{
		Debug.Log("BotInfo clicado");
		if(onoff == !botInfoToggle.isOn)
		{
			infoPanel = (GameObject)Instantiate(botInfoPrefab);
			infoPanel.transform.SetParent(parentCanvas.transform, false);

			infoPanel.GetComponent<Transform>().GetChild(0).GetComponent<TextMeshProUGUI>().text = 
			"<#ff0000ff>botSanguinario:</color> Recarrega e Ataca, priorizando o player\n"+
			"<#add8e6ff>botDefensor:</color> defende todo turno (imortal)\n"+
			"<#00ffffff>botOraculo:</color> não atira em quem for defender; se todos defenderem, defende também.\n"+
			"<#00ff00ff>botAlvoEsperto:</color> se for alvo de alguém, defende; caso contrário atira em alguém random (imortal)\n"+
			"<#808080ff>botRDRA:</color> Recarrega, Defende, Atira, Recarrega e repete\n"+
			"<#800080ff>botMagoSanguinario:</color> Mago que carrega e explode continuamente\n"+
			"<#add8e6ff>botSamuraiMedroso:</color> Samurai que carrega e contra-ataca continuamente\n";

			infoPanel.GetComponent<Transform>().GetChild(1).GetComponent<Button>().onClick.AddListener(delegate {OkButton();});

			Debug.Log("BotInfo clicado TRUE");
		}
		else
		{
			Debug.Log("BotInfo clicado FALSE");
			Destroy(infoPanel);
		}
		
	}

	public void OkButton()
	{
		botInfoToggle.isOn = false;
		Destroy(infoPanel);
	}


}

