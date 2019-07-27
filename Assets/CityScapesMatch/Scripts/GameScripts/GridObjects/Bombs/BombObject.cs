using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class BombObject : GridObject
    {
        public static void ExplodeArea(IEnumerable<GridCell> area, float delay, bool sequenced, bool showPrefab, bool fly, bool hitProtection, Action completeCallBack)
        {
            ParallelTween pt = new ParallelTween();
            TweenSeq expl = new TweenSeq();
            GameObject temp = new GameObject();
            if (delay > 0)
            {
                expl.Add((callBack) => {
                    SimpleTween.Value(temp, 0, 1, delay).AddCompleteCallBack(callBack);
                });
            }
            float incDelay = 0;
            foreach (GridCell mc in area) //parallel explode all cells
            {
                if (sequenced) incDelay += 0.05f;
                float t = incDelay;
                pt.Add((callBack) => {GridCell.ExplodeCell(mc, t, showPrefab, fly, hitProtection, callBack); });
            }

            expl.Add((callBack) => { pt.Start(callBack); });
            expl.Add((callBack) =>
            {
                Destroy(temp);
                completeCallBack?.Invoke(); callBack();
            });

            expl.Start();
        }

        #region virtual
        internal virtual void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            completeCallBack?.Invoke();
        }

        public virtual CellsGroup GetArea(GridCell gCell)
        {
            return new CellsGroup();
        }

        public virtual void ExplodeArea(GridCell gCell, float delay, bool sequenced,  bool showPrefab, bool fly, bool hitProtection, Action completeCallBack)
        {
            completeCallBack?.Invoke();
        }

        public virtual BombDir GetBombDir()
        {
            return BombDir.Horizontal;
        }
        #endregion virtual
    }
}