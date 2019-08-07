using UnityEngine;
using UnityEngine.UI;
using Mkey;
using System.Collections.Generic;
using System.Collections;

public class MapController : MonoBehaviour {

    [HideInInspector]
    public List<LevelButton> MapLevelButtons { get; private set; }
    [HideInInspector]
    public LevelButton ActiveButton;

    [HideInInspector]
    public Canvas parentCanvas;
    public ScrollRect sRect;
    private RectTransform content;

    public RectTransform avatarGroup;
    public Image avatarImage;

    [Header("If true, then the map will scroll to the Active Level Button", order = 1)]
    [SerializeField]
    private bool scrollToActiveButton = true;

    [SerializeField]
    private int gameSceneOffset = 1;

    private List<Biome> biomes;
    private int biomesCount;
    public static MapController Instance;

    private MatchBoard MBoard { get { return MatchBoard.Instance; } }
    private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
    private MatchGUIController MGui { get { return MatchGUIController.Instance; } }
    private MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }

    #region regular
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        MatchBoard.GMode = GameMode.Play;
    }

    private void Start ()
    {
        Debug.Log("Map started");

        // cache biomes
        biomes = new List<Biome>( GetComponentsInChildren<Biome>());
        if (biomes == null)
        {
            Debug.LogError("No  biomes found.");
            return;
        }

        biomes.RemoveAll((b) => { return b == null; });
        biomes.Reverse();
        biomesCount = biomes.Count;

        content = GetComponent<RectTransform>();
        if (!content)
        {
            Debug.LogError("No RectTransform component. Use RectTransform for MapMaker.");
            return;
        }

        // cache level buttons
        MapLevelButtons = new List<LevelButton>();
        foreach (var b in biomes)
        {
            MapLevelButtons.AddRange(b.levelButtons);
        }

        int topPassedLevel = MPlayer.TopPassedLevel;

        // set onclick listeners for level buttons
        for (int i = 0; i < MapLevelButtons.Count; i++)
        {
            //1 add listeners
            int buttonNumber = i + 1;
            int currLev = i;

            MapLevelButtons[i].button.onClick.AddListener(()=> 
            {
                MSound.SoundPlayClick(0, null);
                MatchPlayer.CurrentLevel = currLev;
                if (MPlayer.Life <= 0) { MGui.ShowMessage("Sorry!", "You have no lifes.", 1.5f, () => { MGui.ShowLifeShop(); }); return; }
                Debug.Log("load scene : " + gameSceneOffset + " ;CurrentLevel: " + MatchPlayer.CurrentLevel);
                MatchBoard.showMission = true;
                if (SceneLoader.Instance) SceneLoader.Instance.LoadScene(gameSceneOffset, () => { });
            });
            // activate buttons
            SetButtonActive(buttonNumber, buttonNumber == topPassedLevel + 2, topPassedLevel + 1 >= buttonNumber);
            MapLevelButtons[i].numberText.text = (buttonNumber).ToString();

            foreach (var item in MPlayer.BoostHolder.Boosters)
            {
                if (item.Use) item.ChangeUse();
            }
        }


        // set fb events
        FBholder.LoadPhotoEvent += SetFBPhoto;
        FBholder.LogoutEvent += RemoveFBPhoto;

        parentCanvas = GetComponentInParent<Canvas>();
        sRect = GetComponentInParent<ScrollRect>();
        if (scrollToActiveButton) StartCoroutine(SetMapPositionToAciveButton());
		
		  //update photo
//        SetFBPhoto(FBholder.IsLogined, FBholder.Instance.playerPhoto);
    }

    private void OnDestroy()
    {
        FBholder.LoadPhotoEvent -= SetFBPhoto;
        FBholder.LogoutEvent -= RemoveFBPhoto;
    }
    #endregion regular

    private IEnumerator SetMapPositionToAciveButton()
    {
        yield return new WaitForSeconds(0.1f);
        if (sRect)
        {
            int bCount = biomesCount;
            float contentSizeY = content.sizeDelta.y / (bCount) * (bCount - 1.0f);
            float relPos = content.InverseTransformPoint(ActiveButton.transform.position).y; // Debug.Log("contentY : " + contentSizeY +  " ;relpos : " + relPos + " : " + relPos / contentSizeY);
            float vpos = (-contentSizeY / (bCount * 2.0f) + relPos) / contentSizeY; // 

            SimpleTween.Cancel(gameObject, false);
            float start = sRect.verticalNormalizedPosition;

            SimpleTween.Value(gameObject,start, vpos, 0.25f).SetOnUpdate((float f)=> { sRect.verticalNormalizedPosition = Mathf.Clamp01(f); });
            //sRect.verticalNormalizedPosition = Mathf.Clamp01(vpos); // Debug.Log("vpos : " + Mathf.Clamp01(vpos));
        }
        else
        {
            Debug.Log("no scrolling rect");
        }
    }

    private void SetButtonActive(int sceneNumber, bool active, bool isPassed)
    {
        int activeStarsCount = MPlayer.GetLevelStars(sceneNumber-1);
        MapLevelButtons[sceneNumber - 1].SetActive(active, activeStarsCount, isPassed );
    }

    public void SetControlActivity(bool activity)
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = activity;
        }
    }

    private void SetFBPhoto(bool logined, Sprite photo)
    {
        if (logined && photo && avatarImage) avatarImage.sprite = FBholder.Instance.playerPhoto;
    }

    private void RemoveFBPhoto()
    {
      //  if(avatarImage) avatarImage.sprite = FBholder.Instance.playerPhoto;
    }
}
