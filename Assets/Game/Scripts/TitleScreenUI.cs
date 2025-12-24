using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BulletHell
{
    public class TitleScreenUI : BaseUI
    {
        [SerializeField] private Button StartButton;
        public override void Init(StageManager stageManager)
        {
            base.Init(stageManager);
            StartButton.onClick.AddListener(OnStart);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        void OnStart()
        {
            stageManager.StartGame();
        }
    }
}

