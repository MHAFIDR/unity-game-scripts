using UnityEngine;

public class BombPlacement : MonoBehaviour
{
    public GameObject bombPrefab; 
    private bool isBombPlaced = false;

    public void PlaceBomb()
    {
        if (isBombPlaced) return;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        Vector3 bombPosition = transform.position + new Vector3(0, 0.242f, 0.5f); 
        GameObject bomb = Instantiate(bombPrefab, bombPosition, Quaternion.Euler(90, 0, 180));
        bomb.transform.localScale = new Vector3(20, 20, 20);
        isBombPlaced = true;
        
        Destroy(gameObject);
    }
}