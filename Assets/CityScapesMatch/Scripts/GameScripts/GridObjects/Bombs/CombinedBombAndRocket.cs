using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class CombinedBombAndRocket : CombinedBomb
    {
        public List<DynamicClickBombObject> rockets;
        private ParallelTween pT;

        #region override
        internal override void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            if (!gCell || explodePrefab == null) completeCallBack?.Invoke();
           // Debug.Log(name + "play explode animation");
            TweenSeq anim = new TweenSeq();
            pT = new ParallelTween();
            rockets = new List<DynamicClickBombObject>();

            anim.Add((callBack) => // delay
            {
                delayAction(gameObject, delay, callBack);
            });

            anim.Add((callBack) => // scale out
            {
                SimpleTween.Value(gameObject, 1, 1.5f, 0.2f).SetOnUpdate((float val)=> { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
            });

            anim.Add((callBack) => // scale in and explode prefab
            {
                SimpleTween.Value(gameObject, 1.5f, 1.0f, 0.15f).SetOnUpdate((float val) => { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
                GameObject g = Instantiate(explodePrefab);
                g.transform.position = transform.position;
                g.transform.localScale = transform.localScale * .50f;
            });

            anim.Add((callBack) => // create rockets
            {
                NeighBors nB = gCell.Neighbors;
                if (nB.Left)
                {
                    DynamicClickBombObject rL =   DynamicClickBombObject.Create(nB.Left, GOSet.GetDynamicClickBombObject(BombDir.Vertical, 0), false, false, MBoard.TargetCollectEventHandler);
                    rL.transform.parent = null;
                    rL.SetToFront(true);
                    GridCell c1 = nB.Left;
                    pT.Add((cB) =>
                    {
                        ExplodeRocket(rL, c1, 0, cB);
                    });
                    rockets.Add(rL);
                }
                if (nB.Right)
                {
                    DynamicClickBombObject rR = DynamicClickBombObject.Create(nB.Right, GOSet.GetDynamicClickBombObject(BombDir.Vertical, 0), false, false, MBoard.TargetCollectEventHandler);
                    rR.transform.parent = null;
                    rR.SetToFront(true);
                    GridCell c2 = nB.Right;
                    pT.Add((cB) =>
                    {
                        ExplodeRocket(rR, c2, 0, cB);
                    });
                    rockets.Add(rR);
                }
                if (nB.Top)
                {
                    DynamicClickBombObject rT = DynamicClickBombObject.Create(nB.Top, GOSet.GetDynamicClickBombObject(BombDir.Horizontal, 0), false, false, MBoard.TargetCollectEventHandler);
                    rT.transform.parent = null;
                    rT.SetToFront(true);
                    GridCell c3 = nB.Top;
                    pT.Add((cB) =>
                    {
                        ExplodeRocket(rT, c3, 0, cB);
                    });
                    rockets.Add(rT);
                }
                if (nB.Bottom)
                {
                    DynamicClickBombObject rB = DynamicClickBombObject.Create(nB.Bottom, GOSet.GetDynamicClickBombObject(BombDir.Horizontal, 0), false, false, MBoard.TargetCollectEventHandler);
                    rB.transform.parent = null;
                    rB.SetToFront(true);
                    GridCell c4 = nB.Bottom;
                    pT.Add((cB) =>
                    {
                        ExplodeRocket(rB, c4, 0, cB);
                    });
                    rockets.Add(rB);
                }

                DynamicClickBombObject r1 = DynamicClickBombObject.Create(gCell, GOSet.GetDynamicClickBombObject(BombDir.Horizontal, 0), false, false, MBoard.TargetCollectEventHandler);
                r1.transform.parent = null;
                r1.SetToFront(true);
                pT.Add((cB) =>
                {
                    ExplodeRocket(r1, gCell, 0, cB);
                });
                rockets.Add(r1);
                DynamicClickBombObject r2 = DynamicClickBombObject.Create(gCell, GOSet.GetDynamicClickBombObject(BombDir.Vertical, 0), false, false, MBoard.TargetCollectEventHandler);
                r2.transform.parent = null;
                r2.SetToFront(true);
                pT.Add((cB) =>
                {
                    ExplodeRocket(r2, gCell, 0, cB);
                });
                rockets.Add(r2);

                callBack();
            });

            anim.Add((callBack) => // explode wave
            {
                MBoard.ExplodeWave(0, transform.position, 5, null);
                callBack();
            });

            anim.Add((callBack) => // explode sound
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
                Destroy(gameObject);
                pT.Start(completeCallBack);
            });
           
        }
        #endregion override

        private void ExplodeRocket(DynamicClickBombObject bomb, GridCell gCell, float delay, Action completeCallBack)
        {
            bomb.PlayExplodeAnimation(gCell, delay, () =>
            {
                bomb.ExplodeArea(gCell, 0, true, true, false, true, completeCallBack);
            });
        }
       
    }
}

