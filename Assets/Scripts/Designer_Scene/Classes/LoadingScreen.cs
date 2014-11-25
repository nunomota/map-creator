using UnityEngine;
using System.Collections;

public class LoadingScreen {

	public Texture background;
	public LoadingBar loadingBar;

	public LoadingScreen(Texture bg) {
		background = bg;
		loadingBar = new LoadingBar(Screen.width/2.0f, Screen.height/20.0f, new Vector2(Screen.width/2.0f, 2.0f*Screen.height/3.0f), Resources.Load ("Textures/LoadingBar") as Texture, Resources.Load ("Textures/LoadingBarFill") as Texture);
	}

}
