using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DadController : MovingObject {

    // Config
	private Animator dadAnimator;
	public AudioClip itemPickup;
	public AudioClip itemDropoff;
	public AudioClip itemTrashed;
	new AudioSource audio;

    // Controller constants
    private string horizontalControls = "L_Horizontal";
    private string verticalControls = "L_Vertical";

    // Carrying object
    private Constants carryingObject = Constants.none;	

    protected override void Start() {
		dadAnimator = gameObject.GetComponent<Animator> ();
		audio = GetComponent<AudioSource> ();
		base.Start();
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
                    InteractWithObject(transform);
            }
        }
	}

	private void InteractWithObject (Transform transform) {
		switch (transform.tag) {
		case "Pet":
			PetController pet = (PetController)transform.GetComponent (typeof(PetController));
			if (carryingObject == Constants.food && pet.feed ()) {
				carryingObject = Constants.none;
				dadAnimator.SetTrigger ("dadIdle");
				audio.PlayOneShot (itemDropoff);
			}
			break;
		case "Poop":
			if (carryingObject == Constants.none) {
				Destroy (transform.gameObject);
				carryingObject = Constants.poop;
				dadAnimator.SetTrigger ("dadPickUpPoop");
				audio.PlayOneShot (itemPickup);
			}
			break;
		case "Kitchen":
			if (carryingObject == Constants.delivery) {
				incrementKitchenFood ();
                carryingObject = Constants.none;
				dadAnimator.SetTrigger ("dadIdle");
				audio.PlayOneShot (itemDropoff);
			} else if (carryingObject == Constants.none) {
				if (decrementKitchenFood ()) {
                    carryingObject = Constants.food;
					dadAnimator.SetTrigger ("dadPickUpFud");
					audio.PlayOneShot (itemPickup);
				}
			}
			break;
		case "Door":
			if (carryingObject == Constants.none) {
                carryingObject = Constants.delivery;
				dadAnimator.SetTrigger ("dadPickUpDelivery");
				audio.PlayOneShot (itemPickup);
			}
			break;
		case "Trash":
			if (carryingObject != Constants.none) {
				carryingObject = Constants.none;
				dadAnimator.SetTrigger ("dadIdle");
				audio.PlayOneShot (itemTrashed);
			}
			break;
		}
	}
}

