using System;
using UnityEngine;

public class FireEnemyAttacker : MonoBehaviour
{
    [Header("Transforms for detection")]
    public Transform player;
    public Transform rayPoint;
    public Transform firePoint;

    [Header("Enemy Variables")]
    public float moveSpeed;
    public float waitTimeForChangingDirection = 0.2f;
    public float FrozeTime;
    public bool isFrozen = false;
    public bool isAttacking;

    [Header("Froze Sprite")] 
    public Sprite NormalSprite;
    public Sprite frozeSprite;

    [Header("Projectile")] 
    public string projectileName;

    //Define components
    private Rigidbody2D _rb2D;
    private SpriteRenderer _sr;

    //Define private variables
    private bool _isMoving = true;
    private int _moveDirectionIndex = 1;
    private float _frozeTimer;
    private LayerMask _GroundAndFireEnmeyLayer;
    
    private void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _GroundAndFireEnmeyLayer = (1 << 3) | (1 << 9);
    }

    private void FixedUpdate()
    {
        if (isFrozen)
        {
            _frozeTimer += Time.fixedDeltaTime;
            if (_frozeTimer >= FrozeTime)
            {
                _sr.sprite = NormalSprite;
                isFrozen = false;
                _frozeTimer = 0;
            }
        }

        //Change direction when no ground ahead OR something in front of the enemy
        if (!isFrozen && _isMoving && !isAttacking)
        {
            GroundCheck();
            MovingDirectionCheck();
            _rb2D.velocity = new Vector2(-moveSpeed, _rb2D.velocity.y);         //Using -moveSpeed because enemy facing left at the beginning
        }

        if (isAttacking)
        {
            AimFire();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void GroundCheck()
    {
        if (_isMoving)
        {
            RaycastHit2D groundHit = Physics2D.Raycast(rayPoint.position, Vector2.down, 1f, _GroundAndFireEnmeyLayer);
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
            RaycastHit2D MovingDirectionHit = Physics2D.Raycast(rayPoint.position, Vector2.left * _moveDirectionIndex, 1f,_GroundAndFireEnmeyLayer);
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

    private void AimFire()
    {
        //Aim player & change direction
        if (player.position.x < transform.position.x && _moveDirectionIndex == -1)
        {
            //If player is on the left side of enemy and enemy facing right
            ChangeDirection();
        }
        else if (player.position.x > transform.position.x && _moveDirectionIndex == 1)
        {
            //If player is on the right side of enemy and enemy facing left
            ChangeDirection();
        }
        
        //Get bullet from the ObjectPool
        //GameObject obj = ObjectPool.Instance.GetObject(projectileName, firePoint.position, Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Platform") && col.transform.name[0] == 'W')
        {
            //Froze LavaSlime and change sprite
            isFrozen = true;
            _sr.sprite = frozeSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.transform.CompareTag("Player"))
        {
            isAttacking = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            isAttacking = false;
        }
    }
}
