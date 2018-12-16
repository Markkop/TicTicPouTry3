using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSplatter : MonoBehaviour {

	public Image SangueImagem;
	public Color SangueCor;
	public int SangueSpeed;
	public bool damaged;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if(this.GetComponent<botIA>() == null)
		{
			SangueTela();
		}
		
	}

	public void AtivaDamaged()
	{
		Debug.Log("damaged foi ativado");
		damaged = true;
	}

	void SangueTela()
	{
		if (damaged)
		{
			SangueImagem.color = SangueCor;
		}
		else
		{
			SangueImagem.color = Color.Lerp(SangueImagem.color, Color.clear, SangueSpeed * Time.deltaTime);
		}
		damaged = false;
	}
}
