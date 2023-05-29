using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Transforms for detection")]
    public Transform player;
    public Transform rayPoint;

    [Header("Enemy Variables")]
    public float moveSpeed;
    public float waitTimeForChangingDirection = 0.5f;

    //Define components
    private Rigidbody2D _rb2D;

    //Define private variables
    private bool _isMoving = true;
    private int _moveDirectionIndex = 1;
    
    private void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //Change direction when no ground ahead OR something in front of the enemy
        if (_isMoving)
        {
            GroundCheck();
            MovingDirectionCheck();
            _rb2D.velocity = new Vector2(-moveSpeed, _rb2D.velocity.y);         //Using -moveSpeed because enemy facing left at the beginning
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void GroundCheck()
    {
        if (_isMoving)
        {
            RaycastHit2D groundHit = Physics2D.Raycast(rayPoint.position, Vector2.down, 1f);
            if (!groundHit)
            {
                Debug.DrawRay(rayPoint.position, Vector3.down * 1f, Color.red);
                _isMoving = false;
                Invoke("ChangeDirection", waitTimeForChangingDirection);
            }
            else
            {
                Debug.DrawRay(rayPoint.position, Vector3.down * 1f, Color.green);
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void MovingDirectionCheck()
    {
        if (_isMoving)
        {
            RaycastHit2D MovingDirectionHit = Physics2D.Raycast(rayPoint.position, Vector2.left * _moveDirectionIndex, 1f);
            if (MovingDirectionHit)
            {
                Debug.DrawRay(rayPoint.position, Vector2.left * _moveDirectionIndex, Color.red);
                _isMoving = false;
                Invoke("ChangeDirection", waitTimeForChangingDirection);
            }
            else
            {
                Debug.DrawRay(rayPoint.position, Vector2.left * _moveDirectionIndex, Color.green);
            }
        }
    }

    public void ChangeDirection()
    {
        transform.Rotate(Vector3.up, 180);
        moveSpeed *= -1;
        _moveDirectionIndex *= -1;
        _isMoving = true;
    }
}
