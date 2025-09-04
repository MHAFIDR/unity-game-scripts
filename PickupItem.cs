using UnityEngine;
[System.Serializable]
public class PickupItem : MonoBehaviour
{
    
    public bool isAmmoPickup;
    public bool isHealthPickup;
    public ItemData itemData;
    
    public int ammoAmount = 30; 
    public int healthAmount = 25; 

    public GameObject audioPrefabs;
    public AudioClip pickupSFX;
    public GameObject pickupFX;
    public string mName;
}