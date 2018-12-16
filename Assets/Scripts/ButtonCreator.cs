using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;


public class ButtonCreator : NetworkBehaviour
{
    public GameObject _Manager;
    public GameObject alvosTextPrefab;
    public GameObject buttonPrefab;
    public GameObject panelToAttachButtonsTo1;
    public GameObject panelToAttachButtonsTo2;
    public ToggleGroup toggleGroup1;
    public ToggleGroup toggleGroup2;
    public GameObject alvoRepetido1;
    public GameObject alvoRepetido2;



    //public GameObject[] playersArray;
    public List<GameObject> playersArray = new List<GameObject>();
    private string textButton;

    public List<int> buttonsList1 = new List<int>();
    public List<int> buttonsList2 = new List<int>();

    public GameObject playerz;



    public bool hasStuff = false;


	// Use this for initialization
	void Start () {

        //Pega o Manager da cena
        _Manager = GameObject.FindWithTag("Manager");

        //Popula o playersArray de acordo com o do Manager (p/ evitar o FindGameObjectsWithTag)
        playersArray = _Manager.GetComponent<_Manager>().playersArray;

        CriaBotoes(panelToAttachButtonsTo1, toggleGroup1);
    }

    void Update()
    {
        FazTudo(panelToAttachButtonsTo1, toggleGroup1);

        //Se for cangaceiro, faz o mesmo com o segundo painel
        if(this.GetComponent<Atributos>().classe == 4)
        {
            FazTudo(panelToAttachButtonsTo2, toggleGroup2);
        }

    }

    public void FazTudo(GameObject panelToAttachButtonsTo, ToggleGroup toggleGroup)
    {
        //Re-popula o playersArray
        playersArray = _Manager.GetComponent<_Manager>().playersArray;

        //Se o numero de jogadores mudar, refaz os botoes (o -1 esta ali por causa do text "Alvos:")
         if(playersArray.Count != panelToAttachButtonsTo.GetComponent<Transform>().childCount-1)
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
                if(child.GetComponent<alvoButton>().alvo.name != child.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text)
                {
                    //Se o alvo do botao, for o proprio player
                    if(child.GetComponent<alvoButton>().alvo == gameObject)
                    {
                        child.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = child.GetComponent<alvoButton>().alvo.name+" (self)";
                    }
                    else
                    {
                        //O texto do botao recebe o nome do alvo
                        child.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = child.GetComponent<alvoButton>().alvo.name;
                    }
                }
                
                //Para o primeiro painel de alvos
                if(panelToAttachButtonsTo == panelToAttachButtonsTo1)
                {
                    //Se o alvo do botao estiver morto ou for o próprio player
                    if(child.GetComponent<alvoButton>().alvo.GetComponent<Atributos>().vidas <= 0 || child.GetComponent<alvoButton>().alvo == gameObject)
                    {
                        //Desativa o botão
                        AtivaDesativaBotao(child, false);
                    }
                    //Caso não seja o caso
                    else
                    {   
                        //Se o alvo do botao desse painel já tiver sido escolhido no outro painel
                        if(child.GetComponent<alvoButton>().alvo == this.GetComponent<Atributos>().segundoAlvo)
                        {
                            //Desativa o botão
                            AtivaDesativaBotao(child, false);
                        }
                        //Caso contrário
                        else
                        {
                            //Reativa o botão
                            AtivaDesativaBotao(child, true);
                        }
                    }
                }

                //Faz o mesmo procedimento para o segundo painel (de nada, Cangaceiros)
                if(panelToAttachButtonsTo == panelToAttachButtonsTo2)
                {
                    if(child.GetComponent<alvoButton>().alvo.GetComponent<Atributos>().vidas <= 0 || child.GetComponent<alvoButton>().alvo == gameObject)
                    {
                        AtivaDesativaBotao(child, false);
                    }
                    else
                    {
                        if(child.GetComponent<alvoButton>().alvo == this.GetComponent<Atributos>().alvo)
                        {
                            AtivaDesativaBotao(child, false);
                        }
                        else
                        {
                            AtivaDesativaBotao(child, true);
                        }
                    }
                }
            }
        }
    }

    void AtivaDesativaBotao(Transform childs, bool OnOff)
    {
        //Memoriza a cor (r,g,b,a) do texto do botao
        var cor = childs.gameObject.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().color;
        if(OnOff == false)
        {
            //Diminui a transparencia
            cor.a = 0.2f;    
        }
        else
        {   
            //Restaura a transparência
            cor.a = 1f;
        }
        //Aplica a mudança
        childs.gameObject.GetComponent<Transform>().GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().color = cor;
        childs.GetComponent<Toggle>().interactable = OnOff;

    }

    public void CriaBotoes(GameObject panelToAttachButtonsTo, ToggleGroup toggleGroup)
    {
        //Pega o texto "Alvos:" no prefab
        GameObject textA = (GameObject)Instantiate(alvosTextPrefab);
        textA.transform.SetParent(panelToAttachButtonsTo.transform, false);

        //Cria um botao para cada player
        foreach(GameObject player in playersArray)
        {
            GameObject alvo = player;
            textButton = alvo.name;
            
            //Cria o botao a partir do Prefab
            GameObject button = (GameObject)Instantiate(buttonPrefab);

            //Bota o botao no painel 
            button.transform.SetParent(panelToAttachButtonsTo.transform, false);

            //Pega o Toggle e bota numa variavel para facilitar o coding
            Toggle m_Toggle = button.GetComponent<Toggle>();

            //Para o botão que for do próprio jogador
            if(player == gameObject)
            {
                //Adiciona self no final
                textButton = alvo.name+" (self)";

                //Na real, desativa
                button.SetActive(false);
            }
            

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
                textA.GetComponent<TextMeshProUGUI>().text = "Tiro Duplo:";
                button.GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                    CmdToggleValueChanged2(m_Toggle.isOn, alvo);
                });
            }
            //Pega o primeiro Child do segundo Child do objeto (botao), que eh Text, e muda ele
            button.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = textButton;

            //Bota o Toggle no grupo de Toggles
            button.GetComponent<Toggle>().group = toggleGroup;

            //Bota o player gameobject em questão como objeto alvo do botão
            button.GetComponent<alvoButton>().alvo = alvo;
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
