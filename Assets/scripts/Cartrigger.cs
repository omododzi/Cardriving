using System;
using UnityEngine;

public class Cartrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            TriangleSC hitobj = other.GetComponent<TriangleSC>();
            hitobj.makesmoller = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            TriangleSC hitobj = other.GetComponent<TriangleSC>();
            hitobj.makesmoller = false;
        }
    }
}
