using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;


public class ButtonCreator : NetworkBehaviour
{
    public GameObject buttonPrefab;
    public GameObject panelToAttachButtonsTo;
    public ToggleGroup toggleGroup;

    public GameObject[] playersArray;
    private string textButton;

    public List<int> buttonsList = new List<int>();

    public GameObject playerz;



    public bool hasStuff = false;


	// Use this for initialization
	void Start () {

        //panelToAttachButtonsTo.SetActive(false);
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
            if(child.GetComponent<alvoButton>().alvo.ToString() != child.GetChild(0).GetChild(0).GetComponent<Text>().text)
            {
                Debug.Log("opa");
                child.GetChild(0).GetChild(0).GetComponent<Text>().text = child.GetComponent<alvoButton>().alvo.ToString();

            }
        }

    }

    public void CriaBotoes()
    {
        foreach(GameObject player in playersArray)
        {
            GameObject alvo = player;
            textButton = alvo.name;

            //Cria o botao a partir do Prefab
            GameObject button = (GameObject)Instantiate(buttonPrefab);

            //Bota o botao no painel 
            button.transform.SetParent(panelToAttachButtonsTo.transform);

            //Pega o Toggle e bota numa variavel para facilitar o coding
            Toggle m_Toggle = button.GetComponent<Toggle>();
            

            //Adiciona um Listener onValueChanged no toggle.
            //Nao manjei mto bem do que eh um delegate, peguei como exemplo
            //Passa o toggle em questao e o alvo
            button.GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                CmdToggleValueChanged(m_Toggle.isOn, alvo);
            });

            //Pega o primeiro Child do primeiro Child do objeto (botao), que eh Text, e muda ele
            button.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = textButton;

            //Bota o Toggle no grupo de Toggles
            button.GetComponent<Toggle>().group = toggleGroup;

            button.GetComponent<alvoButton>().alvo = alvo;
        

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
    public void CmdToggleValueChanged(bool change, GameObject alvo)
    {
        Debug.Log("Botao de alvo clicado.." +change);
        playerz.GetComponent<Atributos>().vaiAtirar = true;
        playerz.GetComponent<Atributos>().atiraButton.isOn = true;

        if(change == true)
        {
            Debug.Log("Player recevendo alvo: "+alvo.name);
            playerz.GetComponent<Atributos>().alvo = alvo;
        }
        else
        {
            Debug.Log("Player resetando alvo");
            playerz.GetComponent<Atributos>().alvo = null;   
        }
        //playerz.GetComponent<Atributos>().alvo = this.GetComponent<alvoButton>().alvo;

        // if(playerz.GetComponent<Atributos>().alvo != change.GetComponent<alvoButton>().alvo)
        // {
        //    Debug.Log("To alvo");
        //    playerz.GetComponent<Atributos>().alvo = change.GetComponent<alvoButton>().alvo;
        // }
        // else
        // {   Debug.Log("To null");
        //     playerz.GetComponent<Atributos>().alvo = null;
        // }


    	

    }

    void CmdPedeAlvo(GameObject target)
    {
        Debug.Log("Pedindo alvo chamado " +target.name);
        playerz.GetComponent<Atributos>().alvo = target;
    }


}
