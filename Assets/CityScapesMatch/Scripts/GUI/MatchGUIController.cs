using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Mkey
{
    public class MatchGUIController : GuiController
    {
        [Space(8, order = 0)]
        [Header("PopUp windows prefabs", order = 1)]
        public PopUpsController SettingPrefab;
        public PopUpsController RealCoinsShopPrefab;
        public PopUpsController FailedPrefab;
        public PopUpsController WinPrefab;
        public PopUpsController MissionPrefab;
        public PopUpsController QuitPrefab;
        public PopUpsController PausePrefab;
        public WarningMessController TimeLeftPrefab;
        public WarningMessController MovesLeftPrefab;
        public WarningMessController GoodPrefab;
        public WarningMessController ExcellentPrefab;
        public WarningMessController GreatPrefab;
        public WarningMessController AutoVictoryPrefab;
        public PopUpsController InGameShopBooster;
        public PopUpsController RealLifeShop;
        public PopUpsController AlmostPrefab;
        public PopUpsController ProfilePrefab;

        public Button modeButton;

        public static MatchGUIController Instance;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); }
            else
            {
                Instance = this;
                Application.targetFrameRate = 35;
            }
        }

        protected override void Start()
        {
            base.Start();
#if UNITY_EDITOR
            if (modeButton)
            {
                modeButton.gameObject.SetActive(true);
                modeButton.GetComponentInChildren<Text>().text = (MatchBoard.GMode == GameMode.Edit) ? "GoTo" + System.Environment.NewLine + "PLAY" : "GoTo" + System.Environment.NewLine + "EDIT";
                modeButton.onClick.AddListener(() =>
                {
                    if (MatchBoard.GMode == GameMode.Edit)
                    {
                        MatchBoard.GMode = GameMode.Play;
                        modeButton.GetComponentInChildren<Text>().text = "GoTo" + System.Environment.NewLine + "EDIT";
                    }
                    else
                    {
                        MatchBoard.GMode = GameMode.Edit;
                        modeButton.GetComponentInChildren<Text>().text = "GoTo" + System.Environment.NewLine + "PLAY";
                    }
                    SceneLoader.Instance.ReLoadCurrentScene();
                });
            }
#else
             if (modeButton) modeButton.gameObject.SetActive(false); 
#endif
        }

        public void ShowMessageTimeLeft(string caption, string message, float showTime)
        {
            ShowMessage(TimeLeftPrefab, caption, message, showTime, null);
        }

        public void ShowMessagMovesLeft(string caption, string message, float showTime)
        {
            ShowMessage(MovesLeftPrefab, caption, message, showTime, null);
        }

        public void ShowMessageAutoVictory(string caption, string message, float showTime)
        {
            ShowMessage(AutoVictoryPrefab, caption, message, showTime, null);
        }

        public void ShowMessageGood(string caption, string message, float showTime)
        {
            ShowMessage(GoodPrefab, caption, message, showTime, null);
        }

        public void ShowMessageExcellent(string caption, string message, float showTime)
        {
            ShowMessage(ExcellentPrefab, caption, message, showTime, null);
        }

        public void ShowMessageGreat(string caption, string message, float showTime)
        {
            ShowMessage(GreatPrefab, caption, message, showTime, null);
        }

        public void ShowFailed(Action closeCallback)
        {
            ShowPopUp(FailedPrefab, null, closeCallback);
        }

        public void ShowRealCoinsShop()
        {
            ShowPopUp(RealCoinsShopPrefab);
        }

        public void ShowSettings()
        {
            ShowPopUp(SettingPrefab);
        }

        public void ShowMission(Action closeCallback)
        {
            ShowPopUp(MissionPrefab, null, closeCallback);
        }

        public void ShowWin(Action closeCallback)
        {
            ShowPopUp(WinPrefab, null, closeCallback);
        }

        public void ShowQuit()
        {
            ShowPopUp(QuitPrefab);
        }

        public void ShowPause(Action openCallBack, Action closeCallBack)
        {
            ShowPopUp(PausePrefab, openCallBack, closeCallBack);
        }

        public void ShowInGameShopBooster(string itemID)
        {
            Debug.Log(itemID);
            ShopBoosterWindowController.ID = itemID;
            ShowPopUp(InGameShopBooster);
        }

        public void ShowInGameShopBooster(string itemID, string shopCaption)
        {
            Debug.Log(itemID);
            ShopBoosterWindowController.ID = itemID;
            ShopBoosterWindowController.ShopCaption = shopCaption;
            ShowPopUp(InGameShopBooster);
        }

        public void ShowLifeShop()
        {
            ShowPopUp(RealLifeShop);
        }

        public void ShowProfile()
        {
            ShowPopUp(ProfilePrefab);
        }

        public void ShowAlmost( int coins)
        {
            PopUpsController pUP = ShowPopUp(AlmostPrefab);
            AlmostWindowController awc = pUP.GetComponent<AlmostWindowController>();
            awc.SetCoins(coins);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MatchGUIController))]
    public class MatchGUIControllerEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            #region test
            if (EditorApplication.isPlaying)
            {
                MatchGUIController tg = (MatchGUIController)target;
                if (test = EditorGUILayout.Foldout(test, "Test"))
                {
                    #region fill
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Show moves left"))
                    {
                        tg.ShowMessagMovesLeft("Warning","5 moves left", 2);
                    }

                    if (GUILayout.Button("Show win"))
                    {
                        tg.ShowWin(null);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion 

                    #region mission
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Mission"))
                    {
                        tg.ShowMission(null);
                    }

                    if (GUILayout.Button("Show win"))
                    {
                        tg.ShowWin(null);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion 
                }
                return;
            }
            EditorGUILayout.LabelField("Goto play mode for test");
            #endregion test
        }
    }
#endif
}
