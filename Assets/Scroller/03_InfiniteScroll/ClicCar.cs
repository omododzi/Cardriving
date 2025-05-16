using UnityEngine;
using System.Collections.Generic;
using FancyScrollView.Example03;
using UnityEngine.SceneManagement;
public class ClicCar : MonoBehaviour
{
   private Cell cell;
   public static List<bool> carchanged = new List<bool>(){false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false};
   public int index;
   void Start()
   {
      cell = GetComponentInParent<Cell>();
   }
   public void OnClick()
   {
      index = cell.Index;
      carchanged[index] = true;
      SceneManager.LoadScene("ChangeMap");
   }
}
