using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject rpg;
    public GameObject gun;
    [SerializeField] private ParticleSystem shotEffect;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public Camera playerCamera;

    public float fireRate = 0.1f;
    public float damage = 10f;
    public float range = 100f;
    public bool isFullAuto = true;
    private int selectedWeapon = 1;
    private float nextFireTime = 0f;
    
    private void Update()
    {
        SwitchWeapon();
        if (isFullAuto)
        {
            HandleFullAutoFire();
        }
        else
        {
            HandleSingleFire();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleFireMode();
        }
    }

    private void SwitchWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 1;
            rpg.SetActive(true);
            gun.SetActive(false);
            Debug.Log("Selected Weapon: Projectile");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 2;
            rpg.SetActive(false);
            gun.SetActive(true);
            Debug.Log("Selected Weapon: Hitscan");
        }
    }

    private void HandleFullAutoFire()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            if (selectedWeapon == 1) HandleProjectileFire();
            if (selectedWeapon == 2) HandleHitscanFire();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void HandleSingleFire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedWeapon == 1) HandleProjectileFire();
            if (selectedWeapon == 2) HandleHitscanFire();
        }
    }

    private void HandleProjectileFire()
    {
        if (Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void HandleHitscanFire()
    {
        FireHitscan();
    }

    private void FireProjectile()
    {
        shotEffect.Play();
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = playerCamera.transform.forward * 20f;
        }
    }

    private void FireHitscan()
    {
        RaycastHit hit;
        shotEffect.Play();
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);

            ObjectData targetHealth = hit.transform.GetComponent<ObjectData>();
            if (targetHealth != null)
            {
                targetHealth.DealDamage(damage, false);
            }
        }
    }

    private void ToggleFireMode()
    {
        isFullAuto = !isFullAuto;
        Debug.Log("Fire mode switched: " + (isFullAuto ? "Full Auto" : "Single Fire"));
    }
}
