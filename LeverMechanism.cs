using UnityEngine;

public class LeverMechanism : MonoBehaviour
{
    public GateController gateToOpen;
    private bool hasBeenActivated = false;

    public void ActivateLever()
    {
        if (hasBeenActivated) return;

        if (gateToOpen != null)
        {
            gateToOpen.OpenGate();
            hasBeenActivated = true;
            Destroy(transform.parent.gameObject);
        }
    }
}