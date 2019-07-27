using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class CombinedBomb : MonoBehaviour
    {
        protected MatchBoard MBoard { get { return MatchBoard.Instance; } }
        protected MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        protected MatchGUIController MGui { get { return MatchGUIController.Instance; } }
        protected MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }
        protected GameObjectsSet GOSet { get { return MBoard.GOSet; } }
        protected MatchGrid MGrid { get { return MBoard.grid; } }

        protected Action<GameObject, float, Action> delayAction = (g, del, callBack) => { SimpleTween.Value(g, 0, 1, del).AddCompleteCallBack(callBack); };

        [SerializeField]
        protected AudioClip explodeClip;
        [SerializeField]
        protected GameObject explodePrefab;

        #region virtual
        internal virtual void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            completeCallBack?.Invoke();
        }

        public virtual void ApplyToGrid(GridCell gCell, float delay,  Action completeCallBack)
        {
            completeCallBack?.Invoke();

        }

        public virtual void ExplodeArea(GridCell gCell, float delay, bool sequenced, bool showPrefab, bool fly, bool hitProtection, Action completeCallBack)
        {
            completeCallBack?.Invoke();
        }

        public virtual CellsGroup GetArea(GridCell gCell)
        {
            CellsGroup cG = new CellsGroup();
            return cG;
        }
        #endregion virtual
    }
}

