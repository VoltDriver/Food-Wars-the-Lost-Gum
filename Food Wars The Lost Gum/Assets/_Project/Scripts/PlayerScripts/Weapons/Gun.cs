using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    public GunType gunType;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private float spreadAngle;
    [SerializeField]
    private float reloadTime;
    public int ammoCapacity;
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    public SoundPlayer[] m_shootSounds;
    [SerializeField]
    public SoundPlayer m_reloadSound;

    // knockbackStrenght, range, projectileSpeed and damage could be in a script attached to the bullet prefab

    public enum GunType { Pistol, Magnum, AssaultRifle, SMG, Shotgun, Bazooka, SniperRifle, Sword}
    private int currentAmmoCount;
    //public bool isReloading;
    private float fireDelay;
    private float timeBetweenShots;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        currentAmmoCount = ammoCapacity;
        isReloading = false;
        fireDelay = 0;
        timeBetweenShots = 1 / fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireDelay > 0) fireDelay -= Time.deltaTime;
    }

    IEnumerator ReloadRoutine()
    {
        int ammoLeft = currentAmmoCount;
        int ammoFromInventory = 0;
        Inventory playerInventory = GetComponentInParent<Inventory>();
        int ammoInInventory = playerInventory.GetAmmoQty(gunType);

        // Play a sound effect
        if (m_reloadSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_reloadSound, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(reloadTime);


        // the ammoFromInventory will be the difference of the gun ammoCapacity and the number of bullets left before reloading
        // or what's left in the inventory in the case there are not enough ammo to fully reload
        ammoFromInventory = Mathf.Min(ammoCapacity - ammoLeft, ammoInInventory);         // Temporary

        // TO DO: remove ammo from the player inventory depending on the current ammo count and ammo left in the inventory
        playerInventory.RemoveAmmo(gunType, ammoFromInventory);

        currentAmmoCount = ammoFromInventory + ammoLeft;
        isReloading = false;
        Debug.Log("Gun reloaded");
        Debug.Log("Loaded " + ammoFromInventory + " bullets");
    }

    public void Fire(Quaternion direction)
    {
        //Shoot
        if (!isReloading && fireDelay <= 0)  // TO DO: change Input parameter for "Fire" buttons
        {
            if (currentAmmoCount == 0)
            {
                // TO DO: force reload or out of ammo sound cue
                Debug.Log("Out of ammo");
            }
            else
            {
                --currentAmmoCount;

                // Play a sound effect
                if (m_shootSounds != null && m_shootSounds.Length > 0)
                {
                    SoundPlayer sound = GameObject.Instantiate(m_shootSounds[Random.Range(0, m_shootSounds.Length)], transform.position, Quaternion.identity);
                }

                // fire a bullet giving a direction using the spreadAngle
                // special fire action for shotgun
                if (gunType == GunType.Shotgun)
                {
                    //float spread = 40f;
                    int numberOfPellets = 8;    // exluding the middle one
                    float pelletAngle = spreadAngle / numberOfPellets;

                    for (int i = 1; i <= (numberOfPellets) / 2; i++)
                    {
                        float angle = pelletAngle * i;

                        Quaternion angleRotation = Quaternion.AngleAxis(angle, transform.forward);

                        Instantiate(bulletPrefab, transform.position, direction * angleRotation);

                        angleRotation = Quaternion.AngleAxis(-angle, transform.forward);
                        Instantiate(bulletPrefab, transform.position, direction * angleRotation);
                    }
                    // Fire the center pellet
                    Instantiate(bulletPrefab, transform.position, direction);
                }
                else
                {
                    float bulletAngle = Random.Range(-spreadAngle, spreadAngle);

                    Quaternion angleRotation = Quaternion.AngleAxis(bulletAngle, transform.forward);

                    Instantiate(bulletPrefab, transform.position, direction * angleRotation);
                }
                fireDelay = timeBetweenShots;

                Debug.Log("Bullets left: " + currentAmmoCount);
            }
        }
    }

    public void Reload()
    {
        //Reload
        if (!isReloading)
        {
            if (GetComponentInParent<Inventory>().ammoDict[gunType] == 0)
            {
                Debug.Log("No more ammo of that type in the inventory");
                return;
            }
            if (currentAmmoCount == ammoCapacity) return;  //gun is already full

            else
            {
                Debug.Log("Reloading...");
                isReloading = true;
                StartCoroutine(ReloadRoutine());
            }
        }
    }

    public bool isReloading { get; set; }

    public float GetReloadTime()
    {
        return reloadTime;
    }

    public int GetCurrAmmoCount()
    {
        return currentAmmoCount;
    }

}
