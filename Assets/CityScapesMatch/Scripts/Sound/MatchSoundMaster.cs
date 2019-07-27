using UnityEngine;
using System;

namespace Mkey
{
    public class MatchSoundMaster : SoundMaster
    {
        public static new MatchSoundMaster Instance;

        [Space(8, order = 0)]
        [Header("AudioClips", order = 1)]
        [SerializeField]
        private AudioClip winCoins;
        [SerializeField]
        private AudioClip getStar;
        [SerializeField]
        private AudioClip makeBomb;

        #region regular
        protected override void Awake()
        {
            base.Awake();
            Debug.Log("Awake match sm");
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }
        #endregion rgular

        #region play clips
        public void SoundPlayWinCoins(float playDelay, Action callBack)
        {
            PlayClip(playDelay, winCoins, callBack);
        }

        public void SoundPlayGetStar(float playDelay, Action callBack)
        {
            PlayClip(playDelay, getStar, callBack);
        }

        public void SoundPlayMakeBomb(float playDelay, Action callBack)
        {
            PlayClip(playDelay, makeBomb, callBack);
        }
        #endregion play clips

    }
}