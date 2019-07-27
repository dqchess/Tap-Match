using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public Image LeftStar;
    public Image MiddleStar;
    public Image RightStar;
    public Sprite ActiveButtonSprite;
    public Sprite LockedButtonSprite;
    public Image lockImage;
    public Button button;
    public Text numberText;

    public bool Interactable { get; private set; }

    internal void SetActive(bool active, int activeStarsCount, bool isPassed)
    {
        LeftStar.gameObject.SetActive(activeStarsCount > 0 && isPassed);
        MiddleStar.gameObject.SetActive(activeStarsCount > 1 && isPassed); 
        RightStar.gameObject.SetActive(activeStarsCount > 2 && isPassed);

        Interactable = active || isPassed;
        button.interactable = active || isPassed;


        lockImage.gameObject.SetActive(!isPassed);
        lockImage.sprite = (!active) ? LockedButtonSprite : ActiveButtonSprite;

        if (active)
        {
            MapController.Instance.ActiveButton = this;
        }
    }
}
