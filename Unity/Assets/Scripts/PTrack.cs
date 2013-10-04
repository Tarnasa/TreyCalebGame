using UnityEngine;
using System.Collections;

public class PTrack : MonoBehaviour {
	public GameObject cNode;
	public System.Collections.Generic.List<GameObject> nextPNodes;
	public System.Collections.Generic.List<GameObject> previousPNodes;
	
	void Start ()
	{
		foreach(GameObject next in nextPNodes)
		{
			next.GetComponent<PTrack>().previousPNodes.Add(gameObject);
		}
	}
	
	void OnDrawGizmos()
	{
		if (cNode != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, cNode.transform.position);
		}
		foreach(GameObject next in nextPNodes)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(transform.position, next.transform.position);
		}
	}
}
