using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	
	public GameObject target = null;
	public float followDist = 12.0F;
	public float followTheta = 30.0F;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null)
		{
			transform.position = target.transform.position;
			transform.rotation = target.transform.rotation;
			transform.Rotate(followTheta, 0, 0);
			transform.position += transform.forward * -followDist;
		}
	}
}
