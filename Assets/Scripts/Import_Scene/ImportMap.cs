using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class ImportMap : MonoBehaviour {
	
	private int maxTextureNumber = 256;

	//---------------test---------------
	private Map test;
	private string path = "E:\\Users\\Miguel\\Desktop\\MapCreator\\wow.wmap";

	void Start () {
		test = Import(path, new Vector3(0, 0, 0));
		if (test != null) {
			Debug.Log ("ok");
		} else {
			Debug.Log ("Fail...");
		}
	}
	//----------------------------------

	public Map Import(string path, Vector3 position) {
		if (File.Exists(path)) {
			byte[] byteArray = File.ReadAllBytes(path);
			Map newMap = new Map(position);
			string texturesPath = Path.GetDirectoryName(path);
			newMap.textureList = new List<Texture>();
			StartCoroutine(PopulateTextures(texturesPath, newMap));
			if (newMap.texturesToLoad > 0) {
				StartCoroutine(InstantiateCubes(byteArray, newMap));
			} else {
				Debug.Log ("There are no textures in that folder");
			}
			newMap.parent.name = Path.GetFileNameWithoutExtension(path);

			return newMap;
		}

		return null;
	}

	IEnumerator PopulateTextures(string path, Map map) {
		DirectoryInfo dir = new DirectoryInfo(path);
		FileInfo[] info = dir.GetFiles("*.png");
		int counter = 0;
		map.texturesToLoad = info.Length;
		foreach (FileInfo f in info)
		{
			if (counter < maxTextureNumber) {
				counter++;
				GetImage(f.FullName, map);
			} else {
				//do nothing...
			}
			yield return null;
		}
	}
	
	void GetImage(string url, Map map) {
		WWW www =  new WWW("file://" + url);
		Texture2D newTexture = new Texture2D(32, 32);
		StartCoroutine(WaitForImageDownload(www, newTexture, map));
	}
	
	IEnumerator WaitForImageDownload(WWW www, Texture2D newTexture, Map map) {
		yield return www;
		www.LoadImageIntoTexture(newTexture);
		map.textureList.Add(newTexture);
		map.texturesLoaded++;
	}

	IEnumerator InstantiateCubes(byte[] byteArray, Map map) {
		int index = 2, curTexture = 0, zeroNew = 0, zeroOld = 0;
		Vector3 curVector;
		bool waitingForTexture = true;
		while (map.texturesLoaded < map.texturesToLoad && map.texturesLoaded < maxTextureNumber) {
			//just loop
			yield return null;
		}

		while (index < byteArray.Length) {
			zeroOld = zeroNew;

			if (waitingForTexture) {
				curTexture = byteArray[index];
				index += 1;
				waitingForTexture = false;
			}

			curVector = new Vector3((float)((int)byteArray[index]), (float)((int)byteArray[index+1]), (float)((int)byteArray[index+2]));
			index += 3;

			if (curVector == Vector3.zero) {
				zeroNew += 1;
			} 

			if (zeroNew != zeroOld) {
				if (zeroNew == 3) {
					CreateCube(map.parent.transform.position, curTexture, map);
					zeroNew = 0;
					waitingForTexture = true;
				}
			} else {
				if (zeroNew == 2) {
					index -= 3;
					zeroNew = 0;
					waitingForTexture = true;
				} else if (zeroNew == 1) {
					CreateCube(map.parent.transform.position, curTexture, map);
					CreateCube(map.parent.transform.position + curVector, curTexture, map);
					zeroNew = 0;
				} else {
					CreateCube(map.parent.transform.position + curVector, curTexture, map);
				}
			}
			yield return null;
		}
	}

	void CreateCube (Vector3 position, int texture, Map map) {
		GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		newCube.transform.position = map.parent.transform.position + position;
		if (texture < map.textureList.Count) {
			newCube.renderer.material.mainTexture = map.textureList[texture];
			newCube.name = texture.ToString();
		} else {
			newCube.name = "nan";
		}
		newCube.transform.parent = map.parent.transform;
	}
}
