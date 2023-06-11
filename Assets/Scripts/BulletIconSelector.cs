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

    public void UpdateIconSprite()
    {
        _image.sprite = bulletIconArray[PlayerControllerScript.currentBulletIndex];
    }
}
