using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Cartrigger : MonoBehaviour
{
    public static int map;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            TriangleSC hitobj = other.GetComponent<TriangleSC>();
            hitobj.makesmoller = true;
        }

        if (other.CompareTag("Destroy"))
        {
            if (map == 1)
            {
                SceneManager.LoadScene("Map1");
            }else if (map == 2)
            {
                SceneManager.LoadScene("Map2");
            }else if (map == 3)
            {
                SceneManager.LoadScene("Map3");
            }
            
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
