using UnityEngine;

public class FireEnemy : MonoBehaviour
{
    [Header("Transforms for detection")]
    public Transform player;
    public Transform rayPoint;

    [Header("Enemy Variables")]
    public float moveSpeed;
    public float waitTimeForChangingDirection = 0.2f;
    public bool isFrozen = false;
    public float FrozeTime;

    [Header("Froze Sprite")] 
    public Sprite LavaSlimeSprite;
    public Sprite frozeLavaSlimeSprite;

    //Define components
    private Rigidbody2D _rb2D;
    private SpriteRenderer _sr;

    //Define private variables
    private bool _isMoving = true;
    private int _moveDirectionIndex = 1;
    private float _frozeTimer;
    
    private void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (isFrozen)
        {
            _frozeTimer += Time.fixedDeltaTime;
            if (_frozeTimer >= FrozeTime)
            {
                _sr.sprite = LavaSlimeSprite;
                isFrozen = false;
                _frozeTimer = 0;
            }
        }

        //Change direction when no ground ahead OR something in front of the enemy
        if (!isFrozen && _isMoving)
        {
            GroundCheck();
            MovingDirectionCheck();
            _rb2D.velocity = new Vector2(-moveSpeed, _rb2D.velocity.y);         //Using -moveSpeed because enemy facing left at the beginning
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void GroundCheck()
    {
        if (_isMoving)
        {
            RaycastHit2D groundHit = Physics2D.Raycast(rayPoint.position, Vector2.down, 1f);
            if (!groundHit)
            {
                Debug.DrawRay(rayPoint.position, Vector3.down * 1f, Color.red);
                _isMoving = false;
                Invoke("ChangeDirection", waitTimeForChangingDirection);
            }
            else
            {
                if (groundHit.transform.CompareTag("Platform"))
                {
                    Debug.DrawRay(rayPoint.position, Vector3.down * 1f, Color.red);
                    _isMoving = false;
                    Invoke("ChangeDirection", waitTimeForChangingDirection);
                }
                
                Debug.DrawRay(rayPoint.position, Vector3.down * 1f, Color.green);
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void MovingDirectionCheck()
    {
        if (_isMoving)
        {
            RaycastHit2D MovingDirectionHit = Physics2D.Raycast(rayPoint.position, Vector2.left * _moveDirectionIndex, 1f);
            if (MovingDirectionHit)
            {
                Debug.DrawRay(rayPoint.position, Vector2.left * _moveDirectionIndex, Color.red);
                _isMoving = false;
                Invoke("ChangeDirection", waitTimeForChangingDirection);
            }
            else
            {
                Debug.DrawRay(rayPoint.position, Vector2.left * _moveDirectionIndex, Color.green);
            }
        }
    }

    public void ChangeDirection()
    {
        transform.Rotate(Vector3.up, 180);
        moveSpeed *= -1;
        _moveDirectionIndex *= -1;
        _isMoving = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Platform") && col.transform.name[0] == 'W')
        {
            //Froze LavaSlime and change sprite
            isFrozen = true;
            _sr.sprite = frozeLavaSlimeSprite;
        }
    }
}
