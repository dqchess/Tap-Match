using UnityEngine;
using System;

namespace Mkey
{
    public class BoosterMagnet: BoosterFunc
    {
        [SerializeField]
        private float speed = 20f;

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
            ParallelTween par1 = new ParallelTween();
            ParallelTween par2 = new ParallelTween();
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
                delay += 0.1f;
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

                par0.Add((callBack) =>
                {
                    delayAction(gameObject, d,
                        () =>
                        {
                            c.Match.SideHit(c, callBack);
                        }
                        );

                });

                par0.Add((callBack) =>
                {
                    delayAction(gameObject, d,
                        () =>
                        {
                            c.Match.DirectHitOverlay(c, callBack);
                        }
                        );

                });
            }

            //move to magnet
            delay = 0.0f;
            foreach (var c in area.Cells)
            {
                float d = delay;
                par1.Add((callBack) =>
                {
                    SimpleTween.Move(c.Match.gameObject, c.Match.transform.position, gCell.transform.position, 0.2f).AddCompleteCallBack(() =>
                    {
                        callBack();
                    }).SetDelay(d);
                });
                delay += 0.05f;
            }

            // collect
            delay = 0.0f;
            foreach (var c in area.Cells)
            {
                float d = delay;
                par2.Add((callBack) =>
                {
                    c.CollectMatch(d, true, false, false, false, callBack);
                });
                delay += 0.15f;
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
                par1.Start(() =>
                {
                    callback();
                });
            });

            bTS.Add((callback) =>
            {
                par2.Start(() =>
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
            GridCell [] area = hitGridCell.GRow.cells;
            foreach (var item in area)
            {
              if(item.IsMatchable) cG.Add(item);
            }
            area = hitGridCell.GColumn.cells;
            foreach (var item in area)
            {
                if (item.IsMatchable) cG.Add(item);
            }

            return cG;
        }
        #endregion override
    }
}

