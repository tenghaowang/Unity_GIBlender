using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Rendering;

[CustomEditor (typeof (LightProbeManagement))]
[System.Serializable]
public class BakedGIEditor : Editor {
	/*float minX = Mathf.Infinity;
	float maxX = -Mathf.Infinity;
	float maxZ = -Mathf.Infinity;
	float minZ = Mathf.Infinity;
	int subdivisions_X = 5;
	int subdivisions_Y = 2;
	int subdivisions_Z = 5;
	GameObject[] Select;
	LightProbeGroup LPG;
	public List<int> height=new List<int>(10);
	//int[] height;
	GameObject lightp;
	int h;*/
	
	public override void OnInspectorGUI(){
		//Create LightProbe group
		/*EditorGUILayout.Space ();
		EditorGUILayout.BeginVertical (GUILayout.Width(400));
		EditorGUILayout.HelpBox ("Used for adding light probe groups into the scene by arranging them in a regular 3D grid pattern .", MessageType.Info);
		EditorGUILayout.Space ();
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Space Divisions:",GUILayout.Width(100));
		subdivisions_X = EditorGUILayout.IntField (subdivisions_X);
		subdivisions_Y = EditorGUILayout.IntField (subdivisions_Y);
		subdivisions_Z = EditorGUILayout.IntField (subdivisions_Z);
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.LabelField ("Set height for each layer:");
		if (height.Count > subdivisions_Y) {
			height.RemoveRange (subdivisions_Y, height.Count-subdivisions_Y);
		}
		for(int i = 0; i < subdivisions_Y; i++){
			int temp=0;
			height.Add(temp);
			height[i] = EditorGUILayout.IntField("layer "+i+":", height[i]);
		}

		if (GUILayout.Button ("Create Light Probes Group based on selection")) {
			Select=Selection.gameObjects;
			if(Select.Length<1){
				Debug.LogWarning("please select the area constraint objects ");
			}
			Debug.Log("123");
		}
		EditorGUILayout.EndVertical ();*/


		LightProbeManagement myLPManagement = (LightProbeManagement)target;
		EditorGUILayout.Space ();

		EditorGUILayout.BeginVertical (GUILayout.Width (400));

		EditorGUILayout.HelpBox ("Used for baked global illumination(GI) blend. Save current scene baked GI data as structrued xml file. The file" +
			"will be exported to the folder same to the LightmapSnapshot and lightmaps.", MessageType.Info);
		EditorGUILayout.Space ();
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Baked GI Name:", GUILayout.Width (100));
		myLPManagement.GIDataname = EditorGUILayout.TextField (myLPManagement.GIDataname);
		if (GUILayout.Button ("Save")) {
			if (myLPManagement.GIDataname == string.Empty) {
				Debug.LogWarning ("There is no baked GI name defined, add name to save");
			} else {
				myLPManagement.saveProbeDataXml (getSavePath (myLPManagement.GIDataname));
				AssetDatabase.Refresh ();
			}
		}
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.HelpBox ("Define baked GI data and give each a weight for blend calculation." +
			"It is a linear addiction of each data and if the total weight is far above 1.the color will " +
			"become wierd bright.", MessageType.Info);
		EditorGUILayout.Space ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("GI Data Count:", GUILayout.Width (100));
		myLPManagement.bakedGICount = EditorGUILayout.IntField (myLPManagement.bakedGICount);
		EditorGUILayout.EndHorizontal ();
		bakedGIList (myLPManagement.bakedGICount,myLPManagement.bakedGIData,myLPManagement.bakedGIWeight);
		EditorGUILayout.Space ();
		if (myLPManagement.bakedGICount == 0) {
			EditorGUILayout.HelpBox("Assign baked GI data and begin blend process",MessageType.Warning);
			return;
		}
		if (GUILayout.Button ("Blend Baked GI")) {
			SphericalHarmonicsL2[] bakedProbes = LightmapSettings.lightProbes.bakedProbes;
			int probesCount=LightmapSettings.lightProbes.count;
			for (int i=0;i<probesCount;i++){
				bakedProbes[i].Clear();
			}
			for (int i =0; i < myLPManagement.bakedGICount; i++){
				if (myLPManagement.bakedGIData[i]==null){
					Debug.LogError ("null reference for baked GI data");
					return;
				}
				bakedProbes=myLPManagement.assignData2bakedProbe(myLPManagement.loadProbeDataXml(getXMLPath(myLPManagement.bakedGIData[i])),myLPManagement.bakedGIWeight[i],bakedProbes,probesCount);
			}
			LightmapSettings.lightProbes.bakedProbes=bakedProbes;
		}
		EditorGUILayout.EndVertical ();
		if (GUI.changed) {
			EditorUtility.SetDirty(myLPManagement);
		}
	}

	void bakedGIList(int bakedGICount,List<TextAsset> bakedGIData,List<float> bakedGIWeight){
		//reset unshown content
		if (bakedGICount <bakedGIData.Count) {
			bakedGIData.RemoveRange(bakedGICount,bakedGIData.Count-bakedGICount);
			bakedGIWeight.RemoveRange(bakedGICount,bakedGIWeight.Count-bakedGICount);
		}

		for (int i=0; i<bakedGICount; i++) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField("Baked GI:",GUILayout.Width(100));
			TextAsset tempbakedGIData=new TextAsset();
			float tempbakedGIWeight=new float();
			bakedGIData.Add(tempbakedGIData);
			bakedGIWeight.Add(tempbakedGIWeight);
			bakedGIData[i] = EditorGUILayout.ObjectField (bakedGIData[i], typeof(TextAsset), false) as TextAsset;
			bakedGIWeight[i]=EditorGUILayout.FloatField(bakedGIWeight[i],GUILayout.Width(40));
			//bakedGIweight_normalize();
			EditorGUILayout.EndHorizontal();
		}
	}

	/*void bakedGIweight_normalize(){
		float weightAll = 0.0f;
		for (int i=0; i<bakedGIWeight.Count; i++) {
			weightAll+=bakedGIWeight[i];
		}
		if (weightAll == 0) {
			for (int i=0; i<bakedGIWeight.Count; i++) {
				bakedGIWeight [i] = 1.0f / bakedGIWeight.Count;
			}
		} else {
			for (int i=0; i<bakedGIWeight.Count; i++) {
				bakedGIWeight [i] = 1.0f / weightAll * bakedGIWeight [i];
			}
		}
	}*/

	string getXMLPath(TextAsset GIdata){
		Debug.Log(AssetDatabase.GetAssetPath (GIdata));
		return (AssetDatabase.GetAssetPath (GIdata));
		//string filepath = Application.dataPath+@"/probeData/test_blue.xml";
	}

	string getSavePath(string GIDataname){
		string scenePath = EditorApplication.currentScene;
		scenePath = scenePath.Substring (0, scenePath.Length - 6);
		string folderName = scenePath.Substring (scenePath.LastIndexOf ('/') + 1);
		string savepath=Application.dataPath+@"/"+folderName+"/"+GIDataname+".xml";  
		return savepath;
	}

	

}
