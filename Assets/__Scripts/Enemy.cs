using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public float speed=10f;
	public float fireRate=0.3f;
	public float health = 10;
	public int score=100;

	public int showDamageForFrames=2;
	public float powerUpDropChance=1f;

	public bool _____________________;

	public Color[] originalColors;
	public Material[] materials;
	public int remainingDamageFrames=0;//damage frames left

	public Bounds bounds;
	public Vector3 boundsCenterOffset;

	void Awake(){
		materials = Utils.GetAllMaterials (gameObject);
		originalColors = new Color[materials.Length];
		for (int i = 0; i < materials.Length; i++) {
			originalColors [i] = materials [i].color;
		}
		InvokeRepeating ("CheckOffscreen", 0f, 2f);
	}

	void CheckOffscreen(){
		//if bounds are still their defaulr value
		if (bounds.size == Vector3.zero) {
			bounds = Utils.CombineBoundsOfChildren (this.gameObject);
			boundsCenterOffset = bounds.center - transform.position;
		}
		bounds.center = transform.position + boundsCenterOffset;
		Vector3 off = Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen);
		if (off != Vector3.zero) {
			if (off.y < 0) {
				//then destroy it
				Destroy(this.gameObject);
			}
		}
	}//end check offscreen

	void OnCollisionEnter(Collision coll){
		GameObject other = coll.gameObject;
		switch (other.tag) {
		case "ProjectileHero":
			Projectile p = other.GetComponent<Projectile> ();
			//enemies don't take damage unless they're onscreen
			bounds.center = transform.position + boundsCenterOffset;
			if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen) != Vector3.zero) {
				Destroy (other);
				break;
			}
			//hurt enemy
			ShowDamage();
			//get the damage amount form the projectile type
			health -= Main.W_DEFS[p.type].damageOnHit;
			if (health <= 0) {
				//Tell the Main singleton that ship has been destroyed
				Main.S.ShipDestroyed(this);
				//destroy enemy
				Destroy (this.gameObject);
			}
			Destroy (other);
			break;
		}
	}//end oncollision enter

	void ShowDamage(){
		foreach (Material m in materials) {
			m.color = Color.red;
		}
		remainingDamageFrames = showDamageForFrames;
	}//show damage

	void UnShowDamage(){
		for(int i=0;i<materials.Length;i++){
			materials[i].color=originalColors[i];
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Move ();
		if (remainingDamageFrames > 0) {
			remainingDamageFrames--;
			if (remainingDamageFrames == 0) {
				UnShowDamage ();
			}
		}
	
	}

	public virtual void Move(){
		Vector3 tempPos = pos;
		tempPos.y -=speed * Time.deltaTime;
		pos = tempPos;

	}
	public Vector3 pos{
		get{
			return(this.transform.position);
		}
		set{
			this.transform.position = value;
		}
	}
}
