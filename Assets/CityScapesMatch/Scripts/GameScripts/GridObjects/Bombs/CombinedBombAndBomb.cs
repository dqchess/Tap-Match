using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class CombinedBombAndBomb : CombinedBomb
    {
        [SerializeField]
        private int radius = 2;

        #region override
        internal override void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            if (!gCell || explodePrefab == null) completeCallBack?.Invoke();
           // Debug.Log(name + "play explode animation");
            TweenSeq anim = new TweenSeq();
            if (delay > 0)
            {
                anim.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 0, 1, delay).AddCompleteCallBack(callBack);
                });
            }

            anim.Add((callBack) =>
            {
                SimpleTween.Value(gameObject, 1, 1.5f, 0.2f).SetOnUpdate((float val)=> { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
            });

            anim.Add((callBack) => // scale in explode prefab
            {
                SimpleTween.Value(gameObject, 1.5f, 1.0f, 0.15f).SetOnUpdate((float val) => { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
                GameObject g = Instantiate(explodePrefab);
                g.transform.position = transform.position;
                g.transform.localScale = transform.localScale * 1.0f;
            });

            anim.Add((callBack) => // explode wave
            {
                MBoard.ExplodeWave(0, transform.position, 5, null);
                callBack();
            });

            anim.Add((callBack) => // sound
            {
                MSound.PlayClip(0, explodeClip);
                callBack();
            });

            anim.Add((callBack) => // delay
            { 
                delayAction(gameObject, 0, callBack);
            });

            anim.Add((callBack) =>
            {
                completeCallBack?.Invoke();
                callBack();
            });

            anim.Start();
        }

        public override void ApplyToGrid(GridCell gCell, float delay,  Action completeCallBack)
        {
            if (gCell.Blocked || gCell.IsDisabled)
            {
                completeCallBack?.Invoke();
                return;
            }

            PlayExplodeAnimation(gCell, delay, () =>
            {
               ExplodeArea(gCell, 0, true, true, false, true, completeCallBack);
            });
           
        }

        public override void ExplodeArea(GridCell gCell, float delay, bool sequenced, bool showPrefab, bool fly, bool hitProtection, Action completeCallBack)
        {
            Destroy(gameObject);
            ParallelTween pt = new ParallelTween();
            TweenSeq expl = new TweenSeq();
            if (delay > 0)
            {
                expl.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 0, 1, delay).AddCompleteCallBack(callBack);
                });
            }
            foreach (GridCell mc in GetArea(gCell).Cells) //parallel explode all cells
            {
                float t = 0;
                if (sequenced)
                {
                    float distance = Vector2.Distance(mc.transform.position, gCell.transform.position);
                    t = distance / 15f;
                }
                pt.Add((callBack) => {GridCell.ExplodeCell(mc, t, showPrefab, fly, hitProtection, callBack); });
            }

            expl.Add((callBack) => { pt.Start(callBack); });
            expl.Add((callBack) =>
            {
                completeCallBack?.Invoke(); callBack();
            });

            expl.Start();
        }

        public override CellsGroup GetArea(GridCell gCell)
        {
            CellsGroup cG = new CellsGroup();
            List<GridCell> area = MBoard.grid.GetAroundArea(gCell, radius).Cells;
            cG.Add(gCell);
        
            foreach (var item in area)
            {
                 // if(item.IsMatchable)
                    cG.Add(item);
            }

            return cG;
        }
        #endregion override
    }
}

