using UnityEngine;

public class DestructibleGate : MonoBehaviour
{
    public Vector3 fallOffset;
    public float fallSpeed = 2f;
    private Vector3 initialPosition;
    private Vector3 fallenPosition;
    private bool isFalling = false;

    void Start()
    {
        initialPosition = transform.position;
        fallenPosition = initialPosition + fallOffset;
    }

    void Update()
    {
        if (isFalling)
        {
            transform.position = Vector3.Lerp(transform.position, fallenPosition, Time.deltaTime * fallSpeed);
        }
    }

    public void Shatter()
    {
        if (isFalling) return;

        isFalling = true;
        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col, 1f);
        }
    }
}