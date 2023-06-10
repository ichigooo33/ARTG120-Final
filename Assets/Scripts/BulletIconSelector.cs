using UnityEngine;
using UnityEngine.UI;

public class BulletIconSelector : MonoBehaviour
{
    public Sprite[] bulletIconArray;
    public PlayerController PlayerControllerScript;
    
    private Image _image;
    private void Start()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _image.sprite = bulletIconArray[PlayerControllerScript.currentBulletIndex];
        }
    }
}
