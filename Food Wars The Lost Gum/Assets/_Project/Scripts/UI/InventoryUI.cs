using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform previousWeaponSpritesParent;
    [SerializeField] private Transform nextWeaponSpritesParent;
    [SerializeField] private Transform activeWeaponSpritesParent;

    private Dictionary<Gun.GunType, GameObject> previousWeaponGameObjectsDict = new Dictionary<Gun.GunType, GameObject>();
    private Dictionary<Gun.GunType, GameObject> nextWeaponGameObjectsDict = new Dictionary<Gun.GunType, GameObject>();
    private Dictionary<Gun.GunType, GameObject> activeWeaponGameObjectsDict = new Dictionary<Gun.GunType, GameObject>();

    private GameObject currPreviousWeapon;
    private GameObject currNextWeapon;
    private GameObject currActiveWeapon;


    // reference to player's inventory
    [SerializeField] private Inventory playerInventory;

    // TODO: add some UI texts about ammo and stuff

    private Gun.GunType GetGunTypeByName(string name)
    {
        if (name == "ar")
        {
            return Gun.GunType.AssaultRifle;
        }
        else if (name == "bazooka")
        {
            return Gun.GunType.Bazooka;
        }
        else if (name == "Magnum")
        {
            return Gun.GunType.Magnum;
        }
        else if (name == "Pistol")
        {
            return Gun.GunType.Pistol;
        }
        else if (name == "Shotgun")
        {
            return Gun.GunType.Shotgun;
        }
        else if (name == "SMG")
        {
            return Gun.GunType.SMG;
        }
        else if (name == "Sniper")
        {
            return Gun.GunType.SniperRifle;
        }
        else
        {
            // TODO: add sowrd as a gun type
            return Gun.GunType.Sword;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // initialize weapon dicts
        foreach (Transform t in previousWeaponSpritesParent)
        {
            previousWeaponGameObjectsDict.Add(GetGunTypeByName(t.name), t.gameObject);
            t.gameObject.SetActive(false);
        }

        foreach (Transform t in nextWeaponSpritesParent)
        {
            nextWeaponGameObjectsDict.Add(GetGunTypeByName(t.name), t.gameObject);
            t.gameObject.SetActive(false);
        }

        foreach (Transform t in activeWeaponSpritesParent)
        {
            activeWeaponGameObjectsDict.Add(GetGunTypeByName(t.name), t.gameObject);
            t.gameObject.SetActive(false);
        }

        //Debug.Log(previousWeaponGameObjectsDict.Count);

        currPreviousWeapon = previousWeaponGameObjectsDict[playerInventory.GetPreviousWeaponType()];
        currNextWeapon = nextWeaponGameObjectsDict[playerInventory.GetNextWeaponType()];
        currActiveWeapon = activeWeaponGameObjectsDict[playerInventory.activeGun.gunType];
    }

    // Update is called once per frame
    void Update()
    {
        SetWeaponImage();
    }

    private void SetWeaponImage()
    {
        // first set inactive of previous ones
        currPreviousWeapon.SetActive(false);
        currNextWeapon.SetActive(false);
        currActiveWeapon.SetActive(false);

        // query the current state
        currPreviousWeapon = previousWeaponGameObjectsDict[playerInventory.GetPreviousWeaponType()];
        currNextWeapon = nextWeaponGameObjectsDict[playerInventory.GetNextWeaponType()];
        currActiveWeapon = activeWeaponGameObjectsDict[playerInventory.activeGun.gunType];

        // enable the new images
        currPreviousWeapon.SetActive(true);
        currNextWeapon.SetActive(true);
        currActiveWeapon.SetActive(true);

        // update ammo texts
        int currAmmo = playerInventory.activeGun.GetCurrAmmoCount();
        int ammoInventory = playerInventory.ammoDict[playerInventory.activeGun.gunType];

        Text ammoText = activeWeaponSpritesParent.gameObject.GetComponentInChildren<Text>();
        if(ammoText != null)
        {
            ammoText.text = currAmmo.ToString("D2") + '/' + ammoInventory.ToString("D3");
        }
    }
}
