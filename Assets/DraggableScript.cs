﻿using UnityEngine;
using System.Collections;

/*
Attach this script to the object you want to drag. The object should be a non-kinematic rigidbody. Gravity can be on or off; Freeze Rotation can also be on or off. Mass, drag, and angular drag generally don't matter, although mass matters somewhat when dragging into other non-kinematic rigidbodies. There are some public variables:
Normal Collision Count: This is an important variable, since how well the script works depends on this being correct. If the object is normally resting on the ground while being dragged, set this variable to 1. If it's not in contact with the ground while being dragged (or if there is no ground at all), then set this variable to 0.
Move Limit: Prevents the dragged object from going through other objects when being dragged quickly. This creates some lag when dragging objects at high speeds, since the object won't be able to keep up with the mouse cursor. However, if you want to reduce the lag, it's unlikely you would want to set this above the default 0.5. Instead, you can increase the number of physics frames per second (which increases CPU requirements, so be careful with that...e.g., 0.2 physics timestep is the default 50 fps, and .1 would make it 100 fps).
Collision Move Factor: How much dragging force is slowed down when colliding with an object. The default .01 is good for ensuring that the dragged object remains stable when colliding, but makes it somewhat "sticky" when trying to slide it along other objects. You can increase the value to make it less sticky, but larger values may cause some instability when colliding.
Add Height When Clicked: Number of units to add to the object's height when clicked on. The default 0 means nothing happens. This can be used for "picking up" objects when clicked. If the object is raised enough so that it's no longer in contact with the floor when it's being dragged, remember to set Normal Collision Count to 0. The object will either be restored to its usual height when let go if it doesn't use gravity, or else it will naturally fall back down to the floor if it does use gravity.
Freeze Rotation On Drag: If this is checked, the object will not rotate when being dragged around and colliding with other objects, regardless of whether the rotation is normally frozen. If it's unchecked, then it can rotate when colliding.
Cam: The camera being used. If this is not set, the camera tagged "Main Camera" will be used. (And if that's not found, an error will occur.)
*/

[RequireComponent(typeof(Rigidbody2D))]
public class DraggableScript : MonoBehaviour
{
	
	public int normalCollisionCount = 1;
	public float moveLimit = .5f;
	public float collisionMoveFactor = .01f;
	public float addHeightWhenClicked = 0.0f;
	public bool freezeRotationOnDrag = true;
	public Camera cam;
	private Rigidbody2D myRigidbody ;
	private Transform myTransform  ;
	private bool canMove = false;
	private float yPos;
	private float gravitySetting ;
	private bool freezeRotationSetting ;
	private float sqrMoveLimit ;
	private int collisionCount = 0;
	private Transform camTransform ;
	
	void Start () 
	{
		myRigidbody = GetComponent<Rigidbody2D>();
		myTransform = transform;
		if (!cam) 
		{
			cam = Camera.main;
		}
		if (!cam) 
		{
			Debug.LogError("Can't find camera tagged MainCamera");
			return;
		}
		camTransform = cam.transform;
		sqrMoveLimit = moveLimit * moveLimit;   // Since we're using sqrMagnitude, which is faster than magnitude
	}
	
	void OnMouseDown () 
	{
		canMove = true;
		myTransform.Translate(Vector3.up*addHeightWhenClicked);
		gravitySetting = myRigidbody.gravityScale;
		freezeRotationSetting = myRigidbody.freezeRotation;
		myRigidbody.gravityScale = 0;
		myRigidbody.freezeRotation = freezeRotationOnDrag;
		yPos = myTransform.position.y;
	}	

	void OnMouseUp () 
	{
		canMove = false;
		myRigidbody.gravityScale = gravitySetting;
		myRigidbody.freezeRotation = freezeRotationSetting;
		if (myRigidbody.gravityScale == 1) 
		{
			Vector3 pos = myTransform.position;
			pos.y = yPos-addHeightWhenClicked;
			myTransform.position = pos;
		}
	}
	
	void OnCollisionEnter () 
	{
		collisionCount++;
	}
	
	void OnCollisionExit () 
	{
		collisionCount--;
	}
	
	void FixedUpdate () 
	{
		if (!canMove)
		{
			return;
		}


		myRigidbody.velocity = Vector2.zero;
		myRigidbody.angularVelocity = 0;

		Vector3 pos = myTransform.position;
		pos.y = yPos;
		myTransform.position = pos;
		
		Vector2 mousePos = Input.mousePosition;
		Vector3 move = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camTransform.position.y - myTransform.position.y)) - myTransform.position;
		move.y = 0.0f;
		if (collisionCount > normalCollisionCount)		
		{
			move = move.normalized*collisionMoveFactor;
		}
		else if (move.sqrMagnitude > sqrMoveLimit) 
		{
			move = move.normalized*moveLimit;
		}

		Vector2 move2 = new Vector2 (move.x, move.y);

		myRigidbody.MovePosition(myRigidbody.position + move2);
	}
}
