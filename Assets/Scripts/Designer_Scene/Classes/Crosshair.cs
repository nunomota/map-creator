using UnityEngine;
using System.Collections;

public class Crosshair {

	public float width;
	public float height;
	public Texture texture;

	public Crosshair(float side, Texture new_texture) {
		width = side;
		height = width;
		texture = new_texture;
	}
}
