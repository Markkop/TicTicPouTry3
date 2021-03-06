﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour {

	public AudioClip shotSound;
	public AudioClip reloadSound;
	public AudioClip painSound;
	public AudioClip blockSound;
	public AudioClip explosaoSound;
	public AudioClip carregaKadabraSound;
	public AudioClip swordSheathe;
	public AudioClip swordKatchin;
	public AudioClip ohhSound;
	public AudioClip rock1;
	public AudioClip rock2;
	public AudioClip rock3;
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

	void CarregaKadabraSound()
	{
		audioSource.PlayOneShot(carregaKadabraSound);
	}

	void ExplosaoSound()
	{
		audioSource.PlayOneShot(explosaoSound);
	}

		void SwordSheathe()
	{
		audioSource.PlayOneShot(swordSheathe);
	}

		void SwordKatchin()
	{
		audioSource.PlayOneShot(swordKatchin);
	}

		void OhhSound()
	{
		audioSource.PlayOneShot(ohhSound);
	}

	public void Rock1()
	{
		audioSource.PlayOneShot(rock1);
	}

	public void Rock2()
	{
		audioSource.PlayOneShot(rock2);
	}

	public void Rock3()
	{
		audioSource.PlayOneShot(rock3);
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
