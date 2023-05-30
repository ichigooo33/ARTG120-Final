using UnityEngine;

public class MeteoriteScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.transform.CompareTag("Player"))
        {
            ObjectPool.Instance.SetObject("Meteorite", gameObject);
        }
    }
}
