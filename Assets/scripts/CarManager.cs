using UnityEngine;
using UnityEngine.SceneManagement;
public class CarManager : MonoBehaviour
{
    public static bool Mustang = false;

    public void ChangeMustang()
    {
        Mustang = true;
        SceneManager.LoadScene("ChangeMap");
    }
}
