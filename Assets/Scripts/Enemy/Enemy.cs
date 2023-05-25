using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Transforms for detection")]
    public Transform player;

    [Header("Enemy Variables")]
    public float moveSpeed;

    //Define components
    private Rigidbody2D _rb2D;

    private void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //Change direction when no ground ahead OR something in front of the enemy
        _rb2D.velocity = new Vector2(-moveSpeed, _rb2D.velocity.y);         //Using -moveSpeed because enemy facing left at the beginning
    }

    public void ChangeDirection()
    {
        transform.Rotate(Vector3.up, 180);
        moveSpeed *= -1;
    }
}
