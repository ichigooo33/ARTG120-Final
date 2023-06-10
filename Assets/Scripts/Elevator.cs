using System;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Transform[] destinations;
    public float moveSpeed = 5f;
    public bool contactPlayer;

    private Transform _targetTransform;
    private bool _isReadyToMove = true;

    private void Update()
    {
        CheckInput();
        MoveToDestination();
    }

    private void CheckInput()
    {
        if (contactPlayer && _isReadyToMove)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _isReadyToMove = false;
                _targetTransform = destinations[0];
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _isReadyToMove = false;
                _targetTransform = destinations[1];
            }
        }
    }

    private void MoveToDestination()
    {
        if (!_isReadyToMove)
        {
            //Check if the elevator has arrived destination
            if (transform.position == _targetTransform.position)
            {
                _isReadyToMove = true;
                return;
            }
            
            // move sprite towards the target location
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, _targetTransform.position, step);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!contactPlayer && collision.transform.CompareTag("Player"))
        {
            contactPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            contactPlayer = false;
        }
    }
}
