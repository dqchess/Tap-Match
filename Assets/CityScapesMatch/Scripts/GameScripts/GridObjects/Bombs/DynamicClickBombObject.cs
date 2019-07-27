using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class DynamicClickBombObject : BombObject
    {
        #region properties
        public DynamicClickBombObjectData OData { get; private set; }
        #endregion properties

        #region events
        private Action<int> TargetCollectEvent;
        #endregion events

        #region create
        internal virtual void SetData(DynamicClickBombObjectData oData)
        {
            SRenderer = GetComponent<SpriteRenderer>();
            if (SRenderer) SRenderer.sprite = (oData != null) ? oData.ObjectImage : null;
            OData = oData;
#if UNITY_EDITOR
            gameObject.name = (oData != null && SRenderer.sprite) ? "DynamicClickBomb: " + GetID() + "(" + SRenderer.sprite.name + ")" : "none";
#endif
            SetToFront(false);
        }

        /// <summary>
        /// Create new MainObject for gridcell
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="oData"></param>
        /// <param name="addCollider"></param>
        /// <param name="radius"></param>
        /// <param name="isTrigger"></param>
        /// <returns></returns>
        public static DynamicClickBombObject Create(GridCell parent, DynamicClickBombObjectData oData, bool addCollider, bool isTrigger, Action<int> TargetCollectEvent)
        {
            if (!parent || oData == null) return null;
            SpriteRenderer sR = Creator.CreateSprite(parent.transform, oData.ObjectImage, parent.transform.position);
            GameObject gO = sR.gameObject;
            DynamicClickBombObject gridObject = gO.AddComponent<DynamicClickBombObject>();

            if (addCollider)
            {
                BoxCollider2D cC = gridObject.gameObject.GetOrAddComponent<BoxCollider2D>();
                cC.isTrigger = isTrigger;
            }

            if (oData.iddleAnimPrefab)
                Creator.InstantiatePrefab(oData.iddleAnimPrefab, gridObject.transform, gridObject.transform.position, 0, SortingOrder.Main+2);

            gridObject.SRenderer = sR;
            gridObject.TargetCollectEvent = TargetCollectEvent;
            gridObject.SetData(oData);
            return gridObject;
        }
        #endregion create

        #region override
        internal override void PlayExplodeAnimation(GridCell gCell, float delay, Action completeCallBack)
        {
            if (!gCell || OData == null) completeCallBack?.Invoke();

            Row<GridCell> r = gCell.GRow;
            Column<GridCell> c = gCell.GColumn;
           // Debug.Log(gCell);

            TweenSeq anim = new TweenSeq();
            GameObject g = null;

            anim.Add((callBack) => { delayAction(gameObject, delay, callBack); });

            anim.Add((callBack) => // explode wave
            {
                MBoard.ExplodeWave(0, transform.position, 5, null);
                callBack();
            });

            anim.Add((callBack) =>  //sound
            {
                MSound.PlayClip(0, OData.privateClip);
                callBack();
            });

            if (OData.bombType == BombDir.Horizontal || OData.bombType == BombDir.Vertical)
            {
                anim.Add((callBack) =>
                {
                    g = Instantiate(OData.explodeAnimPrefab);
                    g.transform.position = transform.position;
                    g.transform.localScale = transform.localScale * 1.2f;
                    callBack();
                });
            }
            else if (OData.bombType == BombDir.Radial)
            {
                anim.Add((callBack) =>
                {
                    g = Instantiate(OData.explodeAnimPrefab);
                    g.transform.position = transform.position;
                    g.transform.localScale = transform.localScale * 1.0f;
                    callBack();
                });
            }

            else if (OData.bombType == BombDir.Color)
            {
                anim.Add((callBack) => // scale out
                {
                    SetToFront(true);
                    SimpleTween.Value(gameObject, 1, 1.5f, 0.15f).SetOnUpdate((float val) => { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
                });
                anim.Add((callBack) => // scale in
                {
                    SimpleTween.Value(gameObject, 1.5f, 1.0f, 0.15f).SetOnUpdate((float val) => { transform.localScale = gCell.transform.lossyScale * val; }).AddCompleteCallBack(callBack);
                    g = Instantiate(OData.explodeAnimPrefab);
                    g.transform.position = transform.position;
                    g.transform.localScale = transform.localScale * 1.0f;
                });

                CellsGroup eArea = GetArea(gCell);
                ParallelTween pT = new ParallelTween();
                float incDelay = 0f;
                foreach (var item in eArea.Cells)
                {
                    incDelay += 0.05f;
                    float t = incDelay;
                    pT.Add((cB) =>
                    {
                        delayAction(item.gameObject, t,() =>  // delay tween
                        {
                            Vector2 relativePos = (item.transform.position - gCell.transform.position).normalized;
                            Quaternion rotation = Quaternion.FromToRotation(new Vector2(-1, 0), relativePos); // Debug.Log("Dir: " +(item.transform.position - gCell.transform.position) + " : " + rotation.eulerAngles );
                            GameObject cb = Instantiate(OData.additAnimPrefab, transform.position, rotation);
                            cb.transform.localScale = transform.lossyScale * 1.0f;
                            SimpleTween.Move(cb, cb.transform.position, item.transform.position, 0.2f).AddCompleteCallBack(cB).SetEase(EaseAnim.EaseOutSine);
                        });
                   } );

                }

                anim.Add((callBack) =>
                {
                    pT.Start(callBack);
                });
            }

            anim.Add((callBack) =>
            {
              //  Debug.Log("anim complete");
                TargetCollectEvent?.Invoke(GetID());
                completeCallBack?.Invoke();
                callBack();
            });

            anim.Start();
        }

        public override CellsGroup GetArea(GridCell gCell)
        {
            CellsGroup cG = new CellsGroup();
          //  Debug.Log(gCell);
            if (!gCell) return cG;
            switch (OData.bombType)
            {
                case BombDir.Vertical:
                    cG.AddRange(gCell.GColumn.cells);  //cG.AddRange(gCell.GColumn.GetDynamicArea()); 
                    break;
                case BombDir.Horizontal:
                    cG.AddRange(gCell.GRow.cells); // cG.AddRange(gCell.GRow.GetDynamicArea());
                    break;
                case BombDir.Radial:
                    List<GridCell> areaRad = MBoard.grid.GetAroundArea(gCell, 1).Cells;
                    cG.Add(gCell);
                    foreach (var item in areaRad)
                            cG.Add(item);// if (item.IsMatchable)
                    break;
                case BombDir.Color:
                    cG.AddRange(MGrid.GetAllByID(OData.matchID).SortByDistanceTo(gCell));
                    break;
            }
            return cG;
        }

        public override void ExplodeArea(GridCell gCell, float delay, bool sequenced, bool showPrefab, bool fly, bool hitProtection, Action completeCallBack)
        {
            Destroy(gameObject);
            ParallelTween pt = new ParallelTween();
            TweenSeq expl = new TweenSeq();

            expl.Add((callBack) => { delayAction(gCell.gameObject, delay, callBack); });

            foreach (GridCell mc in GetArea(gCell).Cells) //parallel explode all cells
            {
                float t = 0;
                if (sequenced)
                {
                    float distance = Vector2.Distance(mc.transform.position, gCell.transform.position);
                    t = distance / 15f;
                }
                pt.Add((callBack) =>
                {
                    //  Debug.Log("explode " + mc + " ;time: " + Time.time);
                    GridCell.ExplodeCell(mc, t, showPrefab, fly, hitProtection, callBack);
                });
            }

            expl.Add((callBack) => { pt.Start(callBack); });
            expl.Add((callBack) =>
            {
                completeCallBack?.Invoke(); callBack();
            });

            expl.Start();
        }

        public override int GetID()
        {
            return (OData != null) ? OData.ID : Int32.MinValue;
        }

        public override void CancellTweensAndSequences()
        {
            base.CancellTweensAndSequences();
        }

        public override void SetToFront(bool set)
        {
            GridCell gC = GetComponentInParent<GridCell>();
            int addOrder = (gC) ? gC.AddRenderOrder : 0;

            if (!SRenderer) SRenderer = GetComponent<SpriteRenderer>();
            if (set)
                SRenderer.sortingOrder = SortingOrder.MainToFront + addOrder;
            else
                SRenderer.sortingOrder = SortingOrder.Main + addOrder;
        }

        public override string ToString()
        {
            return "DynamicClickBomb: " + GetID();
        }

        internal void InstantiateScoreFlyerAtPosition(GameObject scoreFlyerPrefab, int score, Vector3 position)
        {
            if (!scoreFlyerPrefab) return;
            GameObject flyer = Instantiate(scoreFlyerPrefab);
            ScoreFlyer sF = flyer.GetComponent<ScoreFlyer>();
            sF.StartFly(score.ToString(), position);
            flyer.transform.localScale = transform.lossyScale;
        }

        public override BombDir GetBombDir()
        {
            return OData.bombType;
        }

        /// <summary>
        /// If matched > = 4 cretate bomb from items
        /// </summary>
        /// <param name="bombCell"></param>
        /// <param name="completeCallBack"></param>
        internal void MoveToBomb(GridCell toCell, float delay, Action completeCallBack)
        {
           // Debug.Log("Move to bomb");

            SetToFront(true);
            //scale
            SimpleTween.Value(gameObject, gameObject.transform.localScale, gameObject.transform.localScale * 1.05f, 0.1f).SetOnUpdate((val) => { gameObject.transform.localScale = val; });

            // move
            SimpleTween.Move(gameObject, transform.position, toCell.transform.position, 0.25f).AddCompleteCallBack(completeCallBack).SetEase(EaseAnim.EaseInBack).SetDelay(delay);
        }
        #endregion override
    }
}