using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovingDirectionCheck : MonoBehaviour
{
    public Enemy enemyScript;

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Whatever is in front of the enemy, change direction
        enemyScript.ChangeDirection();
    }
}
