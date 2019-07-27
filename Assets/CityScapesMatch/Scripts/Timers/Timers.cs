using System;
using System.Collections;
using UnityEngine;
using System.Globalization;
/*  how to use
  
    private GlobalTimer gTimer;
    private string lifeIncTimerName = "lifeinc";
    [Tooltip ("Time span to life increase")]
    [SerializeField]
    private int  lifeIncTime = 15; // 

    [Tooltip("Calc global time (between games)")]
    [SerializeField]
    private bool calcGlobalTime = true; // 
    private float currMinutes = 0;
    private float currSeconds = 0;

    void Start()
    {
        gTimer = new GlobalTimer(lifeIncTimerName, 0, 0, lifeIncTime, 0, !calcGlobalTime);
        gTimer.OnTickRestDaysHourMinSec += TickRestDaysHourMinSecHandler;
        gTimer.OnTimePassed += TimePassed;
    }

    void OnDestroy()
    {
        gTimer.OnTickRestDaysHourMinSec -= TickRestDaysHourMinSecHandler;
        gTimer.OnTimePassed -= TimePassed;
    }

    void Update()
    {
        gTimer.Update();
    }
 
#region timerhandlers
    private void TickRestDaysHourMinSecHandler(int d, int h, int m, float s)
    {
        currMinutes = m;
        currSeconds = s;
        RefresTimerText();
    }

    private void TimePassed()
    {
        BubblesPlayer.Instance.AddLifes(1);
        gTimer.Restart();
    }
#endregion timerhandlers

    private void RefresTimerText()
    {
        if (timerText) timerText.text = currMinutes.ToString("00") + ":" + currSeconds.ToString("00");
    }

*/

/* changes
  
    13.11.18
    add time span validation
        daySpan = Mathf.Max(0, daySpan);
        hoursSpan = Mathf.Max(0, hoursSpan);
        minutesSpan = Mathf.Max(0, minutesSpan);
        secondsSpan = Mathf.Max(0, secondsSpan);
    21.11.18
    add clamp   restTime = Mathf.Max(0, restTime);
        days = Mathf.Max(0, restTime.Days);
        hours = Mathf.Max(0, restTime.Hours);
        minutes = Mathf.Max(0, restTime.Minutes);
        seconds = Mathf.Max(0, restTime.Seconds + Mathf.RoundToInt(restTime.Milliseconds*0.001f));
    24.01.19  public static DateTime DTFromSring(string dtString)
              CustomProvider provider = new CustomProvider(CultureInfo.InvariantCulture);
    19.02.2019
        SessionTimer - only seconds ctor
        c# 6.0  ?.Invoke() events
	21.03.2019 (fix)
         TickRestDaysHourMinSecEvent?.Invoke(rest_days, rest_hours, rest_minutes, rest_seconds);
     24.04.2019
        - addtimspan to session timer
    25.06.2019
      -  if (!double.TryParse(PlayerPrefs.GetString(initTimeSaveKey), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out initTime))
 */

namespace Mkey
{
    /// <summary>
    /// Game timer, calc only ingame time
    /// </summary>
    public class SessionTimer
    {
        #region events
        public Action<float> TickPassedFullSecondsEvent;
        public Action<float> TickRestFullSecondsEvent;
        public Action<int, int, int, float> TickPassedDaysHourMinSecEvent;
        public Action<int, int, int, float> TickRestDaysHourMinSecEvent;
        public Action TimePassedEvent;
        #endregion events

        #region private variables
        private float lastTime = 0;
        private float passedTime = 0;
        private float passedTimeOld = 0;
        private bool pause = false;
        private bool firstUpdate = true;
        private int days = 0;
        private int hours = 0;
        private int minutes = 0;
        private float seconds = 0;
        private int rest_days = 0;
        private int rest_hours = 0;
        private int rest_minutes = 0;
        private float rest_seconds = 0;
        private float rest;
        private float restTime;
        #endregion private variables

        #region properties
        public bool IsTimePassed
        {
            get { return passedTime >= InitTime; }
        }

        public float InitTime
        {
            get; private set;
        }
        #endregion properties

        public void GetPassedTime(out int days, out int hours, out int minutes, out float seconds)
        {
            days = this.days;
            hours = this.hours;
            minutes = this.minutes;
            seconds = this.seconds;
        }

        public void GetRestTime(out int rest_days, out int rest_hours, out int rest_minutes, out float rest_seconds)
        {
            rest_days = this.rest_days;
            rest_hours = this.rest_hours;
            rest_minutes = this.rest_minutes;
            rest_seconds = this.rest_seconds;
        }

        public SessionTimer(float secondsSpan)
        {
            secondsSpan = Mathf.Max(0, secondsSpan);
            InitTime = secondsSpan;
            pause = true;
        }

        public void Start()
        {
            if (IsTimePassed) return;
            pause = false;
        }

        public void Pause()
        {
            pause = true;
        }

        public void Restart()
        {
            passedTime = 0;
            passedTimeOld = 0;
            pause = false;
            firstUpdate = true;
        }

        /// <summary>
        /// for timer update set Time.time param
        /// </summary>
        /// <param name="time"></param>
        public void Update(float gameTime)
        {
            if (firstUpdate)
            {
                firstUpdate = false;
                lastTime = gameTime;

                // zero tick
                CalcTime();
                TickPassedFullSecondsEvent?.Invoke(passedTimeOld);
                TickRestFullSecondsEvent?.Invoke(InitTime - passedTimeOld);

                TickPassedDaysHourMinSecEvent?.Invoke(days, hours, minutes, seconds);
                TickRestDaysHourMinSecEvent?.Invoke(rest_days, rest_hours, rest_minutes, rest_seconds);
            }

            float dTime = gameTime - lastTime;
            lastTime = gameTime;
            if (pause) return;
            passedTime += dTime;

            // tick events
            if (passedTime - passedTimeOld >= 1.0f)
            {
                passedTimeOld = Mathf.Floor(passedTime);

                CalcTime();

                TickPassedFullSecondsEvent?.Invoke(passedTimeOld);
                TickRestFullSecondsEvent?.Invoke(InitTime - passedTimeOld);

                TickPassedDaysHourMinSecEvent?.Invoke(days, hours, minutes, seconds);
                TickRestDaysHourMinSecEvent?.Invoke(rest_days, rest_hours, rest_minutes, rest_seconds);
            }

            // time passed events
            if (IsTimePassed && !pause)
            {
                pause = true;
                TimePassedEvent?.Invoke();
            }
        }

        private void CalcTime()
        {
            days = (int)(passedTime / 86400f);
            rest = passedTime - days * 86400f;

            hours = (int)(rest / 3600f);
            rest = rest - hours * 3600f;

            minutes = (int)(rest / 60f);
            rest = rest - minutes * 60f;

            seconds = rest;

            restTime = InitTime - passedTime;
            restTime = Mathf.Max(0, restTime);

            rest_days = (int)(restTime / 86400f);
            rest = restTime - rest_days * 86400f;

            rest_hours = (int)(rest / 3600f);
            rest = rest - rest_hours * 3600f;

            rest_minutes = (int)(rest / 60f);
            rest = rest - rest_minutes * 60f;

            rest_seconds = rest;
        }
        
        public void AddTimeSpan(float secondsSpan)
        {
            if (IsTimePassed) return;
            secondsSpan = Mathf.Max(0, secondsSpan);
            InitTime += secondsSpan;
        }
    }

    /// <summary>
    /// Game timer, calc ingame time, sleep game time, time between games
    /// </summary>
    public class GlobalTimer
    {
        private double initTime; // time span in seconds
        private bool cancel = false;
        private string name = "timer_default";

        private DateTime startDT;
        private DateTime lastDT;
        private DateTime endDt;
        private DateTime currentDT;

        private string lastTickSaveKey;
        private string startTickSaveKey;
        private string initTimeSaveKey;

        private int days = 0;
        private int hours = 0;
        private int minutes = 0;
        private float seconds = 0;
        private int rest_days = 0;
        private int rest_hours = 0;
        private int rest_minutes = 0;
        private float rest_seconds = 0;
        private bool startTickCreated = false;
        private bool lastTickCreated = false;

        #region events
        public Action<double> TickPassedSecondsEvent;
        public Action<double> TickRestSecondsEvent;
        public Action<int, int, int, float> TickPassedDaysHourMinSecEvent;
        public Action<int, int, int, float> TickRestDaysHourMinSecEvent;
        public Action TimePassedEvent;
        #endregion events

        public bool IsTimePassed
        {
            get { return cancel; }
        }

        /// <summary>
        /// Returns the elapsed time from the beginning
        /// </summary>
        /// <param name="days"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        public void PassedTime(out int days, out int hours, out int minutes, out float seconds)
        {
            days = this.days;
            hours = this.hours;
            minutes = this.minutes;
            seconds = this.seconds;
        }

        /// <summary>
        /// Returns the remaining time to the end
        /// </summary>
        /// <param name="days"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        public void RestTime(out int rest_days, out int rest_hours, out int rest_minutes, out float rest_seconds)
        {
            rest_days = this.rest_days;
            rest_hours = this.rest_hours;
            rest_minutes = this.rest_minutes;
            rest_seconds = this.rest_seconds;
        }

        private void CalcTime()
        {
            TimeSpan passedTime = (!cancel) ? lastDT - startDT : endDt - startDT;
            days = passedTime.Days;
            hours = passedTime.Hours;
            minutes = passedTime.Minutes;
            seconds = passedTime.Seconds + Mathf.RoundToInt(passedTime.Milliseconds * 0.001f);

            TimeSpan restTime = (!cancel) ? endDt - lastDT : endDt - endDt;
            rest_days = Mathf.Max(0, restTime.Days);
            rest_hours = Mathf.Max(0, restTime.Hours);
            rest_minutes = Mathf.Max(0, restTime.Minutes);
            rest_seconds = Mathf.Max(0, restTime.Seconds + Mathf.RoundToInt(restTime.Milliseconds * 0.001f));
        }

        public double InitTime
        {
            get { return initTime; }
        }

        /// <summary>
        /// Create new timer or continue existing timer
        /// </summary>
        /// <param name="timerName"></param>
        /// <param name="daySpan"> value > 0 </param>
        /// <param name="hoursSpan"> value 0 - 24 </param>
        /// <param name="minutesSpan"> value 0 - 60 </param>
        /// <param name="secondsSpan"> value 0 - 60 </param>
        /// <param name="removeOld">Remove old timer with timerName if exist</param>
        public GlobalTimer(string timerName, float daySpan, float hoursSpan, float minutesSpan, float secondsSpan, bool removeOld)
        {
            name = timerName;
            lastTickSaveKey = name + "_lastTick";
            startTickSaveKey = name + "_startTick";
            initTimeSaveKey = name + "_initTime";

            daySpan = Mathf.Max(daySpan, 0);
            hoursSpan = Mathf.Clamp(hoursSpan, 0, 24);
            minutesSpan = Mathf.Clamp(minutesSpan, 0, 60);
            secondsSpan = Mathf.Clamp(secondsSpan, 0, 60);

            if (!removeOld && PlayerPrefs.HasKey(lastTickSaveKey) && PlayerPrefs.HasKey(initTimeSaveKey) && PlayerPrefs.HasKey(startTickSaveKey))
            {
                startDT = DTFromSring(PlayerPrefs.GetString(startTickSaveKey));
                lastDT = DTFromSring(PlayerPrefs.GetString(lastTickSaveKey));
                initTime = daySpan * 24.0 * 3600.0 + hoursSpan * 3600.0 + minutesSpan * 60.0 + secondsSpan;
                if (initTime == 0)
                    if (!double.TryParse(PlayerPrefs.GetString(initTimeSaveKey), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out initTime))
                    {
                        Debug.Log("try parse error");
                        initTime = 0;
                    }
                endDt = startDT.AddSeconds(initTime);
                startTickCreated = true;
                lastTickCreated = true;
                Debug.Log("-------------------- continue timer: " + name + " ; initTime: "+ initTime + " ------------------------");
            }
            else 
            {
                Remove();
                initTime = daySpan * 24.0 * 3600.0 + hoursSpan * 3600.0 + minutesSpan * 60.0 + secondsSpan;
                PlayerPrefs.SetString(initTimeSaveKey, initTime.ToString());
                Debug.Log("-------------------- new timer: " + name + " ; initTime: " + initTime + " -----------------------");
            }
        }

        /// <summary>
        /// Timer update.
        /// </summary>
        /// <param name="time"></param>
        public void Update()
        {
            if (cancel) return;
            currentDT = DateTime.Now;

            if (!startTickCreated)
            {
                startDT = currentDT;
                PlayerPrefs.SetString(startTickSaveKey, currentDT.ToString(CultureInfo.InvariantCulture));
                startTickCreated = true;
                endDt = startDT.AddSeconds(initTime); // Debug.Log( "startTick: "+startTick);
            }
            if (!lastTickCreated)
            {
                lastDT = currentDT;
                PlayerPrefs.SetString(lastTickSaveKey, currentDT.ToString(CultureInfo.InvariantCulture));
                lastTickCreated = true;//  Debug.Log("lastTick: "+lastTick);
            }

            double dTime = (currentDT - startDT).TotalSeconds;
            double passedSeconds = dTime;

            if (dTime >= initTime && !cancel)
            {
                cancel = true;
                TimePassedEvent?.Invoke();
                passedSeconds = initTime;
            }

            if ((currentDT - lastDT).TotalSeconds >= 1.0 || cancel)
            {
                // Debug.Log("dTime: " + dTime +" current: "+ currentDT.ToString() + " last: " + lastDT.ToString());
                lastDT = currentDT;
                CalcTime();
                PlayerPrefs.SetString(lastTickSaveKey, currentDT.ToString(CultureInfo.InvariantCulture));
                TickPassedSecondsEvent?.Invoke(passedSeconds);
                TickRestSecondsEvent?.Invoke(initTime - passedSeconds);
                TickPassedDaysHourMinSecEvent?.Invoke(days, hours, minutes, seconds);
                TickRestDaysHourMinSecEvent?.Invoke(rest_days, rest_hours, rest_minutes, rest_seconds);
            }
        }

        /// <summary>
        /// Restart new timer cycle
        /// </summary>
        public void Restart()
        {
            startTickCreated = false;
            lastTickCreated = false;
            cancel = false;
            Debug.Log("restart");
        }

        public void Pause()
        {

        }

        public void Start()
        {

        }

        public static DateTime DTFromSring(string dtString)
        {
            if (string.IsNullOrEmpty(dtString)) return DateTime.Now;
            DateTime dateValue = DateTime.Now;

            CustomProvider provider = new CustomProvider(CultureInfo.InvariantCulture);
            {
                try
                {
                    dateValue = Convert.ToDateTime(dtString, provider);
                }
                catch (FormatException)
                {
                    Debug.Log(dtString + "--> Bad Format");
                }
                catch (InvalidCastException)
                {
                    Debug.Log(dtString + "--> Conversion Not Supported");
                }
            }
            return dateValue;
        }

        private double GetTimeSpanSeconds(DateTime dtStart, DateTime dtEnd)
        {
            return (dtEnd - dtStart).TotalSeconds;
        }

        private double GetTimeSpanSeconds(string dtStart, string dtEnd)
        {
            return (DTFromSring(dtEnd) - DTFromSring(dtStart)).TotalSeconds;
        }

        private void Remove()
        {
            if (PlayerPrefs.HasKey(lastTickSaveKey))
            {
                PlayerPrefs.DeleteKey(lastTickSaveKey);
            }
            if (PlayerPrefs.HasKey(initTimeSaveKey))
            {
                PlayerPrefs.DeleteKey(initTimeSaveKey);
            }
            if (PlayerPrefs.HasKey(startTickSaveKey))
            {
                PlayerPrefs.DeleteKey(startTickSaveKey);
            }
        }
    }

    public class CustomProvider : IFormatProvider
    {
        private string cultureName;

        public CustomProvider(string cultureName)
        {
            this.cultureName = cultureName;
        }

        public CustomProvider(CultureInfo cInfo)
        {
            cultureName = cInfo.Name;
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(DateTimeFormatInfo))
            {
                // Console.Write("(CustomProvider retrieved.) ");
                return new CultureInfo(cultureName).GetFormat(formatType);
            }
            else
            {
                return null;
            }
        }

    }
}
    

