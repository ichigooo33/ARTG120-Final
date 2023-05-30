using UnityEngine;

public class GrassBullet : MonoBehaviour
{
    public float lifeSpan = 5f;
    public float fallTime = 2f;
    public float _lifeTimeCounter;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        _lifeTimeCounter += Time.deltaTime;
        
        if (_lifeTimeCounter > lifeSpan)
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
            
            if (_lifeTimeCounter > lifeSpan + fallTime)
            {
                BackToObjectPool();
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void BackToObjectPool()
    {
        ObjectPool.Instance.SetObject("GrassBullet", gameObject);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Enemy"))
        {
            Destroy(col.gameObject);
        }
        
        if (col.transform.CompareTag("Map"))
        {
            _rb.bodyType = RigidbodyType2D.Static;
        }
    }
}
