using UnityEngine;
using System;

public class CameraFollowRoute : MonoBehaviour {
	
	public GameObject target;
	
	public GameObject prevPNode;
	public GameObject nextPNode;
	
	private Vector3 goalForward;
	private Vector3 goalPosition;
	
	// Use this for initialization
	void Start () {
		// Find the nearest pNode
		if (target != null && prevPNode == null && nextPNode == null)
		{
			GameObject curPNode = null;
			GameObject otherPNode = null;
			bool otherIsNext = false;
			
			float minDis = 123456789.0F;
			foreach(GameObject pNode in GameObject.FindGameObjectsWithTag("PlayerTrack"))
			{
				float dis = Vector3.SqrMagnitude(transform.position - pNode.transform.position);
				if (dis < minDis)
				{
					minDis = dis;
					curPNode = pNode;
				}
			}
			if (curPNode == null)
			{
				// Failed to find a near point (Wow, what a fail)
				return;	
			}
			// Iterate through all next and previous nodes to find nearest
			minDis = 123456789.0F;
			foreach (GameObject pNode in curPNode.GetComponent<PTrack>().previousPNodes)
			{
				float dis = Vector3.SqrMagnitude(transform.position - pNode.transform.position);
				if (dis < minDis)
				{
					minDis = dis;
					otherPNode = pNode;
				}
			}
			foreach (GameObject pNode in curPNode.GetComponent<PTrack>().nextPNodes)
			{
				float dis = Vector3.SqrMagnitude(transform.position - pNode.transform.position);
				if (dis < minDis)
				{
					minDis = dis;
					otherPNode = pNode;
					otherIsNext = true;
				}
			}
			if (otherIsNext)
			{
				prevPNode = curPNode;
				nextPNode = otherPNode;
			}
			else
			{
				prevPNode = otherPNode;
				nextPNode = curPNode;
			}
		}
		// We only have a previous node
		else if (target != null && prevPNode != null)
		{
			System.Collections.Generic.List<GameObject> nextPNodes = prevPNode.GetComponent<PTrack>().nextPNodes;
			nextPNode = calculateNextPNode(prevPNode, nextPNodes, target.transform.position);
		}
		// we only have a next node
		else if (target != null && nextPNode != null)
		{
			System.Collections.Generic.List<GameObject> previousPNodes = nextPNode.GetComponent<PTrack>().previousPNodes;
			prevPNode = calculateNextPNode(nextPNode, previousPNodes, target.transform.position);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null && nextPNode != null && prevPNode != null)
		{
			System.Collections.Generic.List<GameObject> nextPNodes = nextPNode.GetComponent<PTrack>().nextPNodes;
			System.Collections.Generic.List<GameObject> prevPNodes = prevPNode.GetComponent<PTrack>().previousPNodes;
			
			Vector3 targetPos = target.transform.position;
			
			if (nextPNodes.Count == 1)
			{
				// Simple path
				if (prevPNodes.Count == 1)
				{
					// "Simple" 1 to 1 to 1 to 1 path
					GameObject prevPrevPNode = prevPNodes[0];
					GameObject nextNextPNode = nextPNodes[0];
					
					Vector3 prevPrevPos = prevPrevPNode.transform.position;
					Vector3 prevPos = prevPNode.transform.position;
					Vector3 nextPos = nextPNode.transform.position;
					Vector3 nextNextPos = nextNextPNode.transform.position;
					
					// Take average/midpoint of P-PP and N-P
					Vector3 prevAvg = ((prevPos - prevPrevPos).normalized
						+ (nextPos - prevPos).normalized) / 2;
					prevAvg.y = 0;
					Vector3 prevBorder = Vector3.Cross(prevAvg, Vector3.down);
					// Take average/midpoint of N-P and NN-N
					Vector3 nextAvg = ((nextPos - prevPos).normalized
						+ (nextNextPos - nextPos).normalized) / 2;
					nextAvg.y = 0;
					Vector3 nextBorder = Vector3.Cross(nextAvg, Vector3.down);
					
					// Find intersection of borders (if possible)
					float tPivot = tPlaneAndLine(prevAvg, prevPos, nextBorder, nextPos);
					Vector3 pivotPos = nextPos + tPivot * nextBorder;
					
					// Find intersection of ray cast from pivot to target and our current segment
					Vector3 pivotRayNormal = Vector3.Cross(targetPos - pivotPos, Vector3.down);
					float tRay = tPlaneAndLine(pivotRayNormal, targetPos, nextPos - prevPos, prevPos);
					
					goalForward = getCNode(prevPNode).transform.forward * (1 - tRay)
						+ getCNode(nextPNode).transform.forward * tRay;
					goalPosition = getCNode(prevPNode).transform.position * (1 - tRay)
						+ getCNode(nextPNode).transform.position * tRay;
					
					// Check if the target has moved to another node
					
					if (Vector3.Dot (targetPos - nextPos, nextAvg) > 0)
					{
						prevPNode = nextPNode;
						nextPNode = nextNextPNode;
					}
					else if (Vector3.Dot (targetPos - prevPos, prevAvg) < 0)
					{
						nextPNode = prevPNode;
						prevPNode = prevPrevPNode;
					}
				}
				// Prev is first node
				else if (prevPNodes.Count == 0)
				{
					GameObject nextNextPNode = nextPNodes[0];
					
					Vector3 prevPos = prevPNode.transform.position;
					Vector3 nextPos = nextPNode.transform.position;
					Vector3 nextNextPos = nextNextPNode.transform.position;
					
					Vector3 nextAvg = ((nextPos - prevPos).normalized
						+ (nextNextPos - nextPos).normalized) / 2;
					Vector3 nextBorder = Vector3.Cross(nextAvg, Vector3.up);
					
					float tNext = tFromPathAndBorder(prevPos, nextPos, targetPos, nextBorder);
					
					tNext = tNext < 0 ? 0 : tNext;
					
					goalForward = getCNode(prevPNode).transform.forward * (1 - tNext)
						+ getCNode(nextPNode).transform.forward * tNext;
					goalPosition = getCNode(prevPNode).transform.position * (1 - tNext)
						+ getCNode(nextPNode).transform.position * tNext;
					
					// Check if the target has moved to another node
					
					// Make sure the normals are level
					nextAvg.y = 0;
					
					if (Vector3.Dot (targetPos - nextPos, nextAvg) > 0)
					{
						prevPNode = nextPNode;
						nextPNode = nextNextPNode;
					}
				}
				else
				{
					
				}
			}
			else if (nextPNodes.Count == 0)
			{
				// Next is last node
				if (prevPNodes.Count == 1)
				{
					GameObject prevPrevPNode = prevPNodes[0];
					
					Vector3 prevPrevPos = prevPrevPNode.transform.position;
					Vector3 prevPos = prevPNode.transform.position;
					Vector3 nextPos = nextPNode.transform.position;
					
					Vector3 prevAvg = ((prevPos - prevPrevPos).normalized
						+ (nextPos - prevPos).normalized) / 2;
					Vector3 prevBorder = Vector3.Cross(prevAvg, Vector3.up);
					
					float tPrev = tFromPathAndBorder(prevPos, nextPos, targetPos, prevBorder);
					
					tPrev = tPrev > 1 ? 1 : tPrev;
					
					goalForward = getCNode(prevPNode).transform.forward * (1 - tPrev)
						+ getCNode(nextPNode).transform.forward * tPrev;
					goalPosition = getCNode(prevPNode).transform.position * (1 - tPrev)
						+ getCNode(nextPNode).transform.position * tPrev;
					
					// Check if the target has moved to another node
					
					// Make sure the normals are level
					prevAvg.y = 0;
					
					if (Vector3.Dot (targetPos - prevPos, prevAvg) < 0)
					{
						nextPNode = prevPNode;
						prevPNode = prevPrevPNode;
					}
				}
				else
				{
					
				}
			}
			else
			{
				
			}
			
			// Smoothly move to target position and orientation
			transform.position += (goalPosition - transform.position) * 0.5F;
			transform.forward += (goalForward - transform.forward) * 0.5F;
		}
	}
	
	float tFromPathAndBorder(Vector3 prev, Vector3 next, Vector3 target, Vector3 border)
	{
		Vector3 borderNormal = Vector3.Cross (border, Vector3.up);
		return (Vector3.Dot (target - prev, borderNormal) / Vector3.Dot(next - prev, borderNormal));
	}
	
	GameObject calculateNextPNode(GameObject previousPNode, 
		System.Collections.Generic.List<GameObject> nextPNodes, Vector3 target)
	{
		if (nextPNodes.Count == 1)
		{
			return nextPNodes[0];
		}
		else
		{
			// Finds the (not-normalised) line between the two paths
			Vector3 averageOfSplit = (nextPNodes[0].transform.position
				+ nextPNodes[1].transform.position) - previousPNode.transform.position;
			// Normal vector pointing to the right of the aos
			Vector3 aosNormal = Vector3.Cross(averageOfSplit, Vector3.up);
			// Find out what side nextCNodes are on
			float side = Vector3.Dot(nextPNodes[0].transform.position, aosNormal);
			// Find out what side the target is on
			float targetSide = Vector3.Dot(target, aosNormal);
			if (side < 0)
			{
				if (targetSide < 0)
				{
					return nextPNodes[0];
				}
				else
				{
					return nextPNodes[1];
				}
			}
			else
			{
				if (targetSide > 0)
				{
					return nextPNodes[0];
				}
				else
				{
					return nextPNodes[1];
				}
			}
		}
	}
	
	GameObject getCNode(GameObject pNode)
	{
		return pNode.GetComponent<PTrack>().cNode;
	}
	
	float tPlaneAndLine(Vector3 normal, Vector3 pointOnPlane, Vector3 lineDir, Vector3 pointOnLine)
	{
		return Vector3.Dot(pointOnPlane - pointOnLine, normal) / (Vector3.Dot(lineDir, normal));
	}
	
	float tPointToLine(Vector3 start, Vector3 end, Vector3 point)
	{
		// TODO: cache squared distance on startup
		return -Vector3.Dot(start - point, end - start) / Vector3.SqrMagnitude(end - start);
	}
}
