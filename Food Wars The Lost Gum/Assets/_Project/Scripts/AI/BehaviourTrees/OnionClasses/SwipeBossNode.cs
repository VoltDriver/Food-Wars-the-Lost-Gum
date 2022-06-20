using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeBossNode : ActionNode
{

    public bool leftSwipe;
    private float startTime;
    public float totalTime;
    private OnionBossPhase1 boss;
    private GameObject vine;
    public float vineSpawnZValue;
    public float vineSpawnYValue;

    public SoundPlayer m_swipeAttackSound;
    private SoundPlayer m_swipeAttackSoundInstance;

    public SoundPlayer[] m_angerSounds;

    private bool collided;
    private float speed;
    Vector3 velocity;

    // Start is called before the first frame update
    protected override void OnStart()
    {
        boss = Owner.GetComponent<OnionBossPhase1>();
        boss.AnimationWrapper(4);
        startTime = Time.time;
        vine = Instantiate(boss.swipeVine, leftSwipe? new Vector3(boss.leftWallX,vineSpawnYValue,vineSpawnZValue) :
            new Vector3(boss.rightWallX, vineSpawnYValue, vineSpawnZValue), Quaternion.identity);
        collided = false;
        speed = ((boss.fourthFifthX+boss.thirdFifthX)/2f - boss.leftWallX) / totalTime;
        if (!leftSwipe) speed = -speed;
        velocity = new Vector3(speed, 0, 0);
        vine.GetComponent<AttackingObjectCollider>().parentNode = this;

        // Play a sound effect
        if (m_swipeAttackSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_swipeAttackSound, boss.transform.position, Quaternion.identity);
            m_swipeAttackSoundInstance = sound;
            if(sound.m_loop)
            {
                sound.Play();
            }
        }

        // Play an anger sound effect
        if (m_angerSounds != null && m_angerSounds.Length > 0)
        {
            SoundPlayer sound = GameObject.Instantiate(m_angerSounds[Random.Range(0, m_angerSounds.Length)], boss.transform.position, Quaternion.identity);
        }
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        if (Time.time > startTime + totalTime)
        {
            if(m_swipeAttackSoundInstance != null && m_swipeAttackSoundInstance.m_loop)
            {
                m_swipeAttackSoundInstance.Stop();
            }
            Destroy(vine);
            if (!collided) return State.Success;
            else return State.Failure;
        }
        vine.transform.Translate(velocity * Time.deltaTime);
        
        return State.Running;
    }

    public override void CollisionDetected()
    {
        collided = true;
    }
}
