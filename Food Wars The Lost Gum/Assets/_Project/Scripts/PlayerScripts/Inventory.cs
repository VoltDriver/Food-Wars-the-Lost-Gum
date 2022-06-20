using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    public GameObject[] weaponArray = new GameObject[7];
    public GameObject guns;             // Empty parent object for guns
    GameObject currentWeaponObj;
    public Gun activeGun;
    int currentWeaponObjIndex;
    public Dictionary<Gun.GunType, int> ammoDict;

    public SoundPlayer m_ammoPickupSound;
    public SoundPlayer m_gunPickupSound;
    public SoundPlayer m_weaponSwitchSound;


    // Start is called before the first frame update
    void Awake()
    {
        // Create dictionary for all ammo (gun) type
        
        ResetInventory();
    }

    public void ResetInventory()
    {
        ammoDict = new Dictionary<Gun.GunType, int>();
        foreach (Gun.GunType type in Enum.GetValues(typeof(Gun.GunType)))
        {
            ammoDict.Add(type, 0);
        }

        // remove any other guns besides pistol
        for (int i = 0; i < weaponArray.Length; i++)
        {
            if (weaponArray[i] != null && weaponArray[i].name != "Pistol")
            {
                Destroy(weaponArray[i]);
            }
        }

        // deactivate each weapon and activate only the first one
        for (int i = 0; i < weaponArray.Length; i++)
        {
            if (weaponArray[i] != null)
            {
                weaponArray[i].SetActive(false);
            }
        }
        currentWeaponObj = weaponArray[0];
        currentWeaponObj.SetActive(true);
        currentWeaponObjIndex = 0;
        activeGun = currentWeaponObj.GetComponent<Gun>();

        // TEST
        ammoDict[Gun.GunType.Pistol] = 999;
    }

    // Update is called once per frame
    void Update()
    {
        // Previous weapon
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectPreviousWeapon();
        }

        // Next weapon
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectNextWeapon();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            //FireGun();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadGun();
        }
    }

    public void CollectAmmo(GameObject ammoObj)
    {
        Ammo ammo = ammoObj.GetComponent<Ammo>();
        Gun.GunType ammoType = ammo.ammoType;
        int amount = ammo.amount;

        ammoDict[ammoType] += amount;
        Debug.Log(amount + " bullets of type " + ammoType + " added. Total: " + ammoDict[ammoType]);

        // Play a sound effect
        if(m_ammoPickupSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_ammoPickupSound, transform.position, Quaternion.identity);
        }
    }

    public void CollectWeapon(GameObject weaponObj)
    {
        GunDrop gunDropScript = weaponObj.GetComponent<GunDrop>();
        Gun.GunType dropGunType = gunDropScript.gunType;
        bool hasGun = false;

        // Add the weapon in the inventory if not already collected, else add ammo
        foreach (GameObject weapon in weaponArray)
        {
            if (weapon != null && weapon.GetComponent<Gun>().gunType == dropGunType)
            {
                hasGun = true;
                break;
            }
        }

        if (hasGun) // add ammo to the inventory equal to the ammoCapacity of that gun
        {
            // get the ammoCapacity value from the gunPrefab in the GunDrop script of that object
            int amount = gunDropScript.gunPrefab.GetComponent<Gun>().ammoCapacity;
            ammoDict[dropGunType] += amount;

            // Play a sound effect
            if (m_ammoPickupSound != null)
            {
                SoundPlayer sound = GameObject.Instantiate(m_ammoPickupSound, transform.position, Quaternion.identity);
            }
        }
        else        // add the gun to the inventory
        {
            //find the first empty slot in the inventory
            for (int i = 0; i < weaponArray.Length; i++)
            {
                if (weaponArray[i] == null)
                {
                    weaponArray[i] = Instantiate(gunDropScript.gunPrefab, guns.transform.position, guns.transform.rotation, guns.transform);
                    weaponArray[i].SetActive(false);
                    break;
                }
            }

            //add extra ammo from the gun drop only after collecting it for the first time.
            int amount = gunDropScript.gunPrefab.GetComponent<Gun>().ammoCapacity * gunDropScript.extraMag;
            ammoDict[dropGunType] += amount;

            // Play a sound effect
            if (m_gunPickupSound != null)
            {
                SoundPlayer sound = GameObject.Instantiate(m_gunPickupSound, transform.position, Quaternion.identity);
            }
        }
    }

    public void RemoveAmmo(Gun.GunType ammoType, int amount)
    {
        // Do not remove ammo for pistol gun
        if (ammoType == Gun.GunType.Pistol)
        {
            return;
        }
        
        ammoDict[ammoType] -= amount;
        Debug.Log(ammoDict[ammoType] + " bullets left of type " + ammoType);
    }

    public int GetAmmoQty(Gun.GunType ammoType)
    {
        return ammoDict[ammoType];
    }

    public void SelectPreviousWeapon()
    {
        // To prevent reload interuption
        if (!activeGun.isReloading)
        {
            // deactivate current equipped weapon
            currentWeaponObj.SetActive(false);

            int i = currentWeaponObjIndex;
            int fullLoop = 0;
            while (true)
            {
                // if reached begining of array, set i to Lenght - 1; else decrement i
                if (i == 0)
                {
                    i = weaponArray.Length - 1;
                }
                else
                {
                    i--;
                }

                fullLoop++;

                if (fullLoop > weaponArray.Length + 1)
                {
                    // All weapons are null.
                    currentWeaponObjIndex = 0;
                    currentWeaponObj = weaponArray[0];
                    currentWeaponObj.SetActive(true);
                    activeGun = currentWeaponObj.GetComponent<Gun>();
                    break;
                }

                // if a weapon is found, set as current equipped weapon
                if (weaponArray[i] != null)
                {
                    // Play a sound effect
                    if (m_weaponSwitchSound != null && currentWeaponObjIndex != i)
                    {
                        SoundPlayer sound = GameObject.Instantiate(m_weaponSwitchSound, transform.position, Quaternion.identity);
                    }

                    currentWeaponObjIndex = i;
                    currentWeaponObj = weaponArray[i];
                    currentWeaponObj.SetActive(true);
                    activeGun = currentWeaponObj.GetComponent<Gun>();
                    break;
                }
            }
        }
    }

    public void SelectNextWeapon()
    {
        // To prevent reload interuption
        if (!activeGun.isReloading)
        {
            // deactivate current equipped weapon
            currentWeaponObj.SetActive(false);

            int i = currentWeaponObjIndex;
            int fullLoop = 0;
            while (true)
            {
                // if reached end of array, set i to 0; else increment i
                if (i == weaponArray.Length - 1)
                {
                    i = 0;
                }
                else
                {
                    i++;
                }

                fullLoop++;

                if (fullLoop > weaponArray.Length + 1)
                {
                    // All weapons are null.
                    currentWeaponObjIndex = 0;
                    currentWeaponObj = weaponArray[0];
                    currentWeaponObj.SetActive(true);
                    activeGun = currentWeaponObj.GetComponent<Gun>();
                    break;
                }

                // if a weapon is found, set as current equipped weapon
                if (weaponArray[i] != null)
                {
                    // Play a sound effect
                    if (m_weaponSwitchSound != null && currentWeaponObjIndex != i)
                    {
                        SoundPlayer sound = GameObject.Instantiate(m_weaponSwitchSound, transform.position, Quaternion.identity);
                    }

                    currentWeaponObjIndex = i;
                    currentWeaponObj = weaponArray[i];
                    currentWeaponObj.SetActive(true);
                    activeGun = currentWeaponObj.GetComponent<Gun>();
                    break;
                }
            }
        }
    }


    // for UI use
    public Gun.GunType GetPreviousWeaponType()
    {
        Gun.GunType gunType;
        int i = currentWeaponObjIndex;
        int fullLoop = 0;
        while (true)
        {
            // if reached end of array, set i to 0; else increment i
            if (i == 0)
            {
                i = weaponArray.Length - 1;
            }
            else
            {
                i--;
            }

            fullLoop++;

            if(fullLoop > weaponArray.Length + 1)
            {
                // All weapons are null.
                gunType = Gun.GunType.Pistol;
                break;
            }

            // if a weapon is found, set as current equipped weapon
            if (weaponArray[i] != null)
            {
                gunType = weaponArray[i].GetComponent<Gun>().gunType;
                break;
            }
        }

        return gunType;
    }

    public Gun.GunType GetNextWeaponType()
    {
        Gun.GunType gunType;
        int i = currentWeaponObjIndex;
        int fullLoop = 0;
        while (true)
        {
            // if reached end of array, set i to 0; else increment i
            if (i == weaponArray.Length - 1)
            {
                i = 0;
            }
            else
            {
                i++;
            }

            fullLoop++;

            if (fullLoop > weaponArray.Length + 1)
            {
                // All weapons are null.
                gunType = Gun.GunType.Pistol;
                break;
            }

            // if a weapon is found, set as current equipped weapon
            if (weaponArray[i] != null)
            {
                gunType = weaponArray[i].GetComponent<Gun>().gunType;
                break;
            }
        }

        return gunType;
    }

    public void FireGun(Quaternion direction)
    {
        activeGun.Fire(direction);
    }

    public void ReloadGun()
    {
        activeGun.Reload();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag == "GunDrop")
        {
            CollectWeapon(obj);
            Destroy(obj);
        }
        if (obj.tag == "Ammo")
        {
            CollectAmmo(obj);
            Destroy(obj);
        }
    }
}
