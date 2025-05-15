using UnityEngine;

namespace FancyScrollView.Example03
{
    class ItemData
    {
        public string Message { get; }
        public Sprite Image { get; }
        public Sprite Page { get; }

        public ItemData(string message, Sprite image, Sprite page)
        {
            Message = message;
            Image = image;
            Page = page;
        }
    }
}