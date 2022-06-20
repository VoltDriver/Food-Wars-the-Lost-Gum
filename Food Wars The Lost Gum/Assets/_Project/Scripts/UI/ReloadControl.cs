using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadControl : MonoBehaviour
{
    public Progress_bar progressBar;
    [SerializeField] private Inventory playerInventory;     // assign in Scene

    private float reloadTimer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // get the player's current gun and required field for calculation 
        Gun currGun = playerInventory.activeGun;
        float reloadTime = currGun.GetReloadTime();
        int currAmmoCount = currGun.GetCurrAmmoCount();
        bool isReloading = currGun.isReloading;

        // set the max fill amount of progress bar
        progressBar.max = reloadTime;
        // set the fill amount to full if out of ammo, otherwise set fill to 0 (only when not reloading)
        if (!isReloading)
        {
            reloadTimer = reloadTime;
            if(currAmmoCount == 0)
            {
                progressBar.current = reloadTime;
            }
            else
            {
                progressBar.current = 0;
            }
        }
        // only do the fill image animation when gun is reloading
        else if (isReloading)
        {
            progressBar.current = reloadTimer;
            reloadTimer -= Time.deltaTime;
        }

    }

}
