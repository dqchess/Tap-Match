using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class CombinedRocketAndRocket : CombinedBomb
    {

        public List<DynamicClickBombObject> rockets;
        private ParallelTween pT;

        #region override
        internal override void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            if (!gCell || explodePrefab == null) completeCallBack?.Invoke();
          //  Debug.Log(name + "play explode animation");
            TweenSeq anim = new TweenSeq();
            pT = new ParallelTween();
            rockets = new List<DynamicClickBombObject>();
 
            anim.Add((callBack) => // delay
            {
                delayAction(gameObject, delay, callBack);
            });

            anim.Add((callBack) =>
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

            anim.Add((callBack) =>
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

