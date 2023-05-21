using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public Transform groundDetectionPoint;

    public float moveSpeed;

    private Rigidbody2D _rb2D;

    private void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundDetectionPoint.position, Vector2.down, 3f);

        if (hit.collider == null)
        {
            //Debug.Log("Nothing at front!");
            transform.Rotate(Vector3.up, 180);
            moveSpeed *= -1;
        }
        _rb2D.velocity = Vector2.left * moveSpeed;
    }
}
