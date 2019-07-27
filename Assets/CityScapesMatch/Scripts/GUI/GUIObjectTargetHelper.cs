using UnityEngine.UI;
using UnityEngine;

namespace Mkey {
    public class GUIObjectTargetHelper : MonoBehaviour {

        [SerializeField]
        private Image icon;
        [SerializeField]
        private Text countText;
        [SerializeField]
        private Image complete;
        [SerializeField]
        private Image unComplete;

        public void SetData(TargetData tData, bool showCount)
        {
            string collectedString = (tData.CurrCount > tData.NeedCount) ? tData.NeedCount.ToString() : tData.CurrCount.ToString();
            if (countText)
            {
               // countText.text = collectedString + "/" + tData.NeedCount.ToString();
                countText.text = collectedString;
                countText.enabled = showCount;
            }
            if (complete) complete.enabled = (tData.CurrCount >= tData.NeedCount);
            if (unComplete) unComplete.enabled = false;
        }

        public void SetData(TargetData tData, bool showCount, bool showUnComplete)
        {
            SetData(tData, showCount);
            if (unComplete && showUnComplete) unComplete.enabled = (tData.CurrCount < tData.NeedCount);
        }

        public void SetIcon(Sprite sprite)
        {
            if (icon) icon.sprite = sprite;
        }

        public void SetDataInverse(TargetData tData, bool showCount)
        {
            string collectedString = (tData.CurrCount > tData.NeedCount) ? "0" : (tData.NeedCount - tData.CurrCount).ToString();
            if (countText)
            {
                countText.text = collectedString;
                countText.enabled = showCount;
            }
            if (complete) complete.enabled = (tData.CurrCount >= tData.NeedCount);
            if (unComplete) unComplete.enabled = false;
        }
    }
}