using UnityEngine;
using System.Collections;

public class MoveToScript : MonoBehaviour {

	private Rigidbody2D rb;
	private GameObject go;
	private RectTransform rt;
	bool mousedown = false;

	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody2D> ();
		go = this.gameObject;
		rt = this.GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){
		mousedown = true;
	}

	void OnMouseUp(){
		mousedown = false;

	}

	void FixedUpdate(){
		if (mousedown) {
			float distance_to_screen = Camera.main.WorldToScreenPoint (gameObject.transform.position).z;
			Vector2 mouse = Camera.main.ScreenToWorldPoint (new Vector2 (Input.mousePosition.x, Input.mousePosition.y));
			rt.pivot = mouse;
			rb.MovePosition (mouse);
		}
	}


}
