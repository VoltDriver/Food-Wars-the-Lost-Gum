using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Can only drop ammo of gun type that the player currently has. (cannot drop bazooka ammo crate if the player doesnt have it)
 * Each gun type has an ammo cap that chances of dropping are none if the player has that many ammo or more of that type.
 * Each ammo type and enemies have a base drop chance.
 */
public class LootDrop : MonoBehaviour
{
    Inventory playerInventory;
    PlayerHealth playerHealth;
    Dictionary<Gun.GunType, int> playerAmmoDict;
    Dictionary<Gun.GunType, int> ammoCapDict;
    List<GameObject> dropList;
    GameObject[] playerWeaponArray;
    [SerializeField]
    GameObject magnumAmmo, SMGAmmo, ARAmmo, shotgunAmmo, sniperAmmo, bazookaAmmo, smallHealth, bigHealth;
    [SerializeField]
    float dropChance;

    private void Awake()
    {
        ammoCapDict = new Dictionary<Gun.GunType, int>();
        ammoCapDict.Add(Gun.GunType.Magnum, 100);
        ammoCapDict.Add(Gun.GunType.SMG, 300);
        ammoCapDict.Add(Gun.GunType.AssaultRifle, 300);
        ammoCapDict.Add(Gun.GunType.Shotgun, 60);
        ammoCapDict.Add(Gun.GunType.SniperRifle, 50);
        ammoCapDict.Add(Gun.GunType.Bazooka, 20);

        dropList = new List<GameObject>();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerInventory = GameObject.Find("Player").GetComponent<Inventory>();
        playerAmmoDict = playerInventory.ammoDict;
        playerWeaponArray = playerInventory.weaponArray;
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
    }

    // ugly but simple
    public void DropLoot()
    {
        int listSize = 0;
        // does enemy drops something?
        if (Random.Range(0f, 1f) <= dropChance)
        {
            // Check weapons collected in the player inventory
            // Check amount of ammo in player inventory if the cap is reached
            // Check if player is missing health

            // Magnum
            if (HasWeapon(Gun.GunType.Magnum) && !AmmoCapped(Gun.GunType.Magnum))
            {
                dropList.Add(magnumAmmo);
                listSize++;
            }

            // SMG
            if (HasWeapon(Gun.GunType.SMG) && !AmmoCapped(Gun.GunType.SMG))
            {
                dropList.Add(SMGAmmo);
                listSize++;
            }

            // AR
            if (HasWeapon(Gun.GunType.AssaultRifle) && !AmmoCapped(Gun.GunType.AssaultRifle))
            {
                dropList.Add(ARAmmo);
                listSize++;
            }

            // Shotgun
            if (HasWeapon(Gun.GunType.Shotgun) && !AmmoCapped(Gun.GunType.Shotgun))
            {
                dropList.Add(shotgunAmmo);
                listSize++;
            }

            // Sniper
            if (HasWeapon(Gun.GunType.SniperRifle) && !AmmoCapped(Gun.GunType.SniperRifle))
            {
                dropList.Add(sniperAmmo);
                listSize++;
            }

            // Bazooka
            if (HasWeapon(Gun.GunType.Bazooka) && !AmmoCapped(Gun.GunType.Bazooka))
            {
                dropList.Add(bazookaAmmo);
                listSize++;
            }

            // Small Health
            if (playerHealth.currentHealth < playerHealth.maxHealth)
            {
                dropList.Add(smallHealth);
                listSize++;
            }

            // Big Health
            if (playerHealth.currentHealth + 3 < playerHealth.maxHealth)
            {
                dropList.Add(bigHealth);
                listSize++;
            }

            // Drop ammo or health
            if (listSize > 0)
            {
                int i = Random.Range(0, listSize);
                Instantiate(dropList[i], transform.position, Quaternion.identity);
            }



        }


    }

    bool HasWeapon(Gun.GunType type)
    {
        foreach (GameObject weapon in playerWeaponArray)
        {
            if (weapon != null && weapon.GetComponent<Gun>().gunType == type)
            {
                return true;
            }
        }

        return false;
    }

    bool AmmoCapped(Gun.GunType type)
    {
        return (playerAmmoDict[type] >= ammoCapDict[type]);
    }


}
