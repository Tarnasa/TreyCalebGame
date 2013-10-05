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
					#region Basic
					GameObject prevPrevPNode = prevPNodes[0];
					GameObject nextNextPNode = nextPNodes[0];
					
					Vector3 prevPrevPos = prevPrevPNode.transform.position;
					Vector3 prevPos = prevPNode.transform.position;
					Vector3 nextPos = nextPNode.transform.position;
					Vector3 nextNextPos = nextNextPNode.transform.position;
					
					float tRay = calculateTRay(prevPrevPos, prevPos, nextPos, nextNextPos, targetPos);
					
					goalForward = getCNode(prevPNode).transform.forward * (1 - tRay)
						+ getCNode(nextPNode).transform.forward * tRay;
					goalPosition = calculateGoalPosition(getCNode(prevPrevPNode).transform.position,
						getCNode(prevPNode).transform.position, getCNode(nextPNode).transform.position,
						getCNode(nextNextPNode).transform.position, tRay);
					
					// Check if the target has moved to another node
					
					if (tRay > 1)
					{
						prevPNode = nextPNode;
						nextPNode = nextNextPNode;
					}
					else if (tRay < 0)
					{
						nextPNode = prevPNode;
						prevPNode = prevPrevPNode;
					}
					#endregion
				}
				// Prev is first node
				else if (prevPNodes.Count == 0)
				{
					#region firstBasic
					GameObject nextNextPNode = nextPNodes[0];
					
					Vector3 prevPos = prevPNode.transform.position;
					Vector3 nextPos = nextPNode.transform.position;
					Vector3 nextNextPos = nextNextPNode.transform.position;
					
					// TODO: Change tFromPathAndBorder to take the nextAvg (remove another CrossProduct)
					// OPTI: Cache border
					Vector3 nextAvg = ((nextPos - prevPos).normalized
						+ (nextNextPos - nextPos).normalized) / 2;
					Vector3 nextBorder = Vector3.Cross(nextAvg, Vector3.up);
					
					float tNext = tFromPathAndBorder(prevPos, nextPos, targetPos, nextBorder);
					
					tNext = tNext < 0 ? 0 : tNext;
					
					goalForward = getCNode(prevPNode).transform.forward * (1 - tNext)
						+ getCNode(nextPNode).transform.forward * tNext;
					if (tNext < 0.5F)
					{
						goalPosition = getCNode(prevPNode).transform.position * (1 - tNext)
							+ getCNode(nextPNode).transform.position * tNext;
					}
					else
					{
						goalPosition = quadraticBezier(
							(getCNode(prevPNode).transform.position + getCNode(nextPNode).transform.position) / 2,
							getCNode(nextPNode).transform.position,
							(getCNode(nextPNode).transform.position + getCNode(nextNextPNode).transform.position) / 2,
							tNext - 0.5F);
					}
					
					// Check if the target has moved to another node
					if (tNext > 1)
					{
						prevPNode = nextPNode;
						nextPNode = nextNextPNode;
					}
					#endregion
				}
				// Multiple previous nodes
				else
				{
					#region nPrev
					GameObject prevPrevPNode;
					GameObject nextNextPNode = nextPNodes[0];
					
					Vector3 prevPrevPos;
					Vector3 prevPos = prevPNode.transform.position;
					Vector3 nextPos = nextPNode.transform.position;
					Vector3 nextNextPos = nextNextPNode.transform.position;
					
					prevPrevPNode = calculatePrevPrevPNode(prevPNodes, prevPos, nextPos, targetPos);
					prevPrevPos = prevPrevPNode.transform.position;
					
					float tRay = calculateTRay(prevPrevPos, prevPos, nextPos, nextNextPos, targetPos);
					
					goalForward = getCNode(prevPNode).transform.forward * (1 - tRay)
						+ getCNode(nextPNode).transform.forward * tRay;
					goalPosition = calculateGoalPosition(getCNode(prevPrevPNode).transform.position,
						getCNode(prevPNode).transform.position, getCNode(nextPNode).transform.position,
						getCNode(nextNextPNode).transform.position, tRay);
					
					// Check if the target has moved to another node
					
					if (tRay > 1)
					{
						prevPNode = nextPNode;
						nextPNode = nextNextPNode;
					}
					else if (tRay < 0)
					{
						nextPNode = prevPNode;
						prevPNode = prevPrevPNode;
					}
					#endregion
				}
			}
			// Next is last node
			else if (nextPNodes.Count == 0)
			{
				// Simple
				if (prevPNodes.Count == 1)
				{
					#region lastBasic
					GameObject prevPrevPNode = prevPNodes[0];
					
					Vector3 prevPrevPos = prevPrevPNode.transform.position;
					Vector3 prevPos = prevPNode.transform.position;
					Vector3 nextPos = nextPNode.transform.position;
					
					// OPTI: Cache border
					Vector3 prevAvg = ((prevPos - prevPrevPos).normalized
						+ (nextPos - prevPos).normalized) / 2;
					Vector3 prevBorder = Vector3.Cross(prevAvg, Vector3.up);
					
					float tPrev = tFromPathAndBorder(prevPos, nextPos, targetPos, prevBorder);
					
					tPrev = tPrev > 1 ? 1 : tPrev;
					
					goalForward = getCNode(prevPNode).transform.forward * (1 - tPrev)
						+ getCNode(nextPNode).transform.forward * tPrev;
					if (tPrev > 0.5F)
					{
						goalPosition = getCNode(prevPNode).transform.position * (1 - tPrev)
							+ getCNode(nextPNode).transform.position * tPrev;
					}
					else
					{
						goalPosition = quadraticBezier(
							(getCNode(prevPrevPNode).transform.position + getCNode(prevPNode).transform.position) / 2,
							getCNode(prevPNode).transform.position,
							(getCNode(prevPNode).transform.position + getCNode(nextPNode).transform.position) / 2,
							tPrev + 0.5F);
					}
					
					// Check if the target has moved to another node
					if (tPrev < 0)
					{
						nextPNode = prevPNode;
						prevPNode = prevPrevPNode;
					}
					#endregion
				}
				else if (prevPNodes.Count == 0)
				{
					//Only 2 Nodes in system :(	
				}
				// Split previous path (not likely to happen)
				else
				{
					#region LastNPrev
					GameObject prevPrevPNode;
					
					Vector3 prevPrevPos;
					Vector3 prevPos = prevPNode.transform.position;
					Vector3 nextPos = nextPNode.transform.position;
					
					prevPrevPNode = calculatePrevPrevPNode(prevPNodes, prevPos, nextPos, targetPos);
					prevPrevPos=  prevPrevPNode.transform.position;
					
					// OPTI: Cache border
					Vector3 prevAvg = ((prevPos - prevPrevPos).normalized
						+ (nextPos - prevPos).normalized) / 2;
					Vector3 prevBorder = Vector3.Cross(prevAvg, Vector3.up);
					
					float tPrev = tFromPathAndBorder(prevPos, nextPos, targetPos, prevBorder);
					
					tPrev = tPrev > 1 ? 1 : tPrev;
					
					goalForward = getCNode(prevPNode).transform.forward * (1 - tPrev)
						+ getCNode(nextPNode).transform.forward * tPrev;
					if (tPrev > 0.5F)
					{
						goalPosition = getCNode(prevPNode).transform.position * (1 - tPrev)
							+ getCNode(nextPNode).transform.position * tPrev;
					}
					else
					{
						goalPosition = quadraticBezier(
							(getCNode(prevPrevPNode).transform.position + getCNode(prevPNode).transform.position) / 2,
							getCNode(prevPNode).transform.position,
							(getCNode(prevPNode).transform.position + getCNode(nextPNode).transform.position) / 2,
							tPrev + 0.5F);
					}
					
					// Check if the target has moved to another node
					if (tPrev < 0)
					{
						nextPNode = prevPNode;
						prevPNode = prevPrevPNode;
					}
					#endregion
				}
			}
			// Multiple next nodes
			else
			{
				// Simple
				if (prevPNodes.Count == 1)
				{
					#region nNext
					GameObject prevPrevPNode = prevPNodes[0];
					GameObject nextNextPNode;
					
					Vector3 prevPrevPos = prevPrevPNode.transform.position;
					Vector3 prevPos = prevPNode.transform.position;
					Vector3 nextPos = nextPNode.transform.position;
					Vector3 nextNextPos;
					
					nextNextPNode = calculateNextNextPNode(nextPNodes, prevPos, nextPos, targetPos);
					nextNextPos = nextNextPNode.transform.position;
					
					float tRay = calculateTRay(prevPrevPos, prevPos, nextPos, nextNextPos, targetPos);
					
					goalForward = getCNode(prevPNode).transform.forward * (1 - tRay)
						+ getCNode(nextPNode).transform.forward * tRay;
					goalPosition = calculateGoalPosition(getCNode(prevPrevPNode).transform.position,
						getCNode(prevPNode).transform.position, getCNode(nextPNode).transform.position,
						getCNode(nextNextPNode).transform.position, tRay);
					
					// Check if the target has moved to another node
					
					if (tRay > 1)
					{
						prevPNode = nextPNode;
						nextPNode = nextNextPNode;
					}
					else if (tRay < 0)
					{
						nextPNode = prevPNode;
						prevPNode = prevPrevPNode;
					}
					#endregion
				}
				// Prev is first node
				else if (prevPNodes.Count == 0)
				{
					#region FirstNNext
					GameObject nextNextPNode;
					
					Vector3 prevPos = prevPNode.transform.position;
					Vector3 nextPos = nextPNode.transform.position;
					Vector3 nextNextPos;
					
					nextNextPNode = calculateNextNextPNode(nextPNodes, prevPos, nextPos, targetPos);
					nextNextPos = nextNextPNode.transform.position;
					
					// TODO: Change tFromPathAndBorder to take the nextAvg (remove another CrossProduct)
					// OPTI: Cache border
					Vector3 nextAvg = ((nextPos - prevPos).normalized
						+ (nextNextPos - nextPos).normalized) / 2;
					Vector3 nextBorder = Vector3.Cross(nextAvg, Vector3.up);
					
					float tNext = tFromPathAndBorder(prevPos, nextPos, targetPos, nextBorder);
					
					tNext = tNext < 0 ? 0 : tNext;
					
					goalForward = getCNode(prevPNode).transform.forward * (1 - tNext)
						+ getCNode(nextPNode).transform.forward * tNext;
					if (tNext < 0.5F)
					{
						goalPosition = getCNode(prevPNode).transform.position * (1 - tNext)
							+ getCNode(nextPNode).transform.position * tNext;
					}
					else
					{
						goalPosition = quadraticBezier(
							(getCNode(prevPNode).transform.position + getCNode(nextPNode).transform.position) / 2,
							getCNode(nextPNode).transform.position,
							(getCNode(nextPNode).transform.position + getCNode(nextNextPNode).transform.position) / 2,
							tNext - 0.5F);
					}
					
					// Check if the target has moved to another node
					if (tNext > 1)
					{
						prevPNode = nextPNode;
						nextPNode = nextNextPNode;
					}
					#endregion
				}
				// split previous path
				else
				{
					#region nNextNPrev
					
					#endregion
				}
			}
			
			// Smoothly move to target position and orientation
			transform.position += (goalPosition - transform.position) * 0.5F;
			transform.forward += (goalForward - transform.forward) * 0.5F;
		}
	}
	
	float calculateTRay(Vector3 prevPrevPos, Vector3 prevPos, 
		Vector3 nextPos, Vector3 nextNextPos, Vector3 targetPos)
	{
		// OPTI: Cache border
		// Take average/midpoint of P-PP and N-P
		Vector3 prevAvg = ((prevPos - prevPrevPos).normalized
			+ (nextPos - prevPos).normalized) / 2;
		prevAvg.y = 0;
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
		
		return tRay;
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
	
	GameObject calculatePrevPrevPNode(System.Collections.Generic.List<GameObject> prevPNodes,
		Vector3 prevPos, Vector3 nextPos, Vector3 targetPos)
	{
		GameObject prevRightPNode, prevLeftPNode;
		
		// Determine which previousPath is on the "Right"/"Left"
		// OPTI: Pre-compute the order of paths based on position in list
		// Calculate average of previous paths
		Vector3 prevAvgPath = (prevPos - prevPNodes[0].transform.position).normalized +
			(prevPos - prevPNodes[1].transform.position).normalized;
		Vector3 prevAvgPathNormal = Vector3.Cross(prevAvgPath, Vector3.down);
		if (Vector3.Dot(prevPNodes[0].transform.position - prevPos, prevAvgPathNormal) > 0)
		{
			prevRightPNode = prevPNodes[0];
			prevLeftPNode = prevPNodes[1];
		}
		else
		{
			prevLeftPNode = prevPNodes[0];
			prevRightPNode = prevPNodes[1];
		}
		
		// Find out what side of the path the target is on
		Vector3 pathNormal = Vector3.Cross(nextPos - prevPos, Vector3.down);
		if (Vector3.Dot(targetPos - prevPos, pathNormal) > 0)
		{
			return prevRightPNode;
		}
		else
		{
			return prevLeftPNode;
		}
	}	
	
	GameObject calculateNextNextPNode(System.Collections.Generic.List<GameObject> nextPNodes,
		Vector3 prevPos, Vector3 nextPos, Vector3 targetPos)
	{
		GameObject nextRightPNode, nextLeftPNode;
		
		// Determine which nextPath is on the "Right"/"Left"
		// OPTI: Pre-compute the order of paths based on position in list
		// Calculate average of next paths
		Vector3 nextAvgPath = (nextPNodes[0].transform.position - nextPos).normalized +
			(nextPNodes[1].transform.position - nextPos).normalized;
		Vector3 nextAvgPathNormal = Vector3.Cross(nextAvgPath, Vector3.down);
		if (Vector3.Dot(nextPNodes[0].transform.position - nextPos, nextAvgPathNormal) > 0)
		{
			nextRightPNode = nextPNodes[0];
			nextLeftPNode = nextPNodes[1];
		}
		else
		{
			nextLeftPNode = nextPNodes[0];
			nextRightPNode = nextPNodes[1];
		}
		
		// Find out what side of the path the target is on
		Vector3 pathNormal = Vector3.Cross(nextPos - prevPos, Vector3.down);
		if (Vector3.Dot(targetPos - prevPos, pathNormal) > 0)
		{
			return nextRightPNode;
		}
		else
		{
			return nextLeftPNode;
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
	
	Vector3 calculateGoalPosition(Vector3 prevPrevPos, Vector3 prevPos, Vector3 nextPos, 
		Vector3 nextNextPos, float t)
	{
		if (t < 0.5)
		{
			return quadraticBezier((prevPrevPos + prevPos) / 2,
				prevPos, (prevPos + nextPos) / 2, t + 0.5F);	
		}
		else
		{
			return quadraticBezier((prevPos + nextPos) / 2,
				nextPos, (nextPos + nextNextPos) / 2, t - 0.5F);	
		}
	}
	
	Vector3 quadraticBezier(Vector3 start, Vector3 mid, Vector3 end, float t)
	{
		Vector3 midSM = (mid - start) * t + start;
		Vector3 midME = (end - mid) * t + mid;
		return (midME - midSM) * t + midSM;
	}
}
