using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    //Define transforms
    public Transform spawnPoint;
    
    //Define Variables and Settings
    [Header("Player Variables")]
    public float moveSpeed;
    public float jumpForce;
    public int fruitCount;
    public float movementForceInAir;
    [Range(0, 1)]
    public float airDragMultiplier = 0.85f;

    [Header("Player Status")]
    public bool isGround = false;
    public bool facingRight = true;
    
    [Header("Bullet/Platform Variables")]
    public float bulletForce;
    public float fireCollDownTime = 1;
    public float platformForce;

    private Vector2 _screenCenter = new (Screen.width / 2, Screen.height / 2);

    //Define Components
    private Rigidbody2D _rb;

    //Get Fire Point
    public Transform firePoint;
    
    //Private stuff used for control
    private bool _jumpPressed;
    private float _movementInputDirection;
    private Vector2 _airForce;
    
    private Vector2 _mousePos;
    private Ray _fireRay;
    private float _fireCounter = 0;
    
    void Start()
    {
        //Get player's own components
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //Check player's movement input
        CheckMovementInput();
        
        //Update aim ray
        _mousePos = new Vector2(Input.mousePosition.x - _screenCenter.x, Input.mousePosition.y - _screenCenter.y);
        _fireRay = new Ray(firePoint.position, _mousePos);
        Debug.DrawRay(_fireRay.origin, _fireRay.direction * 20, Color.red);
        
        //Player fire
        Fire();
        
        //Player builds platform
        BuildPlatform();
    }

    private void FixedUpdate()
    {
        //Add time to counter
        _fireCounter += Time.fixedDeltaTime;
        
        //Player control
        ApplyMovementInput();
        Jump();
    }

    private void CheckMovementInput()
    {
        //Check movement input
        if (Input.GetAxis("Horizontal") != 0)
        {
            _movementInputDirection = Input.GetAxisRaw("Horizontal");
        }

        //Check jump input
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }
    }

    private void ApplyMovementInput()
    {
        //Rotate the player if moving toward different direction
        if (_movementInputDirection < 0 && facingRight)
        {
            //Turn left
            transform.Rotate(Vector3.up, 180);
            facingRight = false;
        }
        else if (_movementInputDirection > 0 && !facingRight)
        {
            //Turn right
            transform.Rotate(Vector3.up, 180);
            facingRight = true;
        }
        
        //Apply player's horizontal movement
        if (isGround)
        {
            _rb.velocity = new Vector2(_movementInputDirection * moveSpeed, _rb.velocity.y);
        }
        else if (_movementInputDirection != 0)
        {
            _airForce = new Vector2(movementForceInAir * _movementInputDirection, 0);
            _rb.AddForce(_airForce);

            if (Mathf.Abs(_rb.velocity.x) > moveSpeed)
            {
                _rb.velocity = new Vector2(_movementInputDirection * moveSpeed, _rb.velocity.y);
            }
        }
        else if (_movementInputDirection == 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x * airDragMultiplier, _rb.velocity.y);
        }
    }

    private void Jump()
    {
        //Jump if press space
        if (_jumpPressed && isGround)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
        }

        //Reset _jumpPressed whether the player actually jumps
        _jumpPressed = false;
    }

    private void Fire()
    {
        if (Input.GetMouseButton(0) && _fireCounter > fireCollDownTime)
        {
            //Reset the counter
            _fireCounter = 0;
            
            //Get bullet from the ObjectPool
            GameObject obj = ObjectPool.Instance.GetObject("Bullet", firePoint.position, Quaternion.identity);
            obj.GetComponent<Rigidbody2D>().AddForce(_fireRay.direction * bulletForce);
        }
    }

    private void BuildPlatform()
    {
        if (Input.GetKeyDown(KeyCode.F) && fruitCount > 0)
        {
            fruitCount--;
            GameObject obj = ObjectPool.Instance.GetObject("F_Orange", firePoint.position, quaternion.identity);
            obj.GetComponent<Rigidbody2D>().AddForce(_fireRay.direction * platformForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Fruit"))
        {
            Destroy(col.gameObject);
            fruitCount++;
        }

        if (col.transform.CompareTag("DeadZone") || col.transform.CompareTag("Enemy"))
        {
            transform.position = spawnPoint.position;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Input.GetKeyDown(KeyCode.R) && (collision.transform.name[0].ToString() == "F"))
        {
            ObjectPool.Instance.SetObject("F_Orange", collision.gameObject);
            fruitCount++;
        }
    }
}
