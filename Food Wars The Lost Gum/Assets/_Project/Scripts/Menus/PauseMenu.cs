using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool gameIsPaused = false;    // Check is game is currently paused regardless of specific UI object
    public GameObject pauseMenuUI;  // Refer to pause menu UI in script
    public GameObject InventoryCanvas;
    public GameObject HealthBarCanvas;
    public GameObject optionCanvas;

    public SoundPlayer m_buttonClickSound;

    public SoundPlayer m_pauseSound;
    public SoundPlayer m_unPauseSound;

    public AudioSource m_ambientMusic;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);   // Disable pause menu
        // enable inventory and health
        InventoryCanvas.SetActive(true);
        Time.timeScale = 1f;            // Unfreeze game and return to normal gameplay
        gameIsPaused = false;

        // Play a sound effect
        if (m_unPauseSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_unPauseSound, transform.position, Quaternion.identity);
            sound.Play();
        }

        if (m_ambientMusic != null && !m_ambientMusic.isPlaying)
        {
            m_ambientMusic.Play();
        }

        // also deactivate option menu
        optionCanvas.SetActive(false);
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);    // Enable pause menu
        // disable inventory and health
        InventoryCanvas.SetActive(false);
        Time.timeScale = 0f;            // Freeze game's current state
        gameIsPaused = true;

        // Play a sound effect
        if (m_pauseSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_pauseSound, transform.position, Quaternion.identity);
            sound.Play();
        }

        if (m_ambientMusic != null && m_ambientMusic.isPlaying)
        {
            m_ambientMusic.Pause();
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);

        if(m_ambientMusic != null)
        {
            m_ambientMusic.Stop();
        }

        // destroy all previously dont destroy stuff
        NoDuplicate[] uniqueObjects = FindObjectsOfType<NoDuplicate>();
        for(int i = 0; i < uniqueObjects.Length; i++)
        {
            Destroy(uniqueObjects[i].gameObject);
        }
        gameIsPaused = false;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game successful.");
        Application.Quit();
    }

    public void PlayButtonClickSound()
    {
        // Play a sound effect
        if (m_buttonClickSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_buttonClickSound, transform.position, Quaternion.identity);
            sound.Play();
        }
    }
}
