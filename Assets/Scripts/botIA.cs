using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class botIA : NetworkBehaviour {

//public GameObject _Manager;
public Atributos bot;

private bool allReady;
private GameObject[] playersArray;

public bool botDefensor; //Sempre defende
public bool botSanguinario; //Sempre ataca o player
public bool botOraculo; //Sanguinario, mas se o alvo for defender, mira em outro

	// Use this for initialization
	void Start () {
		
		//allReady = _Manager.GetComponent<_Manager>().allReady;
		allReady = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (isLocalPlayer)
		{
			return;
		}
		else
		{
			BotIA();
		}

	}


	void BotIA()
	{
		if(this.GetComponent<Atributos>().vidas == 0)
		{
			return;
		}

		if (botDefensor == true)
		{
			if (allReady != true)
			{
				bot.vaiDefender = true;
				bot.ready = true;	
			}
		}

		if (botSanguinario == true) // Sempre ataca alguem 
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
					bot.vaiAtirar = true;
					bot.vaiRecarregar = false;

					playersArray = GameObject.FindGameObjectsWithTag("Player");

					bot.alvo = PegaPlayer(); //Foca no player
					//bot.alvo = RandomMenosSiMesmo(); //Random
					bot.ready = true;	

				}
			}
		}

		if (botOraculo == true) // Soh atira em que nao estiver defendendo. Se nao houver: defende
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
					playersArray = GameObject.FindGameObjectsWithTag("Player");
					List<GameObject> list = new List<GameObject>();

					foreach (GameObject go in playersArray) //para cada player em jogo
					{
						if(go != gameObject && go.GetComponent<Atributos>().vaiDefender == false)
						{
							list.Add(go); 	//Bota numa lista apenas players que
											//nao defendem e nao sao o objeto com o script botIA
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
	}



	GameObject RandomMenosSiMesmo()
	{
		playersArray = GameObject.FindGameObjectsWithTag("Player");
		GameObject alvo0 = playersArray[Random.Range(0,playersArray.Length)];

		if(alvo0 == gameObject) //Se o alvo for ele mesmo (inteligente hein)
		{
			return RandomMenosSiMesmo();
		}
		else
		{
			return alvo0;
		}
	}

	GameObject PegaPlayer()
	{
		playersArray = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject go in playersArray) //para cada player em jogo
		{
			if(hasAuthority && go.GetComponent<Atributos>().vidas != 0)
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
