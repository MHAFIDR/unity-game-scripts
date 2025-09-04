using UnityEngine;

public class LeverHolder : MonoBehaviour
{
    public GameObject leverModel; // Model tuas yang akan diaktifkan
    private bool hasLever = false;

    
    public void PlaceLever()
    {
        if (hasLever) return;
        
        leverModel.SetActive(true);
        hasLever = true;
        Debug.Log("Lever has been placed!");

            
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        
    }
}