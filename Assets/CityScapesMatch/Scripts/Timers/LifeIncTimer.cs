using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Mkey
{
    public class LifeIncTimer : MonoBehaviour
    {
        private GlobalTimer gTimer;
        private string lifeIncTimerName = "lifeinc";
        [Tooltip("Time span to life increase, minutes")]
        [SerializeField]
        private int lifeIncTime = 20; 
        [Tooltip("Increase lives if count less than value")]
        [SerializeField]
        private uint incIfLessThan = 3;
        [Tooltip("Calc global time (between games)")]
        [SerializeField]
        private bool calcGlobalTime = true;  

        [SerializeField]
        private bool debugTimer = false;

        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        public static LifeIncTimer Instance;

        #region properties
        public bool IsWork { get; private set; }
        public float RestMinutes { get; private set; }
        public float RestSeconds { get; private set; }
        public float RestDays { get; private set; }
        public float RestHours { get; private set; }
        #endregion properties

        #region events
        public Action<int, int, int, float> TickRestDaysHourMinSecEvent;
        public UnityEvent TimePassedEvent;
        #endregion events

        #region regular
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            // set life handlers
            MPlayer.StartInfiniteLifeEvent += StartInfiniteLifeHandler;
            MPlayer.EndInfiniteLifeEvent += EndInfiniteLifeHandler;
            MPlayer.ChangeLifeEvent += ChangeLifeHandler;

            if (!MPlayer.HasInfiniteLife() && (MPlayer?.Life < incIfLessThan) && !IsWork)
            {
                CreateNewTimerAndStart();
            }
        }

        void OnDestroy()
        {
            MPlayer.StartInfiniteLifeEvent -= StartInfiniteLifeHandler;
            MPlayer.EndInfiniteLifeEvent -= EndInfiniteLifeHandler;
            MPlayer.ChangeLifeEvent -= ChangeLifeHandler;
        }

        void Update()
        {
            if (IsWork)
                gTimer.Update();
        }
        #endregion regular

        #region timerhandlers
        private void TickRestDaysHourMinSecHandler(int d, int h, int m, float s)
        {
            RestDays = d;
            RestHours = h;
            RestMinutes = m;
            RestSeconds = s;
            TickRestDaysHourMinSecEvent?.Invoke(d, h, m, s);
        }

        private void TimePassed()
        {
            if (MPlayer?.Life < incIfLessThan)
            {
                MPlayer.AddLifes(1);
            }
            TimePassedEvent?.Invoke();
            gTimer.Restart();
        }
        #endregion timerhandlers

        #region player life handlers
        private void ChangeLifeHandler(int count)
        {
          if(debugTimer) Debug.Log("change life by timer");
            if (count >= incIfLessThan && IsWork)
            {
                RestDays = 0;
                RestHours = 0;
                RestMinutes = 0;
                RestSeconds = 0;
                IsWork = false;
                if (debugTimer) Debug.Log("timer stop");
            }
            else if (count < incIfLessThan && !IsWork)
            {
                CreateNewTimerAndStart();
            }
        }

        private void StartInfiniteLifeHandler()
        {
            RestDays = 0;
            RestHours = 0;
            RestMinutes = 0;
            RestSeconds = 0;
            IsWork = false;
        }

        private void EndInfiniteLifeHandler()
        {
            if (!MPlayer.HasInfiniteLife() && (MPlayer?.Life < incIfLessThan) && !IsWork)
            {
                CreateNewTimerAndStart();
            }
        }
        #endregion player life handlers

        private void CreateNewTimerAndStart()
        {
            gTimer = new GlobalTimer(lifeIncTimerName, 0, 0, lifeIncTime, 0, true);
            gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecHandler;
            gTimer.TimePassedEvent += TimePassed;
            IsWork = true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LifeIncTimer))]
    public class LiveTimerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (EditorApplication.isPlaying)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Set 0 live"))
                {
                     MatchPlayer.Instance?.SetLifesCount(0);
                }
                if (GUILayout.Button("Inc life"))
                {
                    MatchPlayer.Instance?.AddLifes(1);
                }
                if (GUILayout.Button("Dec life"))
                {
                    MatchPlayer.Instance?.AddLifes(-1);
                }
            }
            else
            {
                GUILayout.Label("Goto play mode for test");
            }
        }
    }
#endif
}