using UnityEngine;
using UnityEngine.UI;

namespace Mkey {
    public class SettingsWindowController : PopUpsController
    {
        [SerializeField]
        private Toggle easyToggle;
        [SerializeField]
        private Toggle hardToggle;
        [SerializeField]
        private Image musikOff;
        [SerializeField]
        private Image soundOff;

        [SerializeField]
        private Text ButtonText;
        [SerializeField]
        private Text musikOnOff;
        [SerializeField]
        private VolumeSlider volumeSlider;

        [Space(8)]
        [SerializeField]
        private string ANDROID_RATE_URL;
        [SerializeField]
        private string IOS_RATE_URL;

        #region initial
        private float volume;
        private bool musikOn;
        private bool soundOn;
        private HardMode hMode;
        #endregion initial

        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchGUIController MGui { get { return MatchGUIController.Instance; } }
        private MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }
            // private FBholder FB { get { return FBholder.Instance; } } 

        #region regular
        private void Start()
        {
            hMode = MPlayer.Hardmode;
            volume = MSound.Volume;
            musikOn = MSound.MusicOn;
            soundOn = MSound.SoundOn;

           if(easyToggle) easyToggle.onValueChanged.AddListener((value) =>
            {
                if (value) MPlayer.SetHardMode(HardMode.Easy);
                else { MPlayer.SetHardMode(HardMode.Hard); }
            });

            RefreshWindow();
        }

        private void OnDestroy()
        {
        }
        #endregion regular

        public override void RefreshWindow()
        {
            RefreshSound();
            RefreshHardMode();
            base.RefreshWindow();
        }

        public void Save_Click()
        {
            CloseWindow();
        }

        public void Cancel_Click()
        {
            CloseWindow();

            // return initial states
            MPlayer.SetHardMode(hMode);
            MSound.SetVolume(volume);
            MSound.SetMusic(musikOn);
            MSound.SetSound(soundOn);

        }

        private void RefreshSound()
        {
            if (volumeSlider)volumeSlider.SetVolume(MSound.Volume);
            if (musikOff) musikOff.gameObject.SetActive(!MSound.MusicOn);
            if (soundOff) soundOff.gameObject.SetActive(!MSound.SoundOn);
            if (musikOnOff)musikOnOff.text = (!MSound.MusicOn) ? "music off" : "music on";
        }



        public void FacebooLoginHandler(bool logined, string message)
        {
            if (logined) MPlayer.AddFbCoins();
        }

        public void FacebooLogoutHandler()
        {
        }

        private void RefreshHardMode()
        {
            if(hardToggle)   hardToggle.isOn = (MPlayer.Hardmode == HardMode.Hard);
            if(easyToggle)  easyToggle.isOn = (MPlayer.Hardmode != HardMode.Hard);
        }

        public void MusikButtonClick()
        {
            MSound.SetMusic(!MSound.MusicOn);
            RefreshSound();
        }

        public void SoundButtonClick()
        {
            MSound.SetSound(!MSound.SoundOn);
            RefreshSound();
        }

        public void FolumeButton_Click(bool plus)
        {
            float currVolume = MatchSoundMaster.Instance.Volume;
            currVolume = (plus) ? currVolume + 0.1f : currVolume - 0.1f;
            currVolume = Mathf.Clamp(currVolume, 0.0f, 1.0f);
            MSound.SetVolume(currVolume);
            if(volumeSlider)  volumeSlider.SetVolume(currVolume);
        }

        public void RateUs_Click()
        {
#if UNITY_ANDROID
            if (!string.IsNullOrEmpty(ANDROID_RATE_URL)) Application.OpenURL(ANDROID_RATE_URL);
#elif UNITY_IOS
            if (!string.IsNullOrEmpty(IOS_RATE_URL)) Application.OpenURL(IOS_RATE_URL);
#else
            if (!string.IsNullOrEmpty(ANDROID_RATE_URL)) Application.OpenURL(ANDROID_RATE_URL);
#endif
        }

        public void Profile_Click()
        {
            MGui.ShowProfile();
        }

    }
}
