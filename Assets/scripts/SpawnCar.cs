using System.Collections;
using System.Collections.Generic;
using FancyScrollView.Example03;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class SpawnCar : MonoBehaviour
{
    public GameObject[] spawnCars;
    public Transform spawnpos;

    void Start()
    {
        Instantiate(spawnCars[YandexGame.savesData.lastcar], spawnpos.position, spawnpos.rotation);
    }

    public void ResetCar()
    {
        SceneManager.LoadScene("ChangeCar");
        Cartrigger.map = 0;
        StartCoroutine(Reclama());
    }

    private IEnumerator Reclama()
    {
        yield return new WaitForSeconds(0.3f);
        Ygadd.TryShowFullscreenAdWithChance(101);
    }
}
