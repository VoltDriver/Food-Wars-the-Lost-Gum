using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public SoundPlayer m_buttonClickSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButtonClickSound()
    {
        // Play a sound effect
        if (m_buttonClickSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_buttonClickSound, transform.position, Quaternion.identity);
        }
    }
}
