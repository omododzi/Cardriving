using System;
using FancyScrollView.Example03;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SpawnCar : MonoBehaviour
{
    public GameObject[] spawnCars;
    public Transform spawnpos;

    void Start()
    {
        Instantiate(spawnCars[ClicCar.index], spawnpos.position, spawnpos.rotation);
    }

    public void ResetCar()
    {
        SceneManager.LoadScene("ChangeCar");
    }
}
