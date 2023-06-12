using UnityEngine;

public class DialogueTriggerScript : MonoBehaviour
{
    public GameObject dialogueBoxUIParentObj;
    public bool dialogueFinished;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.CompareTag("Player") && !dialogueFinished)
        {
            dialogueBoxUIParentObj.SetActive(true);
            dialogueFinished = true;
        }
    }
}
