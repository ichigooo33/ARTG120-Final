using UnityEngine;

public class StartScript : MonoBehaviour
{
    public GameObject dialogueObj;

    private void Start()
    {
        dialogueObj.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            dialogueObj.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
