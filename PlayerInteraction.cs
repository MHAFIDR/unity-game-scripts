using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction instance;

    public float interactionDistance;
    public LayerMask interactionLayer;
    public TextMeshProUGUI interactionText;

    private GameObject currentInteractableObject;
    private bool justInteracted = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Update()
    {
        if (GameManager.instance.isUiOpen)
        {
            interactionText.gameObject.SetActive(false);
            return;
        }

        if (justInteracted)
        {
            justInteracted = false;
            interactionText.gameObject.SetActive(false);
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionDistance, interactionLayer);
        GameObject objectToInteract = null;
        string textToShow = "";

        if (colliders.Length > 0)
        {
            objectToInteract = colliders[0].gameObject;

            if (objectToInteract.GetComponent<ItemPickup>() != null)
            {
                textToShow = "Tekan E untuk mengambil " + objectToInteract.GetComponent<ItemPickup>().itemData.itemName;
            }
            else if (objectToInteract.GetComponent<LeverMechanism>() != null)
            {
                textToShow = "Tekan E untuk mengaktifkan tuas";
            }
            else if (objectToInteract.GetComponent<BombController>() != null)
            {
                textToShow = "Tekan E untuk mengaktifkan bom";
            }
            else if (objectToInteract.GetComponent<LeverHolder>() != null)
            {
            
            }
            else if (objectToInteract.GetComponent<BombPlacement>() != null)
            {
                textToShow = "Anda dapat meletakkan bom di sini";
            }
            else if (objectToInteract.GetComponent<ChargingStation>() != null)
            {
                textToShow = "Tekan E untuk mengisi daya baterai";
            }
            else
            {
                objectToInteract = null;
            }
        }
        
        currentInteractableObject = objectToInteract;

        if (currentInteractableObject != null)
        {
            interactionText.text = textToShow;
            
            if (!string.IsNullOrEmpty(textToShow))
            {
                interactionText.gameObject.SetActive(true);
            }
            else
            {
                interactionText.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentInteractableObject.GetComponent<ItemPickup>() != null)
                    currentInteractableObject.GetComponent<ItemPickup>().PickupItem();
                
                if (currentInteractableObject.GetComponent<LeverMechanism>() != null)
                    currentInteractableObject.GetComponent<LeverMechanism>().ActivateLever();

                if (currentInteractableObject.GetComponent<BombController>() != null)
                    currentInteractableObject.GetComponent<BombController>().Detonate();
                
                if (currentInteractableObject.GetComponent<ChargingStation>() != null)
                    currentInteractableObject.GetComponent<ChargingStation>().ChargeBattery();
            }
        }
        else
        {
            interactionText.gameObject.SetActive(false);
        }
    }

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