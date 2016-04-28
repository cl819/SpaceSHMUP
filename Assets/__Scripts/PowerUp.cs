using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

	public Vector2 rotMinMax = new Vector2 (15, 90);
	public Vector2 driftMinMax = new Vector2 (.25f, 2);
	public float lifeTime = 6f;//seconds the powerup exists
	public float fadeTime=4f; // seconds it will then fade

	public bool ______________________;

	public WeaponType type;
	public GameObject cube;
	public TextMesh letter;
	public Vector3 rotPerSecond;
	public float birthTime;

	void Awake(){
		//find cube reference
		cube=transform.Find("Cube").gameObject;
		//Find the TextMesh
		letter=GetComponent<TextMesh>();

		//set random velocity
		Vector3 vel=Random.onUnitSphere;
		//random gives you vector point
		vel.z = 0;
		vel.Normalize ();//make length of the vel 1
		vel*=Random.Range(driftMinMax.x,driftMinMax.y);

		GetComponent<Rigidbody>().velocity = vel;

		//set rotation of gameobject
		transform.rotation=Quaternion.identity;

		//set up rotpersecond
		rotPerSecond=new Vector3 (Random.Range(rotMinMax.x,rotMinMax.y), Random.Range(rotMinMax.x,rotMinMax.y),Random.Range(rotMinMax.x,rotMinMax.y));

		InvokeRepeating ("CheckOffScreen", 2f, 2f);
		birthTime = Time.time;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		cube.transform.rotation = Quaternion.Euler (rotPerSecond * Time.time);

		//fade out powerup over time
		float u=(Time.time-(birthTime+lifeTime))/fadeTime;

		if (u >= 1) {
			Destroy (this.gameObject);
			return;
		}
		//use u to determine the alpha value of the cube & letter
		if (u > 0) {
			Color c = cube.GetComponent<Renderer>().material.color;
			c.a = 1f - u;
			cube.GetComponent<Renderer>().material.color = c;
			//fade the letter too, just not as much
			c=letter.color;
			c.a = 1f - (u * 0.5f);
			letter.color = c;
		}
	
	}

	public void SetType(WeaponType wt){
		//grab weapon def from main
		WeaponDefinition def=Main.GetWeaponDefinition(wt);
		//Set color of the cube child
		cube.GetComponent<Renderer>().material.color=def.color;
		//letter.color=def.color
		letter.text=def.letter;
		type = wt;
	}

	public void AbsorbedBy(GameObject target){
		Destroy (this.gameObject);
	}
	void CheckOffscreen(){
		//if powerup had drifted off screen
		if (Utils.ScreenBoundsCheck (cube.GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
			Destroy (this.gameObject);
		}
	}
}
