using FancyScrollView.Example03;
using UnityEngine;

public class SpawnCar : MonoBehaviour
{
    public GameObject cellPrefab;
    private ClicCar cliccar;
    public GameObject[] spawnCars;
    public GameObject spawnpos;

    void Start()
    {
        cliccar = cellPrefab.GetComponent<ClicCar>();
        Instantiate(spawnCars[cliccar.index], spawnpos.transform.position, Quaternion.identity);
    }
}
