using System;
using UnityEngine;

//[CreateAssetMenu(fileName ="Readme",menuName = "CreateScriptableObject/ReadmeSO")]
public class Readme : ScriptableObject {
	public Texture2D icon;
	public string title;
	public Section[] sections;
	
	[Serializable]
	public class Section {
		public string heading, text, linkText, url;
	}

	public string computerID;
}
