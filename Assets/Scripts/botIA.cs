using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class botIA : NetworkBehaviour {

//public GameObject _Manager;
public Atributos bot;

private bool allReady;

//private GameObject[] playersArray;
public List<GameObject> playersArray = new List<GameObject>();

public bool botDefensor; //Sempre defende
public bool botSanguinario; //Sempre ataca o player
public bool botOraculo; //Sanguinario, mas se o alvo for defender, mira em outro
public bool botAlvoEsperto; //Se for alvo de alguém, defende

public GameObject[] playersArraySecundario;

public List<GameObject> list = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
		//allReady = _Manager.GetComponent<_Manager>().allReady;
		allReady = false;
	}
	
	// Update is called once per frame
	void Update () {

		playersArray = bot.playersArray;
		// Se for LocalPlayer, retorna
		// O bot eh pra rodar apenas como server
		if (isLocalPlayer)
		{
			return;
		}

		// Se o bot estiver morto, retorna
		if(this.GetComponent<Atributos>().vidas <= 0)
		{
			return;
		}

		// Apenas defender
		if(botDefensor == true)
			BotDefensor();

		// Recarregar e atira sempre
		if(botSanguinario == true)
			BotSanguinario();

		// So atira em quem nao estiver defendendo
		// Se todos defenderem, entao defende.
		if(botOraculo == true)
			BotOraculo();

		//Se for alvo, defende.
		if(botAlvoEsperto == true)
			BotAlvoEsperto();

		

	}

	void BotDefensor()
	{
		//Enquanto ninguem estiver pronto
		if (allReady != true)
		{
			bot.vaiDefender = true;
			bot.ready = true;	
		}
	}

	void BotSanguinario()
	{
		if (allReady != true)
		{
			// Prioridade em recarregar pra ter 1 bala
			if(bot.balas < 1)
			{
				bot.vaiRecarregar = true;
				bot.ready = true;	
			}
			// 
			else
			{
				bot.alvo = PegaPlayer(); //Foca no player
				//bot.alvo = RandomMenosSiMesmo(); //Random

				bot.vaiAtirar = true;
				bot.vaiRecarregar = false;

				bot.ready = true;	

			}
		}
	}

	void BotOraculo()
	{
		if (allReady != true)
		{
			if(bot.balas < 1)
			{
				bot.vaiRecarregar = true;
				bot.ready = true;	
			}
			else
			{
				playersArraySecundario = GameObject.FindGameObjectsWithTag("Player");

				foreach (GameObject go in playersArraySecundario) //para cada player em jogo (incluindo bots)
				{
					if(go.GetComponent<Atributos>().vaiDefender == false)
					{
						if(go != gameObject)
						{
							if(go.GetComponent<Atributos>().vidas > 0)
							{
								if(!list.Contains(go))
								{
									list.Add(go); 	//Bota numa lista apenas players que
									//nao defendem e que nao eh si mesmo
								}
							}
							else
							{
								if(list.Contains(go))
								{
									list.Remove(go);
								}
							}
						}
					}
					else
					{
						list.Remove(go);
					}
				}
				if (list.Count != 0) // Se essa lista nao estiver vazia
				{
					bot.vaiAtirar = true;
					bot.vaiDefender = false;
					bot.alvo = list[Random.Range(0,list.Count)]; //Ataca alguem dela
					bot.ready = true;
				}
				else // Senao, defende
				{
					bot.vaiDefender = true;
					bot.vaiAtirar = false;
					bot.ready = true;						
				}
			}
		}
	}


	void BotAlvoEsperto()
	{
		if (allReady != true)
		{
			playersArraySecundario = GameObject.FindGameObjectsWithTag("Player");
			
			foreach (GameObject player in playersArraySecundario)
			{
				if(player.GetComponent<Atributos>().alvo == gameObject)
				{
					bot.vaiDefender = true;
					bot.vaiRecarregar = false;
					bot.vaiAtirar = false;
					bot.ready = true;
					return;
				}
				else
				{
					if(bot.balas < 1)
					{
						bot.vaiRecarregar = true;
						bot.ready = true;	
					}
					// 
					else
					{
						
						bot.alvo = RandomMenosSiMesmo(); //Random

						bot.vaiAtirar = true;
						bot.vaiDefender = false;
						bot.vaiRecarregar = false;
						bot.ready = true;	
					}
				}
			}
		}
	}

	GameObject RandomMenosSiMesmo()
	{
		//playersArray = GameObject.FindGameObjectsWithTag("Player");
		GameObject alvo0 = playersArray[Random.Range(0,playersArray.Count)];

		if(alvo0 == gameObject || alvo0.GetComponent<Atributos>().vidas <= 0) //Se o alvo for ele mesmo (inteligente hein)
		{
			//Debug.Log(this.name+" pegou si mesmo, tentando de novo...");
			return RandomMenosSiMesmo();
		}
		else
		{
			//Debug.Log(this.name+" pegou "+alvo0+" como alvo...");
			return alvo0;
		}
	}

	GameObject PegaPlayer()
	{
		//playersArray = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject go in playersArray) //para cada player em jogo
		{
			if(go.GetComponent<botIA>() == null && go.GetComponent<Atributos>().vidas > 0)
			{
				return go;
			}

		}
		return RandomMenosSiMesmo();
		
	}

	GameObject SemDefesa()
	{
		GameObject alvo0 = RandomMenosSiMesmo();

		if(alvo0.GetComponent<Atributos>().vaiDefender == false)
		{
			return alvo0;
		}
		else
		{
			return SemDefesa();
		}
		return null;
	}

}
