using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class restore : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<SpriteRenderer>().sharedMaterial.color = Color.white;
	}
}
