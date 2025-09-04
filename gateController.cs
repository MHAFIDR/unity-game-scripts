using UnityEngine;

public class GateController : MonoBehaviour
{
    public Vector3 openOffset;
    public float openSpeed = 2f;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + openOffset;
    }

    void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.Lerp(transform.position, openPosition, Time.deltaTime * openSpeed);
        }
    }

    public void OpenGate()
    {
        isOpening = true;
        Debug.Log("Gate is opening!");
    }
}