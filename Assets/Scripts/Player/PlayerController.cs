using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    //Define transforms
    public Transform spawnPoint;
    public Transform firePoint;
    public Transform groundCheckRayPoint_Left;
    public Transform groundCheckRayPoint_Right;
    public Transform lightPoint;
    
    [Header("Player Sprite Array")]
    public Sprite[] playerSpriteArray;

    [Header("UI Image")] 
    public GameObject bulletIcon;
    public GameObject abilityChangeIcon;
    
    //Define Variables and Settings
    [Header("Player Variables")]
    public float moveSpeed;
    [Range(0, 10)]
    public float moveSpeedChangeFactor;
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
    public int currentSpriteIndex = 0;
    public string currentBulletName;
    public int currentBulletIndex = 0;

    [Header("Player Ability")] 
    public bool unlockBranch = false;
    public bool unlockWater = false;
    public bool unlockFire = false;

    [Header("Bullet/Platform Variables")] 
    public float fireRayDetectionMaxDistance;
    [Range(0, 1)]
    public float firePointMoveRange;
    public float fireCoolDownTime = 1;
    public float bulletForce;
    public float platformForce;
    public string[] bulletNameArray;

    private Vector2 _screenCenter = new (Screen.width / 2, Screen.height / 2);

    //Define Components
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    
    //Private stuff used for control
    private LayerMask _GroundAndFireEnmeyLayer;
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
        _sr = GetComponent<SpriteRenderer>();
        
        _GroundAndFireEnmeyLayer = (1 << 3) | (1 << 9);

        //Set current bullet
        currentBulletName = bulletNameArray[currentBulletIndex];
        
        //Set UI icons as inactive at beginning before the player unlock any ability
        bulletIcon.SetActive(false);
        abilityChangeIcon.SetActive(false);
        
        //Reset player's ability
        unlockBranch = false;
        unlockWater = false;
        unlockFire = false;
    }

    private void Update()
    {
        //Check player's movement input
        CheckMovementInput();

        MouseInputUpdate();

        //Update player's light direction
        ApplyLightDirection();
        
        //Check if the player swaps the bullet
        CheckBulletSwap();
        
        //Player fire
        Fire();
    }

    private void FixedUpdate()
    {
        //Check ground and update isGround
        GroundCheck();

        //Player control
        ApplyMovementInput();
    }

    private void GroundCheck()
    {
        RaycastHit2D groundHitLeft = Physics2D.Raycast(groundCheckRayPoint_Left.position, Vector2.down, 0.5f, _GroundAndFireEnmeyLayer);
        RaycastHit2D groundHitRight = Physics2D.Raycast(groundCheckRayPoint_Right.position, Vector2.down, 0.5f, _GroundAndFireEnmeyLayer);

        //Check left side
        if (!groundHitLeft && !groundHitRight)
        {
            Debug.DrawRay(groundCheckRayPoint_Left.position, Vector3.down * 0.5f, Color.red);
            Debug.DrawRay(groundCheckRayPoint_Right.position, Vector3.down * 0.5f, Color.red);
            
            if (isGround)
            {
                isGround = false;
            }
        }
        else
        {
            if (!groundHitLeft)
            {
                Debug.DrawRay(groundCheckRayPoint_Left.position, Vector3.down * 0.5f, Color.red);
                Debug.DrawRay(groundCheckRayPoint_Right.position, Vector3.down * 0.5f, Color.green);
            }
            else if (!groundHitRight)
            {
                Debug.DrawRay(groundCheckRayPoint_Left.position, Vector3.down * 0.5f, Color.green);
                Debug.DrawRay(groundCheckRayPoint_Right.position, Vector3.down * 0.5f, Color.red);
            }
            else
            {
                Debug.DrawRay(groundCheckRayPoint_Left.position, Vector3.down * 0.5f, Color.green);
                Debug.DrawRay(groundCheckRayPoint_Right.position, Vector3.down * 0.5f, Color.green);
            }
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
        var tempLightPointLocalPosition = lightPoint.localPosition;
        
        if (_movementInputDirection < 0 && facingRight)
        {
            //Turn left
            lightPoint.localPosition = new Vector3(-tempLightPointLocalPosition.x, tempLightPointLocalPosition.y, 0);
            _sr.flipX = !_sr.flipX;
            facingRight = false;
            facingIndex = -1;
        }
        else if (_movementInputDirection > 0 && !facingRight)
        {
            //Turn right
            lightPoint.localPosition = new Vector3(-tempLightPointLocalPosition.x, tempLightPointLocalPosition.y, 0);
            _sr.flipX = !_sr.flipX;
            facingRight = true;
            facingIndex = 1;
        }
        
        //Apply player's horizontal movement
        if (isGround)
        {
            var tempVelocity = _rb.velocity;
            //_rb.velocity = Vector2.SmoothDamp(tempVelocity, new Vector2(_movementInputDirection * moveSpeed, tempVelocity.y), ref tempVelocity, moveSpeedChangeTime);
            _rb.velocity = Vector2.Lerp(tempVelocity, new Vector2(_movementInputDirection * moveSpeed, tempVelocity.y), moveSpeedChangeFactor * Time.fixedDeltaTime);
            //_rb.velocity = new Vector2(_movementInputDirection * moveSpeed, _rb.velocity.y);
        }
        else if (_movementInputDirection != 0)
        {
            _airForce = new Vector2(movementForceInAir * _movementInputDirection, 0);
            _rb.AddForce(_airForce);

            //Clamp _rb.velocity if the velocity exceeds moveSpeed after adding force
            _rb.velocity = new Vector2(Mathf.Clamp(_rb.velocity.x, -moveSpeed, moveSpeed), _rb.velocity.y);
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
        //Update aim ray and move firePoint based on mouse position
        _mousePos = new Vector2(Input.mousePosition.x - _screenCenter.x, Input.mousePosition.y - _screenCenter.y).normalized * firePointMoveRange;  //Multiply normalized vector to control firePoint's range
        Vector3 playerPosition = transform.position;
        firePoint.position = new Vector3(playerPosition.x + _mousePos.x, playerPosition.y + _mousePos.y, 0);
        _fireRay = new Ray(firePoint.position, _mousePos);
        Debug.DrawRay(_fireRay.origin, _fireRay.direction * fireRayDetectionMaxDistance, Color.red);
        
        //If the player aims at platform and right click, take back the platform
        RaycastHit2D fireRayHit = Physics2D.Raycast(_fireRay.origin, _fireRay.direction, fireRayDetectionMaxDistance, _GroundAndFireEnmeyLayer);
        if(Input.GetMouseButtonDown(1) && fireRayHit && fireRayHit.transform.CompareTag("Platform"))
        {
            //Reset platform's layer to default "Ignore Player"
            GameObject tempObj;
            (tempObj = fireRayHit.transform.gameObject).layer = LayerMask.NameToLayer("Ignore Player");
            ObjectPool.Instance.SetObject(fireRayHit.transform.name, tempObj);
        }
    }

    private void CheckBulletSwap()
    {
        //Return if the player doesn't unlock any ability
        
        //When player doesn't have ability or only one
        if (!unlockBranch || !unlockWater)
        {
            return;
        }
        
        if (!unlockFire)
        {
            //When player has two abilities
            if (Input.GetKeyDown(KeyCode.F))
            {
                currentBulletIndex++;
                if (currentBulletIndex > 1)
                {
                    currentBulletIndex -= 2;
                }

                currentSpriteIndex = currentBulletIndex + 1;
                if (currentSpriteIndex > 2)
                {
                    currentSpriteIndex -= 2;
                }
                
                if (currentSpriteIndex == 0)
                {
                    currentSpriteIndex = 1;
                }
            }
        }
        else
        {
            //When player has all three abilities
            if (Input.GetKeyDown(KeyCode.F))
            {
                currentBulletIndex++;
                if (currentBulletIndex >= bulletNameArray.Length)
                {
                    currentBulletIndex -= bulletNameArray.Length;
                }
                
                currentSpriteIndex = currentBulletIndex + 1;
                if (currentSpriteIndex >= playerSpriteArray.Length)
                {
                    currentSpriteIndex -= playerSpriteArray.Length;
                }

                if (currentSpriteIndex == 0)
                {
                    currentSpriteIndex = 1;
                }
            }
        }
        currentBulletName = bulletNameArray[currentBulletIndex];
        _sr.sprite = playerSpriteArray[currentSpriteIndex];
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Fire()
    {
        //Add time to counter
        _fireCounter += Time.fixedDeltaTime;

        if (Input.GetMouseButton(0) && _fireCounter > fireCoolDownTime && unlockBranch)
        {
            //Reset the counter
            _fireCounter = 0;
            
            //Get bullet from the ObjectPool
            GameObject obj = ObjectPool.Instance.GetObject(currentBulletName, firePoint.position, Quaternion.identity);
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

    private void BackToSpawnPoint()
    {
        gameObject.SetActive(false);
        Invoke("ActivatePlayer", 1.5f);
    }

    private void ActivatePlayer()
    {
        transform.position = spawnPoint.position;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.CompareTag("CheckPoint"))
        {
            if (!unlockBranch && col.transform.GetComponent<CheckPointScript>().canUnlockBranch)
            {
                unlockBranch = true;
                Debug.Log("UNLOCK BRANCH");
                
                bulletIcon.SetActive(true);
                abilityChangeIcon.SetActive(true);
                
                currentSpriteIndex = 1;
                _sr.sprite = playerSpriteArray[currentSpriteIndex];   
            }
            else if (!unlockWater && col.transform.GetComponent<CheckPointScript>().canUnlockWater)
            {
                unlockWater = true;
                Debug.Log("UNLOCK WATER");
            }
            else if (!unlockFire && col.transform.GetComponent<CheckPointScript>().canUnlockFire)
            {
                unlockFire = true;
                Debug.Log("UNLOCK FIRE");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Fruit"))
        {
            Destroy(col.gameObject);
            fruitCount++;
        }
        
        //If player collides with LavaSlime and LavaSlime is frozen, LavaSlime doesn't kill the player
        if (col.transform.CompareTag("FireEnemy") && col.transform.GetComponent<FireEnemy>().isFrozen)
        {
            return;
        }

        if (col.transform.CompareTag("DeadZone") || col.transform.CompareTag("Enemy") || col.transform.CompareTag("FireEnemy") || col.transform.CompareTag("Meteorite"))
        {
            BackToSpawnPoint();
        }
    }
}
