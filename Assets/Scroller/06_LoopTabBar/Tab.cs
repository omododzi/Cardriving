/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace FancyScrollView.Example06
{
    class Tab : FancyCell<ItemData, Context>
    {
        [SerializeField] Animator animator = default;
        [SerializeField] Button button = default;

        static class AnimatorHash
        {
            public static readonly int Scroll = Animator.StringToHash("scroll");
        }

        public override void Initialize()
        {
            button.onClick.AddListener(() => Context.OnCellClicked?.Invoke(Index));
        }

        public override void UpdateContent(ItemData itemData)
        {
            //message.text = itemData.Message;
        }

        public override void UpdatePosition(float position)
        {
            currentPosition = position;

            if (animator.isActiveAndEnabled)
            {
                animator.Play(AnimatorHash.Scroll, -1, position);
            }

            animator.speed = 0;
        }

        public void ChangeThis()
        {
            if (currentPosition == 0.5f)
            {
                GameObject window = GameObject.FindWithTag("Windows");
                switch (window.name)
                {
                    case "Window1":
                    {
                        SceneManager.LoadScene("Map1");
                        break;
                    }
                    case "Window2":
                    {
                        SceneManager.LoadScene("Map2");
                        break;
                    }
                    case "Window3":
                    {
                        SceneManager.LoadScene("Map3");
                        break;
                    }
                }
            }
        }

        // GameObject が非アクティブになると Animator がリセットされてしまうため
        // 現在位置を保持しておいて OnEnable のタイミングで現在位置を再設定します
        float currentPosition = 0;

        void OnEnable() => UpdatePosition(currentPosition);
    }
}
