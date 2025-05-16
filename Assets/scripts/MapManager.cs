using FancyScrollView.Example03;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MapManager : MonoBehaviour
{
    public void Map1Load()
    {
        SceneManager.LoadScene("Map1");
    }

    public void Map2Load()
    {
        SceneManager.LoadScene("Map2");
    }

    public void Map3Load()
    {
        SceneManager.LoadScene("Map3");
    }
}
