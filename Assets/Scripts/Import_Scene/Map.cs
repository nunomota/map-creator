using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map {

	public List<Texture> textureList;
	public GameObject parent;
	public int texturesToLoad, texturesLoaded = 0;

	public Map(Vector3 position) {
		parent = new GameObject();
		parent.transform.position = position;
	}
}
