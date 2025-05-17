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
        [SerializeField]Sprite[] icons ; // Для Image
        [SerializeField]Sprite[] pages ; // Для Page
        [SerializeField] public string[] names;
        [SerializeField] Sprite descriptions;

        void Start()
        {
            var items = Enumerable.Range(0, 29)
                .Select(i => new ItemData(
                    names[i % icons.Length], 
                descriptions,
                pages[i % pages.Length]))
                .ToArray();

            scrollView.UpdateData(items);
            scrollView.SelectCell(0);
        }
    }
}
