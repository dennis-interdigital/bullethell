using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace bullethell
{
    public class LoseMenuUI : BaseUI
    {
        [SerializeField] private Button RetryButton;
        public override void Init(StageManager stageManager)
        {
            base.Init(stageManager);
            RetryButton.onClick.AddListener(OnRetry);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        void OnRetry()
        {
            stageManager.RetryGame();
        }
    }
}

