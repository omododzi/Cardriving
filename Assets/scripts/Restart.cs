using UnityEngine;
using UnityEngine.SceneManagement;
public class Restart : MonoBehaviour
{
    public void Rest()
    {
        if (Cartrigger.map == 1)
        {
            SceneManager.LoadScene("Map1");
        }else if (Cartrigger.map == 2)
        {
            SceneManager.LoadScene("Map2");
        }else if (Cartrigger.map == 3)
        {
            SceneManager.LoadScene("Map3");
        }
    }
}
