using System;
using UnityEngine;

namespace XTools
{
//[CreateAssetMenu(fileName ="PreferenceReadme",menuName = "CreateScriptableObject/PreferenceReadmeSO")]
	public class ReadmeScriptableObject : ScriptableObject
	{
		public Texture2D icon;
		public string title;
		public Section[] sections;

		[Serializable]
		public class Section
		{
			public string heading, text, linkText, url;
		}

		public string computerID;
	}
}
