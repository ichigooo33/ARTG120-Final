using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.CompareTag("Map"))
        {
            ObjectPool.Instance.SetObject(this.name, this.gameObject);
        }
    }
}
