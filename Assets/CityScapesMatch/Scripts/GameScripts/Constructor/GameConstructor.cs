using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mkey
{
    public class GameConstructor : MonoBehaviour
    {
#if UNITY_EDITOR

        private List<RectTransform> openedPanels;

        [SerializeField]
        private Text editModeText;

        #region selected brush
        [Space(8, order = 0)]
        [Header("Grid Brushes", order = 1)]
        [SerializeField]
        private Image mainBrushImage;
        [SerializeField]
        private Image selectedMainBrushImage;
        [SerializeField]
        private PanelContainerController MainBrushContainer;
        private BaseObjectData mainBrush;

        [Space(8)]
        [SerializeField]
        private Image overBrushImage;
        [SerializeField]
        private Image selectedOverBrushImage;
        [SerializeField]
        private PanelContainerController OverBrushContainer;
        private BaseObjectData overBrush;

        [Space(8)]
        [SerializeField]
        private Image underBrushImage;
        [SerializeField]
        private Image selectedUnderBrushImage;
        [SerializeField]
        private PanelContainerController UnderBrushContainer;
        private BaseObjectData underBrush;

        [Space(8)]
        [SerializeField]
        private Image disabledBrushImage;
        [SerializeField]
        private Image selectedDisabledBrushImage;
        private BaseObjectData disabledBrush;

        [Space(8)]
        [SerializeField]
        private Image blockedBrushImage;
        [SerializeField]
        private Image selectedBlockedBrushImage;
        [SerializeField]
        private PanelContainerController BlockedBrushContainer;
        private BaseObjectData blockedBrush;

        [Space(8)]
        [SerializeField]
        private Image fallingBrushImage;
        [SerializeField]
        private Image selectedFallingBrushImage;
        [SerializeField]
        private PanelContainerController FallingBrushContainer;
        private BaseObjectData fallingBrush;

        [Space(8)]
        [SerializeField]
        private Image dynamicBlockerBrushImage;
        [SerializeField]
        private Image selectedDynamicBlockerBrushImage;
        [SerializeField]
        private PanelContainerController DynamicBlockerBrushContainer;
        private BaseObjectData dynamicBlockerBrush;

        [Space(8)]
        [SerializeField]
        private Image staticBlockerBrushImage;
        [SerializeField]
        private Image selectedStaticBlockerBrushImage;
        [SerializeField]
        private PanelContainerController StaticBlockerBrushContainer;
        private BaseObjectData staticBlockerBrush;
        #endregion selected brush

        #region match select
        [SerializeField]
        private PanelContainerController MatchSelectContainer;
        #endregion match select

        #region gift
        //[Space(8, order = 0)]
        //[Header("Gift", order = 1)]
        //[SerializeField]
        //private PanelContainerController GiftPanelContainer;
        [SerializeField]
        private IncDecInputPanel IncDecPanelPrefab;
        #endregion gift

        #region mission
        [Space(8, order = 0)]
        [Header("Mission", order = 1)]
        [SerializeField]
        private PanelContainerController MissionPanelContainer;
        [SerializeField]
        private IncDecInputPanel InputTextPanelMissionPrefab;
        [SerializeField]
        private IncDecInputPanel IncDecTogglePanelMissionPrefab;
        [SerializeField]
        private IncDecInputPanel TogglePanelMissionPrefab;
        #endregion mission

        #region grid construct
        [Space(8, order = 0)]
        [Header("Grid", order = 1)]
        [SerializeField]
        private PanelContainerController GridPanelContainer;
        [SerializeField]
        private IncDecInputPanel IncDecGridPrefab;
        #endregion grid construct

        #region game construct
        [Space(8, order = 0)]
        [Header("Game construct", order = 0)]
        [SerializeField]
        private GameObject levelButtonPrefab;
        [SerializeField]
        private GameObject smallButtonPrefab;
        [SerializeField]
        private GameObject constructPanel;
        [SerializeField]
        private Button openConstructButton;
        [SerializeField]
        private ScrollRect LevelButtonsContainer;
        #endregion game construct

        #region private
        private LevelConstructSet lcSet;
        private MissionConstruct levelMission;
        private Dictionary<int, TargetData> targets;
        private GameConstructSet gcSet;
        private GameObjectsSet goSet;
        #endregion private

        public GridCell selected;
        public int selectedTarget = 0;

        //resource folders
        private string levelConstructSetSubFolder = "LevelConstructSets";

        private int minVertSize = 5;
        private int maxVertSize = 15;
        private int minHorSize = 5;
        private int maxHorSize = 15;

        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }

        public void InitStart(GameConstructSet gcSet)
        {
            if (MatchBoard.GMode == GameMode.Edit)
            {
                if (!MBoard) return;
                if (!MPlayer) return;

                Debug.Log("gc init");
                this.gcSet = gcSet;
                if (!gcSet)
                {
                    Debug.Log("Game construct set not found!!!");
                    return;
                }
                if (!gcSet.GOSet)
                {
                    Debug.Log("GameObjectSet not found!!! - ");
                    return;
                }

                mainBrush = gcSet.GOSet.Disabled;
                mainBrushImage.sprite = mainBrush.ObjectImage;
                SelectMainBrush();

                overBrush = gcSet.GOSet.Disabled;
                overBrushImage.sprite = overBrush.ObjectImage;

                underBrush = gcSet.GOSet.Disabled;
                underBrushImage.sprite = underBrush.ObjectImage;

                disabledBrush = gcSet.GOSet.Disabled;
                disabledBrushImage.sprite = disabledBrush.ObjectImage;

                blockedBrush = gcSet.GOSet.Disabled;
                blockedBrushImage.sprite = blockedBrush.ObjectImage;

                fallingBrush = gcSet.GOSet.Disabled;
                fallingBrushImage.sprite = fallingBrush.ObjectImage;

                if (MatchPlayer.CurrentLevel > gcSet.levelSets.Count-1) MatchPlayer.CurrentLevel = gcSet.levelSets.Count-1;

                if (editModeText) editModeText.text = "EDIT MODE" + '\n' + "Level " + (MatchPlayer.CurrentLevel + 1);
                ShowLevelData(false);

                CreateLevelButtons();
                ShowConstructMenu(true);
            }
        }

        private void ShowLevelData()
        {
            ShowLevelData(true);
        }

        private void ShowLevelData(bool rebuild)
        {
            lcSet = MBoard.LcSet;
            goSet = gcSet.GOSet;
            MBoard.GCSet.Clean();
            lcSet.Clean(goSet);
            Debug.Log("Show level data: " + (MatchPlayer.CurrentLevel));
            if (rebuild) MBoard.CreateGameBoard();

            levelMission = lcSet.levelMission;
            targets = MBoard.Targets;
            foreach (var item in targets)
            {
                item.Value.SetCurrCount(0);
                int iCount = levelMission.Targets.CountByID(item.Key);
                if (iCount > 0)
                    item.Value.SetNeedCount(levelMission.Targets.CountByID(item.Key));
                else
                    item.Value.SetNeedCount(0);
            }

            LevelButtonsRefresh();
            if (editModeText) editModeText.text = "EDIT MODE" + '\n' + "Level " + (MatchPlayer.CurrentLevel + 1);
            if (HeaderGUIController.Instance)
            {
                HeaderGUIController.Instance.RefreshTimeMoves();
                HeaderGUIController.Instance.RefreshScore(levelMission.ScoreTarget);
                HeaderGUIController.Instance.RefreshLevel();
            }
        }

        #region construct menus 
        bool openedConstr = false;

        public void OpenConstructPanel()
        {
            SetConstructControlActivity(false);
            RectTransform rt = constructPanel.GetComponent<RectTransform>();//Debug.Log(rt.offsetMin + " : " + rt.offsetMax);
            float startX = (!openedConstr) ? 0 : 1f;
            float endX = (!openedConstr) ? 1f : 0;
           if(!openedConstr) CreateLevelButtons();

            SimpleTween.Value(constructPanel, startX, endX, 0.2f).SetEase(EaseAnim.EaseInCubic).
                               SetOnUpdate((float val) =>
                               {
                                   rt.transform.localScale = new Vector3(val, 1, 1);
                               // rt.offsetMax = new Vector2(val, rt.offsetMax.y);
                           }).AddCompleteCallBack(() =>
                           {
                               SetConstructControlActivity(true);
                               openedConstr = !openedConstr;
                               LevelButtonsRefresh();
                           });
        }

        private void SetConstructControlActivity(bool activity)
        {
            Button[] buttons = constructPanel.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }

        private void ShowConstructMenu(bool show)
        {
            constructPanel.SetActive(show);
            openConstructButton.gameObject.SetActive(show);
        }

        public void CreateLevelButtons()
        {
            Debug.Log("create level buttons");
            MBoard.GCSet.Clean();

            GameObject parent = LevelButtonsContainer.content.gameObject;
            Button[] existButtons = parent.GetComponentsInChildren<Button>();
            for (int i = 0; i < existButtons.Length; i++)
            {
                DestroyImmediate(existButtons[i].gameObject);
            }

            for (int i = 0; i < MBoard.GCSet.levelSets.Count; i++)
            {
                GameObject buttonGO = Instantiate(levelButtonPrefab, Vector3.zero, Quaternion.identity);
                buttonGO.transform.SetParent(parent.transform);
                buttonGO.transform.localScale = Vector3.one;
                Button b = buttonGO.GetComponent<Button>();
                b.onClick.RemoveAllListeners();
                int level = i + 1;
                b.onClick.AddListener(() =>
                {
                    MatchPlayer.CurrentLevel = level - 1; // = level;
                CloseOpenedPanels();
                    ShowLevelData();
                });
                buttonGO.GetComponentInChildren<Text>().text = "" + level.ToString();
            }
        }

        public void RemoveLevel()
        {
            Debug.Log("Click on Button <Remove level...> ");
            if (MBoard.GCSet.LevelCount < 2)
            {
                Debug.Log("Can't remove the last level> ");
                return;
            }
            MBoard.GCSet.RemoveLevel(MatchPlayer.CurrentLevel);
            CreateLevelButtons();
            MatchPlayer.CurrentLevel = (MatchPlayer.CurrentLevel <= MBoard.GCSet.LevelCount - 1) ? MatchPlayer.CurrentLevel : MatchPlayer.CurrentLevel - 1;
            ShowLevelData();
        }

        public void InsertBefore()
        {
            Debug.Log("Click on Button <Insert level before...> ");
            LevelConstructSet lcs = ScriptableObjectUtility.CreateResourceAsset<LevelConstructSet>(levelConstructSetSubFolder, "", " " + 1.ToString());
            MBoard.GCSet.InsertBeforeLevel(MatchPlayer.CurrentLevel, lcs);
            CreateLevelButtons();
            ShowLevelData();
        }

        public void InsertAfter()
        {
            Debug.Log("Click on Button <Insert level after...> ");
            LevelConstructSet lcs = ScriptableObjectUtility.CreateResourceAsset<LevelConstructSet>(levelConstructSetSubFolder, "", " " + 1.ToString());
            MBoard.GCSet.InsertAfterLevel(MatchPlayer.CurrentLevel, lcs);
            CreateLevelButtons();
            MatchPlayer.CurrentLevel += 1;
            ShowLevelData();
        }

        private void LevelButtonsRefresh()
        {
            Button[] levelButtons = LevelButtonsContainer.content.gameObject.GetComponentsInChildren<Button>();
            for (int i = 0; i < levelButtons.Length; i++)
            {
                SelectButton(levelButtons[i], (i == MatchPlayer.CurrentLevel));
            }
        }

        private void SelectButton(Button b, bool select)
        {
            b.GetComponent<Image>().color = (select) ? new Color(0.5f, 0.5f, 0.5f, 1) : new Color(1, 1, 1, 1);
        }
        #endregion construct menus

        #region grid settings
        private void ShowLevelSettingsMenu(bool show)
        {
            constructPanel.SetActive(show);
            openConstructButton.gameObject.SetActive(show);
        }

        bool openedSettings = false;
        public void OpenSettingsPanel_Click()
        {
            Debug.Log("open grid settings click");
            MatchGrid grid = MBoard.grid;

            ScrollPanelController sRC = GridPanelContainer.ScrollPanel;
            if (sRC) // 
            {
                if (sRC) sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = GridPanelContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Grid panel";

                //create  vert size block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "VertSize", grid.Rows.Count.ToString(),
                    () => { IncVertSize(); },
                    () => { DecVertSize(); },
                    (val) => { },
                    () => { return grid.Rows.Count.ToString(); },
                    null);

                //create hor size block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "HorSize", lcSet.HorSize.ToString(),
                    () => { IncHorSize(); },
                    () => { DecHorSize(); },
                    (val) => { },
                    () => { return lcSet.HorSize.ToString(); },
                    null);

                //create background block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "BackGrounds", lcSet.BackGround.ToString(),
                    () => { IncBackGround(); },
                    () => { DecBackGround(); },
                    (val) => { },
                    () => { return lcSet.BackGround.ToString(); },
                    null);

                //create dist X block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "Dist X", lcSet.DistX.ToString(),
                    () => { IncDistX(); },
                    () => { DecDistX(); },
                    (val) => { },
                    () => { return lcSet.DistX.ToString(); },
                    null);

                //create dist Y block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "Dist Y", lcSet.DistY.ToString(),
                    () => { IncDistY(); },
                    () => { DecDistY(); },
                    (val) => { },
                    () => { return lcSet.DistY.ToString(); },
                    null);

                //create scale block
                IncDecInputPanel.Create(sRC.scrollContent, IncDecGridPrefab, "Scale", lcSet.Scale.ToString(),
                    () => { IncScale(); },
                    () => { DecScale(); },
                    (val) => { },
                    () => { return lcSet.Scale.ToString(); },
                    null);

                sRC.OpenScrollPanel(null);
            }




            //SetSettingsControlActivity(false);

            //RectTransform rt = levelSettingsPanel.GetComponent<RectTransform>();//Debug.Log(rt.offsetMin + " : " + rt.offsetMax);
            //float startX = (!openedSettings) ? 0 : 1f;
            //float endX = (!openedSettings) ? 1f : 0;
            //MatchBoard.GCSet.GetLevelConstructSet(MatchPlayer.CurrentLevel).SelectLevel();
            //SimpleTween.Value(levelSettingsPanel, startX, endX, 0.2f).SetEase(EaseAnim.EaseInCubic).
            //                   SetOnUpdate((float val) =>
            //                   {
            //                       rt.transform.localScale = new Vector3(val, 1, 1);
            //                   }).AddCompleteCallBack(() =>
            //                   {
            //                       SetSettingsControlActivity(true);
            //                       openedSettings = !openedSettings;
            //                       MatchButtonsRefresh();
            //                   });
        }

        public void IncVertSize()
        {
            Debug.Log("Click on Button <VerticalSize...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            int vertSize = lcs.VertSize;
            vertSize = (vertSize < maxVertSize) ? ++vertSize : maxVertSize;
            lcs.VertSize = vertSize;
            ShowLevelData();
        }

        public void DecVertSize()
        {
            LevelConstructSet lcs = MBoard.LcSet;
            int vertSize = lcs.VertSize;
            vertSize = (vertSize > minVertSize) ? --vertSize : minVertSize;
            lcs.VertSize = vertSize;
            ShowLevelData();
        }

        public void IncHorSize()
        {
            Debug.Log("Click on Button <HorizontalSize...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            int horSize = lcs.HorSize;
            horSize = (horSize < maxHorSize) ? ++horSize : maxHorSize;
            lcs.HorSize = horSize;
            ShowLevelData();
        }

        public void DecHorSize()
        {
            Debug.Log("Click on Button <HorizontalSize...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            int horSize = lcs.HorSize;
            horSize = (horSize > minHorSize) ? --horSize : minHorSize;
            lcs.HorSize = horSize;
            ShowLevelData();
        }

        public void IncDistX()
        {
            Debug.Log("Click on Button <DistanceX...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            float dist = lcs.DistX;
            dist += 0.05f;
            lcs.DistX = (dist > 1f) ? 1f : dist;
            ShowLevelData();
        }

        public void DecDistX()
        {
            Debug.Log("Click on Button <DistanceX...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            float dist = lcs.DistX;
            dist -= 0.05f;
            lcs.DistX = (dist > 0f) ? dist : 0f;
            ShowLevelData();
        }

        public void IncDistY()
        {
            Debug.Log("Click on Button <DistanceY...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            float dist = lcs.DistY;
            dist += 0.05f;
            lcs.DistY = (dist > 1f) ? 1f : dist;
            ShowLevelData();
        }

        public void DecDistY()
        {
            Debug.Log("Click on Button <DistanceY...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            float dist = lcs.DistY;
            dist -= 0.05f;
            lcs.DistY = (dist > 0f) ? dist : 0f;
            ShowLevelData();
        }

        public void DecScale()
        {
            Debug.Log("Click on Button <Scale...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            float scale = lcs.Scale;
            scale -= 0.05f;
            lcs.Scale = (scale > 0f) ? scale : 0f;
            ShowLevelData();
        }

        public void IncScale()
        {
            Debug.Log("Click on Button <Scale...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            float scale = lcs.Scale;
            scale += 0.05f;
            lcs.Scale = scale;
            ShowLevelData();
        }

        public void IncBackGround()
        {
            Debug.Log("Click on Button <BackGround...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            lcs.IncBackGround();
            ShowLevelData();
        }

        public void DecBackGround()
        {
            Debug.Log("Click on Button <BackGround...> ");
            LevelConstructSet lcs = MBoard.LcSet;
            lcs.DecBackGround();
            ShowLevelData();
        }
        #endregion grid settings

        #region grid brushes
        public void Cell_Click(GridCell cell)
        {
            Debug.Log("Click on cell <" + cell.ToString() + "...> ");
            LevelConstructSet lcs = MBoard.LcSet;

            if (selectedDisabledBrushImage.enabled)
            {
                Debug.Log("disabled brush enabled");
                if (cell.IsDisabled) lcSet.RemoveDisabledCell(new CellData(disabledBrush.ID, cell.Row, cell.Column));
                else lcSet.AddDisabledCell(new CellData(disabledBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedBlockedBrushImage.enabled)
            {
                Debug.Log("blocked brush enabled");
                if (!GameObjectsSet.IsDisabledObject(blockedBrush.ID)) lcs.AddBlockedCell(new CellData(blockedBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveBlockedCell(new CellData(blockedBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedMainBrushImage.enabled)
            {
                Debug.Log("main brush enabled");
                if (!GameObjectsSet.IsDisabledObject(mainBrush.ID)) lcs.AddFeaturedCell(new CellData(mainBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveFeaturedCell(new CellData(mainBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedFallingBrushImage.enabled)
            {
                Debug.Log("falling brush enabled");
                if (!GameObjectsSet.IsDisabledObject(fallingBrush.ID)) lcs.AddFallingCell(new CellData(fallingBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveFallingCell(new CellData(fallingBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedOverBrushImage.enabled)
            {
                Debug.Log("over brush enabled");
                if (!GameObjectsSet.IsDisabledObject(overBrush.ID)) lcs.AddOverlayCell(new CellData(overBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveOverlayCell(new CellData(overBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedUnderBrushImage.enabled)
            {
                Debug.Log("under brush enabled");
                if (!GameObjectsSet.IsDisabledObject(underBrush.ID)) lcs.AddUnderlayCell(new CellData(underBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveUnderlayCell(new CellData(underBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedStaticBlockerBrushImage.enabled)
            {
                Debug.Log("static blocker brush enabled");
                if (!GameObjectsSet.IsDisabledObject(staticBlockerBrush.ID)) lcs.AddStaticBlockerCell(new CellData(staticBlockerBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveStaticBlockerCell(new CellData(staticBlockerBrush.ID, cell.Row, cell.Column));
            }
            else if (selectedDynamicBlockerBrushImage.enabled)
            {
                Debug.Log("dynamic blocker brush enabled");
                if (!GameObjectsSet.IsDisabledObject(dynamicBlockerBrush.ID)) lcs.AddDynamicBlockerCell(new CellData(dynamicBlockerBrush.ID, cell.Row, cell.Column));
                else lcs.RemoveDynamicBlockerCell(new CellData(dynamicBlockerBrush.ID, cell.Row, cell.Column));
            }

            CloseOpenedPanels();
            ShowLevelData();
        }

        public void OpenBlockedBrushPanel_Click()
        {
            Debug.Log("open blocked brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = BlockedBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = BlockedBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Blocked brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.BlockedObjects != null)
                    foreach (var item in gcSet.GOSet.BlockedObjects)
                    {
                        mData.Add(item);
                    }

                //create main bubbles brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        blockedBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetObject(mD.ID) : gcSet.GOSet.Disabled;
                        blockedBrushImage.sprite = blockedBrush.ObjectImage;
                        SelectBlockedBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        public void OpenFallingBrushPanel_Click()
        {
            Debug.Log("open blocked brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = FallingBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = FallingBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Falling brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.FallingObject != null)
                  //  foreach (var item in gcSet.GOSet.BlockedObjects)
                    {
                        mData.Add(gcSet.GOSet.FallingObject);
                    }

                //create brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        fallingBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetObject(mD.ID) : gcSet.GOSet.Disabled;
                        fallingBrushImage.sprite = fallingBrush.ObjectImage;
                        SelectFallingBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        public void OpenMainBrushPanel_Click()
        {
            Debug.Log("open main brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = MainBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = MainBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Main brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.MatchObjects != null)
                    foreach (var item in gcSet.GOSet.MatchObjects)
                    {
                        mData.Add(item);
                    }

                //create main bubbles brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        mainBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetMainObject(mD.ID) : gcSet.GOSet.Disabled;
                        mainBrushImage.sprite = mainBrush.ObjectImage;
                        SelectMainBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        public void OpenOverBrushPanel_Click()
        {
            Debug.Log("open over brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = OverBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = OverBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Over brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.OverlayObjects != null) mData.AddRange(gcSet.GOSet.OverlayObjects.GetBaseList());

                //create over brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        overBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetOverlayObject(mD.ID) : gcSet.GOSet.Disabled;
                        overBrushImage.sprite = overBrush.ObjectImage;
                        SelectOverBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        public void OpenUnderBrushPanel_Click()
        {
            Debug.Log("open under brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = UnderBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = UnderBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Under brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.UnderlayObjects != null) mData.AddRange(gcSet.GOSet.UnderlayObjects.GetBaseList());

                //create over brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        underBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetUnderlayObject(mD.ID) : gcSet.GOSet.Disabled;
                        underBrushImage.sprite = underBrush.ObjectImage;
                        SelectUnderBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        public void OpenDynamicBlockerBrushPanel_Click()
        {
            Debug.Log("open dynamic brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = DynamicBlockerBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = DynamicBlockerBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Dynamic blocker brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.DynamicBlockerObjects != null) mData.AddRange(gcSet.GOSet.DynamicBlockerObjects.GetBaseList());

                //create dynamic blocker brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        dynamicBlockerBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetDynamicBlockerObject(mD.ID) : gcSet.GOSet.Disabled;
                        dynamicBlockerBrushImage.sprite = dynamicBlockerBrush.ObjectImage;
                        SelectDynamicBlockerBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        public void OpenStaticBlockerBrushPanel_Click()
        {
            Debug.Log("open static brush click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = StaticBlockerBrushContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = StaticBlockerBrushContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Static blocker brush panel";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                mData.Add(gcSet.GOSet.Disabled);
                if (gcSet.GOSet.StaticBlockerObjects != null) mData.AddRange(gcSet.GOSet.StaticBlockerObjects.GetBaseList());

                //create dynamic blocker brushes
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        staticBlockerBrush = (!GameObjectsSet.IsDisabledObject(mD.ID)) ? gcSet.GOSet.GetStaticBlockerObject(mD.ID) : gcSet.GOSet.Disabled;
                        staticBlockerBrushImage.sprite = staticBlockerBrush.ObjectImage;
                        SelectStaticBlockerBrush();
                    });
                }
                sRC.OpenScrollPanel(null);
            }
        }

        private void CloseOpenedPanels()
        {
            ScrollPanelController[] sRCs = GetComponentsInChildren<ScrollPanelController>();
            foreach (var item in sRCs)
            {
                item.CloseScrollPanel(true, null);
            }

        }

        private void SetSpriteControlActivity(RectTransform panel, bool activity)
        {
            Button[] buttons = panel.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }

        public void SelectMainBrush()
        {
            DeselectAllBrushes();
            selectedMainBrushImage.enabled = true;
        }

        public void SelectOverBrush()
        {
            DeselectAllBrushes();
            selectedOverBrushImage.enabled = true;
        }

        public void SelectUnderBrush()
        {
            DeselectAllBrushes();
            selectedUnderBrushImage.enabled = true;
        }

        public void SelectDisabledBrush()
        {
            DeselectAllBrushes();
            selectedDisabledBrushImage.enabled = true;
        }

        public void SelectBlockedBrush()
        {
            DeselectAllBrushes();
            selectedBlockedBrushImage.enabled = true;
        }

        public void SelectFallingBrush()
        {
            DeselectAllBrushes();
            selectedFallingBrushImage.enabled = true;
        }

        public void SelectDynamicBlockerBrush()
        {
            DeselectAllBrushes();
            selectedDynamicBlockerBrushImage.enabled = true;
        }

        public void SelectStaticBlockerBrush()
        {
            DeselectAllBrushes();
            selectedStaticBlockerBrushImage.enabled = true;
        }

        private void DeselectAllBrushes()
        {
            selectedBlockedBrushImage.enabled = false;
            selectedDisabledBrushImage.enabled = false;
            selectedMainBrushImage.enabled = false;
            selectedOverBrushImage.enabled = false;
            selectedUnderBrushImage.enabled = false;
            selectedFallingBrushImage.enabled = false;
            selectedDynamicBlockerBrushImage.enabled = false;
            selectedStaticBlockerBrushImage.enabled = false;
        }
        #endregion grid brushes

        public void OpenMatchSelectPanel_Click()
        {
            Debug.Log("open match selector click");
            MatchGrid grid = MBoard.grid;
            LevelConstructSet lcs = MBoard.LcSet;

            ScrollPanelController sRC = MatchSelectContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = MatchSelectContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Select match objects for level";

                List<BaseObjectData> mData = new List<BaseObjectData>();
                if (gcSet.GOSet.MatchObjects != null)
                    foreach (var item in gcSet.GOSet.MatchObjects)
                    {
                        mData.Add(item);
                    }
                Action <Button, bool> selectButton = (b, s) => { Text t = b.GetComponentInChildren<Text>(true); if (t) { t.enabled = true; t.text = (!s) ? "" : "+"; } };
               
                //create match selectors
                for (int i = 0; i < mData.Count; i++)
                {
                    BaseObjectData mD = mData[i];
                    Button b =  CreateButton(smallButtonPrefab, sRC.scrollContent, mD.ObjectImage, () =>
                    {
                        Debug.Log("Click on Button <" + mD.ID + "...> ");
                        lcSet.AddMatch(mD.ID);
                    });
                    b.onClick.AddListener(() => { selectButton(b, lcSet.ContainMatch(mD.ID)); });
                    selectButton(b, lcSet.ContainMatch(mD.ID));
                }
                sRC.OpenScrollPanel(null);
            }
        }

        #region mission
        public void OpenMissionPanel_Click()
        {
            Debug.Log("open mission click");
            MatchGrid grid = MBoard.grid;

            ScrollPanelController sRC = MissionPanelContainer.ScrollPanel;
            if (sRC) // 
            {
                sRC.CloseScrollPanel(true, null);
            }
            else
            {
                CloseOpenedPanels();
                //instantiate ScrollRectController
                sRC = MissionPanelContainer.InstantiateScrollPanel();
                sRC.textCaption.text = "Mission panel";


                IncDecInputPanel movesPanel = null;

                //create time constrain
                bool useTime = false;
                if(useTime)
                IncDecInputPanel.Create(sRC.scrollContent, IncDecPanelPrefab, "Time", levelMission.TimeConstrain.ToString(),
                () => { levelMission.AddTime(1); HeaderGUIController.Instance.RefreshTimeMoves(); },
                () => { levelMission.AddTime(-1); HeaderGUIController.Instance.RefreshTimeMoves(); },
                (val) => { int res; bool good = int.TryParse(val, out res); if (good) { levelMission.SetTime(res); HeaderGUIController.Instance.RefreshTimeMoves(); } },
                () => { movesPanel?.gameObject.SetActive(!levelMission.IsTimeLevel); return levelMission.TimeConstrain.ToString(); },
                null);

                //create mission moves constrain
                movesPanel = IncDecInputPanel.Create(sRC.scrollContent, IncDecPanelPrefab, "Moves", levelMission.MovesConstrain.ToString(),
                    () => { levelMission.AddMoves(1); HeaderGUIController.Instance.RefreshTimeMoves(); },
                    () => { levelMission.AddMoves(-1); HeaderGUIController.Instance.RefreshTimeMoves(); },
                    (val) => { int res; bool good = int.TryParse(val, out res); if (good) { levelMission.SetMovesCount(res); HeaderGUIController.Instance.RefreshTimeMoves(); } },
                    () => { return levelMission.MovesConstrain.ToString(); },
                    null);
                movesPanel.gameObject.SetActive(!levelMission.IsTimeLevel);

                //description input field
                IncDecInputPanel.Create(sRC.scrollContent, InputTextPanelMissionPrefab, "Description", levelMission.Description,
                null,
                null,
                (val) => { levelMission.SetDescription(val); },
                () => { return levelMission.Description; },
                null);

                //create score target
                bool useScore = false;
                if(useScore)
                IncDecInputPanel.Create(sRC.scrollContent, IncDecPanelPrefab,"Score", levelMission.ScoreTarget.ToString(),
                () => { levelMission.AddScoreTarget(1); HeaderGUIController.Instance.RefreshScore(levelMission.ScoreTarget); },
                () => { levelMission.AddScoreTarget(-1); HeaderGUIController.Instance.RefreshScore(levelMission.ScoreTarget); },
                (val) => { int res; bool good = int.TryParse(val, out res); if (good) { levelMission.SetScoreTargetCount(res); HeaderGUIController.Instance.RefreshScore(levelMission.ScoreTarget); } },
                () => {   return levelMission.ScoreTarget.ToString(); },
                null);

                //create object targets
                foreach (var item in targets)
                {
                    int id = item.Key;
                    IncDecInputPanel.Create(sRC.scrollContent, IncDecPanelPrefab, "Target", levelMission.GetTargetCount(id).ToString(),
                    false,
                    () => { levelMission.AddTarget(id, 1); item.Value?.IncNeedCount(1); },
                    () => { levelMission.RemoveTarget(id, 1); item.Value?.IncNeedCount(-1); },
                    (val) => { int res; bool good = int.TryParse(val, out res); if (good) { levelMission.SetTargetCount(id, res); item.Value?.SetNeedCount(res); } },
                    null,
                    () => { return levelMission.GetTargetCount(id).ToString(); }, // grid.GetObjectsCountByID(id).ToString()); },
                    item.Value.GetImage(goSet));
                }

                sRC.OpenScrollPanel(null);
            }
        }
        #endregion mission

        #region load assets
        void LoadGameConstructAsset(string gameConstructSetSubFolder)
        {
            if (MBoard.GCSet != null)
            {
                return;
            }
            GameConstructSet[] os = LoadResourceAssets<GameConstructSet>(gameConstructSetSubFolder);
            if (os.Length > 0)
            {
                // MatchBoard.GCSet = os[0];
            }
            else
            {
                // MatchBoard.Instance.GCSet = ScriptableObjectUtility.CreateAsset<GameConstructSet>(gameConstructSetSubFolder, ""," "+ 1.ToString());
            }
        }

        List<GameObjectsSet> LoadMatchSetAssets(string matchSetSubFolder)
        {
            List<GameObjectsSet> MatchSets = new List<GameObjectsSet>(LoadResourceAssets<GameObjectsSet>(matchSetSubFolder));
            if (MatchSets == null || MatchSets.Count == 0)
            {
                MatchSets = new List<GameObjectsSet>();
                MatchSets.Add(ScriptableObjectUtility.CreateResourceAsset<GameObjectsSet>(matchSetSubFolder, "", " " + 1.ToString()));
                Debug.Log("New MatchSet created: " + MatchSets[0].ToString());
            }
            return MatchSets;
        }

        List<LevelConstructSet> LoadLevelConstructAssets()
        {
            List<GameObjectsSet> goSets = null;
            List<LevelConstructSet> LevelConstructSets = new List<LevelConstructSet>(LoadResourceAssets<LevelConstructSet>(levelConstructSetSubFolder));
            if (LevelConstructSets == null || LevelConstructSets.Count == 0)
            {
                LevelConstructSets = new List<LevelConstructSet>();
                LevelConstructSets.Add(ScriptableObjectUtility.CreateResourceAsset<LevelConstructSet>(levelConstructSetSubFolder, "", " " + 1.ToString()));
                Debug.Log("New LevelConstructSet created: " + LevelConstructSets[0].ToString());
            }
            // all empty level MatchSets - set to default
            // LevelConstructSets.ForEach((l)=> { if (!l.mSet) l.mSet = goSets[0]; });
            return LevelConstructSets;
        }

        T[] LoadResourceAssets<T>(string subFolder) where T : BaseScriptable
        {
            T[] t = Resources.LoadAll<T>(subFolder);
            if (t != null && t.Length > 0)
            {
                string s = "";
                foreach (var m in t)
                {
                    s += m.ToString() + "; ";
                }
                Debug.Log("Scriptable assets loaded," + typeof(T).ToString() + ", count: " + t.Length + "; sets : " + s);
            }
            else
            {
                Debug.Log("Scriptable assets " + typeof(T).ToString() + " not found!!!");
            }
            return t;
        }

        #endregion load assets

        #region utils
        private void DestroyGoInChildrenWithComponent<T>(Transform parent) where T : Component
        {
            T[] existComp = parent.GetComponentsInChildren<T>();
            for (int i = 0; i < existComp.Length; i++)
            {
                DestroyImmediate(existComp[i].gameObject);
            }
        }

        private Button CreateButton(GameObject prefab, Transform parent, Sprite sprite, System.Action listener)
        {
            GameObject buttonGO = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            buttonGO.transform.SetParent(parent);
            buttonGO.transform.localScale = new Vector3(1, 1, 1);
            Button b = buttonGO.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            b.GetComponent<Image>().sprite = sprite;
            if (listener != null) b.onClick.AddListener(() =>
            {
                listener();
            });

            return b;
        }

        private void SelectButton(Button b)
        {
            Text t = b.GetComponentInChildren<Text>();
            if (!t) return;
            t.enabled = true;
            t.gameObject.SetActive(true);
            t.text = "selected";
            t.color = Color.black;
        }

        private void DeselectButton(Button b)
        {
            Text t = b.GetComponentInChildren<Text>();
            if (!t) return;
            t.enabled = true;
            t.gameObject.SetActive(true);
            t.text = "";
        }
        #endregion utils

#endif
    }

#if UNITY_EDITOR
    public static class ScriptableObjectUtility //http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
    {
        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static T CreateAsset<T>(string subFolder, string namePrefix, string nameSuffics) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            Debug.Log(path);
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/Resources/" + subFolder + "/" + namePrefix + typeof(T).ToString() + nameSuffics + ".asset");
            Debug.Log(assetPathAndName);
            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files in Resource/Subfolder .
        /// </summary>
        public static T CreateResourceAsset<T>(string subFolder, string namePrefix, string nameSuffics) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            string path = "Assets/CityScapesMatch/Resources/";
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + subFolder + "/" + namePrefix + typeof(T).ToString() + nameSuffics + ".asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files in Resource/Subfolder .
        /// </summary>
        public static void DeleteResourceAsset(UnityEngine.Object o)
        {
            string path = AssetDatabase.GetAssetPath(o);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }

    }
#endif
}
/*
 *TODO

 */