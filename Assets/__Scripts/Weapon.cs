using UnityEngine;
using System.Collections;

public enum WeaponType{
	none, //the default/no weapon
	blaster, //simple blaster
	spread, //two shots that move in waves[NI]
	missile, //homing missiles [NI]
	laser, //Damage over time [NI]
	shield //Raise shieldLevel
}

[System.Serializable]

public class WeaponDefinition{
	public WeaponType type=WeaponType.none;
	public string letter; //the letter to show on the power up
	public Color color=Color.white;// color of collar and power up
	public GameObject projectilePrefab;
	public Color projectileColor=Color.white;
	public float damageOnHit=0; //Amount of damage caused
	public float continuousDamage=0; //Damage per second(Laser)
	public float delayBetweenShots=0;
	public float velocity=20; //speed of projectiles
}
//Note: Weapon prefabs, colors, and so on are set in the class main

public class Weapon : MonoBehaviour {

	static public Transform PROJECTILE_ANCHOR;

	public bool _______________________;

	[SerializeField]
	private WeaponType _type=WeaponType.none;
	public WeaponDefinition def;
	public GameObject collar;
	public float lastShot;


	void Awake(){
		collar = transform.Find ("Collar").gameObject;
	}
	// Use this for initialization
	void Start () {
		//call settype properly for the default _type
		SetType(_type);

		if (PROJECTILE_ANCHOR == null) {
			GameObject go = new GameObject ("_Projectile_Anchor");
			PROJECTILE_ANCHOR = go.transform;
		}
		GameObject parentGO = transform.parent.gameObject;
		if (parentGO.tag == "Hero") {
			Hero.S.fireDelegate += Fire;
		}//end if
	}//end start

	public WeaponType type{
		get{ return(_type); }
		set{ SetType (value); }
	}

	public void SetType(WeaponType wt){
		_type = wt;
		if (type == WeaponType.none) {
			this.gameObject.SetActive (false);
			return;
		} else {
			this.gameObject.SetActive (true);
		}//end else
		def=Main.GetWeaponDefinition(_type);
		collar.GetComponent<Renderer>().material.color = def.color;
		lastShot = 0;//You can always fire immediately after _type is set
	}

	public void Fire(){
		//if this.gameobject is inactive, return
		if(!gameObject.activeInHierarchy)return;
		//hasn't been enough time between shots return
		if (Time.time-lastShot < def.delayBetweenShots) {
			return;
		}
		Projectile p;
		switch (type) {
		case WeaponType.blaster:
			p = MakeProjectile ();
			p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
			break;

		case WeaponType.spread:
			p = MakeProjectile ();
			p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
			p = MakeProjectile ();
			p.GetComponent<Rigidbody>().velocity = new Vector3 (-.2f, 0.9f, 0) * def.velocity;
			p = MakeProjectile ();
			p.GetComponent<Rigidbody>().velocity = new Vector3 (.2f, 0.9f, 0) * def.velocity;
			break;
		}
	}

	public Projectile MakeProjectile(){
		GameObject go = Instantiate (def.projectilePrefab) as GameObject;
		if (transform.parent.gameObject.tag == "Hero") {
			go.tag = "ProjectileHero";
			go.layer = LayerMask.NameToLayer ("ProjectileHero");
		} else {
			go.tag = "ProjectileEnemy";
			go.layer = LayerMask.NameToLayer ("ProjectileEnemy");
		}
		go.transform.position = collar.transform.position;
		go.transform.parent = PROJECTILE_ANCHOR;
		Projectile p = go.GetComponent<Projectile> ();
		p.type = type;
		lastShot = Time.time;
		return(p);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
