using UnityEngine;
using System;
using System.Collections.Generic;

namespace Mkey
{
    public enum GameResult {None, WinAuto, Win, PreLoose, Loose}
    public class WinController
    {
        #region events
        public Action <int> ChangeMovesEvent;
        public Action  MovesLeft5Event;
        public Action <int> ChangeTimeEvent;
        public Action <float> TimerTickEvent;
        public Action  TimerLeft30Event;
        public Action  TimerPassEvent;
        public Action LevelWinEvent;
        public Action AutoWinEvent;
        public Action LevelLooseEvent;
        public Action LevelPreLooseEvent;
        private Action CheckTargetResultEvent;          //for current target
        #endregion events

        #region properties
        public GameResult Result { get; private set; }
        public int TimeLimit { get; private set; } // initial
        public int MovesRest { get; private set; }
        public SessionTimer Timer { get; private set; }
        public bool IsTimeLevel { get; private set; }
        public int ScoreTarget { get; private set; }
        public bool HasScoreTarget { get { return ScoreTarget > 0; } }
        public int TimeRest { get; private set; }  //in seconds
        #endregion properties

        #region private temp
        private int timeCostrain;
        private int movesCostrain;
        private List<GridCell> botDynCells; // for faling object control
        private MatchGrid mGrid;
        private MatchBoard mBoard;
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        #endregion private temp

        public WinController(MissionConstruct mc, MatchBoard gB)
        {
            Result = GameResult.None;

            mGrid = gB.grid;
            mBoard = gB;
            movesCostrain = mc.MovesConstrain;
            MovesRest = movesCostrain;
            ScoreTarget = mc.ScoreTarget;
            botDynCells = mBoard.grid.GetBottomDynCells();

            #region time 
            timeCostrain = mc.TimeConstrain;
            IsTimeLevel = mc.IsTimeLevel;
            if (IsTimeLevel)
            {
                MovesRest = 0;
                Timer = new SessionTimer(timeCostrain);
                Timer.TickRestFullSecondsEvent +=(s)=> { TimerTickEvent?.Invoke(s); };
                Timer.TickRestFullSecondsEvent += (sec) => { TimeRest = (int)sec; if (TimeRest == 30) { TimerLeft30Event?.Invoke(); } };
                Timer.TimePassedEvent += () => { if (Result == GameResult.None) { CheckResult(); } };
            }
            #endregion time 
        }

        /// <summary>
        /// Check swap result
        /// </summary>
        /// <param name="completeCallBack"></param>
        public void CheckResult()
        {
            bool targetAchieved = ObjectTargetsIsCollected() && ScoreTarget <= MPlayer.LevelScore;

            if (!targetAchieved)
            {
                if (IsTimeLevel && Timer.IsTimePassed)
                {
                    Result = GameResult.Loose;
                    LevelLooseEvent?.Invoke();
                }
                else if ((!IsTimeLevel && MovesRest <= 0) && mBoard.NeedAlmostMessage() && (Result != GameResult.PreLoose))
                {
                    Result = GameResult.PreLoose;
                    LevelPreLooseEvent?.Invoke();
                }
                else if ((!IsTimeLevel && MovesRest <= 0) && !mBoard.NeedAlmostMessage())
                {
                    Result = GameResult.Loose;
                    LevelLooseEvent?.Invoke();
                }
                else if ((!IsTimeLevel && MovesRest > 0) && (Result == GameResult.PreLoose))
                {
                    Result = GameResult.None;
                }
            }

            if (targetAchieved && Result != GameResult.Win)
            {
                if (MovesRest == 0)
                {
                    Result = GameResult.Win;
                    LevelWinEvent?.Invoke();
                }
                else
                {
                    if (Result != GameResult.WinAuto)
                    {
                        AutoWinEvent?.Invoke();
                    }
                    Result = GameResult.WinAuto;
                }
                return;
            }
        }

        private bool ObjectTargetsIsCollected()
        {
            foreach (var item in mBoard.Targets)
            {
                if (!item.Value.Achieved)
                    return false;
            }
            return true;
        }

        public void Update(float time)
        {
            if (IsTimeLevel)
            {
                Timer.Update(time);
              //  Debug.Log("wc update");
            }
        }

        public bool HasTimeToMove()
        {
            return(IsTimeLevel)? TimeRest > 0 : true;
        }

        public bool HasMoves()
        {
            return (!IsTimeLevel) ? MovesRest > 0 : true;
        }

        public void MakeMove()
        {
            AddMoves(-1);
        }

        public void MakeMove(int moves)
        {
            AddMoves(-moves);
        }

        public void AddMoves(int count)
        {
            SetMoves(MovesRest + count);
        }

        public void AddSeconds(float seconds)
        {
            if (Timer != null) Timer.AddTimeSpan(seconds);
        }

        public void SetMoves(int count)
        {
            count = Mathf.Max(0, count);
            bool changed = (MovesRest != count);
            MovesRest = count;
            if (changed) ChangeMovesEvent?.Invoke(MovesRest);
            if (MovesRest == 5) MovesLeft5Event?.Invoke();
        }
    }
}
