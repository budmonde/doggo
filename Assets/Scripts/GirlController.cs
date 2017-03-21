using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlController : MovingObject {

	private Constants carryingObject = Constants.none;
    private string horizontalControls = "R_Horizontal";
    private string verticalControls = "R_Vertical";

	public AudioClip itemPickup;
	AudioSource audio;

    protected override void Start() {
        base.Start();
		audio = GetComponent<AudioSource> ();
    }
		
	void Update() {
		if (CanMove()) {
			int horizontal = (int)(Input.GetAxisRaw(horizontalControls));
			int vertical = (int)(Input.GetAxisRaw(verticalControls));
			if (horizontal != 0) {
				vertical = 0;
			}
			if (horizontal != 0 || vertical != 0) {
				RaycastHit2D hit;
				Transform transform = Move(horizontal, vertical, out hit);
				if (transform)
					interactWithObject(transform);
			}
		}
	}

	//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
	private void interactWithObject (Transform transform) {
		switch (transform.tag) {
		case "Pet":
			Debug.Log ("found pet!");
			PetController pet = (PetController)transform.GetComponent (typeof(PetController));
			if (carryingObject == Constants.food && pet.feed ()) {
				carryingObject = Constants.none;
			}
			break;
		case "Poop":
			Debug.Log ("found poop :(");
			if (!isCarrying ()) {
				carryingObject = Constants.poop;
				Destroy (transform);
				audio.PlayOneShot (itemPickup);
			}
			break;		
		case "Kitchen":
			Debug.Log ("found kitchen");
			if (!isCarrying ()) {
				audio.PlayOneShot (itemPickup);
				if (decrementKitchenFood ()) {
					carryingObject = Constants.food;
				}
			}
			break;
		case "Trash":
			Debug.Log ("found trash");
			carryingObject = Constants.none;
			break;
		}
	}

	public bool isCarrying() {
		return carryingObject != Constants.none;
	}

	public Constants getCarryingObject() {
		if (isCarrying ()) {
			return carryingObject;
		} else {
			return Constants.none;
		}
	}

}

