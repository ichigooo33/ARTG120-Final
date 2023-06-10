using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Player"))
        {
            Debug.Log("ELEVATOR TRIGGERED");
        }
    }
}
