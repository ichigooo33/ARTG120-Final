using UnityEngine;
using UnityEngine.UI;

public class BulletIconSelector : MonoBehaviour
{
    public Sprite[] bulletIconArray;
    
    private Image _image;
    private int _bulletIconIndex;
    private void Start()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _bulletIconIndex++;
            if (_bulletIconIndex >= bulletIconArray.Length)
            {
                _bulletIconIndex -= bulletIconArray.Length;
            }

            _image.sprite = bulletIconArray[_bulletIconIndex];
        }
    }
}
