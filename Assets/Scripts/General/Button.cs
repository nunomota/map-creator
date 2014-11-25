using UnityEngine;
using System.Collections;

public class Button {

	public float width;
	public float height;
	public string text;
	public Vector2 position;
	public Texture2D texture;
	public Rect rect;

	public Button(float w, float h, Vector2 pos, string t, Texture2D tex) {
		width = w;
		height = h;
		position = pos;
		text = t;
		texture = tex;
		rect = new Rect(position.x - width/2.0f, position.y - height/2.0f, width, height); /*
																							The coordinates are at the center of 
																							the button,
																							not in the top left corner
																							*/
	}
}