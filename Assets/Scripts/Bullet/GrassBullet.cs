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
        //Reset platform's layer to default "Ignore Player"
        _lifeTimeCounter = 0;
        gameObject.layer = LayerMask.NameToLayer("Ignore Player");
        ObjectPool.Instance.SetObject("GrassBullet", gameObject);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Enemy"))
        {
            Destroy(col.gameObject);
        }
        else if (col.transform.CompareTag("Meteorite") || col.transform.CompareTag("Platform"))
        {
            return;
        }
        else
        {
            //Once collide with Map, platform changes its layer from "Ignore Player" to "Ground"
            gameObject.layer = LayerMask.NameToLayer("Ground");
            _rb.bodyType = RigidbodyType2D.Static;
        }
    }
}
