using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Textures should be editable (Read/Write = true) and have supported format (ex. RGBA32bit)

public class TextureArrayCreator : ScriptableWizard
{
	[MenuItem("Window/Create Texture Array")]
	public static void ShowWindow()
	{
		ScriptableWizard.DisplayWizard<TextureArrayCreator>("Create Texture Array", "Create");
	}

	public string Path = "Assets/";
	public string Filename = "TextureArray";
	public List<Texture2D> Textures = new();

	void OnWizardCreate()
	{
		if(Textures == null || Textures.Count == 0)
		{
			Debug.LogError("No textures assigned");
			return;
		}

		Texture2D sample = Textures[0];
		Texture2DArray textureArray = new Texture2DArray(sample.width, sample.height, Textures.Count, sample.format, false);
		textureArray.filterMode = FilterMode.Trilinear;
		textureArray.wrapMode = TextureWrapMode.Repeat;

		for (int i = 0; i < Textures.Count; i++)
		{
			Texture2D tex = Textures[i];
			textureArray.SetPixels(tex.GetPixels(0), i, 0);
		}
		textureArray.Apply();
			
		string uri = Path + Filename+".asset";
		AssetDatabase.CreateAsset(textureArray, uri);
		Debug.Log("Saved asset to " + uri);
	}
}