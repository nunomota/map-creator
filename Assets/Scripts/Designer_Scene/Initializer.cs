using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Initializer : MonoBehaviour {

	private int width = 50;						//the width of the basic cube grid generated
	private int height = 50;					//the height of the basic cube grid generated

	private int buildDistance = 12;				//the max distance at which the player can create cubes
	private int renderDistance;					//the max distance of the camera rendering
	private int mouseSensitivity;				//the sensitivity of the mouse cursor, while playing
	private int maxTextureNumber = 256;			//represents the maximum number of textures that can be loaded
	private int maxCubeCoord = 256;				//represents the maximum value for any (x,y,z) coordinate

	private GameObject player;					//The gameobject that will be controlled
	private GameObject previewCube;				//the preview of the place where to create a new cube
	private float moveSpeed = 10.0f;			//the movement speed of the player

	private bool deleteMode = false;			//means the user is/not deleting cubes, instead of creating (toggled with "Right-Shift")

	private bool playing = false;				//means movement and creation commands are enabled/disabled
	private bool loading = true;				//means it is/not still loading the scene
	private bool saving = false;				//means it is/not still saving the scene
	private bool isSavingMap = false;			//means user is/not in a deep menu, "Save current map"
	private bool isChangingSettings = false;	//means user is/not in a deep menu, "Change settings"
	private bool isReturningToMenu = false;		//means user is/not in a deep menu, "Return to menu"

	private LoadingScreen loadingScreen;		//represents the screen with the loading bar, befora opening the Designer window

	private Crosshair crosshair;				//the crosshair that is in the middle of the screen
	private GUIMenu guiMenu;					//the menu that is shown on the right side of the screen
	private int guiMenuSlots;					//the number of slots in the guiMenu
	private GUISelectedMenu guiSelectedMenu;	//the preview of the textures that are being held by the player (bottom-left of the screen)

	private int texturesToLoad = 0;				//textures in the folder
	private int texturesLoaded = 0;				//textures loaded by the editor
	private int cubesToLoad = 0;				//cubes to generate
	private int cubesLoaded = 0;				//cubes loaded by the editor
	private float loadingProgress = 0.0f;		//the current loading porgress
	private int selectedTexture1 = -1;			//selected texture on the left hand
	private int selectedTexture2 = -1;			//selected texture on the right hand
	private int focusedTextureSlot = 0;			//the focused texture on the menu

	private int curGuiMenu = 0;					//the group of textures to display (as there can be many textures but just 4 will be shown)
	private int nGuiMenu;						//the number of guiMenus needed to display all textures loaded

	private GameObject objectToDestroy;			//object selected while in "deleteMode", that will be deleted

	private List<Button> menuButtonList = new List<Button>();					//the buttons of the main menu (toggled with "esc")
	private List<List<Button>> deepMenuButtonList = new List<List<Button>>();	//the buttons of the deep menus, toggled inside the main menu
	private GUIStyle buttonStyle = new GUIStyle();								//the style created for the buttons

	private List<Texture2D> textureList = new List<Texture2D>();				//the list of the textures loaded into the scene

	private string texturesPath;				//path from where the textures will be loaded
	private string newMapName = "";				//the name of the new map
	List<List<Vector3>> cubeGrid = new List<List<Vector3>>();					//used to store all the cubes, for saving

	void Start () {
		LoadSettings();								//loads the settings the were read from the file
		CreateLoadingScreen();						//Creates the loading screen related stuff
		cubesToLoad = width*height;					//sets the amount of basic cubes to generate
		StartCoroutine(CreateBasicBlocks());		//creates a square of basic blocks to allow construction
		InstantiatePlayer();						//creates the player
		InstantiatePreviewCube();					//creates the cube preview
		crosshair = new Crosshair(Screen.width/40.0f, (Texture)Resources.Load ("Textures/Crosshair"));				//creates the crosshair
		guiMenu = new GUIMenu(Screen.width/24.0f, guiMenuSlots, (Texture)Resources.Load ("Textures/Menu_Slot"));	//creates the guiMenu
		guiSelectedMenu = new GUISelectedMenu(new Vector2(Screen.width/10.0f, 3.0f*Screen.height/4.0f), Screen.width/20.0f, (Texture)Resources.Load ("Textures/Menu_Slot"), 1.0f);		//creates the selected textures preview

		PopulateTextures(texturesPath);				//loads the textures, from the path provided, into "textureList"
		SetNGuiMenu();								//sets the nGuiMenu value

		CreateMenuButtons();						//creates the buttons of the menu
		CreateDeepMenuButtons();					//creates the buttons of the deep menus

		buttonStyle.fontSize = 28;					//sets the size of the buttonStyle font

		DrawLimitWalls();							//Draws the walls that limit from 0 to 256, in all axys

	}

	void Update () {

		if (playing) {
			CreateCubePreview();	//calculates where to build and shows the preview
			CheckUserAction();		//checks for all sorts of inputs while playing
			CheckOtherInput();		//check for input like "esc"
		} else if (loading) {
			if ((texturesToLoad == texturesLoaded || texturesLoaded == maxTextureNumber) && cubesToLoad == cubesLoaded) {
				loading = false;
				loadingProgress = 100.0f;
				if (texturesToLoad > 0) {
					EnableControls();
					selectedTexture1 = 0;
					selectedTexture2 = texturesToLoad-1;
				} else {
					Application.LoadLevel("MapCreator_Menu");
				}
			} else {
				loadingProgress = (texturesLoaded + cubesLoaded) * 100.0f / (texturesToLoad + cubesToLoad);		//percentage of the loading progress
			}
		} else if (saving) {
			//do nothing; it is saving...
		} else {
			CheckOtherInput();		//check for input like "esc"
		}
	}

	void OnGUI() {

		if (playing) {
			GUI.DrawTexture(new Rect(Screen.width/2.0f - crosshair.width/2.0f, Screen.height/2.0f - crosshair.height/2.0f, crosshair.width, crosshair.height), crosshair.texture, ScaleMode.ScaleToFit);

			float offset;

			//draw the guiMenu;
			GUI.skin.label.fontSize = 24;
			GUI.Label(new Rect(guiMenu.position.x - guiMenu.width/2.0f, guiMenu.position.y + guiMenu.width*guiMenu.n_elements/2.0f - GUI.skin.label.fontSize*0.75f, GUI.skin.label.fontSize*1.5f, GUI.skin.label.fontSize*1.5f), curGuiMenu.ToString());
			for (int i = 0; i < guiMenu.n_elements; i++) {

				//Just to increase the size of the focused texture
				if (i == focusedTextureSlot) {
					offset = guiMenu.width/10.0f;
				} else {
					offset = 0;
				}

				GUI.DrawTexture(new Rect(guiMenu.position.x - offset, guiMenu.position.y + i*(guiMenu.width) - offset, guiMenu.width + 2.0f*offset, guiMenu.height/guiMenu.n_elements + 2.0f*offset), guiMenu.texture, ScaleMode.ScaleToFit);
				if (i + guiMenu.n_elements*curGuiMenu < textureList.Count) {
					offset += guiMenu.width / 8.0f;
					//GUI.Label(new Rect(guiMenu.position.x - offset, guiMenu.position.y + i*(guiMenu.width) - offset, offset*3.0f, offset*3.0f), (i+3).ToString());
					GUI.DrawTexture(new Rect(guiMenu.position.x + offset, guiMenu.position.y + i*(guiMenu.width) + offset, guiMenu.width - 2.0f*offset, guiMenu.width - 2.0f*offset), textureList[i + guiMenu.n_elements*curGuiMenu], ScaleMode.ScaleToFit);
				}
			}

			//draw the gui selected textures
			offset = guiSelectedMenu.width/8.0f;
			GUI.DrawTexture(guiSelectedMenu.rect1, guiSelectedMenu.texture, ScaleMode.ScaleToFit);
			GUI.DrawTexture(new Rect(guiSelectedMenu.rect1.x + offset, guiSelectedMenu.rect1.y + offset, guiSelectedMenu.width - 2.0f*offset, guiSelectedMenu.width - 2.0f*offset), textureList[selectedTexture1], ScaleMode.ScaleToFit);
			GUI.Label(new Rect(guiSelectedMenu.position.x - GUI.skin.label.fontSize*0.75f, guiSelectedMenu.position.y - GUI.skin.label.fontSize*0.75f, GUI.skin.label.fontSize*1.5f, GUI.skin.label.fontSize*1.5f), "1");
			GUI.DrawTexture(guiSelectedMenu.rect2, guiSelectedMenu.texture, ScaleMode.ScaleToFit);
			GUI.DrawTexture(new Rect(guiSelectedMenu.rect2.x + offset, guiSelectedMenu.rect2.y + offset, guiSelectedMenu.width - 2.0f*offset, guiSelectedMenu.width - 2.0f*offset), textureList[selectedTexture2], ScaleMode.ScaleToFit);
			GUI.Label(new Rect(guiSelectedMenu.rect2.x + guiSelectedMenu.width, guiSelectedMenu.rect2.y + guiSelectedMenu.width, GUI.skin.label.fontSize*1.5f, GUI.skin.label.fontSize*1.5f), "2");

		} else {
			if (loading) {

				//Draw the loading screen
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), loadingScreen.background, ScaleMode.StretchToFill);
				GUI.DrawTexture(new Rect(loadingScreen.loadingBar.rect.x, loadingScreen.loadingBar.rect.y, loadingProgress*loadingScreen.loadingBar.rect.width/100.0f, loadingScreen.loadingBar.rect.height), loadingScreen.loadingBar.fill, ScaleMode.StretchToFill);
				GUI.DrawTexture(loadingScreen.loadingBar.rect, loadingScreen.loadingBar.texture, ScaleMode.StretchToFill);
			
			} else if (saving) {
				//Draw something to prove it is actually saving

			} else {

				if (!IsInDeepMenu()) {

					//Draw the menu buttons
					for (int z = 0; z < menuButtonList.Count; z++) {
						//buttonStyle.fontSize = (int)(1.5f*menuButtonList[z].width/menuButtonList[z].text.Length);
						GUI.DrawTexture(menuButtonList[z].rect, menuButtonList[z].texture, ScaleMode.StretchToFill);
						if (GUI.Button(menuButtonList[z].rect, menuButtonList[z].text, buttonStyle)) {
							if (z == 0) {				//clicked the "save current map" button
								ClearDeepMenu();
								isSavingMap = true;
							} else if (z == 1) {		//clicked the "change settings" button
								ClearDeepMenu();
								isChangingSettings = true;
							} else if (z == 2) {		//clicked the "return to menu" button
								ClearDeepMenu();
								isReturningToMenu = true;
							}
						}
					}
				} else {

					if (isSavingMap) {

						string text = "Name of your map:";
						GUI.skin.textField.fontSize = GUI.skin.label.fontSize;
						GUI.Label(new Rect(Screen.width/2.0f - GUI.skin.label.fontSize*text.Length/4.0f, 3.0f*Screen.height/8.0f, GUI.skin.label.fontSize*text.Length/2.0f, GUI.skin.label.fontSize*text.Length/2.0f), text);
						newMapName = GUI.TextField(new Rect(Screen.width/3.0f, Screen.height/2.0f - GUI.skin.textField.fontSize*0.75f, Screen.width/3.0f, GUI.skin.textField.fontSize*1.5f), newMapName);
						newMapName = TreatedString(newMapName);

						for (int p = 0; p < deepMenuButtonList[0].Count; p++) {
							GUI.DrawTexture(deepMenuButtonList[0][p].rect, deepMenuButtonList[0][p].texture, ScaleMode.StretchToFill);
							if (GUI.Button(deepMenuButtonList[0][p].rect, deepMenuButtonList[0][p].text, buttonStyle)) {
								if (p == 0) {
									if (!File.Exists(texturesPath + "/" + newMapName + ".wmap")) {
										saving = true;
										WriteMapToFile(texturesPath + "/" + newMapName + ".wmap");
										saving = false;
										isSavingMap = false;
									} else {
										//TODO some warning: "File already exists"
									}
								} else if (p == 1) {
									isSavingMap = false;
								}
							}
						}

					} else if (isChangingSettings) {
						string message = "Mouse sensitivity:";
						GUI.Label(new Rect(Screen.width/2.0f - GUI.skin.label.fontSize*message.Length/4.5f, deepMenuButtonList[1][0].position.y - GUI.skin.label.fontSize/1.5f - deepMenuButtonList[1][0].height, 1.5f*GUI.skin.label.fontSize*message.Length, 1.5f*GUI.skin.label.fontSize), message);
						GUI.Label(new Rect(Screen.width/2.0f - GUI.skin.label.fontSize*(mouseSensitivity.ToString().Length)/3.0f, deepMenuButtonList[1][0].position.y - GUI.skin.label.fontSize/1.5f, 1.5f*GUI.skin.label.fontSize*(mouseSensitivity.ToString().Length), 1.5f*GUI.skin.label.fontSize), mouseSensitivity.ToString());
						message = "Render distance:";
						GUI.Label(new Rect(Screen.width/2.0f - GUI.skin.label.fontSize*message.Length/4.5f, deepMenuButtonList[1][2].position.y - GUI.skin.label.fontSize/1.5f - deepMenuButtonList[1][2].height, 1.5f*GUI.skin.label.fontSize*message.Length, 1.5f*GUI.skin.label.fontSize), message);
						GUI.Label(new Rect(Screen.width/2.0f - GUI.skin.label.fontSize*(renderDistance.ToString().Length)/3.0f, deepMenuButtonList[1][2].position.y - GUI.skin.label.fontSize/1.5f, 1.5f*GUI.skin.label.fontSize*(renderDistance.ToString().Length), 1.5f*GUI.skin.label.fontSize), renderDistance.ToString());

						for (int u = 0; u < deepMenuButtonList[1].Count; u++) {
							GUI.DrawTexture(deepMenuButtonList[1][u].rect, deepMenuButtonList[1][u].texture, ScaleMode.StretchToFill);
							if (GUI.Button(deepMenuButtonList[1][u].rect, deepMenuButtonList[1][u].text, buttonStyle)) {
								if (u == 0) {			//decrease mouse sensitivity
									if (mouseSensitivity > 1) {
										mouseSensitivity -= 1;
										SetMouseSensitivity();
									}
								} else if (u == 1) {	//increase mouse sensitivity
									if (mouseSensitivity < 10) {
										mouseSensitivity += 1;
										SetMouseSensitivity();
									}
								} else if (u == 2) {	//decrease render distance
									if (renderDistance > 20) {
										renderDistance -= 10;
										SetRenderDistance();
									}
								} else if (u == 3) {	//increase render distance
									if (renderDistance < 1000) {
										renderDistance += 10;
										SetRenderDistance();
									}
								}
							}
						}

					} else if (isReturningToMenu) {

						string message = "Are you sure?";
						GUI.Label(new Rect(Screen.width/2.0f - GUI.skin.label.fontSize*message.Length/4.0f, 3.0f*Screen.height/8.0f, GUI.skin.label.fontSize*message.Length/2.0f, GUI.skin.label.fontSize*message.Length/2.0f), message);

						for (int g = 0; g < deepMenuButtonList[2].Count; g++) {
							GUI.DrawTexture(deepMenuButtonList[2][g].rect, deepMenuButtonList[2][g].texture, ScaleMode.StretchToFill);
							if (GUI.Button(deepMenuButtonList[2][g].rect, deepMenuButtonList[2][g].text, buttonStyle)) {
								if (g == 0) {
									Application.LoadLevel("MapCreator_Menu");
								} else if (g == 1) {
									isReturningToMenu = false;
								}
							}
						}
					}
				}
			}
		}
	}

	void LoadSettings() {
		guiMenuSlots = ProjectSettings.guiMenuSlots;
		texturesPath = ProjectSettings.texturesPath;
		renderDistance = ProjectSettings.renderDistance;
		mouseSensitivity = ProjectSettings.mouseSensitivity;
	}

	void SetMouseSensitivity () {
		ProjectSettings.mouseSensitivity = mouseSensitivity;
		player.GetComponent<MouseLook>().sensitivityX = (float)mouseSensitivity;
		player.GetComponent<MouseLook>().sensitivityY = (float)mouseSensitivity;
	}

	void SetRenderDistance() {
		ProjectSettings.renderDistance = renderDistance;
		Camera.main.farClipPlane = (float)renderDistance;
	}

	void CreateLoadingScreen() {
		loadingScreen = new LoadingScreen(Resources.Load ("Textures/LoadingScreenBackground") as Texture);
	}

	void PopulateTextures(string path) {
		DirectoryInfo dir = new DirectoryInfo(path);
		FileInfo[] info = dir.GetFiles("*.png");
		int counter = 0;
		texturesToLoad = info.Length;
		foreach (FileInfo f in info)
		{
			if (counter < maxTextureNumber) {
				counter++;
				GetImage(f.FullName);
			} else {
				return;
			}
		}
	}

	void GetImage(string url) {
		WWW www =  new WWW("file://" + url);
		Texture2D newTexture = new Texture2D(32, 32);
		StartCoroutine(WaitForImageDownload(www, newTexture));
	}
	
	IEnumerator WaitForImageDownload(WWW www, Texture2D newTexture) {
		yield return www;
		www.LoadImageIntoTexture(newTexture);
		textureList.Add(newTexture);
		texturesLoaded++;
	}

	void EnableControls() {
		loading = false;
		playing = true;
		Screen.lockCursor = true;
		player.GetComponent<MouseLook>().enabled = true;
	}

	IEnumerator CreateBasicBlocks() {

		GameObject basicCube = Resources.Load ("Prefabs/Basic_Cube") as GameObject;
		Vector3 centerVector = new Vector3((float)((int)(maxCubeCoord/2.0f) - (int)(width/2.0f)), 0, (float)((int)(maxCubeCoord/2.0f) - (int)(width/2.0f)));

		for (int x = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {
				Instantiate (basicCube, new Vector3(x, -1, z) + centerVector, Quaternion.identity);
				cubesLoaded++;
				yield return null;
			}
		}
	}

	void InstantiatePlayer() {
		Vector3 centerVector = new Vector3((float)((int)(maxCubeCoord/2.0f) - (int)(width/2.0f)), 0, (float)((int)(maxCubeCoord/2.0f) - (int)(width/2.0f)));
		player = (GameObject)Instantiate(Resources.Load ("Prefabs/Player_Prefab") as GameObject, new Vector3(-6, 3, -6) + centerVector, Quaternion.Euler(0, 45, 0));
		mouseSensitivity = ProjectSettings.mouseSensitivity;		//loads the mouse sensitivity
		SetMouseSensitivity();
		SetRenderDistance();
	}

	void InstantiatePreviewCube() {
		previewCube = (GameObject)Instantiate (Resources.Load("Prefabs/Preview_Cube") as GameObject, new Vector3(0, 1, 0), Quaternion.identity);
		previewCube.renderer.enabled = false;
	}

	void SetNGuiMenu() {
		nGuiMenu = (int)(texturesToLoad/guiMenu.n_elements);
		if (texturesToLoad%guiMenu.n_elements != 0) {
			nGuiMenu++;
		}
	}

	void CreateMenuButtons() {
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		Texture2D buttonBackground = Resources.Load ("Textures/Preview_Cube") as Texture2D;

		menuButtonList.Add(new Button(Screen.width/5.0f, Screen.height/10.0f, new Vector2(Screen.width/2.0f, 3.0f*Screen.height/8.0f), "Save current map", buttonBackground));			//"Save" button
		menuButtonList.Add(new Button(Screen.width/5.0f, Screen.height/10.0f, new Vector2(Screen.width/2.0f, 4.0f*Screen.height/8.0f), "Change settings", buttonBackground));			//"Settings" button
		menuButtonList.Add(new Button(Screen.width/5.0f, Screen.height/10.0f, new Vector2(Screen.width/2.0f, 5.0f*Screen.height/8.0f), "Return to menu", buttonBackground));			//"exit" button
	}

	void CreateDeepMenuButtons() {
		Texture2D buttonBackground = Resources.Load ("Textures/Preview_Cube") as Texture2D;

		deepMenuButtonList.Add(new List<Button>());		//list of buttons for the "Save current map" deep menu
		deepMenuButtonList.Add(new List<Button>());		//list of buttons for the "Change settings" deep menu
		deepMenuButtonList.Add(new List<Button>());		//list of buttons for the "Return to menu" deep menu

		//add the buttons for "Save current map"
		deepMenuButtonList[0].Add(new Button(Screen.width/10.0f, Screen.height/10.0f, new Vector2(3.0f*Screen.width/7.0f, 5.0f*Screen.height/8.0f), "Save", buttonBackground));
		deepMenuButtonList[0].Add(new Button(Screen.width/10.0f, Screen.height/10.0f, new Vector2(4.0f*Screen.width/7.0f, 5.0f*Screen.height/8.0f), "Cancel", buttonBackground));

		//add the buttons for "Change settings"

		deepMenuButtonList[1].Add(new Button(Screen.height/10.0f, Screen.height/10.0f, new Vector2(4.0f*Screen.width/10.0f , 2.0f*Screen.height/5.0f), "<", buttonBackground));
		deepMenuButtonList[1].Add(new Button(Screen.height/10.0f, Screen.height/10.0f, new Vector2(6.0f*Screen.width/10.0f , 2.0f*Screen.height/5.0f), ">", buttonBackground));
		deepMenuButtonList[1].Add(new Button(Screen.height/10.0f, Screen.height/10.0f, new Vector2(4.0f*Screen.width/10.0f , 3.0f*Screen.height/5.0f), "<", buttonBackground));
		deepMenuButtonList[1].Add(new Button(Screen.height/10.0f, Screen.height/10.0f, new Vector2(6.0f*Screen.width/10.0f , 3.0f*Screen.height/5.0f), ">", buttonBackground));

		//add the buttons for "Return to Menu"
		deepMenuButtonList[2].Add(new Button(Screen.width/10.0f, Screen.height/10.0f, new Vector2(3.0f*Screen.width/7.0f, 5.0f*Screen.height/8.0f), "Yes", buttonBackground));
		deepMenuButtonList[2].Add(new Button(Screen.width/10.0f, Screen.height/10.0f, new Vector2(4.0f*Screen.width/7.0f, 5.0f*Screen.height/8.0f), "No", buttonBackground));
	}

	void DrawLimitWalls() {
		GameObject wall;
		wall = (GameObject)Instantiate(Resources.Load("Prefabs/Limit_Cube"), new Vector3((float)maxCubeCoord/2.0f - 0.5f, (float)maxCubeCoord/2.0f - 0.5f, -1.0f), Quaternion.identity);
		wall.transform.localScale = new Vector3((float)maxCubeCoord, (float)maxCubeCoord, 1);
		wall = (GameObject)Instantiate(Resources.Load("Prefabs/Limit_Cube"), new Vector3((float)maxCubeCoord/2.0f - 0.5f, (float)maxCubeCoord/2.0f - 0.5f, (float)(maxCubeCoord)), Quaternion.identity);
		wall.transform.localScale = new Vector3((float)maxCubeCoord, (float)maxCubeCoord, 1);
		wall = (GameObject)Instantiate(Resources.Load("Prefabs/Limit_Cube"), new Vector3(-1.0f, (float)maxCubeCoord/2.0f - 0.5f, (float)maxCubeCoord/2.0f - 0.5f), Quaternion.identity);
		wall.transform.localScale = new Vector3(1, (float)maxCubeCoord, (float)maxCubeCoord);
		wall = (GameObject)Instantiate(Resources.Load("Prefabs/Limit_Cube"), new Vector3((float)(maxCubeCoord), (float)maxCubeCoord/2.0f - 0.5f, (float)maxCubeCoord/2.0f - 0.5f), Quaternion.identity);
		wall.transform.localScale = new Vector3(1, (float)maxCubeCoord, (float)maxCubeCoord);
		wall = (GameObject)Instantiate(Resources.Load("Prefabs/Limit_Cube"), new Vector3((float)maxCubeCoord/2.0f - 0.5f, (float)(maxCubeCoord), (float)maxCubeCoord/2.0f - 0.5f), Quaternion.identity);
		wall.transform.localScale = new Vector3((float)maxCubeCoord, 1, (float)maxCubeCoord);
	}

	void CheckUserAction() {
		MovePlayer();
		HandleBlockAction();

		//change the texture being focused in the guiMenu
		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
			focusedTextureSlot -= (int)(Input.GetAxis("Mouse ScrollWheel")/Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")));

			if (focusedTextureSlot >= guiMenu.n_elements) {
				focusedTextureSlot -= guiMenu.n_elements;
			} else if (focusedTextureSlot < 0) {
				focusedTextureSlot += guiMenu.n_elements;
			}
		}

		CheckNumericInput();
	}

	void MovePlayer() {

		Vector3 moveDirection = new Vector3(0, 0, 0);
		float up = 0;

		if (Input.GetButton ("Horizontal")) {
			moveDirection += Input.GetAxis("Horizontal") * player.transform.right;
		}

		if (Input.GetButton ("Vertical")) {
			moveDirection += Input.GetAxis("Vertical") * player.transform.forward;
		}

		if (Input.GetButton("Jump")) {
			up = 1;
		} else if (Input.GetKey(KeyCode.LeftShift)) {
			up = -1;
		}
		
		moveDirection.y = 0.4f * up;

		player.transform.rigidbody.velocity = moveDirection * moveSpeed;
	}

	void HandleBlockAction() {
		if (previewCube.renderer.enabled) {
			if (Input.GetMouseButtonDown(0)) {
				if (!deleteMode) {
					CreateBlock(selectedTexture1);
				} else {
					DestroyBlock();
				}
			} else if (Input.GetMouseButtonDown(1)) {
				if (!deleteMode) {
					CreateBlock(selectedTexture2);
				} else {
					//do something...?
				}
			}
		}
	}

	void CheckNumericInput() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {					//change the texture that is being held in the left hand
			ChangeHeldTexture(1);
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {			//change the texture that is being held in the right hand
			ChangeHeldTexture(2);
		} else if (Input.GetKeyDown(KeyCode.Alpha3)) {			//change the set of thextures to show (changes the curGuiMenu), backwards
			ChangeCurGuiMenu(-1);
		} else if (Input.GetKeyDown(KeyCode.Alpha4)) {			//change the set of thextures to show (changes the curGuiMenu), onwards
			ChangeCurGuiMenu(1);
		}
	}

	void CreateBlock(int selectedTexture) {
		GameObject new_cube;
		new_cube = (GameObject)Instantiate(Resources.Load("Prefabs/Basic_Cube"), previewCube.transform.position, Quaternion.identity);
		new_cube.renderer.material.mainTexture = textureList[selectedTexture];
		new_cube.tag = "Editable";
		new_cube.name = selectedTexture.ToString();
	}

	void DestroyBlock() {
		if (objectToDestroy != null) {
			Destroy(objectToDestroy);
		}
	}

	void ChangeHeldTexture(int n) {
		if (focusedTextureSlot + guiMenu.n_elements*curGuiMenu < textureList.Count) {
			if (n == 1) {
				selectedTexture1 = focusedTextureSlot + guiMenu.n_elements*curGuiMenu;
			} else {
				selectedTexture2 = focusedTextureSlot + guiMenu.n_elements*curGuiMenu;
			}
		}
	}

	void ChangeCurGuiMenu(int n) {
		curGuiMenu += n;
		if (curGuiMenu < 0) {
			curGuiMenu += nGuiMenu;
		} else if (curGuiMenu >= nGuiMenu) {
			curGuiMenu -= nGuiMenu;
		}
	}

	void CheckOtherInput() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (!IsInDeepMenu()) {

				playing = !playing;
				player.GetComponent<MouseLook>().enabled = playing;
				Screen.lockCursor = playing;

			} else {
				ClearDeepMenu();
			}
		}

		if (Input.GetKeyDown (KeyCode.RightShift)) {
			deleteMode = !deleteMode;
		}
	}

	bool IsInDeepMenu() {
		if (!isSavingMap && !isChangingSettings && !isReturningToMenu) {
			return false;
		}
		return true;
	}

	void ClearDeepMenu() {
		isSavingMap = false;
		isChangingSettings = false;
		isReturningToMenu = false;
	}

	void CreateCubePreview() {

		Ray ray = Camera.main.ViewportPointToRay (new Vector3(0.5f,0.5f,0));
		RaycastHit hitInfo;

		if (Physics.Raycast(ray, out hitInfo) && Vector3.Distance(hitInfo.point, player.transform.position) < buildDistance) {
			Transform gridCube = hitInfo.transform;
			Vector3 previewPosition;
			objectToDestroy = null;
			if (!deleteMode) {
				previewPosition = gridCube.position + hitInfo.normal.normalized;
				previewCube.renderer.material.color = Color.white;
				previewCube.transform.localScale = new Vector3(1, 1, 1);
			} else {
				previewPosition = gridCube.position;
				if (gridCube.gameObject.tag == "Editable") {
					objectToDestroy = gridCube.gameObject;
				}
				previewCube.renderer.material.color = Color.red;
				previewCube.transform.localScale = new Vector3(1.02f, 1.02f, 1.02f);
			}
			previewCube.transform.position = previewPosition;

			if ((previewPosition.x >= 0 && previewPosition.x < maxCubeCoord) && (previewPosition.y >= 0 && previewPosition.y < maxCubeCoord) && (previewPosition.z >= 0 && previewPosition.z < maxCubeCoord)) {
				previewCube.renderer.enabled = true;
			} else {
				previewCube.renderer.enabled = false;
			}
		} else {
			previewCube.renderer.enabled = false;
		}
	}

	string TreatedString(string str) {
		string temp = "";
		for (int i = 0; i < str.Length; i++) {
			if (CharAllowed(str[i])) {
				temp += str[i];
			}
		}
		return temp;
	}

	bool CharAllowed(char car) {
		string allowedChars = "_";
		string character = "";
		character += car;
		if (char.IsLetterOrDigit(car) || allowedChars.Contains(character)) {
			return true;
		}
		return false;
	}

	void WriteMapToFile(string path) {
		GameObject[] cubeArray;
		Vector3 smallestVector;
		cubeArray = GameObject.FindGameObjectsWithTag("Editable");
		ClearFileRelatedVariables();

		if (cubeArray.Length > 0) {
			for (int i = 0; i < textureList.Count; i++) {
				cubeGrid.Add(new List<Vector3>());
			}
			smallestVector = GetSmallestVector(cubeArray);
			AdjustCubeGrid(smallestVector);
			/*
			TODO write the file header (8 bit signature, 8 bit version)
			TODO write 3 bits to the file (x); these 3 bits represent the integer number of bits needed to represent "biggestCoord", less one -> the "biggestCoord" will never be 0 bits long. 3 bits now represent [1...8]
			TODO write 3 bits to the file (y); these 3 bits represent the integer number of bits needed to represent "textureList.Count", less one -> same explanation as above
			TODO write y bits to the file (represents the texture's number being addressed)
			TODO write x bits to the file, for each cube with that texture, for each of his x,y,z coordinates
			TODO write "0" 6 times, with x bits each of the times; This would mean 2 cubes at (0, 0, 0), which is not possible -> this way I know that, after this sequence, a new texture is goind to start
			-------Preview-------
			|Defines "x"|Defines "y"|Texture nº | Vector.x  | Vector.y  | Vector.z  | Vector.x  | Vector.y  | Vector.z  |  0  |  0  |  0  |  0  |  0  |  0  |...
			|     3     |     3     |     y     |     x     |     x     |     x     |     x     |     x     |     x     |  x  |  x  |  x  |  x  |  x  |  x  |...
			---------------------
			 */

			FillByteArray();
			WriteByteArrayToFile(path);
		}
	}

	void ClearFileRelatedVariables() {
		ProjectSettings.byteNumber = 0;
		for (int i = 0; i < cubeGrid.Count; i++) {
			cubeGrid[i].Clear();
		}
		cubeGrid.Clear();

	}

	Vector3 GetSmallestVector(GameObject[] cubeArray) {
		Vector3 smallestVector = cubeArray[0].transform.position;
		foreach(GameObject cube in cubeArray) {
			if (cube.transform.position.x < smallestVector.x) {
				smallestVector.x = cube.transform.position.x;
			}
			if (cube.transform.position.y < smallestVector.y) {
				smallestVector.y = cube.transform.position.y;
			}
			if (cube.transform.position.z < smallestVector.z) {
				smallestVector.z = cube.transform.position.z;
			}
			AddCubeToGrid(cube);
		}
		return smallestVector;
	}

	void AddCubeToGrid(GameObject cube) {
		cubeGrid[int.Parse(cube.name)].Add(cube.transform.position);
		ProjectSettings.byteNumber += 3;
	}

	void AdjustCubeGrid(Vector3 smallestVector) {
		//subtract "smallestVector" to every vector in the grid
		for (int i = 0; i < cubeGrid.Count; i++) {
			for (int j = 0; j < cubeGrid[i].Count; j++) {
				cubeGrid[i][j] -= smallestVector;
			}
		}
	}

	void FillByteArray() {
		int index, bytesToSave;
		bytesToSave = 2 + ProjectSettings.byteNumber + GetNumberOfTexturesUsed()*7;
		ProjectSettings.byteArray = new byte[bytesToSave];
		ProjectSettings.byteArray[0] = (byte)ProjectSettings.fileSignature;
		ProjectSettings.byteArray[1] = (byte)ProjectSettings.programVersion;

		index = 2;
		for (int i = 0; i < cubeGrid.Count; i++) {
			if (cubeGrid[i].Count > 0) {
				ProjectSettings.byteArray[index] = (byte)((int)i);			//write the byte with the texture number
				index += 1;
				for (int j = 0; j < cubeGrid[i].Count; j++) {
					ProjectSettings.byteArray[index] = (byte)((int)cubeGrid[i][j].x);		//write the byte with the X value
					ProjectSettings.byteArray[index+1] = (byte)((int)cubeGrid[i][j].y);		//write the byte with the Y value
					ProjectSettings.byteArray[index+2] = (byte)((int)cubeGrid[i][j].z);		//write the byte with the Z value
					index += 3;
				}

				for (int g = 0; g < 6; g++) {
					ProjectSettings.byteArray[index] = (byte)((int)0);		//write six "0" bytes
					index += 1;
				}
			}
		}
	}

	int GetNumberOfTexturesUsed() {
		int texturesUsed = 0;
		for (int i = 0; i < cubeGrid.Count; i++) {
			if (cubeGrid[i].Count > 0) {
				texturesUsed++;
			}
		}
		return texturesUsed;
	}

	void WriteByteArrayToFile(string path) {
		File.WriteAllBytes(path, ProjectSettings.byteArray);
	}	
}