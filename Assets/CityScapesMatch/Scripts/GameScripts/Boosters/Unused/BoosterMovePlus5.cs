using UnityEngine;

namespace Mkey
{
    public class BoosterMovePlus5 : BoosterFunc
    {
        #region override
        public override void InitStart ()
        {
            Debug.Log("base init start");
        }

        public override bool ActivateApply(Booster b)
        {
            MBoard.WinContr.AddMoves(5);
            MSound.PlayClip(0.2f, b.bData.privateClip);
            return true;
        }
        #endregion override
    }
}

