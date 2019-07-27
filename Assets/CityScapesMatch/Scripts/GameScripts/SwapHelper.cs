using System;
using UnityEngine;

namespace Mkey
{
    public class SwapHelper 
    {
        public static GridCell Source;
        public static GridCell Target;

        public static Action<GridCell> SwapBeginEvent;
        public static Action<GridCell> SwapEndEvent;
        private static TouchManager Touch { get { return TouchManager.Instance; } }

        public static void Swap()
        {
            Source = (Touch.Draggable) ? Touch.Source : null;
            Target = Touch.Target;

            Swap(Source, Target);
            Touch.SetDraggable(null, null);
            Touch.SetTarget(null);
        }

        public static void Swap(GridCell gc1, GridCell gc2)
        {
            Source = gc1;
            Target = gc2;

            if (Source && Source.CanSwap(Target))
            {
                SwapBeginEvent?.Invoke(Target);
                MatchObject dM = Source.Match;
                MatchObject tM = Target.Match;
                dM.SwapTime = Time.time;
                tM.SwapTime = Time.time;
                Source.GrabDynamicObject(tM.gameObject,false, null);
                Target.GrabDynamicObject(dM.gameObject, false, () =>
                {
                    SwapEndEvent?.Invoke(Target);
                });
            }
            else if (Source)
            {
                Touch.ResetDrag(null);
            }
        }

        public static void UndoSwap(Action callBack)
        {
            MatchObject dM = Source.Match;
            MatchObject tM = Target.Match;
            Source.GrabDynamicObject(tM.gameObject, false, null);
            Target.GrabDynamicObject(dM.gameObject, false, () =>
            {
                callBack?.Invoke();
            });
        }
    }
}