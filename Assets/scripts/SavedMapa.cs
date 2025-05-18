using UnityEngine;
using UnityEngine.SceneManagement;
using YG;
public class SavedMapa : MonoBehaviour
{
    void Awake()
    {
        switch (YandexGame.savesData.lastmap)
        {
            case 1:
            {
                SceneManager.LoadScene("Map1");
                break;
            }
            case 2:
            {
                SceneManager.LoadScene("Map2");
                break;
            }
            case 3:
            {
                SceneManager.LoadScene("Map3");
                break;
            }
        }
    }
}
