using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.CM.Common.CmCallContext;

namespace bullethell
{
    public enum UIState
    {
        TitleMenu,
        MainMenu,
        GameMenu,
        ShopMenu,
        InventoryMenu
    }


    [Serializable]
    public class UIClass
    {
        public UIState state;
        public BaseUI baseUI;
    }

    public class UIManager : MonoBehaviour
    {
        private StageManager stageManager;
        [SerializeField] private List<UIClass> baseUIList; 
        public UIState currentUI;
        public BaseUI currentActiveUI;

        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            foreach (var ui in baseUIList)
            {
                ui.baseUI.Init(stageManager);
            }
        }

        public void DoUpdate(float dt)
        {
            if (currentActiveUI != null)
            {
                currentActiveUI.DoUpdate(dt);
            }
        }

        public void ShowUI(UIState state)
        {
            UIClass toActive = null;
            int count = baseUIList.Count;
            foreach (UIClass ui in baseUIList)
            {
                ui.baseUI.Hide();

                if (ui.state == state)
                {
                    toActive = ui;
                }
            }

            toActive.baseUI.Show();
            currentActiveUI = toActive.baseUI;
            currentUI = state;
        }


    }
}
