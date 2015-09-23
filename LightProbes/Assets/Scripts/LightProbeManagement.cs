using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System;

[System.Serializable]
public class LightProbeManagement : MonoBehaviour {

	public string GIDataname="";
	public int bakedGICount;
	public List<TextAsset> bakedGIData=new List<TextAsset>(10);
	public List<float> bakedGIWeight=new List<float>(10);

	void GetProbeInformation(List<List<float>> probedata){
		SphericalHarmonicsL2[] bakedProbes = LightmapSettings.lightProbes.bakedProbes;
		//Vector3[] probePositions = LightmapSettings.lightProbes.positions;
		int probeCount = LightmapSettings.lightProbes.count;
		//get all the coeffiencts
		for (int i=0; i<probeCount; i++) {
			List<float> probeCoefficient=new List<float>();
			for (int j=0; j<3; j++) {
				for (int k=0; k<9; k++) {
					probeCoefficient.Add(bakedProbes[i][j,k]);
				}
			}
			probedata.Add(probeCoefficient);
		}
	}

	public void saveProbeDataXml(string filepath){
		List<List<float>> probedata=new List<List<float>>();
		GetProbeInformation (probedata);
		//string filepath = Application.dataPath+@"/probeData/test_green.xml";
		XmlDocument xmldoc = new XmlDocument ();
		XmlElement rootNode = xmldoc.CreateElement ("probedata");
		xmldoc.AppendChild (rootNode);
		for (int i =0; i<probedata.Count; i++) {
			string probename = "probe"+i.ToString();
			XmlElement probeNode = xmldoc.CreateElement (probename); // create the rotation node.
			rootNode.AppendChild (probeNode);
			for (int j=0; j<probedata[i].Count;j++){
				XmlElement coefNode = xmldoc.CreateElement ("coefficient"); // create the x node.
				coefNode.InnerText = (probedata[i][j]).ToString(); // apply to the node text the values of the variable.
				probeNode.AppendChild(coefNode);
			}
		}
		//print (xmldoc.OuterXml);
		xmldoc.Save (filepath); // save file.
	}

	public List<List<float>> loadProbeDataXml(string filepath){
		//string filepath = Application.dataPath+@"/probeData/test_blue.xml";
		XmlDocument xmldoc = new XmlDocument ();
		List<List<float>> tempProbeData = new List<List<float>> ();
		xmldoc.Load (filepath);
		XmlNodeList rootNodeList = xmldoc.GetElementsByTagName("probedata");
		foreach (XmlNode rootNode in rootNodeList){
			XmlNodeList probeNodeList= rootNode.ChildNodes;
			foreach (XmlNode probeNode in probeNodeList){
				List<float> probeCoefficient = new List<float>();
				XmlNodeList coefList=probeNode.ChildNodes;
				foreach (XmlNode coefNode in coefList){
					//print (coefNode.InnerText);
					float coeffcient = float.Parse(coefNode.InnerText);
					probeCoefficient.Add(coeffcient);
				}
				tempProbeData.Add(probeCoefficient);
			}
		}
		return tempProbeData;
	}

	//read data from probeDataXML and assign them to the baked probe
	public SphericalHarmonicsL2[] assignData2bakedProbe(List<List<float>> tempProbeData,float GIweight,SphericalHarmonicsL2[] bakedProbes,int probeCount){
		for (int i=0; i<probeCount; i++) {
			for (int j=0; j<3; j++) {
				for (int k=0; k<9; k++) {
					bakedProbes[i][j,k]+=tempProbeData[i][9*j+k]*GIweight;
				}
			}
		}
		return bakedProbes;
		//LightmapSettings.lightProbes.bakedProbes = bakedProbes;
	}
}
