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

    public List<int> buttonsList = new List<int>();

    public GameObject playerz;



    public bool hasStuff = false;


	// Use this for initialization
	void Start () {

        panelToAttachButtonsTo.SetActive(false);
        playersArray = GameObject.FindGameObjectsWithTag("Player");
        CriaBotoes();


    }

    void Update()
    {
        playersArray = GameObject.FindGameObjectsWithTag("Player");
        if(playersArray.Length != panelToAttachButtonsTo.GetComponent<Transform>().childCount)
        {
            Destroi();
            CriaBotoes();            
        }


        foreach(Transform child in panelToAttachButtonsTo.transform)
        {
            if(playerz.GetComponent<Atributos>().alvo.name == child.transform.GetChild(0).GetComponent<Text>().text)
            {
                
                ColorBlock cb = child.GetComponent<Button>().colors;
                cb.normalColor = new Vector4(0f,10f,0f,10f);
                child.GetComponent<Button>().colors = cb;
            }
        }

    }

    public void CriaBotoes()
    {
        foreach(GameObject player in playersArray)
        {
            GameObject alvo = player;
            textButton = alvo.name;

            GameObject button = (GameObject)Instantiate(buttonPrefab);

            button.transform.SetParent(panelToAttachButtonsTo.transform);// Defina como Parent o panelAlvos
            button.GetComponent<Button>().onClick.AddListener(() => CmdOnClick(alvo));// Adiciona funcao no click

            //Pega o primeiro Child do objeto (botao), que eh Text, e muda ele
            button.transform.GetChild(0).GetComponent<Text>().text = textButton;

       }        
    }

    public void AtivaPainel()
    {
        panelToAttachButtonsTo.SetActive(true);
    }

    public void Desativa()
    {	//panelToAttachButtonsTo
    	foreach (Transform child in panelToAttachButtonsTo.transform)
        {
    		foreach(GameObject player in playersArray)
            {
                if (player.name == child.GetComponent<Text>().text && player.GetComponent<Atributos>().vidas == 0)
                {
                    child.GetComponent<Toggle>().interactable = false;
                }
            }
    		
    	}

    }

    public void Destroi()
    {   
        foreach (Transform child in panelToAttachButtonsTo.transform){
            GameObject.Destroy(child.gameObject);
        }

    }
    
    [Command]
    public void CmdOnClick(GameObject target)
    {
        Debug.Log("Botao de alvo clicado..");
        if(playerz.GetComponent<Atributos>().alvo == target)
        {
            playerz.GetComponent<Atributos>().alvo = null;
            return;    
        }
        else
        {
            CmdPedeAlvo(target);    
        }


    	

    }

    void CmdPedeAlvo(GameObject target)
    {
        Debug.Log("Pedindo alvo chamado " +target.name);
        playerz.GetComponent<Atributos>().alvo = target;
    }


}
