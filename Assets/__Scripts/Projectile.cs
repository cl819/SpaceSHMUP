using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	private WeaponType _type;

	public WeaponType type{
		get{
			return(_type);
		}
		set{
			SetType (value);
		}
	}

	void Awake(){
		//test whether this has passed off screen every 2 seconds
		InvokeRepeating("CheckOffscreen", 2f,2f);
	}

	public void SetType(WeaponType eType){
		//set the _type
		_type=eType;
		WeaponDefinition def = Main.GetWeaponDefinition (_type);
		GetComponent<Renderer>().material.color = def.projectileColor;
	}

	void CheckOffscreen(){
		if (Utils.ScreenBoundsCheck (GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
			Destroy (this.gameObject);
		}//end if
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
