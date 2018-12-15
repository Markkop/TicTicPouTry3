using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;


public class ButtonCreator : NetworkBehaviour
{
    public GameObject alvosTextPrefab;
    public GameObject buttonPrefab;
    public GameObject panelToAttachButtonsTo1;
    public GameObject panelToAttachButtonsTo2;
    public ToggleGroup toggleGroup1;
    public ToggleGroup toggleGroup2;

    public GameObject[] playersArray;
    private string textButton;

    public List<int> buttonsList1 = new List<int>();
    public List<int> buttonsList2 = new List<int>();

    public GameObject playerz;



    public bool hasStuff = false;


	// Use this for initialization
	void Start () {

        //panelToAttachButtonsTo.SetActive(false);
        playersArray = GameObject.FindGameObjectsWithTag("Player");
        CriaBotoes(panelToAttachButtonsTo1, toggleGroup1);


    }

    void Update()
    {
        FazTudo(panelToAttachButtonsTo1, toggleGroup1);
        if(this.GetComponent<Atributos>().classe == 4)
        {
            FazTudo(panelToAttachButtonsTo2, toggleGroup2);
        }

    }

    public void FazTudo(GameObject panelToAttachButtonsTo, ToggleGroup toggleGroup)
    {
        //Se o numero de jogadores mudar, refaz os botoes (o -1 esta ali por causa do text "Alvos:")
        playersArray = GameObject.FindGameObjectsWithTag("Player");
         if(playersArray.Length != panelToAttachButtonsTo.GetComponent<Transform>().childCount-1)
        {
            Destroi(panelToAttachButtonsTo);
            CriaBotoes(panelToAttachButtonsTo, toggleGroup);            
        }

        //Atualiza nomes caso alguem mude de nome (gambiarra)
        foreach(Transform child in panelToAttachButtonsTo.transform)
        {
            if(child.GetComponent<alvoButton>() != null) //Para que nao faca isso com o texto "Alvos:"
            {
                //Se o alvo do botao não for igual ao texto do botao
                if(child.GetComponent<alvoButton>().alvo.name != child.GetChild(0).GetChild(0).GetComponent<Text>().text)
                {
                    //Se o alvo do botao, for o proprio player
                    if(child.GetComponent<alvoButton>().alvo == gameObject)
                    {
                        child.GetChild(0).GetChild(0).GetComponent<Text>().text = child.GetComponent<alvoButton>().alvo.name+" (self)";
                    }
                    else
                    {
                        //O texto do botao recebe o nome do alvo
                        child.GetChild(0).GetChild(0).GetComponent<Text>().text = child.GetComponent<alvoButton>().alvo.name;
                    }

                }
                
                //Se o alvo do botao estiver morto
                if(child.GetComponent<alvoButton>().alvo.GetComponent<Atributos>().vidas <= 0)
                {
                    //Desativa o botao
                    child.GetComponent<Toggle>().interactable = false;   
                }
            }
        }
    }

    public void CriaBotoes(GameObject panelToAttachButtonsTo, ToggleGroup toggleGroup)
    {
        GameObject textA = (GameObject)Instantiate(alvosTextPrefab);
        textA.transform.SetParent(panelToAttachButtonsTo.transform, false);

        foreach(GameObject player in playersArray)
        {
            GameObject alvo = player;

            textButton = alvo.name;
            if(player == gameObject)
            {
                textButton = alvo.name+" (self)";
            }
            

            //Cria o botao a partir do Prefab
            GameObject button = (GameObject)Instantiate(buttonPrefab);

            //Bota o botao no painel 
            button.transform.SetParent(panelToAttachButtonsTo.transform, false);

            //Pega o Toggle e bota numa variavel para facilitar o coding
            Toggle m_Toggle = button.GetComponent<Toggle>();
            

            //Adiciona um Listener onValueChanged no toggle.
            //Nao manjei mto bem do que eh um delegate, peguei como exemplo
            //Passa o toggle em questao e o alvo
            if(panelToAttachButtonsTo == panelToAttachButtonsTo1)
            {
                button.GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                    CmdToggleValueChanged(m_Toggle.isOn, alvo);
                });    
            }
            else
            {
                textA.GetComponent<Text>().text = "Tiro Duplo:";
                button.GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                    CmdToggleValueChanged2(m_Toggle.isOn, alvo);
                });
            }
            //Pega o primeiro Child do primeiro Child do objeto (botao), que eh Text, e muda ele
            button.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = textButton;

            //Bota o Toggle no grupo de Toggles
            button.GetComponent<Toggle>().group = toggleGroup;

            button.GetComponent<alvoButton>().alvo = alvo;

            if(player == gameObject)
            {
                button.GetComponent<Toggle>().interactable = false;
            }

        

       }        
    }

    public void AtivaPainel()
    {
        //panelToAttachButtonsTo.SetActive(true);
    }

    public void Desativa()
    {	//panelToAttachButtonsTo
    	foreach (Transform child in panelToAttachButtonsTo1.transform)
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

    public void Destroi(GameObject panelToAttachButtonsTo)
    {   
        foreach (Transform child in panelToAttachButtonsTo.transform){
            GameObject.Destroy(child.gameObject);
        }

    }
    
    [Command]
    public void CmdToggleValueChanged(bool change, GameObject alvo)
    {
        //Debug.Log("Botao de alvo clicado.." +change);
        playerz.GetComponent<Atributos>().vaiAtirar = true;
        playerz.GetComponent<Atributos>().atiraButton.isOn = true;

        if(change == true)
        {
           //Debug.Log("Player recevendo alvo: "+alvo.name);
            playerz.GetComponent<Atributos>().alvo = alvo;
        }
        else
        {
            //Debug.Log("Player resetando alvo");
            playerz.GetComponent<Atributos>().alvo = null;   
        }
    }

    [Command]
    public void CmdToggleValueChanged2(bool change, GameObject alvo)
    {
        //Debug.Log("Botao de alvo clicado.." +change);
        playerz.GetComponent<Atributos>().vaiAtirar = true;
        playerz.GetComponent<Atributos>().atiraButton.isOn = true;

        if(change == true)
        {
           //Debug.Log("Player recevendo alvo: "+alvo.name);
            playerz.GetComponent<Atributos>().segundoAlvo = alvo;
        }
        else
        {
            //Debug.Log("Player resetando alvo");
            playerz.GetComponent<Atributos>().segundoAlvo = null;   
        }
    }

    void CmdPedeAlvo(GameObject target)
    {
        //Debug.Log("Pedindo alvo chamado " +target.name);
        playerz.GetComponent<Atributos>().alvo = target;
    }


}
