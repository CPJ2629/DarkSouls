using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tip : MonoBehaviour
{
    public CapsuleCollider col;
    public GameObject tip;

    private void OnTriggerStay(Collider col)
    {
        tip.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        tip.SetActive(false);
    }
}
