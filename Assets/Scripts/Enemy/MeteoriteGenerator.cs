using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeteoriteGenerator : MonoBehaviour
{
    public GameObject meteorite;
    public float generateTime;
    public float generateRangeMin;
    public float generateRangeMax;

    private float _generateTimer;

    // Update is called once per frame
    void Update()
    {
        _generateTimer += Time.deltaTime;

        if (_generateTimer >= generateTime)
        {
            ObjectPool.Instance.GetObject("Meteorite", new Vector3(Random.Range(generateRangeMin, generateRangeMax), 40, 0),
                Quaternion.identity);
            _generateTimer = 0;
        }
    }
}
