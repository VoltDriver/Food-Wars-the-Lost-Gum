using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDuplicate : MonoBehaviour
{
    public string m_ID = "";
    public bool m_transferAudio = false;

    // Start is called before the first frame update
    void Start()
    {
        // ensure that we dont have a duplicate already existing.
        NoDuplicate[] uniqueObjects = FindObjectsOfType<NoDuplicate>();

        foreach (var unique in uniqueObjects)
        {
            // Checking if this unique is not us, but has the same ID as us.
            if(unique.m_ID == this.m_ID && 
               this.m_ID != "" &&
               unique != this)
            {
                // A duplicate has been found. We dont need this.

                // If the object duplicate has audio, we want to copy our values to it.
                if(m_transferAudio)
                {
                    AudioSource audio = unique.GetComponent<AudioSource>();
                    AudioSource thisSource = GetComponent<AudioSource>();

                    if (audio != null &&
                        thisSource != null)
                    {
                        audio.volume = thisSource.volume;
                        audio.clip = thisSource.clip;

                        // Finally we play the audio.
                        audio.Play();
                    }
                }

                Destroy(this.gameObject);
            }
        }
    }

}
