using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Transform[] destinations;
    public Transform targetTransform;
    public float moveSpeed = 5f;
    public bool contactPlayer;
    public bool isReadyToMove = true;

    private void Update()
    {
        CheckInput();
        MoveToDestination();
    }

    private void CheckInput()
    {
        if (contactPlayer && isReadyToMove)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                isReadyToMove = false;
                targetTransform = destinations[0];
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                isReadyToMove = false;
                targetTransform = destinations[1];
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                isReadyToMove = false;
                targetTransform = destinations[2];
            }
        }
    }

    private void MoveToDestination()
    {
        if (!isReadyToMove)
        {
            //Check if the elevator has arrived destination
            if (transform.position == targetTransform.position)
            {
                isReadyToMove = true;
                return;
            }
            
            // move sprite towards the target location
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetTransform.position, step);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!contactPlayer && collision.transform.CompareTag("Player"))
        {
            contactPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            contactPlayer = false;
        }
    }
}
