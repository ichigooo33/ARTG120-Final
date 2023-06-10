using System;
using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    private BoxCollider2D _col;
    public PlayerController PlayerControllerScript;

    public bool canUnlockBranch;
    public bool canUnlockWater;
    public bool canUnlockFire;
    
    private void Start()
    {
        _col = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.CompareTag("Player"))
        {
            Debug.Log("CHECK POINT ENTER");
            PlayerControllerScript.spawnPoint = transform;
        }
    }
}
