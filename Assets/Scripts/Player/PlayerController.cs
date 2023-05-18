using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    //Define Variables and Settings
    [Header("Player Variables")]
    public float moveSpeed;
    public float jumpForce;
    public float playerWidth;
    
    [Header("Player Status")]
    public bool isGround = false;
    public bool facingRight = true;
    
    [Header("Bullet Variables")]
    public float bulletForce;
    public float fireCollDownTime = 1;
    private float _fireCounter = 0;

    private Vector2 _screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

    //Define Components
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;

    //Get Fire Point
    public Transform firePoint;

    // Start is called before the first frame update
    void Start()
    {
        //Get player's own components
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //Player fire
        Fire();
    }

    private void FixedUpdate()
    {
        //Add time to counter
        _fireCounter += Time.deltaTime;
        
        //Player control
        Move();
        Jump();
    }

    private void Move()
    {
        //Move right or left based on keyboard input
        if (Input.GetAxis("Horizontal") != 0)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            
            //Rotate the player if moving toward different direction
            if (horizontalInput < 0 && facingRight)
            {
                //Turn left
                _sr.flipX = !_sr.flipX;
                facingRight = false;
                
                //Update firePoint's position
                firePoint.Translate(Vector3.left * playerWidth);
            }
            else if (horizontalInput > 0 && !facingRight)
            {
                //Turn right
                _sr.flipX = !_sr.flipX;
                facingRight = true;
                
                //Update firePoint's position
                firePoint.Translate(-Vector3.left * playerWidth);
            }

            float tempVelocityY = _rb.velocity.y;
            _rb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, tempVelocityY);
        }
    }

    private void Jump()
    {
        //Jump if press space
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            _rb.AddForce(Vector2.up * jumpForce);
        }
    }

    private void Fire()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x - _screenCenter.x, Input.mousePosition.y - _screenCenter.y);
        Ray fireRay = new Ray(firePoint.position, mousePos);
        Debug.DrawRay(fireRay.origin, fireRay.direction * 20, Color.red);
        
        if (Input.GetMouseButton(0) && _fireCounter > fireCollDownTime)
        {
            //Reset the counter
            _fireCounter = 0;
            
            //Get bullet from the ObjectPool
            GameObject obj = ObjectPool.Instance.GetObject("Bullet", firePoint.position, Quaternion.identity);
            obj.GetComponent<Rigidbody2D>().AddForce(fireRay.direction * bulletForce);
        }
    }
}
