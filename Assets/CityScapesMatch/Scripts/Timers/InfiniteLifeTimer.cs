using System;
using UnityEngine;

namespace Mkey
{
    public class InfiniteLifeTimer : MonoBehaviour
    {
        private GlobalTimer gTimer;
        private string lifeInfTimerName = "lifeinfinite";

        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        public static InfiniteLifeTimer Instance;

        #region properties
        public float RestDays { get; private set; }
        public float RestHours { get; private set; }
        public float RestMinutes { get; private set; }
        public float RestSeconds { get; private set; }
        public bool IsWork { get; private set; }
        #endregion properties

        #region regular
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            IsWork = false;
            MPlayer.StartInfiniteLifeEvent +=  StartInfiniteLifeHandler;
            MPlayer.EndInfiniteLifeEvent += EndInfiniteLifeHandler;
            if (MatchPlayer.Instance.HasInfiniteLife())
            {
                StartInfiniteLifeHandler();
            }
        }

        void Update()
        {
            if (IsWork)
                gTimer.Update();
        }

        private void OnDestroy()
        {
            MPlayer.StartInfiniteLifeEvent -= StartInfiniteLifeHandler;
            MPlayer.EndInfiniteLifeEvent -= EndInfiniteLifeHandler;
        }
        #endregion regular

        #region timerhandlers
        private void TickRestDaysHourMinSecHandler(int d, int h, int m, float s)
        {
            RestDays = d;
            RestHours = h;
            RestMinutes = m;
            RestSeconds = s;
        }

        private void TimePassed()
        {
            IsWork = false;
            MPlayer.EndInfiniteLife();
        }
        #endregion timerhandlers

        #region player life handlers
        // start timer
        private void StartInfiniteLifeHandler()
        {
            TimeSpan ts = MatchPlayer.Instance.GetInfLifeTimeRest();
            gTimer = new GlobalTimer(lifeInfTimerName, ts.Days, ts.Hours, ts.Minutes, ts.Seconds, true);
            gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecHandler;
            gTimer.TimePassedEvent += TimePassed;
            IsWork = true;
        }

        private void EndInfiniteLifeHandler()
        {
            IsWork = false;
        }
        #endregion player life handlers
    }
}