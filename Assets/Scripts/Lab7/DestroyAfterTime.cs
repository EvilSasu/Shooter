using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float duration = 4f;
    void Start()
    {
        Invoke("DestroyAfter", duration);
    }

    private void DestroyAfter()
    {
        Destroy(this.gameObject);
    }
}
