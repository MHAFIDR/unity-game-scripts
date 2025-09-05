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
    private bool justInteracted = false;

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

        if (GameManager.instance != null && GameManager.instance.isUiOpen)
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

        // Reset just interacted flag
        if (justInteracted)
        {
            justInteracted = false;
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, interactionDistance))
        {
            currentInteractableObject = hit.collider.gameObject;
            string textToShow = "";

            // Handle different object types
            if (currentInteractableObject.CompareTag("PickupItem"))
            {
                PickupItem item = currentInteractableObject.GetComponent<PickupItem>();
                if (item != null && !item.isAmmoPickup && !item.isHealthPickup)
                {
                    textToShow = "Tekan [E] untuk mengambil " + item.mName;
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PerformPickup(item, currentInteractableObject);
                    }
                }
            }
            // Handle ItemPickup (dari PlayerInteraction)
            else if (currentInteractableObject.GetComponent<ItemPickup>() != null)
            {
                ItemPickup itemPickup = currentInteractableObject.GetComponent<ItemPickup>();
                textToShow = "Tekan E untuk mengambil " + itemPickup.itemData.itemName;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    itemPickup.PickupItem();
                }
            }
            else if (currentInteractableObject.CompareTag("LeverHolder"))
            {
                textToShow = "Buka inventaris untuk menempatkan Tuas";
            }
            else if (currentInteractableObject.CompareTag("LeverMechanism") || 
                     currentInteractableObject.GetComponent<LeverMechanism>() != null)
            {
                textToShow = "Tekan [E] untuk mengaktifkan tuas";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    LeverMechanism leverMechanism = currentInteractableObject.GetComponent<LeverMechanism>();
                    if (leverMechanism != null)
                    {
                        leverMechanism.ActivateLever();
                    }
                }
            }
            else if (currentInteractableObject.CompareTag("Kayu"))
            {
                textToShow = "Tekan [E] untuk Menyingkirkan Kayu";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Rigidbody rb = currentInteractableObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.AddForce(mainCamera.transform.forward * 5f, ForceMode.Impulse);
                    }
                }
            }
            else if (currentInteractableObject.CompareTag("ChargerStation") ||
                     currentInteractableObject.GetComponent<ChargingStation>() != null)
            {
                textToShow = "Tekan E untuk mengisi daya baterai";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ChargingStation station = currentInteractableObject.GetComponent<ChargingStation>();
                    if (station != null)
                    {
                        station.ChargeBattery();
                    }
                }
            }
            else if (currentInteractableObject.CompareTag("PlaceBomTime"))
            {
                textToShow = "Buka inventaris untuk meletakkan bom di sini";
            }
            else if (currentInteractableObject.CompareTag("BombWaktu"))
            {
                textToShow = "Tekan E untuk mengaktifkan bom";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    BombController bomb = currentInteractableObject.GetComponent<BombController>();
                    if (bomb != null)
                    {
                        bomb.Detonate();
                    }
                }
            }

            // Show interaction text if valid
            if (!string.IsNullOrEmpty(textToShow))
            {
                interactionText.text = textToShow;
                interactionText.gameObject.SetActive(true);
            }
        }
    }

    void PerformPickup(PickupItem item, GameObject itemObject)
    {
        if (item.itemData != null)
        {
            bool success = InventoryManager.instance.playerInventory.Add(item.itemData);
            if (!success)
            {
                Debug.Log("Inventory penuh!");
                return;
            }
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
                justInteracted = true;
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

    // TAMBAHAN: Method untuk item-item lain (dari PlayerInteraction)
    public void UseLever(LeverItem leverItem)
    {
        if (currentInteractableObject != null)
        {
            LeverHolder holder = currentInteractableObject.GetComponent<LeverHolder>();
            if (holder != null)
            {
                holder.PlaceLever();
                InventoryManager.instance.playerInventory.Remove(leverItem);
                InventoryManager.instance.UpdateUI();
                justInteracted = true;
            }
            else
            {
                Debug.Log("You must be looking at a lever holder to use this.");
            }
        }
        else
        {
            Debug.Log("You can't use that here.");
        }
    }

    public void UseBombItem(TimedBombItem bombItem)
    {
        if (currentInteractableObject != null)
        {
            BombPlacement placementZone = currentInteractableObject.GetComponent<BombPlacement>();
            if (placementZone != null)
            {
                placementZone.PlaceBomb();
                InventoryManager.instance.playerInventory.Remove(bombItem);
                InventoryManager.instance.UpdateUI();
                justInteracted = true;
            }
            else
            {
                Debug.Log("You must be at a designated placement zone to use the bomb.");
            }
        }
        else
        {
            Debug.Log("You can't use that here.");
        }
    }
}
