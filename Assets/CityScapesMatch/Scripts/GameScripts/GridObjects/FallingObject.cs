using System;
using UnityEngine;

namespace Mkey
{
    public class FallingObject : GridObject
    {
        #region properties
        public FallingObjectData OData { get; private set; }
        #endregion properties

        #region events
        private Action<int> TargetCollectEvent;
        #endregion events

        #region create
        internal virtual void SetData(FallingObjectData oData)
        {
            SRenderer = GetComponent<SpriteRenderer>();
            if (SRenderer) SRenderer.sprite = (oData != null) ? oData.ObjectImage : null;
            OData = oData;
#if UNITY_EDITOR
            gameObject.name = (oData != null) ? "Falling: " + GetID() + "(" + SRenderer.sprite.name + ")" : "none";
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
        public static FallingObject Create(GridCell parent, FallingObjectData oData, bool addCollider, bool isTrigger, Action<int> TargetCollectEvent)
        {
            if (!parent || oData == null) return null;
            SpriteRenderer sR = Creator.CreateSprite(parent.transform, oData.ObjectImage, parent.transform.position);
            GameObject gO = sR.gameObject;
            FallingObject gridObject = gO.AddComponent<FallingObject>();

            if (addCollider)
            {
                BoxCollider2D cC = gridObject.gameObject.GetOrAddComponent<BoxCollider2D>();
                cC.isTrigger = isTrigger;
            }

            if (oData.iddleAnimPrefab)
                Creator.InstantiatePrefab(oData.iddleAnimPrefab, gridObject.transform, gridObject.transform.position, 0, SortingOrder.MainToFront);

            gridObject.SRenderer = sR;
            gridObject.TargetCollectEvent = TargetCollectEvent;
            gridObject.SetData(oData);
            return gridObject;
        }
        #endregion create

        /// <summary>
        /// Collect match object, hit overlays, hit underlays
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void Collect( float delay, bool showPrefab, bool fly,  Action completeCallBack)
        {
            transform.parent = null;
            GameObject animPrefab = OData.collectAnimPrefab;
    
            TweenSeq  cSequence = new TweenSeq();
            if (delay > 0)
            {
                cSequence.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 0, 1, delay).AddCompleteCallBack(callBack);
                });
            }

            cSequence.Add((callBack) =>
            {
                if (this) SetToFront(true);
                MSound.PlayClip(0, OData.privateClip);
                callBack();
            });

            // sprite seq animation
            if (showPrefab)
                cSequence.Add((callBack) =>
                {
                    Creator.InstantiateAnimPrefab(animPrefab, transform, transform.position, SortingOrder.MainToFront+1, false,
                       () =>
                       {
                           callBack();
                       });
                });

            //fly
            if (fly)
            {
                cSequence.Add((callBack) =>
                {
                    SimpleTween.Move(gameObject, transform.position, MatchBoard.Instance.FlyTarget, 0.4f).AddCompleteCallBack(() =>
                    {
                        //  callBack();
                    });
                    callBack(); // not wait
                });
                cSequence.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 0, 1, 0.15f).AddCompleteCallBack(callBack);
                });
            }
            //finish
            cSequence.Add((callBack) =>
            {
                TargetCollectEvent?.Invoke(OData.ID);
                completeCallBack?.Invoke();
                Destroy(gameObject, (fly) ? 0.6f : 0);
                callBack();
            });

            cSequence.Start();
        }

        #region override
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
                SRenderer.sortingOrder = SortingOrder.GuiOverlay;
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
        #endregion override
    }
}