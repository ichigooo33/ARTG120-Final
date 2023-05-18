using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot : MonoBehaviour
{
    //Get playerController script
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = transform.parent.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Update isGround if collide with "Map" object
        if (other.CompareTag("Map"))
        {
            _playerController.isGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Update isGround if collide with "Map" object
        if (other.CompareTag("Map"))
        {
            _playerController.isGround = false;
        }
    }
}
