using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class MissionBoosterHelper : MonoBehaviour
    {
        public Button boosterButton;
        public Button shopButton;
        public Text boosterCounter;
        public Image boosterImage;
        public Booster booster;

        #region regular
        public void InitStart()
        {
            booster.ChangeCountEvent += ChangeCountEventHandler;
        }

        private void OnDestroy()
        {
          if(booster!=null)  booster.ChangeCountEvent -= ChangeCountEventHandler;
        }
        #endregion regular

        private void ChangeCountEventHandler()
        {
            if (boosterCounter) boosterCounter.text = booster.Count.ToString();
        }

        public static MissionBoosterHelper CreateProfileBooster(RectTransform parent, MissionBoosterHelper prefab, Booster b, Action GotoShopHandler)
        {
            if(prefab) prefab.boosterImage.sprite = b.bData.GuiImage; // unity 2019 fix

            MissionBoosterHelper missionBooster = Instantiate(prefab);
            missionBooster.transform.localScale = parent.transform.lossyScale;
            missionBooster.transform.SetParent(parent);
            missionBooster.boosterImage.sprite = b.bData.GuiImage;
            missionBooster.boosterCounter.text = b.Count.ToString();

            // add footer click handlers
            missionBooster.boosterButton.onClick.AddListener(GotoShopHandler.Invoke);
            missionBooster.shopButton.onClick.AddListener(GotoShopHandler.Invoke);
            missionBooster.booster = b;
            missionBooster.InitStart();
            return missionBooster;
        }

        public static MissionBoosterHelper CreateMissionBooster(RectTransform parent, MissionBoosterHelper prefab, Booster b, Action GotoShopHandler)
        {
            if (prefab) prefab.boosterImage.sprite = b.bData.GuiImage; // unity 2019 fix

            MissionBoosterHelper missionBooster = Instantiate(prefab);
            missionBooster.transform.localScale = parent.transform.lossyScale;
            missionBooster.transform.SetParent(parent);
            missionBooster.boosterImage.sprite = b.bData.GuiImage;
            missionBooster.boosterCounter.text = b.Count.ToString();

            // add footer click handlers
            missionBooster.boosterButton.onClick.AddListener(() => { if (b.Count > 0) b.ChangeUse(); else GotoShopHandler?.Invoke(); });
            missionBooster.shopButton.onClick.AddListener(GotoShopHandler.Invoke);
            missionBooster.booster = b;
            missionBooster.InitStart();
            return missionBooster;
        }
    }
}