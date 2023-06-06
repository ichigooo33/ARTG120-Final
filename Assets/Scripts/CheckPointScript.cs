using System;
using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    private BoxCollider2D _col;

    private void Start()
    {
        _col = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.CompareTag("Player"))
        {
            Debug.Log("ENTER");
            col.transform.GetComponent<PlayerController>().spawnPoint = transform;
        }
    }
}
