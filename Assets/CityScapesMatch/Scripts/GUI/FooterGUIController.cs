using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class FooterGUIController : MonoBehaviour
    {
        [SerializeField]
        private FooterBoosterHelper footerBoosterPrefab;
        [SerializeField]
        private RectTransform BoostersParent;
        [SerializeField]
        private GameObject PauseButton;


        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private  MatchGUIController MGui { get { return MatchGUIController.Instance; } }

        public static FooterGUIController Instance;

        #region regular
        void Awake()
        {
            if (Instance) Destroy(Instance.gameObject);
            Instance = this;
        }

        void Start()
        {
            // set booster events
            foreach (var item in MPlayer.BoostHolder.Boosters)
            {
                item.ChangeUseEvent+= ChangeBoosterUseEventHandler;
            }
            CreateBoostersPanel();
            if (MBoard.WinContr !=null && MBoard.WinContr.IsTimeLevel && PauseButton) PauseButton.SetActive(false);
        }

        private void OnDestroy()
        {
            // remove boostar events
            foreach (var item in MPlayer.BoostHolder.Boosters)
            {
                item.ChangeUseEvent -= ChangeBoosterUseEventHandler;
            }
        }
        #endregion regular

        private void CreateBoostersPanel()
        {
            FooterBoosterHelper[] ms = BoostersParent.GetComponentsInChildren<FooterBoosterHelper>();
            foreach (FooterBoosterHelper item in ms)
            {
                 DestroyImmediate(item.gameObject);
            }

            foreach (var item in MPlayer.BoostHolder.Boosters)
            {
               if(item.Use) item.CreateFooterBooster(BoostersParent, footerBoosterPrefab,()=> {  MGui.ShowInGameShopBooster(item.bData.ID.ToString(), item.bData.ObjectImage.name.ToString()); });
            }
        }

        private void ChangeBoosterUseEventHandler(Booster booster)
        {
            if (booster.Use)
            {
               booster.CreateFooterBooster(BoostersParent, footerBoosterPrefab,()=> { MGui.ShowInGameShopBooster(booster.bData.ID.ToString(), booster.bData.ObjectImage.name.ToString()); });
            }
            else
            {
                //destroy not used footer booster
                FooterBoosterHelper[] ms = BoostersParent.GetComponentsInChildren<FooterBoosterHelper>();
                foreach (FooterBoosterHelper item in ms)
                {
                   if(item.booster == booster)  DestroyImmediate(item.gameObject);
                }
            }
        }

        public void SetControlActivity(bool activity)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }

        public void Map_Click()
        {
            if (Time.timeScale == 0) return;
            if (MatchBoard.GMode == GameMode.Play)
            {
                MGui.ShowQuit();
            }
        }

        public void Pause_Click()
        {
            if (MGui) MGui.ShowPause(() => { if (MBoard) MBoard.Pause(); }, null);
        }

    }
}
