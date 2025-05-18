using FancyScrollView.Example03;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class MapManager : MonoBehaviour
{
   
    public void Map1Load()
    {
        Cartrigger.map = 1;
        YandexGame.savesData.lastmap = Cartrigger.map;
        SceneManager.LoadScene("Map1");
    }

    public void Map2Load()
    {
        Cartrigger.map = 2;
        YandexGame.savesData.lastmap = Cartrigger.map;
        SceneManager.LoadScene("Map2");
    }

    public void Map3Load()
    {
        Cartrigger.map = 3;
        YandexGame.savesData.lastmap = Cartrigger.map;
        SceneManager.LoadScene("Map3");
    }
}
