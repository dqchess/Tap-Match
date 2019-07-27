
namespace Mkey
{
    public class PauseWindowController : PopUpsController
    {
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        public MatchGUIController MGui { get { return MatchGUIController.Instance; } }

        public void Exit_Click()
        {
            MBoard.Pause();
            CloseWindow();
            MGui.ShowQuit();
        }

        public void Map_Click()
        {
            MBoard.Pause();
            CloseWindow();
            SceneLoader.Instance.LoadScene(0);
        }

        public void Resume_Click()
        {
            MBoard.Pause();
            CloseWindow();
        }
    }
}
