using UnityEngine;
using System.Collections;

public class ProjectSettings : MonoBehaviour {

	//file stored variables
	static public int guiMenuSlots;
	static public int renderDistance;
	static public int mouseSensitivity;

	//runtime variables
	static public string texturesPath = "";
	static public string mapPath = "";
	static public int fileSignature = 1;
	static public int programVersion = 1;

	static public byte[] byteArray;
	static public int byteNumber = 0;
}
