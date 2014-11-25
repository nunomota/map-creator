using UnityEngine;
using System.Collections;

public class LoadingBar {

	public Vector2 position;
	public Texture texture;
	public Texture fill;
	public float width, height;
	public Rect rect;

	public LoadingBar(float w, float h, Vector2 pos, Texture tex, Texture fillTexture) {
		width = w;
		height = h;
		position = pos;
		texture = tex;
		fill = fillTexture;

		rect = new Rect(position.x - width/2.0f, position.y - height/2.0f, width, height);	/*
																							The coordinates are at the center of 
																							the button,
																							not in the top left corner
																							*/
	}

}
