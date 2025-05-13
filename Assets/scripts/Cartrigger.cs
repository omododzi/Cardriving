using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Cartrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            TriangleSC hitobj = other.GetComponent<TriangleSC>();
            hitobj.makesmoller = true;
        }

        if (other.CompareTag("Destroy"))
        {
            SceneManager.LoadScene("Map1");
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
