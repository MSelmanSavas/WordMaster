using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterGivenTime : MonoBehaviour
{
    [SerializeField]
    int DestroyTime;

    private void Start()
    {
        Destroy(gameObject, DestroyTime);
    }
}
