using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This controls the basic movement animation of enemy by reading the direction of the velocity from AIAgent
/// </summary>


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class BaseAnimationControlEnemy : MonoBehaviour
{
    private AI.AIAgent agent;
    private Animator animator;
    private SpriteRenderer spr;
    public bool invertX;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<AI.AIAgent>();
        animator = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // get the velocity of the AIAgent
        Vector3 velocity = agent.Velocity;
        if(Mathf.Approximately(velocity.magnitude, 0))
        {
            animator.SetTrigger("horizontal");
            return;
        }

        // check which quadrant the velocity is in (normal coord system rotated by 45 degrees)
        // can do the check by calculating its angle between Vector3.up
        float angle = Vector3.SignedAngle(velocity, Vector3.up, Vector3.forward);
        // angle: -45 ~ 45 = up
        if(angle >= -45 && angle <= 45)
        {
            animator.SetTrigger("up");
        }
        // angle: -135 ~ -45 (left) or 45 ~ 135 (right) = horizontal
        else if((angle >= -135 && angle < -45))
        {
            animator.SetTrigger("horizontal");
            spr.flipX = false || invertX;
        }
        else if(angle > 45 && angle <= 135)
        {
            animator.SetTrigger("horizontal");
            spr.flipX = true && !invertX;
        }
        // angle: -135 ~ 135 (<-135, >135)
        else if(angle < -135 || angle > 135)
        {
            animator.SetTrigger("down");
        }

    }
}
