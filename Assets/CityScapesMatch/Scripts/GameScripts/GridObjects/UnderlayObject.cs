using System;
using UnityEngine;

namespace Mkey
{
    public class UnderlayObject : GridObject
    {
        #region properties
        public UnderlayObjectData OData { get; private set; }
        protected int hits = 0;
        public int Protection
        {
            get { return OData.protectionStateImages.Length + 1 - hits; }
        }
        #endregion properties

        #region events
        private Action<int> TargetCollectEvent;
        #endregion events

        #region create
        internal virtual void SetData(UnderlayObjectData mData)
        {
            OData = mData;
            SetToFront(false);
        }

        public void SetProtection(int protection)
        {
            //    Protection = protection;
           // ChangProtectionEvent?.Invoke();
        }

        /// <summary>
        /// Create new OverlayObject for gridcell
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="oData"></param>
        /// <param name="addCollider"></param>
        /// <param name="radius"></param>
        /// <param name="isTrigger"></param>
        /// <returns></returns>
        public static UnderlayObject Create(GridCell parent, UnderlayObjectData oData, Action<int> TargetCollectEvent)
        {
            if (!parent || oData == null) return null;
            GameObject gO = null;
            SpriteRenderer sR = null;
            UnderlayObject gridObject = null;

            sR = Creator.CreateSprite(parent.transform, oData.ObjectImage, parent.transform.position);
            gO = sR.gameObject;

            gridObject = gO.GetOrAddComponent<UnderlayObject>();
#if UNITY_EDITOR
            gO.name = "underlay " + parent.ToString();
#endif
            gridObject.SRenderer = sR;
            gridObject.TargetCollectEvent = TargetCollectEvent;
            gridObject.SetData(oData);

            return gridObject;
        }
        #endregion create

        #region override
        public override int GetID()
        {
            return (OData != null) ? OData.ID : Int32.MinValue;
        }

        public override void Hit(GridCell gCell, Action completeCallBack)
        {
            hits++;
            int protection = Protection;
            if (OData.protectionStateImages.Length > 0)
            {
                int i = Mathf.Min(hits - 1, OData.protectionStateImages.Length - 1);
                GetComponent<SpriteRenderer>().sprite = OData.protectionStateImages[i];
            }

            if (OData.hitAnimPrefab)
            {
                Creator.InstantiateAnimPrefab(OData.hitAnimPrefab, transform.parent, transform.position, SortingOrder.MainExplode, true, null);
            }

            MSound.PlayClip(0, OData.privateClip);

            if (protection == 0)
            {
                hitDestroySeq = new TweenSeq();

                SetToFront(true);

                hitDestroySeq.Add((callBack) => // play preexplode animation
                {
                    SimpleTween.Value(gameObject, 0, 1, 0.050f).AddCompleteCallBack(() =>
                    {
                        callBack();
                    });
                });

                hitDestroySeq.Add((callBack) =>
                {
                    TargetCollectEvent?.Invoke(GetID());
                    MSound.PlayClip(0, OData.privateClip, transform.position, null);
                    callBack();
                });

                hitDestroySeq.Add((callBack) =>
                {
                    completeCallBack?.Invoke();
                    Destroy(gameObject);
                    callBack();
                });

                hitDestroySeq.Start();
            }
            else
            {
                completeCallBack?.Invoke();
            }
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
                SRenderer.sortingOrder = SortingOrder.UnderToFront + addOrder;
            else
                SRenderer.sortingOrder = SortingOrder.Under + addOrder;
        }

        public override string ToString()
        {
            return "Underlay: " + GetID();
        }
        #endregion override
    }
}
