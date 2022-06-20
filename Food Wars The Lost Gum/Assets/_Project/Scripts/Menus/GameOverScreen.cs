using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject gameOverScreen;
    public GameObject InventoryCanvas;

    public AudioSource m_ambientMusic;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resume()
    {
        gameOverScreen.SetActive(false);
        InventoryCanvas.SetActive(true);
        Time.timeScale = 1f;
        gameIsPaused = false;

        if (m_ambientMusic != null && !m_ambientMusic.isPlaying)
        {
            m_ambientMusic.Play();
        }

        // Might want to add something to restore player's health / position / inventory etc
        // load scene to restore other stuff.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // since the player has DontDestroy, we need to reassign some values below
        // restore player health by half
        GameObject player = GameObject.Find("Player");
        player.GetComponent<PlayerHealth>().currentHealth = player.GetComponent<PlayerHealth>().maxHealth / 2;

        // restore player position to PlayerBeginSpot
        GameObject playerBeginSpot = GameObject.Find("PlayerBeginSpot");
        player.transform.position = playerBeginSpot.transform.position;

        // remove player's all other guns and bullets.
        player.GetComponent<Inventory>().ResetInventory();

        // allow player to move
        player.GetComponent<PlayerControl>().canMove = true;

        // reset player animation
        player.GetComponent<PlayerControl>().ResetAnimation();
    }

    public void Pause()
    {
        gameOverScreen.SetActive(true);
        InventoryCanvas.SetActive(false);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        if (m_ambientMusic != null)
        {
            m_ambientMusic.Stop();
        }
        // destroy all previously dont destroy stuff
        NoDuplicate[] uniqueObjects = FindObjectsOfType<NoDuplicate>();
        for (int i = 0; i < uniqueObjects.Length; i++)
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
}
