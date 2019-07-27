using System;
using UnityEngine;

namespace Mkey
{
    public class BlockedObject : GridObject
    {
        #region properties
        public BaseObjectData OData { get; private set; }
        #endregion properties

        #region events
        private Action<int> TargetCollectEvent;
        #endregion events

        #region create
        internal virtual void SetData(BaseObjectData mData)
        {
            OData = mData;
            SetToFront(false);
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
        public static BlockedObject Create(GridCell parent, BaseObjectData oData, Action<int> TargetCollectEvent)
        {
            if (!parent || oData == null) return null;
            GameObject gO = null;
            SpriteRenderer sR = null;
            BlockedObject gridObject = null;

            sR = Creator.CreateSprite(parent.transform, oData.ObjectImage, parent.transform.position);
            gO = sR.gameObject;

            gridObject = gO.GetOrAddComponent<BlockedObject>();
#if UNITY_EDITOR
            gO.name = "blocked " + parent.ToString();
#endif
            gridObject.SetData(oData);
            gridObject.SRenderer = sR;
            gridObject.TargetCollectEvent = TargetCollectEvent;
            return gridObject;
        }
        #endregion create

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
                SRenderer.sortingOrder = SortingOrder.Blocked + addOrder;
            else
                SRenderer.sortingOrder = SortingOrder.Blocked + addOrder;
        }

        public override string ToString()
        {
            return "Blocked: " + GetID();
        }
        #endregion override
    }
}
