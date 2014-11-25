using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class MenuButtons : MonoBehaviour {

	private List<Button> buttonList = new List<Button>();
	private GUIStyle buttonStyle = new GUIStyle();

	private Texture2D buttonBackground;

	private bool inMenu = true;
	private string message = "";
	private int deepMenu;

	private List<Button> deepMenuButtonList = new List<Button>();

	void Start () {
		//Debug.Log (Application.persistentDataPath);

		UpdateProjectSettings();
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		buttonBackground = Resources.Load ("Textures/Preview_Cube") as Texture2D;
		CreateDeepMenuButtons();
		CreateButtons();

		buttonStyle.fontSize = 28;
	}

	void Update () {
	
	}

	void OnGUI () {

		if (inMenu) {
			for (int i = 0; i < buttonList.Count; i++) {
				buttonStyle.fontSize = (int)(1.5f*buttonList[i].width/buttonList[i].text.Length);
				GUI.DrawTexture(buttonList[i].rect, buttonList[i].texture, ScaleMode.StretchToFill);
				if (GUI.Button(buttonList[i].rect, buttonList[i].text, buttonStyle)) {
					inMenu = false;
					if (i == 0) {	//clicked the "create new" button
						message = "Textures Directory:";
						deepMenu = 0;
					} else if (i == 1) {
						message = "Map Directory:";
						deepMenu = 1;
					}
				}
			}
		} else {
			GUI.skin.label.fontSize = 24;
			GUI.skin.textField.fontSize = 24;
			GUI.Label(new Rect(Screen.width/2.0f - (GUI.skin.label.fontSize*message.Length)/4.0f, 3.0f*Screen.height/8.0f, GUI.skin.label.fontSize*1.5f*message.Length, GUI.skin.label.fontSize*1.5f), message);

			if (deepMenu == 0) {
				ProjectSettings.texturesPath = GUI.TextField(new Rect(Screen.width/3.0f, Screen.height/2.0f - GUI.skin.textField.fontSize*0.75f, Screen.width/3.0f, GUI.skin.label.fontSize*1.5f), ProjectSettings.texturesPath);
			} else if (deepMenu == 1) {
				ProjectSettings.mapPath = GUI.TextField(new Rect(Screen.width/3.0f, Screen.height/2.0f - GUI.skin.textField.fontSize*0.75f, Screen.width/3.0f, GUI.skin.label.fontSize*1.5f), ProjectSettings.mapPath);
			}

			for (int j = 0; j < deepMenuButtonList.Count; j++) {
				//buttonStyle.fontSize = (int)(1.5f*deepMenuButtonList[j].width/deepMenuButtonList[j].text.Length);
				GUI.DrawTexture(deepMenuButtonList[j].rect, deepMenuButtonList[j].texture, ScaleMode.StretchToFill);
				if (GUI.Button(deepMenuButtonList[j].rect, deepMenuButtonList[j].text, buttonStyle)) {
					if (deepMenu == 0) {					//selected "Create New"
						if (j == 0) {
							if (Directory.Exists(ProjectSettings.texturesPath)) {
								Application.LoadLevel("MapCreator_Designer");
							} else {
								//TODO show a warning -> "Does not exist that directory"
							}
						} else if (j == 1) {
							inMenu =  true;
						}
					} else if (deepMenu == 1) {			//selected "Edit existing"
						if (j == 0) {
							if (Directory.Exists(ProjectSettings.mapPath)) {
								//TODO load the map
							} else {
								//TODO show a warning -> "Does not exist that directory"
							}
						} else if (j == 1) {
							inMenu = true;
						}
					}
				}
			}
		}
	}

	void UpdateProjectSettings() {
		ProjectSettings.texturesPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
		ProjectSettings.mapPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
		string settingsDirectory = Application.persistentDataPath + "/Settings.txt";
		if (File.Exists(settingsDirectory)) {
			ReadSettings(settingsDirectory);
		} else {
			string[] settingArray =  new string[3];
			settingArray[0] = "GUI Menu Slots = 5;";
			settingArray[1] = "Render Distance = 50;";
			settingArray[2] = "Mouse sensitivity = 7;";

			System.IO.File.WriteAllLines(settingsDirectory, settingArray);
			ProjectSettings.guiMenuSlots = 5;
			ProjectSettings.renderDistance = 50;
			ProjectSettings.mouseSensitivity = 7;
		}
	}

	void ReadSettings(string path) {
		string[] settings = System.IO.File.ReadAllLines(path);

		foreach (string setting in settings) {
			string temp = "";
			int index = setting.IndexOf("=") + 2;

			while (char.IsDigit(setting[index])) {
				temp += setting[index];
				index++;
			}

			if (setting.Contains("GUI Menu Slots")) {
				ProjectSettings.guiMenuSlots = int.Parse(temp);
			} else if (setting.Contains("Render Distance")) {
				ProjectSettings.renderDistance = int.Parse(temp);
			} else if (setting.Contains("Mouse sensitivity")) {
				ProjectSettings.mouseSensitivity = int.Parse(temp);
			}
		}
	}

	void CreateDeepMenuButtons() {
		deepMenuButtonList.Add(new Button(Screen.width/10.0f, Screen.height/10.0f, new Vector2(3.0f*Screen.width/7.0f, 5.0f*Screen.height/8.0f), "Check", buttonBackground));
		deepMenuButtonList.Add(new Button(Screen.width/10.0f, Screen.height/10.0f, new Vector2(4.0f*Screen.width/7.0f, 5.0f*Screen.height/8.0f), "Cancel", buttonBackground));
	}

	void CreateButtons() {
		buttonList.Add(new Button(Screen.width/5.0f, Screen.height/10.0f, new Vector2(Screen.width/3.0f, Screen.height/2.0f), "Create New Map", buttonBackground)); //"create new" button
		buttonList.Add(new Button(Screen.width/5.0f, Screen.height/10.0f, new Vector2(2.0f*Screen.width/3.0f, Screen.height/2.0f), "Edit Existing", buttonBackground)); //"edit existent" button
	}
}
