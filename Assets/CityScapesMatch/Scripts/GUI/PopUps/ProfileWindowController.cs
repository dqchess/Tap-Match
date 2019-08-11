using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class ProfileWindowController : PopUpsController
    {
        [SerializeField]
        private Text levelText;

        [SerializeField]
        private Text playerName;

        [SerializeField]
        private InputField inputField;

        [SerializeField]
        private Text coinsText;

        [SerializeField]
        private Text starsText;

        [SerializeField]
        private Image avatarImage;

        [SerializeField]
        private Image[] lifes;

        [SerializeField]
        private Text lifesText;

        [SerializeField]
        private Button changeButton;

        [Space(8)]
        [Header("Boosters")]
        [SerializeField]
        private MissionBoosterHelper missionBoosterPrefab;
        [SerializeField]
        private RectTransform BoostersParent;

        private Sprite defaultAvatar;

        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchGUIController MGui { get { return MatchGUIController.Instance; } }
        // private FBholder FB { get { return FBholder.Instance; } }

        #region regular
        private void Start()
        {
            if (inputField)
            {
                inputField.gameObject.SetActive(false);
                inputField.onEndEdit.AddListener((name) =>
                {
                    MPlayer.SetFullName(name);
                    inputField.gameObject.SetActive(false);
                    if (changeButton) changeButton.gameObject.SetActive(true);
                    if (playerName) playerName.enabled = true;
                    SetControlActivity(true);
                });
                inputField.enabled = false;
            }
            // if (FBholder.IsLogined && FB.playerPhoto)
            // {
            //     avatarImage.sprite = FB.playerPhoto;
            // }

            // set fb events
            // FBholder.LoadPhotoEvent += SetFBPhoto;
            // FBholder.LogoutEvent += RemoveFBPhoto;

            MPlayer.ChangeFullNameEvent += RefreshFullName;
            MPlayer.ChangeLifeEvent += RefreshLife;
            MPlayer.ChangeCoinsEvent += RefreshCoins;
            if (avatarImage) defaultAvatar = avatarImage.sprite;
        }

        private void OnDestroy()
        {
            // FBholder.LoadPhotoEvent -= SetFBPhoto;
            // FBholder.LogoutEvent -= RemoveFBPhoto;
            MPlayer.ChangeFullNameEvent -= RefreshFullName;
            MPlayer.ChangeLifeEvent -= RefreshLife;
            MPlayer.ChangeCoinsEvent -= RefreshCoins;
        }
        #endregion regular

        public override void RefreshWindow()
        {
            RefreshFullName(MPlayer.FullName);
            if (levelText) levelText.text = (MPlayer.TopPassedLevel + 1).ToString();
            CreateBoostersPanel();
            RefreshLife(MPlayer.Life);
            if (starsText) starsText.text = MPlayer.GetAllStars().ToString();
            //  if (inputField) inputField.text = MPlayer.FullName;
            if (playerName) playerName.text = MPlayer.FullName;
			RefreshCoins(MPlayer.Coins);
            base.RefreshWindow();
        }

        private void CreateBoostersPanel()
        {
            MissionBoosterHelper[] ms = BoostersParent.GetComponentsInChildren<MissionBoosterHelper>();
            foreach (MissionBoosterHelper item in ms)
            {
                DestroyImmediate(item.gameObject);
            }
            List<Booster> bList = new List<Booster>();

            foreach (var b in MPlayer.BoostHolder.Boosters)
            {
                if (b.Count > 0) bList.Add(b);
            }

            bList.Shuffle();
            for (int i = 0; i < bList.Count; i++)
            {
                Booster b = bList[i];
                string id = b.bData.ID.ToString();
                MissionBoosterHelper bM = MissionBoosterHelper.CreateProfileBooster(BoostersParent, missionBoosterPrefab, b, () => { MGui.ShowInGameShopBooster(id); });
            }
        }

        public void Change_Click()
        {
            if (!inputField) return;
            if (changeButton) changeButton.gameObject.SetActive(false);
            inputField.gameObject.SetActive(true);
            if (playerName) playerName.enabled = false;
            SetControlActivity(false);

            inputField.enabled = true;
            inputField.interactable = true;
            inputField.ActivateInputField();
            inputField.Select();
            Debug.Log("change : " + inputField);
        }

        #region event handlers
        private void RefreshCoins(int coins)
        {
            if (coinsText) coinsText.text = coins.ToString();
        }

        private void RefreshLife(int life)
        {
            if (lifesText) lifesText.text = life.ToString();
            if (lifes != null)
            {
                for (int i = 0; i < lifes.Length; i++)
                {
                    lifes[i].gameObject.SetActive(i < life);
                }
            }
        }

        private void SetFBPhoto(bool logined, Sprite photo)
        {
            // if (logined && photo && avatarImage) avatarImage.sprite = FB.playerPhoto;
        }

        private void RemoveFBPhoto()
        {
            if (avatarImage) avatarImage.sprite = defaultAvatar;
        }

        private void RefreshFullName(string pName)
        {
            if (playerName) playerName.text = pName;
            if (inputField) inputField.text = MPlayer.FullName;
        }
        #endregion event handlers 
    }
}