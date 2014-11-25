using UnityEngine;
using System.Collections;

public class GUISelectedMenu {
	public Vector2 position;
	public Texture texture;
	public float width;
	public Rect rect1, rect2;
	public float scale;

	public GUISelectedMenu(Vector2 p, float w, Texture tex, float s) {
		position = p;
		width = w;
		texture = tex;
		scale = s;
		rect1 = new Rect(position.x, position.y, width, width);
		rect2 = new Rect(position.x + width/1.2f, position.y + width/1.2f, width*scale, width*scale);
	}
}
