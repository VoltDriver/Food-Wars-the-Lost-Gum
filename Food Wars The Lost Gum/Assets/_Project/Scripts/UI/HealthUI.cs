using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    // we support max 10 health now
    public Sprite fullHeart;
    public Sprite poppedHeart;

    public PlayerHealth playerHealth;

    private List<GameObject> AllHealthIcons = new List<GameObject>();
    private int maxHealthSupported = 10;

    private List<Image> HealthIcons = new List<Image>();

    // Start is called before the first frame update
    void Start()
    {
        // initialize player's health UI
        foreach(Transform t in transform)
        {
            AllHealthIcons.Add(t.gameObject);
        }

        // disable the hearts thats not needed
        int playerMaxHealth = Mathf.Clamp(playerHealth.maxHealth, 0, 10);
        for(int i = maxHealthSupported-1; i > playerMaxHealth-1; i--)
        {
            AllHealthIcons[i].SetActive(false);
        }

        // populate HealthIcons with the rest of the hearts
        for(int i = 0; i < playerMaxHealth; i++)
        {
            HealthIcons.Add(AllHealthIcons[i].GetComponent<Image>());
        }

    }

    // Update is called once per frame
    void Update()
    {
        int currHealth = playerHealth.currentHealth;
        // read player's health and update
        for(int i = 0; i < HealthIcons.Count; i++)
        {
            // life not used
            if(i+1 <= currHealth)
            {
                HealthIcons[i].sprite = fullHeart;
            }
            // life used
            else
            {
                HealthIcons[i].sprite = poppedHeart;
            }
        }
    }
}
