using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    public AudioClip fallSound;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void StartFalling()
    {
        rb.isKinematic = false;
        AudioSource.PlayClipAtPoint(fallSound, transform.position);
    }
}
