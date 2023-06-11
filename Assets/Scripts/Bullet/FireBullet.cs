using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public float lifeSpan = 5f;
    public float _lifeTimeCounter;

    private void Update()
    {
        _lifeTimeCounter += Time.deltaTime;
        
        if (_lifeTimeCounter > lifeSpan)
        {
            BackToObjectPool();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void BackToObjectPool()
    {
        //Reset platform's layer to default "Ignore Player"
        _lifeTimeCounter = 0;
        ObjectPool.Instance.SetObject("FireBullet", gameObject);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Enemy"))
        {
            Destroy(col.gameObject);
        }
        else if (col.transform.CompareTag("Wood"))
        {
            Destroy(col.gameObject);
        }
        BackToObjectPool();
    }
}
