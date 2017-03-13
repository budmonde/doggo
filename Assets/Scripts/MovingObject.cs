﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;
    public int kitchenCounter = 0;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float speed;

    protected virtual void Start() {
        boxCollider = GetComponent<BoxCollider2D>();

        rb2D = GetComponent<Rigidbody2D>();

        speed = 1f / moveTime;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit) {

        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null) {
            StartCoroutine(SmoothMovement(end));
            return true;
        } else {
            return false;
        }
    }

    protected IEnumerator SmoothMovement(Vector3 end) {

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, speed * Time.deltaTime);
            rb2D.MovePosition(newPostion);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected void incrementKitchenFood() {
        kitchenCounter++;
    }

    protected bool decrementKitchenFood() {
        if (kitchenCounter > 0) {
            kitchenCounter--;
            return true;
        }
        return false;
    }
}