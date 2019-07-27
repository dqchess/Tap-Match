using System;
using UnityEngine;

namespace Mkey
{
    public class MatchObject : GridObject, IEquatable<MatchObject>
    {
        #region properties
        public MatchObjectData OData { get; private set; }

        public bool IsExploidable
        {
            get; internal set;
        }

        public float SwapTime { get; set; } // save last swap
        #endregion properties

        #region events
        private Action<int> TargetCollectEvent;
        private Action ScoreCollectEvent;
        #endregion events

        #region private
        private TweenSeq zoomSequence;
        private TweenSeq explodeSequence;
        private GridCell gCell;
        #endregion private

        /// <summary>
        /// Return true if object IDs is Equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MatchObject other)
        {
            if (other == null) return false;
            if ((OData == null) || (other.OData == null)) return false;
            return ((OData.ID > 0) && (OData.ID == other.OData.ID));
        }

        #region create
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mData"></param>
        public void SetData(MatchObjectData mData)
        {
            OData = mData;
            SRenderer = GetComponent<SpriteRenderer>();
            if (SRenderer) SRenderer.sprite = (mData!=null) ? mData.ObjectImage : null;

#if UNITY_EDITOR
            gameObject.name = (mData!=null)? "match id: " + GetID() +"(" + SRenderer.sprite.name+")": "none";
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
        public static MatchObject Create(GridCell parent, MatchObjectData oData, bool addCollider,  bool isTrigger, Action<int> TargetCollectEvent, Action ScoreCollectEvent)
        {
            if (!parent || oData == null) return null;
            SpriteRenderer sR = Creator.CreateSprite(parent.transform, oData.ObjectImage, parent.transform.position);
            GameObject gO = sR.gameObject;
            MatchObject  gridObject = gO.AddComponent<MatchObject>();

          //  gObject.Init(parent.Row, parent.Column);
            if (addCollider)
            {
                BoxCollider2D cC = gridObject.gameObject.GetOrAddComponent<BoxCollider2D>();
                cC.isTrigger = isTrigger;
            }
            gridObject.SRenderer = sR;
            gridObject.TargetCollectEvent = TargetCollectEvent;
            gridObject.ScoreCollectEvent = ScoreCollectEvent;
            gridObject.SetData(oData);
            return gridObject;
        }

        /// <summary>
        /// Create new MainObject not initialized
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="mData"></param>
        /// <param name="addCollider"></param>
        /// <param name="radius"></param>
        /// <param name="isTrigger"></param>
        /// <returns></returns>
        public static MatchObject Create( MatchObjectData mData, Vector3 position,  bool addCollider, bool isTrigger, Action<int> TargetCollectEvent, Action ScoreCollectEvent)
        {
            if (mData == null) return null;
            SpriteRenderer sR = Creator.CreateSprite(MatchBoard.Instance.transform, mData.ObjectImage, position, SortingOrder.Main);
            GameObject gO = sR.gameObject;
            MatchObject gridObject = gO.AddComponent<MatchObject>();

            if (addCollider)
            {
                BoxCollider2D cC = gridObject.gameObject.GetOrAddComponent<BoxCollider2D>();
                cC.isTrigger = isTrigger;
            }
            gridObject.SRenderer = sR;
            gridObject.TargetCollectEvent = TargetCollectEvent;
            gridObject.ScoreCollectEvent = ScoreCollectEvent;
            gridObject.SetData(mData);
            return gridObject;
        }
        #endregion create

        /// <summary>
        /// Reset localscale, reset alpha
        /// </summary>
        public void ResetTween()
        {
            transform.localScale = transform.parent.localScale;
            SRenderer.color = new Color(1, 1, 1, 1);
        }

        /// <summary>
        /// Show simple zoom sequence on main object
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void Zoom(Action completeCallBack)
        {
            if (zoomSequence != null)
            {
                zoomSequence.Break();
            }

            zoomSequence = new TweenSeq();
            for (int i = 0; i < 2; i++)
            {
                zoomSequence.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 1.0f, 1.2f, 0.07f).SetOnUpdate((float val) =>
                    {
                        SetLocalScale(val);
                    }).AddCompleteCallBack(() =>
                    {
                        callBack();
                    });
                });
                zoomSequence.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 1.2f, 1.0f, 0.09f).SetOnUpdate((float val) =>
                    {
                       SetLocalScale(val);

                    }).AddCompleteCallBack(() =>
                    {
                        callBack();
                    });
                });
            }

            zoomSequence.Add((callBack) => { completeCallBack?.Invoke(); callBack(); });
            zoomSequence.Start();
        }

        /// <summary>
        /// Collect match object, hit overlays, hit underlays
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void Collect(GridCell gCell, float delay, bool showPrefab, bool fly, bool hitProtection, bool sideHitProtection, Action completeCallBack)
        {
            this.gCell = gCell;
            transform.parent = null;

            GameObject animPrefab = OData.collectAnimPrefab;

            collectSequence = new TweenSeq();
            if (delay > 0)
            {
                collectSequence.Add((callBack) => {
                    SimpleTween.Value(gameObject, 0,1, delay).AddCompleteCallBack(callBack);
              });
            }

            // play sound
            collectSequence.Add((callBack) =>
            {
                MSound.PlayClip(0, OData.privateClip);
                callBack();
            });

            // sprite seq animation
            if (showPrefab)
                collectSequence.Add((callBack) =>
                {
                    if (this && !fly) GetComponent<SpriteRenderer>().enabled = false;
                    GameObject aG = Creator.InstantiateAnimPrefab(animPrefab, transform, transform.position, SortingOrder.Main, false,
                       () =>
                       {
                           if (this && fly) SetToFront(true);
                           callBack();
                       });
                    aG.transform.parent = null; // unparent for play full animation
                });

            // hit protection
            collectSequence.Add((callBack) =>
            {
                if (hitProtection)
                {
                    gCell.DirectHitUnderlay(null);
                    gCell.DirectHitOverlay(null);
                   
                }
                if (sideHitProtection)
                {
                    gCell.Neighbors.Cells.ForEach((GridCell c) => { c.SideHit(null); });
                }
                callBack();
            });

            //fly
            if (fly)
            {
                collectSequence.Add((callBack) =>
                {
                    SimpleTween.Move(gameObject, transform.position, MatchBoard.Instance.FlyTarget, 0.4f).AddCompleteCallBack(() =>
                    {
                    //  callBack();
                });
                    callBack(); // not wait
            });
                collectSequence.Add((callBack) => {
                    SimpleTween.Value(gameObject, 0, 1, 0.15f).AddCompleteCallBack(callBack);
                });
            }
            // finish
            collectSequence.Add((callBack) =>
            {
               // Debug.Log(ToString()+  " collected");
                ScoreCollectEvent?.Invoke();
                TargetCollectEvent?.Invoke(OData.ID);
                completeCallBack?.Invoke();
                Destroy(gameObject,  (fly) ? 0.4f: 0);
            });

            collectSequence.Start();
        }

        internal void SideHit(GridCell gCell, Action completeCallBack)
        {
            this.gCell = gCell;
            gCell.Neighbors.Cells.ForEach((GridCell c) => { c.SideHit(null); });
            completeCallBack?.Invoke();
        }

        internal void DirectHitOverlay(GridCell gCell, Action completeCallBack)
        {
            gCell.DirectHitUnderlay(null);
            gCell.DirectHitOverlay(null);
            completeCallBack?.Invoke();
        }

        /// <summary>
        /// show explode effect and collect match
        /// </summary>
        /// <param name="completeCallBack"></param>
        /// <param name="bomb"></param>
        /// <param name="bombType"></param>
        internal void Explode(GridCell gCell, bool showPrefab, bool fly, bool hitProtection, bool sideHitProtection, float delay, Action completeCallBack)
        {
            explodeSequence = new TweenSeq();
            transform.parent = null;
            if (delay > 0)
            {
                explodeSequence.Add((callBack) => {
                    SimpleTween.Value(gameObject, 0, 1, delay).AddCompleteCallBack(callBack);
                });
            }
            explodeSequence.Add((callBack) => { Collect(gCell, 0, showPrefab, fly, hitProtection, sideHitProtection, callBack); });
            explodeSequence.Add((callBack) => { completeCallBack?.Invoke(); callBack(); });
            explodeSequence.Start();
        }

        /// <summary>
        /// If matched > = 4 cretate bomb from items
        /// </summary>
        /// <param name="bombCell"></param>
        /// <param name="completeCallBack"></param>
        internal void MoveMatchToBomb(GridCell fromCell, GridCell toCell, float delay, bool hitProtection, Action completeCallBack)
        {
          //  Debug.Log("Move to bomb");
            if (hitProtection)
            {
                fromCell.DirectHitUnderlay(null);
                fromCell.DirectHitOverlay(null);
                fromCell.Neighbors.Cells.ForEach((GridCell c) => { c.SideHit(null); });
            }

            SetToFront(true);
            //scale
            SimpleTween.Value(gameObject, gameObject.transform.localScale, gameObject.transform.localScale * 1.05f, 0.1f).SetOnUpdate( (val) => { gameObject.transform.localScale = val; }); 

            // move
            SimpleTween.Move(gameObject, transform.position, toCell.transform.position, 0.25f).AddCompleteCallBack(completeCallBack).SetEase(EaseAnim.EaseInBack).SetDelay(delay);
        }

        internal void SetGroupImage(MatchGroupType matchGroupType)
        {
            if (OData == null || !SRenderer) return;

            switch (matchGroupType)
            {
                case MatchGroupType.Simple:
                    SRenderer.sprite = OData.ObjectImage;
                    break;
                case MatchGroupType.HorBomb:
                    if (OData.ObjectGroup5Image_H) SRenderer.sprite = OData.ObjectGroup5Image_H;
                    break;
                case MatchGroupType.VertBomb:
                    if(OData.ObjectGroup5Image_V) SRenderer.sprite = OData.ObjectGroup5Image_V;
                    break;
                case MatchGroupType.Bomb:
                    if (OData.ObjectGroup7Image) SRenderer.sprite = OData.ObjectGroup7Image;
                    break;
                case MatchGroupType.ColorBomb:
                    if (OData.ObjectGroup9Image) SRenderer.sprite = OData.ObjectGroup9Image;
                    break;
                default:
                    break;
            }
        }

        internal void ReSetGroupImage()
        {
            if (OData == null || !SRenderer) return;
            SRenderer.sprite = OData.ObjectImage;
            return;
        }

        #region override
        public override int GetID()
        {
            return (OData != null) ? OData.ID : Int32.MinValue;
        }

        public override void CancellTweensAndSequences()
        {
            SimpleTween.Cancel(gameObject, false);
            zoomSequence?.Break();
            explodeSequence?.Break();
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
            return "MainObject: " + GetID();
        }

        internal void InstantiateScoreFlyerAtPosition(GameObject scoreFlyerPrefab, int score, Vector3 position)
        {
            if (!scoreFlyerPrefab) return;
            GameObject flyer = Instantiate(scoreFlyerPrefab);
            ScoreFlyer sF = flyer.GetComponent<ScoreFlyer>();
            sF.StartFly(score.ToString(), position);
            flyer.transform.localScale = transform.lossyScale;
        }
        #endregion override
    }
}


