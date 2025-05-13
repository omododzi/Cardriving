using System;
using UnityEngine;

public class Lerning : MonoBehaviour
{
   public Transform spheare;
   public GameObject cube;

   void Start()
   {
      
   }

   void Update()
   {
      Vector3 pos = spheare.position;
      cube.transform.position += pos * Time.deltaTime;
   }

   void FixedUpdate()
   {
      
   }
}
