using UnityEngine;

namespace Mkey
{
    public class BoosterShuffle : BoosterFunc
    {
        #region override
        public override void InitStart ()
        {
            Debug.Log("base init start");
        }

        public override bool ActivateApply(Booster b)
        {
            MBoard.MixGrid(null);
            MSound.PlayClip(0.2f, b.bData.privateClip);
            return true;
        }
        #endregion override
    }
}

