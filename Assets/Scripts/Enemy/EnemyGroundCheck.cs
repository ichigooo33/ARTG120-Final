using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundCheck : MonoBehaviour
{
    public Enemy enemyScript;

    private void OnTriggerExit2D(Collider2D col)
    {
        //If GroundCheck trigger leave "MAP" object, which means nothing ahead, change direction
        if (col.CompareTag("Map"))
        {
            enemyScript.ChangeDirection();
        }
    }
}
