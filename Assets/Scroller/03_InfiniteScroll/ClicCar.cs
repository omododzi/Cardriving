using UnityEngine;
using System.Collections.Generic;
using FancyScrollView.Example03;
using UnityEngine.SceneManagement;
using YG;

public class ClicCar : MonoBehaviour
{
   private Cell cell;
   public static List<bool> carchanged = new List<bool>(){false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false};
   public static int index;
   void Start()
   {
      cell = GetComponentInParent<Cell>();
   }
   public void OnClickThis()
   {
      if (cell.currentPosition >= 0.48f && cell.currentPosition <= 0.6f)
      {
         index = cell.Index;
         YandexGame.savesData.lastcar = index;
         carchanged[index] = true;
         SceneManager.LoadScene("ChangeMap");
         
      }
   }
}
