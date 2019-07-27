using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class CombinedColorBombAndBomb : CombinedBomb
    {
        private ParallelTween pT;
        private CellsGroup eArea;
        public DynamicClickBombObjectData OData { get; set; }

        #region override
        internal override void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            if (!gCell || explodePrefab == null || OData == null) completeCallBack?.Invoke();
            Debug.Log(name + ": play explode animation");
            TweenSeq anim = new TweenSeq();
            pT = new ParallelTween();

            anim.Add((callBack) => // delay
            {
                delayAction(gameObject, delay, callBack);
            });

            anim.Add((callBack) => // scale out
            {
                SimpleTween.Value(gameObject, 1, 1.5f, 0.2f).SetOnUpdate((float val)=> { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
            });

            anim.Add((callBack) => // scale in explode prefab
            {
                SimpleTween.Value(gameObject, 1.5f, 1.0f, 0.15f).SetOnUpdate((float val) => { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
                GameObject g = Instantiate(explodePrefab);
                g.transform.position = transform.position;
                g.transform.localScale = transform.localScale * .50f;
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

            eArea = GetArea(gCell); // trails
            ParallelTween pT1 = new ParallelTween();
            float incDelay = 0f;
            foreach (var item in eArea.Cells)
            {
                incDelay += 0.0f;
                float t = incDelay;
                pT1.Add((cB) =>
                {
                    delayAction(item.gameObject, t, () =>  // delay tween
                    {
                        Vector2 relativePos = (item.transform.position - gCell.transform.position).normalized;
                        Quaternion rotation = Quaternion.FromToRotation(new Vector2(-1, 0), relativePos); // Debug.Log("Dir: " +(item.transform.position - gCell.transform.position) + " : " + rotation.eulerAngles );
                        GameObject cb = Instantiate(OData.additAnimPrefab, transform.position, rotation);
                        cb.transform.localScale = transform.lossyScale * 1.0f;
                        SimpleTween.Move(cb, cb.transform.position, item.transform.position, 0.2f).AddCompleteCallBack(cB).SetEase(EaseAnim.EaseOutSine);
                    });
                });
            }

            anim.Add((callBack) =>
            {
                pT1.Start(callBack);
            });

            anim.Add((callBack) => // create bombs
            {
                foreach (var item in eArea.Cells)
                {
                    BombDir bd = BombDir.Radial;
                    DynamicClickBombObject r = DynamicClickBombObject.Create(item, GOSet.GetDynamicClickBombObject(bd, 0), false, false, MBoard.TargetCollectEventHandler);
                    r.transform.parent = null;
                    r.SetToFront(true);
                    pT.Add((cB) =>
                    {
                        ExplodeBomb(r, item, 0.5f, cB);
                    });
                }
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

        private void ExplodeBomb(DynamicClickBombObject bomb, GridCell gCell, float delay, Action completeCallBack)
        {
            bomb.PlayExplodeAnimation(gCell, delay, () =>
            {
                bomb.ExplodeArea(gCell, 0, true, true, false, true, completeCallBack);
            });
        }

        public override CellsGroup GetArea(GridCell gCell)
        {
            CellsGroup cG = new CellsGroup();
            if (!gCell) return cG;

            cG.AddRange(MGrid.GetAllByID(OData.matchID).SortByDistanceTo(gCell));
            return cG;
        }
    }
}

