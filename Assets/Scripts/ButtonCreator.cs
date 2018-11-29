using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;


public class ButtonCreator : NetworkBehaviour
{
    public GameObject buttonPrefab;
    public GameObject panelToAttachButtonsTo;

    public GameObject[] playersArray;
    private string textButton;

    public GameObject player;


    public bool hasStuff = false;


	// Use this for initialization
	void Start () {

        //this.GetComponent<NetworkIdentity>().AssignClientAuthority ( connectionToClient );
	}


    public void Cria()//Creates a button and sets it up
    {
        player.GetComponent<Atributos>().alvosPanel.gameObject.SetActive (true);
    	if(!hasStuff)
    	{
	    	playersArray = GameObject.FindGameObjectsWithTag("Player");
	    	for(int i = 0; i < playersArray.Length; i++) //O ideal deve ser usar "foreach"
	    	{

                Debug.Log("Criou botao");
	    		textButton = playersArray[i].name; //Nome do player 
	  			GameObject alvo = playersArray[i]; 

		        GameObject button = (GameObject)Instantiate(buttonPrefab);
                //GameObject button = Instantiate(buttonPrefab);

                //NetworkServer.SpawnWithClientAuthority(button, connectionToClient);

		        button.transform.SetParent(panelToAttachButtonsTo.transform);//Setting button parent
		        button.GetComponent<Button>().onClick.AddListener(() => CmdOnClick(alvo));//Setting what button does when clicked

				//Next line assumes button has child with text as first gameobject like button created from GameObject->UI->Button
		        button.transform.GetChild(0).GetComponent<Text>().text = textButton;//Changing text	
		        hasStuff = true;
    	    }
    	}
    	
    }

    public void Destroi()
    {	//panelToAttachButtonsTo
    	foreach (Transform child in panelToAttachButtonsTo.transform){
    		GameObject.Destroy(child.gameObject);
    		hasStuff = false;
    	}

    }
    
    [Command]
    public void CmdOnClick(GameObject target)
    {
        Debug.Log("Botao de alvo clicado..");
    	CmdPedeAlvo(target);

    }

    void CmdPedeAlvo(GameObject target)
    {
        Debug.Log("Pedindo alvo chamado " +target.name);
        player.GetComponent<Atributos>().alvo = target;
    }


}
