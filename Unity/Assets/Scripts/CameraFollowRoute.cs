using UnityEngine;
using System.Collections;

public class CameraFollowRoute : MonoBehaviour {
	
	public GameObject target;
	
	public int currentNode = 0;
	
	public GameObject[] nodes;
	
	// Use this for initialization
	void Start () {
		nodes = GameObject.FindGameObjectsWithTag("CameraTrack");
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null && nodes.Length > 0)
		{
			float currentDis = findSqrDistanceToNode(currentNode);
			
			if (currentNode <= 0)
			{
				// First and Last Node
				if (nodes.Length == 1)
				{
					transform.position = nodes[currentNode].transform.position;
				}
				// First node
				else
				{
					followLine(nodes[currentNode], nodes[currentNode + 1],	target);
					float nextDis = findSqrDistanceToNode(currentNode + 1);
					if (nextDis < currentDis)
					{
						currentNode += 1;	
					}
				}
			}
			else
			{
				// Last node
				if (currentNode >= nodes.Length - 1)
				{
					followLine(nodes[currentNode - 1], nodes[currentNode], target);
					float prevDis = findSqrDistanceToNode(currentNode - 1);
					if (prevDis < currentDis)
					{
						currentNode -= 1;	
					}
				}
				// Middle node
				else
				{
					followLine(nodes[currentNode], nodes[currentNode + 1], target);
					// Two nodes to compare with
					float prevDis = findSqrDistanceToNode(currentNode - 1);
					float nextDis = findSqrDistanceToNode(currentNode + 1);
					if (prevDis < currentDis && prevDis < nextDis)
					{
						currentNode -= 1;	
					}
					else if (nextDis < currentDis && nextDis < prevDis)
					{
						currentNode += 1;
					}
				}
			}
		}
	}
	
	float tPointToLine(Vector3 start, Vector3 end, Vector3 point)
	{
		// TODO: cache squared distance on startup
		return -Vector3.Dot(start - point, end - point) / Vector3.SqrMagnitude(end - start);
	}
	
	void followLine(GameObject start, GameObject end, GameObject point)
	{
		float t = tPointToLine(
			start.transform.position,
			end.transform.position,
			point.transform.position);
		transform.position = Vector3.Lerp(
			start.transform.position,
			end.transform.position,
			t);
		transform.rotation = Quaternion.Lerp(
			start.transform.rotation,
			end.transform.rotation,
			t);
	}
	
	float findSqrDistanceToNode(int nodeNum)
	{
		return Vector3.SqrMagnitude(target.transform.position - nodes[nodeNum].transform.position);
	}
}
