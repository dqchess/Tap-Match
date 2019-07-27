using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class BoosterHammer: BoosterFunc
    {
        [SerializeField]
        private float speed = 20f;

        #region override
        public override void InitStart ()
        {
            
        }

        public override void ApplyToGrid(GridCell gCell, BoosterObjectData bData, Action completeCallBack)
        {
            if (!gCell.Overlay)
            {
                if (!gCell.Match &&  !gCell.StaticBlocker && !gCell.DynamicBlocker)
                {
                    Booster.ActiveBooster.DeActivateBooster();
                    completeCallBack?.Invoke();
                    return;
                }
            }

            Booster b = Booster.ActiveBooster;
            b.AddCount(-1);

            ParallelTween par0 = new ParallelTween();
            TweenSeq bTS = new TweenSeq();

            //move activeBooster
            Vector3 pos = transform.position;
            float dist = Vector3.Distance(transform.position, gCell.transform.position);
            Vector3 rotPivot = Vector3.zero;
            float rotRad = 6f;
            bTS.Add((callBack) =>
            {
                SetToFront(true);
                SimpleTween.Move(b.SceneObject, b.SceneObject.transform.position, gCell.transform.position, dist / speed).AddCompleteCallBack(() =>
                {
                    rotPivot = transform.position - new Vector3(0, rotRad, 0);
                    callBack();
                }).SetEase(EaseAnim.EaseInSine);
            });


            // back move
            bTS.Add((callBack) =>
            {
                SimpleTween.Value(gameObject, Mathf.Deg2Rad * 90f, Mathf.Deg2Rad * 180f, 0.25f).SetEase(EaseAnim.EaseInCubic). //
                 SetOnUpdate((float val) => { transform.position = new Vector3(rotRad * Mathf.Cos(val), rotRad * Mathf.Sin(val), 0) + rotPivot; }).
                 AddCompleteCallBack(() => { callBack(); });
            });
            //forward move
            bTS.Add((callBack) =>
            {

                SimpleTween.Value(gameObject, Mathf.Deg2Rad * 180f, Mathf.Deg2Rad * 100f, 0.2f).SetEase(EaseAnim.EaseOutBounce).
                    SetOnUpdate((float val) =>
                    {
                        transform.position = new Vector3(rotRad * Mathf.Cos(val), rotRad * Mathf.Sin(val), 0) + rotPivot;
                    }).
                    AddCompleteCallBack(() =>
                    {
                        MSound.PlayClip(0, bData.privateClip);
                        Destroy(gameObject, 0.25f);
                        Creator.InstantiateAnimPrefab(bData.animPrefab, gCell.transform, gCell.transform.position, SortingOrder.BoosterToFront + 2, true, callBack);
                    });

            });

          //  if (gCell.IsMatchable)
            {
                bTS.Add((callBack) =>
                {
                    GridCell.ExplodeCell(gCell, 0, true, false, true,  callBack);
                   // gCell.CollectMatch(0, true, false, true, true, callBack);
                });
            }


            bTS.Add((callback) =>
            {
                Booster.ActiveBooster.DeActivateBooster();
                completeCallBack?.Invoke();
                callback();
            });

            bTS.Start();
        }
        #endregion override
    }
}


