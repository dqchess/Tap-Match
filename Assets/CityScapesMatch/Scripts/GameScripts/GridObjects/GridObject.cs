using System;
using UnityEngine;

namespace Mkey
{
    public class GridObject : MonoBehaviour
    {
        #region properties
        protected SpriteRenderer SRenderer { get; set; }
        protected MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }
        protected MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        protected MatchBoard MBoard { get { return MatchBoard.Instance; } }
        protected MatchGrid MGrid{ get { return MBoard.grid; } }
        #endregion properties

        protected Action<GameObject, float, Action> delayAction = (g, del, callBack) => { SimpleTween.Value(g, 0, 1, del).AddCompleteCallBack(callBack); };

        protected TweenSeq collectSequence;
        protected TweenSeq hitDestroySeq;
     

        #region regular
        void OnDestroy()
        {
            CancellTweensAndSequences();
        }
        #endregion regular

        /// <summary>
        /// Return true if is the same object (the same reference)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        internal bool ReferenceEquals(GridObject other)
        {
            return System.Object.ReferenceEquals(this, other);//Determines whether the specified Object instances are the same instance.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        internal void SetLocalScale(float scale)
        {
            transform.localScale = (transform.parent) ? transform.parent.localScale * scale : new Vector3(scale, scale,scale);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        internal void SetLocalScaleX(float scale)
        {
            Vector3 parLS = (transform.parent) ? transform.parent.localScale : Vector3.one;
            float ns = parLS.x * scale ;
            transform.localScale = new Vector3(ns, parLS.y, parLS.z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        internal void SetLocalScaleY(float scale)
        {
            Vector3 parLS = (transform.parent) ? transform.parent.localScale : Vector3.one;
            float ns = parLS.y * scale;
            transform.localScale = new Vector3(parLS.x, ns, parLS.z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alpha"></param>
        internal void SetAlpha(float alpha)
        {
            if (!SRenderer) GetComponent<SpriteRenderer>();
            if (SRenderer)
            {
                Color c = SRenderer.color;
                Color newColor = new Color(c.r, c.g, c.b, alpha);
                SRenderer.color = newColor;
            }
        }

        #region virtual
        /// <summary>
        /// Hit object from collected
        /// </summary>
        /// <param name="completeCallBack"></param>
        public virtual void Hit(GridCell gCell,  Action completeCallBack)
        {
            completeCallBack?.Invoke();
        }

        /// <summary>
        /// Cancel all tweens and sequences
        /// </summary>
        public virtual void CancellTweensAndSequences()
        {
            collectSequence?.Break();
            hitDestroySeq?.Break();
            SimpleTween.Cancel(gameObject, false);
        }

        public virtual void SetToFront(bool set)
        {

        }

        public virtual int GetID()
        {
            return Int32.MinValue;
        }
        #endregion virtual
       
    }
}

