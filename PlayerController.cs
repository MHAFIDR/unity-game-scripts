using UnityEngine;
using TMPro;
using Lean.Pool;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Weapon Settings")]
    public Weapon weapon;
    public Camera mainCamera;
    public float aimSpeed;
    public float aimLimit;

    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public TextMeshProUGUI interactionText;
    
    private float defaultAim;
    private float camFOV;
    private GameObject currentInteractableObject;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        if (mainCamera != null)
        {
            defaultAim = mainCamera.fieldOfView;
            camFOV = defaultAim;
        }
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        HandleInteraction();

        if (GameManager.instance.isUiOpen)
        {
            return;
        }

        HandleWeapon();
        Aim();
    }

    private void OnTriggerEnter(Collider other)
    {
        PickupItem item = other.GetComponent<PickupItem>();
        if (item != null && (item.isAmmoPickup || item.isHealthPickup))
        {
            PerformPickup(item, other.gameObject);
        }
    }

    void HandleInteraction()
    {
        interactionText.gameObject.SetActive(false);
        currentInteractableObject = null; 

        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, interactionDistance))
        {
            currentInteractableObject = hit.collider.gameObject;

            if (currentInteractableObject.CompareTag("PickupItem"))
            {
                PickupItem item = currentInteractableObject.GetComponent<PickupItem>();
                if (item != null && !item.isAmmoPickup && !item.isHealthPickup)
                {
                    interactionText.text = "Tekan [E] untuk mengambil " + item.mName;
                    interactionText.gameObject.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PerformPickup(item, currentInteractableObject);
                    }
                }
            }
            else if (currentInteractableObject.CompareTag("Kayu"))
            {
                interactionText.text = "Tekan [E] untuk Mengambil Kayu";
                interactionText.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Rigidbody rb = currentInteractableObject.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.isKinematic = false; // Disable physics
                        }
                }
            }
            else if (currentInteractableObject.CompareTag("LeverHolder"))
            {
                interactionText.text = "Buka inventaris untuk menempatkan Tuas";
                interactionText.gameObject.SetActive(true);
            }
            else if (currentInteractableObject.CompareTag("LeverMechanism"))
            {
                interactionText.text = "Tekan [E] untuk Mengaktifkan Tuas";
                interactionText.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    LeverMechanism leverMechanism = currentInteractableObject.GetComponent<LeverMechanism>();
                    if (leverMechanism != null)
                    {
                        leverMechanism.ActivateLever();
                    }
                }
            }
        }
    }

    void PerformPickup(PickupItem item, GameObject itemObject)
    {
        if (item.itemData != null)
        {
            InventoryManager.instance.playerInventory.Add(item.itemData);
            InventoryManager.instance.UpdateUI();
        }

        if (item.isAmmoPickup)
        {
            if (weapon != null)
            {
                weapon.currentMag = Mathf.Min(weapon.currentMag + item.ammoAmount, weapon.setMag);
                weapon.UpdateAmmoUI();
            }
        }
        
        if (item.isHealthPickup)
        {
            HealthManager healthManager = GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.CURRENTHEALTH += item.healthAmount;
            }
        }

        if (item.pickupFX != null)
        {
            LeanPool.Spawn(item.pickupFX, itemObject.transform.position, Quaternion.identity);
        }
        
        if (item.pickupSFX != null)
        {
            AudioSource.PlayClipAtPoint(item.pickupSFX, itemObject.transform.position);
        }

        Destroy(itemObject);
    }
    
    void HandleWeapon()
    {
        if (weapon == null || EventSystem.current.IsPointerOverGameObject()) return;
        
        if (Input.GetMouseButton(0)) { weapon.shoot(); }
        else if (Input.GetMouseButtonUp(0)) { weapon.StopShoot(); }
        if (weapon.currentAmmo < weapon.maxAmmo && Input.GetKeyDown(KeyCode.R)) { StartCoroutine(weapon.Reload()); }
    }

    void Aim()
    {
        if (mainCamera == null || EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButton(1)) { camFOV = Mathf.Lerp(camFOV, aimLimit, Time.deltaTime * aimSpeed); }
        else { camFOV = Mathf.Lerp(camFOV, defaultAim, Time.deltaTime * aimSpeed); }
        mainCamera.fieldOfView = camFOV;
    }

    public void UseLeverFromInventory(ItemData leverItemData)
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, interactionDistance))
        {
            LeverHolder holder = hit.collider.GetComponent<LeverHolder>();
            if (holder != null)
            {
                InventoryManager.instance.playerInventory.Remove(leverItemData);
                holder.PlaceLever();
                InventoryManager.instance.UpdateUI();
                Debug.Log("Lever placed from inventory!");
            }
            else
            {
                Debug.Log("You must be looking at a lever holder to use this.");
            }
        }
        else
        {
            Debug.Log("You are not looking at anything interactable.");
        }
    }
}