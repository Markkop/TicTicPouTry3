using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour {

	public AudioClip shotSound;
	public AudioClip reloadSound;
	public AudioClip painSound;
	public AudioClip blockSound;
	public AudioSource audioSource;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void TiroSound()
	{
		audioSource.PlayOneShot(shotSound);
	}

	void ReloadSound()
	{
		audioSource.PlayOneShot(reloadSound);
	}

	public void PainSound()
	{
		audioSource.PlayOneShot(painSound);
	}

	public void BlockSound()
	{
		audioSource.PlayOneShot(blockSound);
	}


}
