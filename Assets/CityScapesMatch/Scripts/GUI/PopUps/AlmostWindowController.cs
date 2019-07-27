using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class AlmostWindowController : PopUpsController
    {
        [SerializeField]
        private Text coinsText;
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }

        private int  Coins{ get; set; }

        public override void RefreshWindow()
        {
            base.RefreshWindow();
        }

        public void SetCoins(int coins)
        {
            Coins = coins;
            if (coinsText) coinsText.text = Coins.ToString();
        }

        public void Close_Click()
        {
            CloseWindow();
            MBoard.showAlmostMessage = false;
            MBoard.WinContr.CheckResult();
        }

        public void Play_Click()
        {
            CloseWindow();
            MPlayer.AddCoins(-Coins);
            MBoard.WinContr.AddMoves(5);
        }
    }
}
