using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public int inventorySize = 20;
    public Inventory playerInventory;

    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform slotsPanel;
    public GameObject inventorySlotPrefab;
    public GameObject contextMenu;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Image itemIconImage;
    public Button useButton;
    public Button discardButton;
    public Button combineButton;

    [Header("Combine Recipes")]
    public List<CombinationRecipe> combinationRecipes;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private int selectedSlotIndex = -1;
    private List<ItemData> itemsToCombine = new List<ItemData>();

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        playerInventory = new Inventory(inventorySize);
        InitializeInventoryUI();
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool isActive = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(isActive);
            GameManager.instance.isUiOpen = isActive;

            if (isActive)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                DeselectAllSlots();
                itemsToCombine.Clear();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (!inventoryPanel.activeSelf) return;

        HandleNavigation();
    }

    void InitializeInventoryUI()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject slotGO = Instantiate(inventorySlotPrefab, slotsPanel);
            slots.Add(slotGO.GetComponent<InventorySlot>());
        }

        useButton.onClick.AddListener(OnUseButton);
        discardButton.onClick.AddListener(OnDiscardButton);
        combineButton.onClick.AddListener(OnCombineButton);
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
        slots[i].border.enabled = false; 

        if (i < playerInventory.items.Count)
        {
            slots[i].AddItem(playerInventory.items[i]);
        }
        else
        {
            slots[i].ClearSlot();
        }
        }
    }

    void HandleNavigation()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedSlotIndex == -1)
            {
                SelectSlot(0);
                return;
            }

            int columns = 5;
            int nextIndex = selectedSlotIndex;

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) nextIndex++;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) nextIndex--;
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) nextIndex += columns;
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) nextIndex -= columns;

            nextIndex = Mathf.Clamp(nextIndex, 0, inventorySize - 1);
            SelectSlot(nextIndex);
        }
    }

    void SelectSlot(int index)
    {
        if (selectedSlotIndex != -1)
        {
            slots[selectedSlotIndex].border.enabled = false;
        }

        selectedSlotIndex = index;
        InventorySlot selectedSlot = slots[selectedSlotIndex];
        ItemData itemInSlot = selectedSlot.GetItem();

        if (itemInSlot != null)
        {
            selectedSlot.border.enabled = true;
            UpdateItemInfo(itemInSlot);
            ShowContextMenu(selectedSlot.transform.position);
        }
        else
        {
            selectedSlot.border.enabled = false;
            UpdateItemInfo(null);
            contextMenu.SetActive(false);
        }
    }

    void DeselectAllSlots()
    {
        if (selectedSlotIndex != -1)
        {
            slots[selectedSlotIndex].border.enabled = false;
        }
        selectedSlotIndex = -1;
        UpdateItemInfo(null);
        contextMenu.SetActive(false);
    }

    void UpdateItemInfo(ItemData item)
    {
        if (item != null)
        {
            itemNameText.text = item.itemName;
            itemDescriptionText.text = item.description;
            itemIconImage.sprite = item.icon;
            itemIconImage.enabled = true;
        }
        else
        {
            itemNameText.text = "";
            itemDescriptionText.text = "";
            itemIconImage.sprite = null;
            itemIconImage.enabled = false;
        }
    }

    void ShowContextMenu(Vector3 position)
    {
        contextMenu.SetActive(true);
        contextMenu.transform.position = position + new Vector3(100, -50, 0);
    }

    void OnUseButton()
    {
        if (selectedSlotIndex != -1 && slots[selectedSlotIndex].GetItem() != null)
        {
            slots[selectedSlotIndex].GetItem().Use();
        }
        DeselectAllSlots();
    }

    void OnDiscardButton()
    {
        if (selectedSlotIndex != -1 && slots[selectedSlotIndex].GetItem() != null)
        {
            playerInventory.Remove(slots[selectedSlotIndex].GetItem());
            UpdateUI();
        }
        DeselectAllSlots();
    }

    void OnCombineButton()
    {
        if (selectedSlotIndex == -1) return;

        ItemData selectedItem = slots[selectedSlotIndex].GetItem();
        
        if (selectedItem != null && !itemsToCombine.Contains(selectedItem))
        {
            itemsToCombine.Add(selectedItem);
            Debug.Log("Added " + selectedItem.itemName + " to combination list. Total ingredients: " + itemsToCombine.Count);
            TryToCraft();
        }
        DeselectAllSlots();
    }

    void TryToCraft()
    {
        foreach (var recipe in combinationRecipes)
        {
            if (recipe.requiredIngredients.Count == itemsToCombine.Count)
            {
                bool allIngredientsMatch = true;
                foreach (var selectedIngredient in itemsToCombine)
                {
                    if (!recipe.requiredIngredients.Contains(selectedIngredient))
                    {
                        allIngredientsMatch = false;
                        break;
                    }
                }

                if (allIngredientsMatch)
                {
                    Debug.Log("Combination successful! Created " + recipe.resultItem.itemName);

                    foreach (var ingredient in itemsToCombine)
                    {
                        playerInventory.Remove(ingredient);
                    }
                    
                    playerInventory.Add(recipe.resultItem);

                    itemsToCombine.Clear();
                    UpdateUI();
                    return;
                }
            }
        }
        Debug.Log("No matching recipe found with the current items.");
    }
}