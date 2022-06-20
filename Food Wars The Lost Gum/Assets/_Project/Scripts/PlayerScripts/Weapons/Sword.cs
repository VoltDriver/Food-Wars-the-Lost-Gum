using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public GameObject player;
    public int damage;
    public float knockback;
    public Sprite[] sprites;
    private SpriteRenderer renderer;
    private BoxCollider2D collider;
    float attackCD = 1.5f;
    float attackTimer;
    float spriteChangeTime = 0.05f; 

    public SoundPlayer m_swordSound;


    // Start is called before the first frame update
    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        EnableComponents(false);
        attackTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        RotateSword();

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.F) && attackTimer <= 0f)
            StartCoroutine(AttackRoutine());
    }

    void RotateSword()
    {
        // UP
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.position = player.transform.position + new Vector3(0f, 0.5f, 0f);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }

        // DOWN
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position = player.transform.position + new Vector3(0f, -0.5f, 0f);
            transform.rotation = Quaternion.Euler(180, 0, 0);
            return;
        }

        // LEFT
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position = player.transform.position + new Vector3(-0.7f, 0.2f, 0f);
            transform.rotation = Quaternion.Euler(0, 180, 0);
            return;
        }

        // RIGHT
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position = player.transform.position + new Vector3(0.7f, 0.2f, 0f);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            return;
        }
    }

    void EnableComponents(bool b)
    {
        //anim.enabled = b;
        this.renderer.enabled = b;
        this.collider.enabled = b;
    }

    IEnumerator AttackRoutine()
    {
        attackTimer = attackCD;
        EnableComponents(true);

        // Play a sound effect
        if (m_swordSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_swordSound, transform.position, Quaternion.identity);
        }

        for (int i = 0; i < sprites.Length; i++)
        {
            renderer.sprite = sprites[i];
            yield return new WaitForSeconds(spriteChangeTime);
        }

        EnableComponents(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 direction = (other.transform.position - transform.position).normalized;

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<DamageHandler>().TakeDamage(damage, direction * knockback);
            Debug.Log("Sword Damage");
        }
        else if (other.CompareTag("Boss"))
        {
                other.GetComponent<OnionBossPhase1>().HurtTheBoss(damage);
                Debug.Log("Sword Damage");
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Vector2 direction = (collision.transform.position - transform.position).normalized;

    //    if (collision.gameObject.CompareTag("Enemy"))
    //    {
    //        collision.gameObject.GetComponent<DamageHandler>().TakeDamage(damage, direction * knockback);
    //        Debug.Log("Sword Damage");
    //    }
    //}
}
