using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnionBossPhase1 : MonoBehaviour
{
    //Attack List: 0 - Spawn Peas, 1 - Center roots, 2 - Swipe Attack From Left, 3 - Swipe Attack From Right, 4 - Charge Attack, 5 - Rock Drop Rage

    private int _lastAttack = -1;
    private int _nextAttack = 1;
    [SerializeField] private int _health = 100;
    [SerializeField] private int _maxHealth = 100;

    [SerializeField] private GameObject _playerRef;

    public int LastAttack { get => _lastAttack; set =>_lastAttack = value; }

    public int NextAttack { get => _nextAttack; set => _nextAttack = value; }

    public int Health { get => _health; set => _health = value; }

    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }

    [SerializeField] public float wallY = 1f;
    [SerializeField] public float bottomY = -5f;
    [SerializeField] public float leftWallX = -9f;
    [SerializeField] public float firstFifthX = -5f;
    [SerializeField] public float secondFifthX = -2f;
    [SerializeField] public float thirdFifthX = 2f;
    [SerializeField] public float fourthFifthX = 5f;
    [SerializeField] public float rightWallX = 9f;

    [SerializeField] private OnionPhase1AnimationManager animationManager;

    [SerializeField] public GameObject swipeVine;
    [SerializeField] public GameObject floorVine;
    [SerializeField] public GameObject[] enemyToSpawn;
    [SerializeField] public GameObject agentParent;


    [SerializeField] private GameObject _deathAnim;
    [SerializeField] public GameObject exitCrown;

    public SoundPlayer m_bossDeathSound;
    public SoundPlayer[] m_bossHurtSounds;

    //  [SerializeField] public GameObject fallingObject;
    //  [SerializeField] public GameObject fallingObjectShadow;
    private bool inPain;
    private float painStartTime;
    public float painBlinkTime = 0.1f;

    public bool m_isDead = false;
    public float timeOfDeath;
    private GameObject theCrown;
    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;
        _playerRef = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (inPain && Time.time >= painStartTime + painBlinkTime)
        {
            inPain = false;
            transform.GetComponent<SpriteRenderer>().color = Color.white;
        }
        if (theCrown != null && m_isDead && Time.time > timeOfDeath + 3.0f && theCrown.transform.position.y > -1f)
            theCrown.transform.position += Vector3.down * Time.deltaTime;
    }

    public Vector3 GetPlayerPosition()
    {
        return _playerRef.transform.position;
    }

    public void AnimationWrapper(int t)
    {
        animationManager.SetTrigger(t);
    }
    public void HurtTheBoss(int i)
    {
        if (Health <= 0) KillTheBoss();
        else
        {
            Health -= i;
            inPain = true;
            painStartTime = Time.time;
            transform.GetComponent<SpriteRenderer>().color = Color.red;
            Debug.Log("Boss Health: " + Health);

            // Play a sound effect
            if (m_bossHurtSounds != null && m_bossHurtSounds.Length > 0)
            {
                SoundPlayer sound = GameObject.Instantiate(m_bossHurtSounds[Random.Range(0, m_bossHurtSounds.Length)], transform.position, Quaternion.identity);
            }
        }
        
    }

    public void KillTheBoss()
    {
        if(!m_isDead)
        {
            m_isDead = true;
            transform.GetComponent<BehaviourTreeRunner>().enabled = false;
            Instantiate(_deathAnim, new Vector3(0, 2.5f, 0), Quaternion.identity);
            timeOfDeath = Time.time;
            transform.GetComponent<Renderer>().enabled = false;
            theCrown = Instantiate(exitCrown, new Vector3(0, 3.5f, 0), Quaternion.identity);
            // Play a sound effect
            if (m_bossDeathSound != null)
            {
                SoundPlayer sound = GameObject.Instantiate(m_bossDeathSound, transform.position, Quaternion.identity);
            }
        }
    }
}
