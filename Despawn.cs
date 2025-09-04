using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
public class Despawn : MonoBehaviour
{
    public float delay = 3;
    private void OnEnable()
    {
        StartCoroutine(IEdelay());
        IEnumerator IEdelay()
        {
            yield return new WaitForSeconds(delay);
            LeanPool.Despawn(this);
        }
    }
}
