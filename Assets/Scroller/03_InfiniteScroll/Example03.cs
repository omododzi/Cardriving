/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace FancyScrollView.Example03
{
    class Example03 : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;
        [SerializeField]Sprite[] icons = Resources.LoadAll<Sprite>("Icons"); // Для Image
        [SerializeField]Sprite[] pages = Resources.LoadAll<Sprite>("Pages"); // Для Page

        void Start()
        {
         
    
            var items = Enumerable.Range(0, 3)
                .Select(i => new ItemData(
                    $"Cell {i}", 
                    icons[i % icons.Length],
                    pages[i % pages.Length]))
                .ToArray();

            scrollView.UpdateData(items);
            scrollView.SelectCell(0);
        }
    }
}
