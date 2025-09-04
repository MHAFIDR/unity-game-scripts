using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using Lean.Transition;
using TMPro;

public class Weapon : MonoBehaviour
{
    [Header("RIFFLE STATS")]
    public bool isAutoShoot;
    public float shootDelay;
    public float range;
    public float limitRecoilHorizontal;
    public float limitRecoilVertical;
    public Transform aimPoint;
    public GameObject bulletShell;
    public Transform bulletShellSpawn;
    public GameObject muzzleFlashFX;
    public Transform muzzleFXSpawn;
    public Transform cameraLaser;
    public AudioClip reloadClip;
    public float maxAmmo;
    public float setMag;
    public TMP_Text ammoText;

    [Header("GENERAL STATS")]
    public float damage;
    public GameObject audioPrefabs;
    public GameObject impactFX;
    public AudioClip shootClip;
    public AudioClip impactClip;
    public string targetTag;
    public Animator userAnimator;
    public RuntimeAnimatorController weaponAnimatorController;

    [Header("MELEE")]
    public SphereCollider sCollider;

    [Header("DEBUG OUTPUT")]
    public float currentMag;
    public float currentAmmo;


    bool isShoot;
    bool isReloading;
    Vector3 defaultPosisiAimPoint;

    private void Start()
    {
        if (sCollider != null)
        {
            sCollider.enabled = false;
        }
        if (cameraLaser != null)
        {
            defaultPosisiAimPoint = aimPoint.localPosition;
        }
        currentMag = setMag;
        currentAmmo = maxAmmo; 
        UpdateAmmoUI();
        userAnimator.runtimeAnimatorController = weaponAnimatorController;
    }


    public void shoot()
    {
        if (currentAmmo >= 1 && !isReloading)
        {
            if (isAutoShoot)
            {
                if (!isShoot)
                {
                    StartCoroutine(IEShootDelay());
                    IEnumerator IEShootDelay()
                    {
                        shootRay();
                        isShoot = true;
                        yield return new WaitForSeconds(shootDelay);
                        isShoot = false;
                    }
                }
            }
            else
            {
                if (!isShoot)
                {
                    shootRay();
                    isShoot = true;
                }
            }
        }
        else
        {
            StartCoroutine(Reload());
        }
    }

    Quaternion RandomQuaternion()
    {
        return new Quaternion(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
    }

    void shootRay()
    {
        RaycastHit hit;
        LeanPool.Spawn(muzzleFlashFX, muzzleFXSpawn.position, muzzleFXSpawn.rotation);
        LeanPool.Spawn(bulletShell, bulletShellSpawn.position, RandomQuaternion());
        GameObject audioClone = LeanPool.Spawn(audioPrefabs, muzzleFXSpawn.position, Quaternion.identity);
        AudioSource audioSourceClone = audioClone.GetComponent<AudioSource>();
        audioSourceClone.clip = shootClip;
        audioSourceClone.Play();
        Recoil();
        currentAmmo--;
        UpdateAmmoUI();

        if (Physics.Raycast(cameraLaser.position, cameraLaser.forward, out hit, range))
        {
            LeanPool.Spawn(impactFX, hit.point, RandomQuaternion());
            audioClone = LeanPool.Spawn(audioPrefabs, hit.point, Quaternion.identity);
            audioSourceClone = audioClone.GetComponent<AudioSource>();
            audioSourceClone.clip = impactClip;
            audioSourceClone.Play();
            Debug.Log("Berinteraksi dengan objek: " + hit.transform.name);



            if (hit.transform.tag == "Enemy")
            {
                HealthManager hitHealthmanager = hit.transform.GetComponent<HealthManager>();
                hitHealthmanager.TakeDamage(damage);
                hitHealthmanager.isCustomBloodSplat = true;
                hitHealthmanager.customBloodSplat = hit.point;
            }
        }   
    }

    public void StopShoot()
    {
        isShoot = false;
    }

    void Recoil()
    {
        float recoilHorizontal = Random.Range(-limitRecoilHorizontal, limitRecoilHorizontal);
        float recoilVertical = Random.Range(-limitRecoilVertical, limitRecoilVertical);
        Vector3 recoilAimPointRandom = new Vector3(recoilHorizontal, recoilVertical, 0);
        Vector3 recoilAimPoint = aimPoint.localPosition + recoilAimPointRandom;

        aimPoint.localPositionTransition(recoilAimPoint, 0.1f).JoinTransition().localPositionTransition(defaultPosisiAimPoint, 0.01f);
    }

    public void MeleeAttack()
    {
        sCollider.enabled = true;
        GameObject audioClone = LeanPool.Spawn(audioPrefabs, transform.position, RandomQuaternion());
        AudioSource audioSourceClone = audioClone.GetComponent<AudioSource>();
        audioSourceClone.clip = shootClip;
        audioSourceClone.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Berhasil Berinteraksi" + other.gameObject.name);
        GameObject audioClone = LeanPool.Spawn(audioPrefabs, transform.position, RandomQuaternion());
        AudioSource audioSourceClone = audioClone.GetComponent<AudioSource>();
        audioSourceClone.clip = impactClip;
        audioSourceClone.Play();
        if (other.CompareTag(targetTag))
        {
            HealthManager hitHealthmanager = other.GetComponent<HealthManager>();
            hitHealthmanager.TakeDamage(damage);
            hitHealthmanager.isCustomBloodSplat = false;
        }
        sCollider.enabled = false;
    }

    public IEnumerator Reload()
    {
        if (currentMag >= 1 && currentAmmo < maxAmmo && !isReloading)
        {
        GameObject audioClone = LeanPool.Spawn(audioPrefabs, transform.position, RandomQuaternion());
        AudioSource audioSourceClone = audioClone.GetComponent<AudioSource>();
        audioSourceClone.clip = reloadClip;
        audioSourceClone.Play();

        userAnimator.Play("Reload");
        isReloading = true;
        yield return new WaitForSeconds(reloadClip.length);
        currentAmmo = maxAmmo;
        currentMag -= 1;
        isReloading = false;
        UpdateAmmoUI();
      }
    }

public void UpdateAmmoUI()
{
    if (ammoText != null)
    {
        ammoText.text = $"{currentAmmo} / {currentMag}";
    }
}
}