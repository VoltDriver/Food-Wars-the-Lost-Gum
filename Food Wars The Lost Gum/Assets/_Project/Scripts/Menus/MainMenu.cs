using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SoundPlayer m_buttonClickSound;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit successful.");
        Application.Quit();
    }

    public void PlayButtonClickSound()
    {
        // Play a sound effect
        if (m_buttonClickSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_buttonClickSound, transform.position, Quaternion.identity);
        }
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}