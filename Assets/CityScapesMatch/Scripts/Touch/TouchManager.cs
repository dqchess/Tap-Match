using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace Mkey
{
    public class TouchManager : MonoBehaviour
    {
        public static TouchManager Instance;
        public bool dlog = false;

        [SerializeField]
        private bool showDrag = true;

        #region properties
        public GridCell Target
        {
            get; private set;
        }

        public GridCell Source
        {
            get; private set;
        }

        public MatchObject Draggable
        {
            get; private set;
        }
        #endregion properties

        private Vector3 dragPos;
        private Vector3 startDragPos;
        private Action <Action> ResetDragEvent;

        #region regular
        void Start()
        {
            if (Instance != null) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }
        #endregion regular

        /// <summary>
        /// Return true if touchpad is touched with mouse or finger
        /// </summary>
        public static bool IsTouched
        {
            get
            {
                return TouchPad.Instance.IsTouched;
            }
        }

        /// <summary>
        /// Return true touch pad run on mobile device
        /// </summary>
        public static bool IsMobileDevice()
        {
            //check if our current system info equals a desktop
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                //we are on a desktop device, so don't use touch
                return false;
            }
            //if it isn't a desktop, lets see if our device is a handheld device aka a mobile device
            else if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                //we are on a mobile device, so lets use touch input
                return true;
            }
            return false;
        }

        /// <summary>
        /// Enable or disable touch pad callbacks handling.
        /// </summary>
        internal static void SetTouchActivity(bool activity)
        {
            TouchPad.Instance.SetTouchActivity(activity);
        }

        public void Drag(TouchPadEventArgs tpea)
        {
            if (dlog) Debug.Log("drag: " + gameObject.name);
            if (Draggable)
            {
                dragPos = new Vector3(tpea.WorldPos.x, tpea.WorldPos.y, Draggable.transform.position.z);
                if (showDrag) Draggable.transform.position = new Vector3(tpea.WorldPos.x, tpea.WorldPos.y, Draggable.transform.position.z);
                if (Draggable && Vector3.Distance(startDragPos, dragPos) > MatchBoard.MaxDragDistance) ResetDrag(null);
            }
        }

        public void SetDraggable(GridCell source, Action<Action> resetDrag)
        {
            Source = source;
            if (source)
            {
                Draggable = source.Match;
                startDragPos = source.transform.position;
            }
            else
            {
                Draggable = null;
            }
            ResetDragEvent = resetDrag;
        }

        public void SetTarget(GridCell target)
        {
            Target = target;
        }

        public void ResetDrag(Action completeCallBack)
        {
            ResetDragEvent?.Invoke(completeCallBack);
            Draggable = null;
            Source = null;
            Target = null;
            ResetDragEvent = null;
        }
    }


    /// <summary>
    /// Interface for handling touchpad events.
    /// </summary>
    public interface ICustomMessageTarget : IEventSystemHandler
    {
        void PointerDown(TouchPadEventArgs tpea);
        void DragBegin(TouchPadEventArgs tpea);
        void DragEnter(TouchPadEventArgs tpea);
        void DragExit(TouchPadEventArgs tpea);
        void DragDrop(TouchPadEventArgs tpea);
        void PointerUp(TouchPadEventArgs tpea);
        void Drag(TouchPadEventArgs tpea);
        GameObject GetDataIcon();
        GameObject GetGameObject();
        bool IsDraggable();
    }

    public enum DirectionType { Right, Left, Top, Bottom }
    [Serializable]
    public class TouchPadEventArgs
    {
        /// <summary>
        /// First selected object.
        /// </summary>
        public ICustomMessageTarget firstSelected;
        /// <summary>
        /// The cast results.
        /// </summary>
        public Collider2D[] hits;
        /// <summary>
        /// Priority dragging direction.  (0,1) or (1,0)
        /// </summary>
        public Vector2 PriorAxe
        {
            get { return priorityAxe; }
        }
        /// <summary>
        /// Touch delta position in screen coordinats;
        /// </summary>
        public Vector2 DragDirection
        {
            get { return touchDeltaPosRaw; }
        }
        /// <summary>
        /// Last drag direction.
        /// </summary>
        public Vector2 LastDragDirection
        {
            get { return lastDragDir; }
        }
        /// <summary>
        /// Return touch world position.
        /// </summary>
        public Vector3 WorldPos
        {
            get { return wPos; }
        }

        private Vector2 touchDeltaPosRaw;
        private Vector2 priorityAxe;
        private Vector2 lastDragDir;
        private Vector3 wPos;
        private Vector2 touchPos;

        /// <summary>
        /// Fill touch arguments from touch object;
        /// </summary>
        public void SetTouch(Touch touch)
        {
            touchPos = touch.position;
            wPos = Camera.main.ScreenToWorldPoint(touchPos);
            hits = Physics2D.OverlapPointAll(new Vector2(wPos.x, wPos.y));
            touchDeltaPosRaw = touch.deltaPosition;

            if (touch.phase == TouchPhase.Moved)
            {
                lastDragDir = touchDeltaPosRaw;
                priorityAxe = GetPriorityOneDirAbs(touchDeltaPosRaw);
            }
        }

        /// <summary>
        /// Fill touch arguments.
        /// </summary>
        public void SetTouch(Vector2 position, Vector2 deltaPosition, TouchPhase touchPhase)
        {
            touchPos = position;
            wPos = Camera.main.ScreenToWorldPoint(touchPos);
            hits = Physics2D.OverlapPointAll(new Vector2(wPos.x, wPos.y));
            touchDeltaPosRaw = deltaPosition;

            if (touchPhase == TouchPhase.Moved)
            {
                lastDragDir = touchDeltaPosRaw;
                priorityAxe = GetPriorityOneDirAbs(touchDeltaPosRaw);
            }
        }

        /// <summary>
        /// Return drag icon for firs touched elment or null.
        /// </summary>
        public GameObject GetIconDrag()
        {
            if (firstSelected != null)
            {
                GameObject icon = firstSelected.GetDataIcon();
                return icon;
            }
            else
            {
                return null;
            }

        }

        private static Vector2 GetPriorityOneDirAbs(Vector2 sourceDir)
        {

            if (Mathf.Abs(sourceDir.x) > Mathf.Abs(sourceDir.y))
            {
                float x = (sourceDir.x > 0) ? 1 : 1;
                return new Vector2(x, 0f);
            }
            else
            {
                float y = (sourceDir.y > 0) ? 1 : 1;
                return new Vector2(0f, y);
            }
        }

        public static DirectionType GetDirType(Vector2 sourceDir)
        {
            Vector2 priorDir = GetPriorityOneDirAbs(sourceDir);
            if (priorDir.x != 0 && sourceDir.x > 0)
            {
                return DirectionType.Right;
            }
            else if (priorDir.x != 0 && sourceDir.x < 0)
            {
                return DirectionType.Left;
            }
            else if (priorDir.y != 0 && sourceDir.y > 0)
            {
                return DirectionType.Top;
            }
            else
            {
                return DirectionType.Bottom;
            }
        }
    }
}