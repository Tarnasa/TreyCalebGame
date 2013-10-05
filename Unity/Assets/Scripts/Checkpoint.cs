using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {
	public bool startingSpawnAssigned = true;
	public bool characterIsDead = false;
	public Vector3 startingSpawnPoint;
	
	public GameObject checkPoint;
	
	// Used to detect collision.
	//void OnCollisionEnter(Collision collision)
	void OnControllerColliderHit(ControllerColliderHit collision)
	{
		// If we collide with any CheckPoint, make it our spawnPoint
		if (collision.gameObject.tag == "CheckPoint")
		{
			Debug.Log ("Hit " + collision.gameObject.name);
			checkPoint = collision.gameObject;
		}
		Debug.Log ("Hit");
	}
	// Use this for initialization.
	void Start ()
	{
		startingSpawnPoint = transform.position;
	}
	
	// Update is called once per frame.
	void Update ()
	{
		// If character is dead then run these if statements.
		if (characterIsDead)
		{
			// Spawn above our checkPoint
			if (checkPoint != null)
			{
				transform.position = checkPoint.transform.position + Vector3.up * 5;
				characterIsDead = false;
			}
		}
	}
}
