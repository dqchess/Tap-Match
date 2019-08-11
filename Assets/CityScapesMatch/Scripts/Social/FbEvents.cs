using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class FbEvents : MonoBehaviour
    {
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchGUIController MGui { get { return MatchGUIController.Instance; } }
        private MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }
        // private FBholder FB { get { return FBholder.Instance; } }

        void Start()
        {
            // FBholder.LoadTextEvent += SetPlayerName;
          //  FBholder.LogoutEvent += SetDefName;
        }

        void OnDestroy()
        {
            // FBholder.LoadTextEvent -= SetPlayerName;
            // FBholder.LogoutEvent -= SetDefName;
        }

        public void SetPlayerName(bool logined, string firstName, string lastName)
        {
            if (logined)
            {
                string fullName = firstName + lastName;
                if (!string.IsNullOrEmpty(fullName))
                    MPlayer.SetFullName(firstName + " " + lastName);
            }
            else MPlayer.SetDefaultFullName();
        }

        public void SetDefName()
        {
            MPlayer.SetDefaultFullName();
        }
    }
}