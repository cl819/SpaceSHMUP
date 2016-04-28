using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

	static public Hero		S;

	public float gameRestartDelay=2f;

	public float	speed = 30;
	public float	rollMult = -45;
	public float  	pitchMult=30;

	[SerializeField]
	private float	_shieldLevel=1;

	//weapon fields
	public Weapon[] weapons;

	public bool	_____________________;
	public Bounds bounds;

	public delegate void WeaponFireDelegate();
	//create weapon fire delegate field
	public WeaponFireDelegate fireDelegate;

	void Awake(){
		S = this;
		bounds = Utils.CombineBoundsOfChildren (this.gameObject);
	}

	void Start(){

		//reset weapons to start
		ClearWeapons();
		weapons[0].SetType (WeaponType.blaster);
	}
		

	
	// Update is called once per frame
	void Update () {
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis("Vertical");

		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;
		
		bounds.center = transform.position;
		
		// constrain to screen
		Vector3 off = Utils.ScreenBoundsCheck(bounds,BoundsTest.onScreen);
		if (off != Vector3.zero) {  // we need to move ship back on screen
			pos -= off;
			transform.position = pos;
		}
		
		// rotate the ship to make it feel more dynamic
		transform.rotation =Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult,0);

		if(Input.GetAxis("Jump")==1&& fireDelegate!=null){
			fireDelegate();
		}
	}



	public GameObject lastTriggerGo = null;

	void OnTriggerEnter(Collider other){
		//find tag of other.gameObject or its parent GameObjects
		GameObject go= Utils.FindTaggedParent(other.gameObject);
		//if there is a parent with a tag
		if (go != null) {
			if (go == lastTriggerGo) {
				return;
			}
			lastTriggerGo = go;
			if (go.tag == "Enemy") {
				//shield triggered by enemy decrease the level of the shield by 1
				shieldLevel--;
				//Destroy enemy
				Destroy (go);
			} else if (go.tag == "PowerUp") {
				AbsorbPowerUp (go);
			}else {
				print ("Triggered:" + go.name);
			}
		} else {
			//otherwise announce the original other.gameobject
			print ("Triggered:" + other.gameObject.name);
		}
	}//end onTriggerEnter

	public void AbsorbPowerUp(GameObject go){
		PowerUp pu = go.GetComponent<PowerUp> ();
		switch (pu.type) {
		case WeaponType.shield:
			shieldLevel++;
			break;

		default:
			if (pu.type == weapons [0].type) {
				Weapon w = GetEmptyWeaponSlot ();
				if (w != null) {
					w.SetType (pu.type);
				} 
			}else {
				ClearWeapons ();
				weapons [0].SetType (pu.type);
			}
			break;

		}
		pu.AbsorbedBy (this.gameObject);
	}

	Weapon GetEmptyWeaponSlot(){
		for (int i = 0; i < weapons.Length; i++) {
			if (weapons [i].type == WeaponType.none) {
				return(weapons [i]);
			}
		}
		return(null);
	}

	void ClearWeapons(){
		foreach (Weapon w in weapons) {
			w.SetType (WeaponType.none);
		}
	}

	public float shieldLevel{
		get{
			return(_shieldLevel);
		}
		set{
			_shieldLevel = Mathf.Min (value, 4);
			//if the shield is going to be set to less than zero
			if (value < 0) {
				Destroy (this.gameObject);
				//tell main.s to restart the game after a delay
				Main.S.DelayedRestart(gameRestartDelay);
			}//end if
		}//end set
	}//end shield level
}