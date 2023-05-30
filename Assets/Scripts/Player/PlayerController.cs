using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    //Define transforms
    public Transform spawnPoint;
    public Transform firePoint;
    public Transform groundCheckRayPoint;
    public Transform lightPoint;
    
    //Define Variables and Settings
    [Header("Player Variables")]
    public float moveSpeed;
    public float jumpForce;
    public int fruitCount;
    public float movementForceInAir;
    [Range(0, 1)]
    public float airDragMultiplier = 0.85f;
    [Range(0, 1)]
    public float smallJumpMultiplier = 0.5f;

    [Header("Player Status")]
    public bool isGround = false;
    public bool facingRight = true;
    public int facingIndex = 1;
    public string currentBullet;

    [Header("Bullet/Platform Variables")] 
    public float fireRayDetectionMaxDistance;
    public float bulletForce;
    public float fireCoolDownTime = 1;
    public float platformForce;

    private Vector2 _screenCenter = new (Screen.width / 2, Screen.height / 2);

    //Define Components
    private Rigidbody2D _rb;
    
    //Private stuff used for control
    private LayerMask _onlyGroundLayer;
    private LayerMask _ignorePlayerLayer;
    
    private float _movementInputDirection;
    private Vector2 _airForce;
    
    private Vector2 _mousePos;
    private Ray _fireRay;
    private float _fireCounter = 0;
    
    private void Start()
    {
        //Get player's own components
        _rb = GetComponent<Rigidbody2D>();
        
        _onlyGroundLayer = 1 << 3;

        currentBullet = "GrassBullet";
    }

    private void Update()
    {
        //Check player's movement input
        CheckMovementInput();

        MouseInputUpdate();

        //Update player's light direction
        ApplyLightDirection();
        
        //Player fire
        Fire();
    }

    private void FixedUpdate()
    {
        //Add time to counter
        _fireCounter += Time.fixedDeltaTime;
        
        //Check ground and update isGround
        GroundCheck();

        //Player control
        ApplyMovementInput();
    }

    private void GroundCheck()
    {
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckRayPoint.position, Vector2.down, 0.5f, _onlyGroundLayer);
        if (!groundHit)
        {
            Debug.DrawRay(groundCheckRayPoint.position, Vector3.down * 0.5f, Color.red);
            if (isGround)
            {
                isGround = false;
            }
        }
        else
        {
            Debug.DrawRay(groundCheckRayPoint.position, Vector3.down * 0.5f, Color.green);
            if (!isGround)
            {
                isGround = true;
            }
        }
    }

    private void CheckMovementInput()
    {
        //Check movement input
        if (Input.GetAxis("Horizontal") != 0)
        {
            _movementInputDirection = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            _movementInputDirection = 0;
        }

        //Check jump input
        Jump();
    }

    private void ApplyMovementInput()
    {
        //Rotate the player if moving toward different direction
        if (_movementInputDirection < 0 && facingRight)
        {
            //Turn left
            transform.Rotate(Vector3.up, 180);
            facingRight = false;
            facingIndex = -1;
        }
        else if (_movementInputDirection > 0 && !facingRight)
        {
            //Turn right
            transform.Rotate(Vector3.up, 180);
            facingRight = true;
            facingIndex = 1;
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

            //Lerp _rb.velocity if the velocity exceeds moveSpeed after adding force 
            if (Mathf.Abs(_rb.velocity.x) > moveSpeed)
            {
                _rb.velocity = new Vector2(_movementInputDirection * moveSpeed, _rb.velocity.y);
            }
        }
        else if (_movementInputDirection == 0)
        {
            var velocity = _rb.velocity;
            velocity = new Vector2(velocity.x * airDragMultiplier, velocity.y);
            _rb.velocity = velocity;
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGround)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void MouseInputUpdate()
    {
        //Update aim ray
        _mousePos = new Vector2(Input.mousePosition.x - _screenCenter.x, Input.mousePosition.y - _screenCenter.y).normalized;
        _fireRay = new Ray(firePoint.position, _mousePos);
        Debug.DrawRay(_fireRay.origin, _fireRay.direction * fireRayDetectionMaxDistance, Color.red);

        RaycastHit2D fireRayHit = Physics2D.Raycast(_fireRay.origin, _fireRay.direction, fireRayDetectionMaxDistance, _onlyGroundLayer);

        if(Input.GetMouseButtonDown(1) && fireRayHit && fireRayHit.transform.CompareTag("Platform"))
        {
            ObjectPool.Instance.SetObject(fireRayHit.transform.name, fireRayHit.transform.gameObject);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Fire()
    {
        if (Input.GetMouseButton(0) && _fireCounter > fireCoolDownTime)
        {
            //Reset the counter
            _fireCounter = 0;
            
            //Get bullet from the ObjectPool
            GameObject obj = ObjectPool.Instance.GetObject(currentBullet, firePoint.position, Quaternion.identity);
            obj.GetComponent<Rigidbody2D>().AddForce(_fireRay.direction * bulletForce);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void BuildPlatform()
    {
        /*if (Input.GetKeyDown(KeyCode.F) && fruitCount > 0)
        {
            fruitCount--;
            GameObject obj = ObjectPool.Instance.GetObject("F_Orange", firePoint.position, quaternion.identity);
            obj.GetComponent<Rigidbody2D>().AddForce(_fireRay.direction * platformForce);
        }*/
    }

    private void ApplyLightDirection()
    {
        lightPoint.rotation = Quaternion.Euler(0, 0, InBetweenAngleOfVectors(Vector3.right, _fireRay.direction));
    }

    private float InBetweenAngleOfVectors(Vector3 fromVector, Vector3 toVector)
    {
        Vector3 tempVector3 = Vector3.Cross(fromVector, toVector);
        if (tempVector3.z > 0)
        {
            return Vector3.Angle(fromVector, toVector);
        }
        else
        {
            return 360 - Vector3.Angle(fromVector, toVector);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Fruit"))
        {
            Destroy(col.gameObject);
            fruitCount++;
        }

        if (col.transform.CompareTag("DeadZone") || col.transform.CompareTag("Enemy") || col.transform.CompareTag("Meteorite"))
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
