using System.Collections.Generic;
using UnityEngine;
using System;

namespace Mkey
{
    public class BoosterColorBomb: BoosterFunc
    {
        [SerializeField]
        private float speed = 5f;

        #region override
        public override void InitStart ()
        {
            
        }

        public override void ApplyToGrid(GridCell gCell, BoosterObjectData bData, Action completeCallBack)
        {
            if (!gCell.IsMatchable)
            {
                Booster.ActiveBooster.DeActivateBooster();
                completeCallBack?.Invoke();
                return;
            }

            Booster b = Booster.ActiveBooster;
            b.AddCount(-1);

            ParallelTween par0 = new ParallelTween();
            TweenSeq bTS = new TweenSeq();
            CellsGroup area = GetArea(gCell);
           // ScoreCollectHandler?.Invoke(area);

            //move activeBooster
            float dist = Vector3.Distance(transform.position, gCell.transform.position);
            bTS.Add((callBack) =>
            {
                SimpleTween.Move(gameObject, transform.position, gCell.transform.position, dist / speed).AddCompleteCallBack(() =>
                {
                    MSound.PlayClip(0, bData.privateClip);
                    Destroy(b.SceneObject, 0.25f);
                    callBack();
                }).SetEase(EaseAnim.EaseInSine);
            });

            //apply effect for each cell parallel
            float delay = 0.0f;
            foreach (var c in area.Cells)
            {
                delay += 0.15f;
                float d = delay;
                par0.Add((callBack) =>
                {
                    delayAction(gameObject, d,
                        () =>
                        {
                            Creator.InstantiateAnimPrefab(bData.animPrefab, c.transform, c.transform.position, SortingOrder.Booster + 1, true, callBack);
                        }
                        );

                });
            }

            delay = 0.15f;
            foreach (var c in area.Cells)
            {
                delay += 0.15f;
                float d = delay;
                par0.Add((callBack) =>
                {
                    c.CollectMatch(d, true, false, true, true, callBack);
                });
            }

            bTS.Add((callback) =>
            {
                par0.Start(() =>
                {
                    callback();
                });
            });

            bTS.Add((callback) =>
            {
                Booster.ActiveBooster.DeActivateBooster();
                completeCallBack?.Invoke();
                callback();
            });

            bTS.Start();
        }

        public override CellsGroup GetArea(GridCell hitGridCell)
        {
            CellsGroup cG = new CellsGroup();
            List<GridCell> area = new NeighBors(hitGridCell, true).Cells;
            cG.Add(hitGridCell);
            foreach (var item in area)
            {
              if(hitGridCell.IsMatchObjectEquals(item) && item.IsMatchable) cG.Add(item);
            }

            return cG;
        }
        #endregion override
    }
}

