/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;


namespace FancyScrollView.Example03
{
    class Example03 : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;
        [SerializeField]Sprite[] icons ; // Для Image
        [SerializeField]Sprite[] pages ; // Для Page
        [SerializeField] public string[] namesEN;
        [SerializeField] public string[] namesRU;
        [SerializeField] Sprite descriptions;

        void Start()
        {
            if (YandexGame.savesData.language == "en")
            {
                  var items = Enumerable.Range(0, 29)
                                .Select(i => new ItemData(
                                    namesEN[i % icons.Length], 
                                descriptions,
                                pages[i % pages.Length]))
                                .ToArray();
                  scrollView.UpdateData(items);
                  scrollView.SelectCell(0);
            }else if (YandexGame.savesData.language == "ru")
            {
                var items = Enumerable.Range(0, 29)
                    .Select(i => new ItemData(
                        namesRU[i % icons.Length], 
                        descriptions,
                        pages[i % pages.Length]))
                    .ToArray();
                scrollView.UpdateData(items);
                scrollView.SelectCell(0);
            }
        }
    }
}
