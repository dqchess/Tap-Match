using UnityEngine;
using UnityEngine.UI;

namespace Mkey {
    public class ScoreFlyer : MonoBehaviour
    {
        public Text scoreText;
        public RectTransform rTransform;

        // Use this for initialization
        public void StartFly(string score, Vector3 wPos)
        {
            scoreText.text = score;
            Canvas c = GameObject.Find("CanvasMain").GetComponent<Canvas>();
            rTransform.SetParent(c.transform);
            rTransform.anchoredPosition = Coordinats.WorldToCanvasCenterCenter(wPos, c);
            Vector2 pos = rTransform.anchoredPosition;

            SimpleTween.Value(gameObject, 0f, 500f, 2f).SetOnUpdate((float val) =>
            {
                Vector2 npos = pos + new Vector2(0, val);
                rTransform.anchoredPosition = npos;

            }).SetEase(EaseAnim.EaseOutCubic).AddCompleteCallBack(() =>
            {
                Destroy(gameObject);
            });
        }
    }

}
