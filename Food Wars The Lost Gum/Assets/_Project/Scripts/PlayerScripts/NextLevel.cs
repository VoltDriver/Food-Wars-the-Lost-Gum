using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


public class NextLevel : MonoBehaviour
{
    [SerializeField] private string nextLevelName;
    [SerializeField] private Vector3 nextPlayerSpot;
    private Transform player;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player = collision.transform;
            player.position = nextPlayerSpot;

            if(nextLevelName == "Epilogue")
            {
                // Deactivate player and UI.
                player.gameObject.SetActive(false);
                Destroy(player.gameObject);
                GameObject ui = GameObject.FindObjectsOfType<NoDuplicate>().First(obj => obj.m_ID == "UI").gameObject;
                ui.SetActive(false);
                Destroy(ui);
            }

            // dont use cinemachine if we enter boss level, because we don't want camera to follow. 
            if(nextLevelName == "Boss")
            {
                GameObject cam = GameObject.Find("CM vcam1");
                Destroy(cam);
            }

            SceneManager.LoadScene(nextLevelName);
        }
    }
}
