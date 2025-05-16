using FancyScrollView.Example03;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MapManager : MonoBehaviour
{
    public void Map1Load()
    {
        Cartrigger.map = 1;
        SceneManager.LoadScene("Map1");
    }

    public void Map2Load()
    {
        Cartrigger.map = 2;
        SceneManager.LoadScene("Map2");
    }

    public void Map3Load()
    {
        Cartrigger.map = 2;
        SceneManager.LoadScene("Map3");
    }
}
