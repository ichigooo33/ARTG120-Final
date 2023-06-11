using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    public Elevator elevatorScript;
    public Transform levelTransform;
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Player") && elevatorScript.isReadyToMove)
        {
            Debug.Log("ELEVATOR TRIGGERED");
            elevatorScript.isReadyToMove = false;
            elevatorScript.targetTransform = levelTransform;
        }
    }
}
