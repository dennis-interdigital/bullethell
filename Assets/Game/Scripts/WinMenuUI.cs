using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BulletHell
{
    public class WinMenuUI : BaseUI
    {
        [SerializeField] private Button ContinueButton;
        public override void Init(StageManager stageManager)
        {
            base.Init(stageManager);
            ContinueButton.onClick.AddListener(OnContinue);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        void OnContinue()
        {
            stageManager.ContinueGame();
        }
    }
}

