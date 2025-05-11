using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleSC : MonoBehaviour
{
   
   public float offset = 1;
   public bool makesmoller;
  
   private void Update()
   {
      if (makesmoller)
      {
         StartCoroutine(MakeSmoller());
      }

      if (gameObject.transform.localScale.x<= 0)
      {
         Destroy(gameObject);
      }
   }

   public IEnumerator MakeSmoller()
   {
      yield return new WaitForSeconds(1f);
      Vector3 newskale = gameObject.transform.localScale;
      newskale.x -= offset;
      newskale.y -= offset;
      gameObject.transform.localScale = newskale;
   }
}
