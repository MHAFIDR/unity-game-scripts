using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public FallingBlock[] blocksToFall;
    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            foreach (FallingBlock block in blocksToFall)
            {
                if (block != null)
                    block.StartFalling();
            }
        }
    }
}