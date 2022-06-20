using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDummy : MonoBehaviour
{
    public int health;
    public float speed;
    public GameObject target;
    public bool isHit;

    private Rigidbody2D m_rigidbody;
    private float knockbackDuration = 1f;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHit)
        {
            Vector2 direction2D = (target.transform.position - transform.position).normalized;
            Vector3 direction = new Vector3(direction2D.x, direction2D.y, 0);
            m_rigidbody.MovePosition(transform.position + direction * speed * Time.deltaTime);
        }
        else
        {
            if (timer < knockbackDuration)
            {
                timer += Time.deltaTime;
            }
            else
            {
                isHit = false;
                timer = 0f;
            }
        }

    }

    public void CheckHealthStatus()
    {
        Debug.Log("health remaining: " + health);
        if (health <= 0) Destroy(gameObject);
    }
}
