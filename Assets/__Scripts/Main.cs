﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {
	static public Main S;

	static public Dictionary<WeaponType,WeaponDefinition> W_DEFS;

	public GameObject[] prefabEnemies;
	public float enemySpawnPerSecond=0.5f;
	public float enemySpawnPadding=1.5f;
	public WeaponDefinition[] weaponDefinitions;

	public GameObject prefabPowerUp;
	public WeaponType[] powerUpFrequency=new WeaponType[]{
		WeaponType.blaster, WeaponType.blaster, 
		WeaponType.spread, WeaponType.shield
	};

	public bool _________________________;

	public void ShipDestroyed(Enemy e){
		//Potentially generate powerup
		if (Random.value <= e.powerUpDropChance) {
			int ndx = Random.Range (0, powerUpFrequency.Length);
			WeaponType puType = powerUpFrequency [ndx];

			//spwan power up
			GameObject go=Instantiate(prefabPowerUp) as GameObject;
			PowerUp pu = go.GetComponent<PowerUp> ();

			pu.SetType (puType);

			pu.transform.position = e.transform.position;
		}
	}

	public WeaponType[] activeWeaponTypes;

	public float enemySpawnRate;//delay between enemy spawns

	void Awake(){
		S = this;
		//Set Utils.cam bounds
		Utils.SetCameraBounds(this.GetComponent<Camera>());
		enemySpawnRate = 1f / enemySpawnPerSecond;
		Invoke ("SpawnEnemy", enemySpawnRate);

		W_DEFS = new Dictionary<WeaponType,WeaponDefinition> ();
		foreach (WeaponDefinition def in weaponDefinitions) {
			W_DEFS [def.type] = def;
		}
	}
	static public WeaponDefinition GetWeaponDefinition(WeaponType wt){
		//make sure key exists in dictionary
		if (W_DEFS.ContainsKey (wt)) {
			return(W_DEFS [wt]);
		}
		return(new WeaponDefinition ());
	}

	void Start(){
		activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
		for (int i = 0; i < weaponDefinitions.Length; i++) {
			activeWeaponTypes [i] = weaponDefinitions [i].type;
		}
	}

	public void SpawnEnemy(){
		//pick random enemy to instantiate
		int ndx=Random.Range(0,prefabEnemies.Length);
		GameObject go = Instantiate (prefabEnemies [ndx]) as GameObject;
		//position enemy above the screen with a random x position
		Vector3 pos=Vector3.zero;
		float xMin = Utils.camBounds.min.x + enemySpawnPadding;
		float xMax = Utils.camBounds.max.x - enemySpawnPadding;
		pos.x = Random.Range (xMin, xMax);
		pos.y = Utils.camBounds.max.y + enemySpawnPadding;
		go.transform.position = pos;
		//call spawn enemy again in a couple of sec
		Invoke("SpawnEnemy", enemySpawnRate);
	}

	public void DelayedRestart(float delay){
		//Invoke the Restart() method in delay seconds
		Invoke("Restart", delay);
	}

	public void Restart(){
		//Reload _Scene_0
		Application.LoadLevel("_Scene_0"); 

	}
		
	// Update is called once per frame
	void Update () {
	
	}
}
