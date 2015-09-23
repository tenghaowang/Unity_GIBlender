using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class AutomaticLightProbeEditor : EditorWindow {

	float minX = Mathf.Infinity;
	float maxX = -Mathf.Infinity;
	float maxZ = -Mathf.Infinity;
	float minZ = Mathf.Infinity;
	int subdivisions_X = 15;
	int subdivisions_Y = 2;
	int subdivisions_Z = 15;
	GameObject[] Select;
	LightProbeGroup LPG;
	int[] height;
	bool ifSure = false;
	GameObject lightp;
	int h;

	// Add menu item named "automatic light probe" to the GI Tools menu
	[MenuItem("GI Tools/Automatic Light Probes")]
	
	static void Init () {
		// Get existing open window or if none, make a new one:
		AutomaticLightProbeEditor window = (AutomaticLightProbeEditor)EditorWindow.GetWindow (typeof (AutomaticLightProbeEditor), true, "Create Light Probes");
		window.Show();
	}

	void OnGUI () {
		GUILayout.Label ("Divisions:", EditorStyles.boldLabel);
		subdivisions_X = EditorGUILayout.IntField("X:", subdivisions_X);
		subdivisions_Y = EditorGUILayout.IntField("Y:", subdivisions_Y);
		subdivisions_Z = EditorGUILayout.IntField("Z:", subdivisions_Z);
		if(GUILayout.Button("Set height for each layer")){
			height = new int[subdivisions_Y];
			ifSure = !ifSure;
			h = subdivisions_Y;
		}
		if(ifSure){
			if(h != subdivisions_Y){
			
			}else{
				GUILayout.Label ("Height Between ground and each layer", EditorStyles.boldLabel);
				for(int i = 0; i < subdivisions_Y; i++){
					height[i] = EditorGUILayout.IntField("layer"+ i+":", height[i]);
				}
			}
		}
		GUILayout.Space(20);
		if(GUILayout.Button("Create Light Probes Group based on selection")){
			Select = Selection.gameObjects;
			if( Select.Length < 1 )
			{
				Debug.Log( "please define area constraint objects" );
				return;
			}
			if(ifSure == false){
				Debug.Log( "please define height first" );
				return;
			}
			bool BoundSuccess = false;
			BoundSuccess = Bounds();
			if(BoundSuccess){
				lightp = LightProbeGrid();
				ifSure = false;
			}else{
				Debug.Log( "created failed" ); 
			}


		}
		if(GUILayout.Button("Clear created Light Probes Group")){
			ClearLightProbeGrid(lightp);
		}
	}
	
	bool Bounds(){
		for(int i=0; i<Select.Length; i++){			
			Renderer renderer = Select[i].GetComponent<Renderer>();
			if(renderer == null) {
				Debug.Log( "please use gameobjects with renderer component for area constraint" ); 
				return false;
			}
				//continue;
			Bounds box=renderer.bounds;
			//Bounds box = col.bounds;
			if(box.min.x < minX) minX = box.min.x;
			if(box.min.z < minZ) minZ = box.min.z;
			if(box.max.x > maxX) maxX = box.max.x;
			if(box.max.z > maxZ) maxZ = box.max.z;
		}
		return true;
	}

	GameObject LightProbeGrid(){
		List<Vector3> position = new List<Vector3>();
		Vector3 layer;
		float interval_axisX = (maxX - minX) / subdivisions_X;
		float interval_axisZ = (maxZ - minZ) / subdivisions_Z;
		for(int x = 0; x <=subdivisions_X; x++){
			for(int z = 0; z <= subdivisions_Z; z++){	
				for(int y = 0; y < subdivisions_Y; y++){
					layer = new Vector3(minX + x * interval_axisX, height[y], minZ + z * interval_axisZ);
					/*if(Terrain.activeTerrain == null){
						Debug.Log("场景中没有Terrain");
						return null;
					}*/
					//else{
					//layer.y = height[y];
					position.Add(layer);
					//}
				}
			}
		}
		var go = new GameObject();
		go.name = "lightProbes";
		LPG = go.gameObject.AddComponent<LightProbeGroup>() as LightProbeGroup;
		LPG.probePositions = position.ToArray();
		height = null;
		position = null;
		Debug.Log( "created successfully!，group name：lightProbes" ); 
		return go;
	}

	void ClearLightProbeGrid(GameObject go){
		if (go == null) {
			Debug.Log ("please create Light Probes Group");
		} else {
			LPG.probePositions = null;
			DestroyImmediate (go);
			Debug.Log( "removed successfully" );
		}
	}
}

