using UnityEngine;
using System.Collections;

public class GUIMenu {

	public Vector2 position;
	public float width;
	public float height;
	public int n_elements;
	public Texture texture;

	public GUIMenu (float w, int n, Texture new_texture) {
		width = w;
		n_elements = n;
		height = width*n_elements;
		position.x = Screen.width - width;
		position.y = Screen.height/2.0f - (width*n)/2.0f;
		texture = new_texture;
	}
}
