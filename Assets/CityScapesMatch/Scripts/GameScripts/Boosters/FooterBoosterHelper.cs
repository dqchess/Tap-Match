using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class FooterBoosterHelper : MonoBehaviour
    {
        public Button boosterButton;
        public Button shopButton;
        public Text boosterCounter;
        public RectTransform instantiatePosition;
        public Image boosterImage;
        public Booster booster;
        private Action goToShop;



        #region regular
        public void InitStart()
        {
            if (booster != null)
            {
                booster.ChangeCountEvent += ChangeCountEventHandler;
                booster.FooterClickEvent += FooterClickEventHandler;
                booster.ActivateEvent += ShowActive;

            }
            RefreshFooterGui();
        }

        private void OnDestroy()
        {
            if (gameObject) SimpleTween.Cancel(gameObject, true);
            if (booster != null)
            {
                booster.ChangeCountEvent -= ChangeCountEventHandler;
                booster.FooterClickEvent -= FooterClickEventHandler;
                booster.ActivateEvent -= ShowActive;
            }
        }
        #endregion regular

        /// <summary>
        /// Show active footer booster with another color
        /// </summary>
        /// <param name="active"></param>
        private void ShowActive()
        {
            if (gameObject) SimpleTween.Cancel(gameObject, true);
            if (boosterImage)
            {
                Color c = boosterImage.color;
                boosterImage.color = new Color(1, 1, 1, 1);
                if (booster.IsActive)
                {
                    SimpleTween.Value(gameObject, 1.0f, 0.5f, 0.5f).SetEase(EaseAnim.EaseLinear).
                                SetOnUpdate((float val) =>
                                {
                                    if (booster.IsActive) boosterImage.color = new Color(1, val, val, 1);
                                    else
                                    {
                                        boosterImage.color = new Color(1, 1, 1, 1);
                                        SimpleTween.Cancel(gameObject, true);
                                    }

                                }).SetCycled();
                }
            }
        }

        /// <summary>
        /// Refresh booster count and booster visibilty
        /// </summary>
        private void RefreshFooterGui()
        {
            //Debug.Log("refresh");
            if (booster != null)
            {
                if (boosterCounter) boosterCounter.text = booster.Count.ToString();
                gameObject.SetActive(booster.Use);
            }
        }

        #region handlers
        public void ChangeCountEventHandler()
        {
            RefreshFooterGui();
        }

        public void FooterClickEventHandler()
        {
            ShowActive();
        }
        #endregion handlers
    }
}