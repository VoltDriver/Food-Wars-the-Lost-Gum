using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public bool m_loop = false;
    public bool m_followPlayer = false;

    [HideInInspector]
    public AudioSource m_audioSource;

    [HideInInspector]
    public bool m_flaggedForDeletion = false;

    private PlayerControl m_player;

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_player = GameObject.FindObjectOfType<PlayerControl>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_flaggedForDeletion && !m_audioSource.isPlaying)
        {
            Destroy(this.gameObject);
        }

        if (m_followPlayer && m_player != null)
        {
            transform.position = m_player.transform.position;
        }
    }

    public void Play()
    {
        if(!m_audioSource.isPlaying)
        {
            if (m_loop)
                m_audioSource.loop = true;

            m_audioSource.Play();
            
            if(!m_loop)
                m_flaggedForDeletion = true;
        }
    }

    public void Stop()
    {
        if(m_audioSource.isPlaying)
        {
            m_audioSource.Stop();

            if (m_loop)
                m_flaggedForDeletion = true;
        }
    }
}
