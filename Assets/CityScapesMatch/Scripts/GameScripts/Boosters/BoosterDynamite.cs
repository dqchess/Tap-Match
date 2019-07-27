using UnityEngine;
using System;

namespace Mkey
{
    public class BoosterDynamite : BoosterFunc
    {
        [SerializeField]
        private float speed = 20f;
        [SerializeField]
        private GameObject explodePrefab;


        #region override
        public override void InitStart()
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

            //move activeBooster
            float dist = Vector3.Distance(transform.position, gCell.transform.position);
            bTS.Add((callBack) =>
            {
                SetToFront(true);
                SimpleTween.Move(gameObject, transform.position, gCell.transform.position, dist / speed).AddCompleteCallBack(() =>
                {
                    ValuesTween(gameObject, new float[] { 1, 1.3f, 1 }, 0.3f, (val) => { transform.localScale = gCell.transform.lossyScale * val; }, callBack);
                    MSound.PlayClip(0, bData.privateClip);

                }).SetEase(EaseAnim.EaseInSine);
            });

            bTS.Add((callBack) => // explode wave
            {
                MBoard.ExplodeWave(0, transform.position, 5, null);
                callBack();
            });

            bTS.Add((callBack) =>
            {
                GameObject g = Instantiate(explodePrefab);
                g.transform.position = transform.position;
                g.transform.localScale = transform.localScale * 1.0f;
                callBack();
            });

            bTS.Add((callBack) =>
            {
                Destroy(b.SceneObject, 0.1f);
                callBack();
            });

            //apply effect for each cell parallel
            float delay = 0.0f;
            foreach (var c in area.Cells)
            {
                if (!c.IsDisabled && !c.Blocked)
                {
                    float d = delay;
                    float distance = Vector2.Distance(c.transform.position, gCell.transform.position);
                    d = distance / 15f + delay;
                    par0.Add((callBack) =>
                    {
                        delayAction(gameObject, d,
                            () =>
                            {
                                Creator.InstantiateAnimPrefab(bData.animPrefab, c.transform, c.transform.position, SortingOrder.BoosterToFront + 2, true, null);
                                callBack();
                            }
                            );
                    });
                }
            }

            delay = 0.15f;
            foreach (GridCell mc in area.Cells) //parallel explode all cells
            {
                float t = 0;
                float distance = Vector2.Distance(mc.transform.position, gCell.transform.position);
                t = distance / 15f;
                par0.Add((callBack) =>
                {
                    GridCell.ExplodeCell(mc, t, false, false, true, callBack);
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

        public override CellsGroup GetArea(GridCell gCell)
        {
            CellsGroup cG = new CellsGroup();
            cG.AddRange(gCell.GRow.cells);
            return cG;
        }
        #endregion override
    }
}

