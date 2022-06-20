using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    [SerializeField] private float walkingSpeed;
    private Vector3 moveDir;
    private Rigidbody2D playerRigidBody;
    private Animator animator;
    private SpriteRenderer spr;
    private string[] animationTriggerNames = {"up", "down", "horizontal", "idle"};
    private PlayerHealth playerHealth;
    private BoxCollider2D playerCollider;
    private Vector2 initialColliderSize;

    public GameObject guns;
    private Inventory inventory;

    public float dashDistance;
    bool isDashing;
    Vector3 initialScale;

    public bool canMove = true;

    public SoundPlayer m_dashSound;

    public bool isGodModeActive = false;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        playerHealth = GetComponent<PlayerHealth>();
        initialScale = transform.localScale;
    }
    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<BoxCollider2D>();
        initialColliderSize = playerCollider.size;
    }

    // Update is called once per frame
    void Update()
    {
        // prevent the player from moving if died 
        if (!canMove)
        {
            return;
        }
        
        if (!isDashing)
        {
            RotateGun();
            Shoot(); 
        }


        //Dash
        if (!isDashing && Input.GetKeyDown(KeyCode.Space))
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector2 direction = new Vector3(horizontal, vertical).normalized;
            if (direction != Vector2.zero)
                StartCoroutine(DashRoutine(direction));
        }

        // god Mode
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            if(!isGodModeActive)
            {
                // activate god mode
                isGodModeActive = true;
            }
            else
            { 
                // deactivate god mode
                isGodModeActive = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }
        
        if (!isDashing)
        {
            MovePlayer();
            AnimationControl();
        }
    }
    private void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, vertical, 0);
        moveDir = direction.normalized;

        // Update player position
        playerRigidBody.MovePosition(transform.position +
            moveDir * walkingSpeed * Time.deltaTime);
    }

    private void AnimationControl()
    {
        float angle = -1f;
        bool isShooting = (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow));
        // angle only available when player is actually moving
        if (moveDir != Vector3.zero)
            angle = Vector3.SignedAngle(moveDir, Vector3.up, Vector3.forward);


        // UP
        if (Input.GetKey(KeyCode.UpArrow))
        {

            animator.SetTrigger("up");
            ResetOtherAnimationTrigger("up");
            if (angle == -1)
            {
                animator.SetTrigger("idle");
            }
            return;
        }

        // DOWN
        if (Input.GetKey(KeyCode.DownArrow))
        {

            animator.SetTrigger("down");
            ResetOtherAnimationTrigger("down");
            if (angle == -1)
            {
                animator.SetTrigger("idle");
            }
            return;
        }

        // LEFT
        if (Input.GetKey(KeyCode.LeftArrow))
        {

            animator.SetTrigger("horizontal");
            ResetOtherAnimationTrigger("horizontal");
            spr.flipX = true;
            if (angle == -1)
            {
                animator.SetTrigger("idle");
            }
            return;
        }

        // RIGHT
        if (Input.GetKey(KeyCode.RightArrow))
        {

            animator.SetTrigger("horizontal");
            ResetOtherAnimationTrigger("horizontal");
            spr.flipX = false;
            if (angle == -1)
            {
                animator.SetTrigger("idle");
            }
            return;
        }

        if (!isShooting)
        {
            // IDLE
            if (angle == -1f)
            {
                animator.SetTrigger("idle");
                // reset other triggers
                ResetOtherAnimationTrigger("idle");
            }
            // UP
            else if (angle == 0 || angle == 45f || angle == -45f)
            {
                animator.SetTrigger("up");
                ResetOtherAnimationTrigger("up");
            }
            // DOWN
            else if (angle == 180f || angle == 135f || angle == -135f)
            {
                animator.SetTrigger("down");
                ResetOtherAnimationTrigger("down");
            }
            // LEFT
            else if (angle == -90f)
            {
                animator.SetTrigger("horizontal");
                ResetOtherAnimationTrigger("horizontal");
                spr.flipX = true;
            }
            // RIGHT
            else if (angle == 90f)
            {
                animator.SetTrigger("horizontal");
                ResetOtherAnimationTrigger("horizontal");
                spr.flipX = false;
            } 
        }
        
    }

    private void ResetOtherAnimationTrigger(string currTrigger)
    {
        foreach(string s in animationTriggerNames)
        {
            if(s == currTrigger)
            {
                continue;
            }
            animator.ResetTrigger(s);
        }
    }

    // Move/flip the weapon's sprite
    private void RotateGun()
    {
        float xOffset = 0f;
        float yOffset = 0f;
        bool isShooting = (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow));

        //      When shooting
        // UP
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            guns.transform.position = transform.position + new Vector3(xOffset, yOffset, 0f);
            guns.transform.rotation = Quaternion.Euler(0, 180, 90);
            return;
        }

        // DOWN
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            guns.transform.position = transform.position + new Vector3(xOffset, yOffset, 0f);
            guns.transform.rotation = Quaternion.Euler(0, 0, -90);
            return;
        }

        // LEFT
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            guns.transform.position = transform.position + new Vector3(-xOffset, yOffset, 0f);
            guns.transform.rotation = Quaternion.Euler(0, 180, 0);
            return;
        }

        // RIGHT
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            guns.transform.position = transform.position + new Vector3(xOffset, yOffset, 0f);
            guns.transform.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }

        //  When moving and not shooting
        if (!isShooting)
        {
            // UP
            if (Input.GetKeyDown(KeyCode.W))
            {
                guns.transform.position = transform.position + new Vector3(xOffset, yOffset, 0f);
                guns.transform.rotation = Quaternion.Euler(0, 180, 90);
                return;
            }

            // DOWN
            if (Input.GetKeyDown(KeyCode.S))
            {
                guns.transform.position = transform.position + new Vector3(xOffset, yOffset, 0f);
                guns.transform.rotation = Quaternion.Euler(0, 0, -90);
                return;
            }

            // LEFT
            if (Input.GetKeyDown(KeyCode.A))
            {
                guns.transform.position = transform.position + new Vector3(-xOffset, yOffset, 0f);
                guns.transform.rotation = Quaternion.Euler(0, 180, 0);
                return;
            }

            // RIGHT
            if (Input.GetKeyDown(KeyCode.D))
            {
                guns.transform.position = transform.position + new Vector3(xOffset, yOffset, 0f);
                guns.transform.rotation = Quaternion.Euler(0, 0, 0);
                return;
            }
        }
        

    }

    private void Shoot()
    {
        // UP
        if (Input.GetKey(KeyCode.UpArrow))
        {
            inventory.FireGun(Quaternion.Euler(0, 0, 0));
            return;
        }

        // DOWN
        if (Input.GetKey(KeyCode.DownArrow))
        {
            inventory.FireGun(Quaternion.Euler(0, 0, 180));
            return;
        }

        // Left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            inventory.FireGun(Quaternion.Euler(0, 0, 90));
            return;
        }

        // Right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            inventory.FireGun(Quaternion.Euler(0, 0, -90));
            return;
        }
    }

    IEnumerator DashRoutine(Vector2 direction)
    {
        isDashing = true;
        playerHealth.isInvincible = true;
        playerRigidBody.velocity = direction;
        playerRigidBody.AddForce(direction * dashDistance, ForceMode2D.Impulse);
        if (direction == Vector2.up || direction == Vector2.down)
        {
            transform.localScale = new Vector3(0.04f, 0.3f, 1f);
            playerCollider.size = new Vector2(initialColliderSize.x*5, initialColliderSize.y);
        }
        else
        { 
            transform.localScale = new Vector3(0.3f, 0.04f, 1f);
            playerCollider.size = new Vector2(initialColliderSize.x, initialColliderSize.y*5);

        }

        // Play a sound effect
        if (m_dashSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_dashSound, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.3f);

        isDashing = false;
        playerHealth.isInvincible = false;
        playerRigidBody.velocity = new Vector2(0, 0);
        transform.localScale = initialScale;
        playerCollider.size = initialColliderSize;
    }

    public void ResetAnimation()
    {
        ResetOtherAnimationTrigger("down");
        animator.Play("idle-player");
    }
}
