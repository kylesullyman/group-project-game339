using System;
using UnityEngine;

public class DeleteInSeconds : MonoBehaviour
{
    public float deleteTime;

    private void Start()
    {
        Destroy(this.gameObject, deleteTime);
    }
}
