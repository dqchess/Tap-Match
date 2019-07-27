using UnityEngine;

namespace Mkey
{
    public class BoosterTimePlus : BoosterFunc
    {
        #region override
        public override void InitStart ()
        {
            Debug.Log("base init start");
        }

        public override bool ActivateApply(Booster b)
        {
            MBoard.WinContr.AddSeconds(30);
            MSound.PlayClip(0.2f, b.bData.privateClip);
            return true;
        }
        #endregion override
    }
}

