using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeSpan = 2f;
    private float _lifeTimeCounter;

    private void Update()
    {
        _lifeTimeCounter += Time.deltaTime;
        
        if (_lifeTimeCounter > lifeSpan)
        {
            _lifeTimeCounter = 0;
            ObjectPool.Instance.SetObject(name, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.CompareTag("Enemy"))
        {
            Destroy(col.gameObject);
        }
        
        if (col.transform.CompareTag("Map"))
        {
            ObjectPool.Instance.SetObject(name, gameObject);
        }
    }
}
