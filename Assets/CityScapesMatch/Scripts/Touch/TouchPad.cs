using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Mkey
{
    public class TouchPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IBeginDragHandler, IDropHandler, IPointerExitHandler
    {
        public List<Collider2D> hitList;
        public List<Collider2D> newHitList;
        private TouchPadEventArgs tpea;

        private bool touched = false;
        [SerializeField]
        private bool isActive = true;
        private int pointerID;
        private Vector2 screenTouchPos;
        private Vector2 oldPosition;
        public bool dlog = true;
        public static TouchPad Instance;

        /// <summary>
        /// Return true if touchpad is touched with mouse or finger
        /// </summary>
        public bool IsTouched
        {
            get
            {
                return touched;
            }
        }

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            hitList = new List<Collider2D>();
            newHitList = new List<Collider2D>();
        }

        #region handlers
        public void OnPointerDown(PointerEventData data)
        {
            if (isActive)
            {
                if (!touched)
                {
                    if (dlog) Debug.Log("----------------POINTER Down--------------( " + data.pointerId);
                    touched = true;
                    tpea = new TouchPadEventArgs();
                    screenTouchPos = data.position;
                    oldPosition = screenTouchPos;
                    pointerID = data.pointerId;

                    tpea.SetTouch(screenTouchPos, Vector2.zero, TouchPhase.Began);
                    hitList = new List<Collider2D>();
                    hitList.AddRange(tpea.hits);
                    if (hitList.Count > 0)
                    {
                        for (int i = 0; i < hitList.Count; i++)
                        {
                            ExecuteEvents.Execute<ICustomMessageTarget>(hitList[i].transform.gameObject, null, (x, y) => x.PointerDown(tpea));
                            // if (tpea.firstSelected == null)  tpea.firstSelected = hitList[i].transform.gameObject.GetInterface<ICustomMessageTarget>();
                        }
                    }
                }
            }
        }

        public void OnBeginDrag(PointerEventData data)
        {
            if (isActive)
            {
                if (data.pointerId == pointerID)
                {
                    if (dlog) Debug.Log("----------------BEGIN DRAG--------------( " + data.pointerId);
                    screenTouchPos = data.position;

                    tpea.SetTouch(screenTouchPos, screenTouchPos - oldPosition, TouchPhase.Moved);
                    oldPosition = screenTouchPos;

                    //0 ---------------------------------- send drag begin message --------------------------------------------------
                    for (int i = 0; i < hitList.Count; i++)
                    {
                        if (hitList[i]) ExecuteEvents.Execute<ICustomMessageTarget>(hitList[i].transform.gameObject, null, (x, y) => x.DragBegin(tpea));
                    }
                }
            }
        }

        public void OnDrag(PointerEventData data)
        {
            if (isActive)
            {
                if (data.pointerId == pointerID)
                {
                    if (dlog) Debug.Log("---------------- ONDRAG --------------( " + data.pointerId + " : " + pointerID);
                    TouchManager.Instance.Drag(tpea);
                    screenTouchPos = data.position;

                    tpea.SetTouch(screenTouchPos, screenTouchPos - oldPosition, TouchPhase.Moved);
                    oldPosition = screenTouchPos;

                    newHitList = new List<Collider2D>(tpea.hits); // garbage

                    //1 ------------------ send drag exit message and drag message --------------------------------------------------
                    foreach (Collider2D cHit in hitList)
                    {
                        if (newHitList.IndexOf(cHit) == -1)
                        {
                            if (cHit) ExecuteEvents.Execute<ICustomMessageTarget>(cHit.transform.gameObject, null, (x, y) => x.DragExit(tpea));
                        }
                        else
                        {
                            if (cHit) ExecuteEvents.Execute<ICustomMessageTarget>(cHit.transform.gameObject, null, (x, y) => x.Drag(tpea));
                        }

                    }

                    //2 ------------------ send drag enter message -----------------------------------------------------------------
                    for (int i = 0; i < newHitList.Count; i++)
                    {
                        if (hitList.IndexOf(newHitList[i]) == -1)
                        {
                            if (newHitList[i]) ExecuteEvents.Execute<ICustomMessageTarget>(newHitList[i].gameObject, null, (x, y) => x.DragEnter(tpea));
                        }
                    }

                    hitList = newHitList;
                }
            }
        }

        public void OnPointerUp(PointerEventData data)
        {

            //  if (isActive)
            {
                if (dlog) Debug.Log("----------------POINTER UP--------------( " + data.pointerId + " : " + pointerID);
                if (data.pointerId == pointerID)
                {

                    screenTouchPos = data.position;
                    tpea.SetTouch(screenTouchPos, screenTouchPos - oldPosition, TouchPhase.Ended);
                    oldPosition = screenTouchPos;

                    touched = false;
                    foreach (Collider2D cHit in hitList)
                    {
                        if (cHit) ExecuteEvents.Execute<ICustomMessageTarget>(cHit.transform.gameObject, null, (x, y) => x.PointerUp(tpea));
                    }

                    newHitList = new List<Collider2D>(tpea.hits);
                    foreach (Collider2D cHit in newHitList)
                    {
                        if (hitList.IndexOf(cHit) == -1)
                        {
                            if (cHit) ExecuteEvents.Execute<ICustomMessageTarget>(cHit.transform.gameObject, null, (x, y) => x.PointerUp(tpea));
                        }
                        if (cHit) ExecuteEvents.Execute<ICustomMessageTarget>(cHit.transform.gameObject, null, (x, y) => x.DragDrop(tpea));
                    }
                    if (dlog) Debug.Log("clear lists");
                    hitList = new List<Collider2D>();
                    newHitList = new List<Collider2D>();
                }
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (isActive)
            {
                if (data.pointerId == pointerID)
                {
                    if (dlog) Debug.Log("----------------POINTER EXIT--------------( " + data.pointerId + " : " + pointerID);
                    screenTouchPos = data.position;
                    tpea.SetTouch(screenTouchPos, screenTouchPos - oldPosition, TouchPhase.Ended);
                    oldPosition = screenTouchPos;

                    touched = false;
                    foreach (Collider2D cHit in hitList)
                    {
                        if (cHit) ExecuteEvents.Execute<ICustomMessageTarget>(cHit.transform.gameObject, null, (x, y) => x.PointerUp(tpea));
                    }

                    newHitList = new List<Collider2D>(tpea.hits);
                    foreach (Collider2D cHit in newHitList)
                    {
                        if (hitList.IndexOf(cHit) == -1)
                        {
                            if (cHit) ExecuteEvents.Execute<ICustomMessageTarget>(cHit.transform.gameObject, null, (x, y) => x.PointerUp(tpea));
                        }
                        if (cHit) ExecuteEvents.Execute<ICustomMessageTarget>(cHit.transform.gameObject, null, (x, y) => x.DragDrop(tpea));
                    }
                    hitList = new List<Collider2D>();
                    newHitList = new List<Collider2D>();
                }
            }
        }

        public void OnDrop(PointerEventData data)
        {
            if (isActive)
            {
                if (data.pointerId == pointerID)
                {
                    if (dlog) Debug.Log("----------------ONDROP--------------( " + data.pointerId + " : " + pointerID);
                }
            }

        }
        #endregion handlers

        /// <summary>
        /// Return world position of touch.
        /// </summary>
        public Vector3 GetWorldTouchPos()
        {
            return Camera.main.ScreenToWorldPoint(screenTouchPos);
        }

        /// <summary>
        /// Enable or disable touch pad callbacks handling.
        /// </summary>
        internal void SetTouchActivity(bool activity)
        {
            isActive = activity;
            if (dlog) Debug.Log("touch");
        }

    }
}