using UnityEditor;
using UnityEngine;

public class PTrackWindow : EditorWindow {
	
	GameObject prevPNode;
	GameObject nextPNode;
	bool autoNode = false;
	bool placePNode = false;
	
    [MenuItem ("Window/PTrack Window")]
    static void ShowWindow () {
        EditorWindow.GetWindow (typeof(PTrackWindow));
    }
	
	void OnEnable()
	{
		// Add our SceneGUI function so that it can receive mouse presses
		SceneView.onSceneGUIDelegate += OnSceneGUI;
	}
	
	void OnSceneGUI (SceneView sceneView)
	{
		if (placePNode)
		{
			if (Event.current.type == EventType.MouseDown)
			{
				Debug.Log ("Thing 1");
				
				Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				RaycastHit rayInfo;
				
				if (Physics.Raycast(worldRay, out rayInfo))
				{
					GameObject prefab = Resources.LoadAssetAtPath("Assets/Prefabs/PTrack-prefab.prefab", typeof(GameObject)) as GameObject;
					GameObject[] pNodes = GameObject.FindGameObjectsWithTag("PlayerTrack");
					GameObject newPNode = Instantiate(prefab) as GameObject;
					newPNode.name = "PTrack" + (pNodes.Length + 1).ToString();
					newPNode.tag = "PlayerTrack";
					newPNode.transform.parent = prefab.transform.parent;
					
					newPNode.transform.position = rayInfo.point;
					
					if (nextPNode != null)
					{
						newPNode.GetComponent<PTrack>().nextPNodes.Add(nextPNode);
						if (autoNode && prevPNode == null)
						{
							nextPNode = newPNode;	
						}
					}
					if (prevPNode != null)
					{
						prevPNode.GetComponent<PTrack>().nextPNodes.Add(newPNode);
						if (autoNode && nextPNode == null)
						{
							prevPNode = newPNode;	
						}
					}
				}
				
				Event.current.Use();
			
				placePNode = false;
			}
		}	
	}
	
    void OnGUI () {
        GUILayout.Label("PTrack editor");
		autoNode = EditorGUILayout.Toggle("Auto-node", autoNode);
		placePNode = EditorGUILayout.Toggle("Place next PNode", placePNode);
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Prev Node");
		prevPNode = (GameObject)EditorGUILayout.ObjectField(prevPNode, typeof(GameObject), true);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Next Node");
		nextPNode = (GameObject)EditorGUILayout.ObjectField(nextPNode, typeof(GameObject), true);
		EditorGUILayout.EndHorizontal();
		
		if (GUILayout.Button ("Make Camera Node"))
		{
			if (Selection.transforms.Length == 1)
			{
				GameObject prefab = Resources.LoadAssetAtPath("Assets/Prefabs/CTrack-Prefab.prefab", typeof(GameObject)) as GameObject;
				GameObject[] cNodes = GameObject.FindGameObjectsWithTag("CameraTrack");
				GameObject newCNode = Instantiate(prefab) as GameObject;
				newCNode.name = "CTrack" + (cNodes.Length + 1).ToString();
				newCNode.tag = "CameraTrack";
				newCNode.layer = 8; // CameraTrack
				newCNode.transform.parent = prefab.transform.parent;
				
				newCNode.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
				newCNode.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;

				
				Selection.transforms[0].GetComponent<PTrack>().cNode = newCNode;
			}
		}
    }
}