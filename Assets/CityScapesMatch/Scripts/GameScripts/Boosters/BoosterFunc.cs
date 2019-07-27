using UnityEngine;
using System;
using System.Collections.Generic;

namespace Mkey
{
    public class BoosterFunc : MonoBehaviour
    {
        [SerializeField]
        protected List<ObjectRenderOrder> renderers;

        protected MatchBoard MBoard { get { return MatchBoard.Instance; } }
        protected MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        protected MatchGUIController MGui { get { return MatchGUIController.Instance; } }
        protected MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }

        protected Action<GameObject, float, Action> delayAction = (g, del, callBack) => { SimpleTween.Value(g, 0, 1, del).AddCompleteCallBack(callBack); };
        protected Action<GameObject, float [], float, Action<float>, Action> ValuesTween = (g, values, time, update, completeCallBack) => 
        {
            if(values==null || values.Length == 0)
            {
                completeCallBack?.Invoke();
                return;
            }
            if (values.Length == 1)
            {
                update?.Invoke(values[0]);
                completeCallBack?.Invoke();
                return;
            }

            float[] v = new float[values.Length];
            values.CopyTo(v, 0);

            TweenSeq tS = new TweenSeq();
            for (int i = 0; i < v.Length-2; i++)
            {
                tS.Add((callBack)=> { SimpleTween.Value(g, v[i], v[i+1], time/(v.Length-1)).SetOnUpdate((float val)=> { update?.Invoke(val); }).AddCompleteCallBack(callBack); });
            }

            tS.Add((callBack)=> 
            {
                completeCallBack?.Invoke();
                callBack();
            });

            tS.Start();
        };

        public virtual void InitStart ()
        {
            Debug.Log("base init start");
        }

        public virtual void ApplyToGrid(GridCell hitGridCell, BoosterObjectData bData,  Action completeCallBack)
        {
            Debug.Log("base apply to grid booster");
            completeCallBack?.Invoke();
        }

        public virtual bool  ActivateApply(Booster b)
        {
            Debug.Log("base activate apply booster");
            return false;
        }

        public virtual CellsGroup GetArea(GridCell hitGridCell)
        {
            Debug.Log("base get shoot area");
            CellsGroup cG = new CellsGroup();
            return cG;
        }

        public virtual void SetToFront(bool set)
        {
            int order = SortingOrder.Booster;
            if (set)
            {
                order = SortingOrder.BoosterToFront;
                
            }
            if (renderers != null)
            {
                foreach (var item in renderers)
                {
                    item.SetOrder(order++);
                }
            }
        }
    }
}