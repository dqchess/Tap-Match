using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class GridCell : MonoBehaviour, ICustomMessageTarget
    {
        [SerializeField]
        private AudioClip fillSound;

        #region debug
        private bool debug = false;
        #endregion debug

        #region stroke
        [SerializeField]
        private Transform LeftTopCorner;
        [SerializeField]
        private Transform RightTopCorner;
        [SerializeField]
        private Transform LeftBotCorner;
        [SerializeField]
        private Transform RightBotCorner;
        [SerializeField]
        private Sprite Left;
        [SerializeField]
        private Sprite Right;
        [SerializeField]
        private Sprite Top;
        [SerializeField]
        private Sprite Bottom;
        [SerializeField]
        private Sprite OutTopLeft;
        [SerializeField]
        private Sprite OutBotLeft;
        [SerializeField]
        private Sprite OutTopRight;
        [SerializeField]
        private Sprite OutBotRight;

        [SerializeField]
        private Sprite InTopLeft;
        [SerializeField]
        private Sprite InBotLeft;
        [SerializeField]
        private Sprite InTopRight;
        [SerializeField]
        private Sprite InBotRight;

        #endregion stroke

        #region row column
        public Column<GridCell> GColumn { get; private set; }
        public Row<GridCell> GRow { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public List<Row<GridCell>> Rows { get; private set; }
        #endregion row column

        #region objects
        public GameObject DynamicObject
        {
            get
            {
                if (Match) return Match.gameObject;
                if (DynamicBlocker) return DynamicBlocker.gameObject;
                if (DynamicClickBomb) return DynamicClickBomb.gameObject;
                if (Falling) return Falling.gameObject;
                return null;
            }
        }
        public MatchObject Match { get { return   GetComponentInChildren<MatchObject>(); } }
        public FallingObject Falling { get { return GetComponentInChildren<FallingObject>(); } }
        public DynamicBlockerObject DynamicBlocker { get { return GetComponentInChildren<DynamicBlockerObject>(); } }
        public StaticBlockerObject StaticBlocker { get { return GetComponentInChildren<StaticBlockerObject>(); } }
        public OverlayObject Overlay { get; private set; }
        public BlockedObject Blocked { get; private set; }
        public UnderlayObject Underlay { get; private set; }
        public DynamicClickBombObject DynamicClickBomb { get { return GetComponentInChildren<DynamicClickBombObject>(); } }
        #endregion objects

        #region cache fields
        private BoxCollider2D coll2D;
        private SpriteRenderer sRenderer;
        #endregion cache fields

        #region events
        public Action<GridCell> PointerDownEvent;
        public Action<GridCell> DoubleClickEvent;
        public Action<GridCell> DragEnterEvent;
        #endregion events

        #region properties 
        /// <summary>
        /// Return true if mainobject and mainobject IsMatchedById || IsMatchedWithAny
        /// </summary>
        /// <returns></returns>
        public bool IsMatchable
        {
            get
            {
                if (!Overlay) return Match;
                return (Match && !Overlay.BlockMatch);
            }
        }

        public bool IsMixable
        {
            get
            {
                if(Match || DynamicClickBomb) return true ;
                return false;
            }
        }

        public bool IsTopCell { get { return Row == 0; } }

        /// <summary>
        /// Return true if gridcell has no dynamic object
        /// </summary>
        public bool IsDynamicFree
        {
            get { return !DynamicObject; }
        }

        public bool IsDisabled
        {
            get; private set;
        }

        /// <summary>
        /// Return true if gcell protected with Overlay && Overlay.BlockMatch
        /// </summary>
        public bool MatchProtected
        {
            get
            {
                if (Overlay && Overlay.BlockMatch && Overlay.Protection > 0) return true;
                return false;
            }
        }

        public bool HasBomb
        {
            get { return (DynamicClickBomb); }
        }

        public bool PhysStep { get; private set; }

        public NeighBors Neighbors;//{ get; private set; }

        public int AddRenderOrder { get { return (GRow != null && GColumn != null) ? (GColumn.Length - Row)*2 : 0;  } }

        private MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private GameObjectsSet GOSet { get { return MBoard.GOSet; } }
        private TouchManager Touch { get { return TouchManager.Instance; } }
        #endregion properties 

        #region temp
        private TweenSeq collectSequence;
        private MatchObject mObjectOld;
        private TweenSeq tS;

        private GameMode gMode;

        public PFCell pfCell;
        [Header("Fill Path to spawner")]
        public List<GridCell> fillPath;
        public Spawner spawner;

        private static Action<GameObject, float, Action> delayAction = (g, del, callBack) => { SimpleTween.Value(g, 0, 1, del).AddCompleteCallBack(callBack); };
        #endregion temp

        #region touchbehavior
        public void PointerDown(TouchPadEventArgs tpea)
        {
            PointerDownEvent?.Invoke(this);
        }

        public void Drag(TouchPadEventArgs tpea)
        {
           
        }

        public void DragBegin(TouchPadEventArgs tpea)
        {
           
        }

        public void DragDrop(TouchPadEventArgs tpea)
        {
          
        }

        public void DragEnter(TouchPadEventArgs tpea)
        {
           
        }

        public bool CanSwap(GridCell gCellOther)
        {
            if (!gCellOther) return false;
            if (!gCellOther.IsDraggable()) return false;
            if (IsDraggable() && Neighbors.Contain(gCellOther)) return true;
            return false;
        }

        public void DragExit(TouchPadEventArgs tpea)
        {
           
        }

        public void PointerUp(TouchPadEventArgs tpea)
        {
           
        }

        public GameObject GetDataIcon()
        {
            return gameObject;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public bool IsDraggable()
        {
            return false;
        }
        #endregion touchbehavior

        #region set mix grab clean
        internal void SetObject(int ID)
        {
            IsDisabled = false;
            MatchObjectData mOD = GOSet.GetMainObject(ID);
            if (mOD != null)
            {
                SetMatchObject(mOD);
                return;
            }

            OverlayObjectData oOD = GOSet.GetOverlayObject(ID);
            if (oOD != null)
            {
                SetOverlay(oOD);
                return;
            }

            UnderlayObjectData uOD = GOSet.GetUnderlayObject(ID);
            if (uOD != null)
            {
                SetUnderlay(uOD);
                return;
            }

            DynamicClickBombObjectData cdOD = GOSet.GetDynamicClickBombObject(ID);
            if (cdOD != null)
            {
                SetDynamicClickBomb(cdOD);
                return;
            }
            
            if (ID == GOSet.FallingObject.ID)
            {
                SetFalling(GOSet.FallingObject);
                return;
            }

            BaseObjectData bOD = GOSet.GetObject(ID);

            if (bOD != null && GameObjectsSet.IsDisabledObject(bOD.ID))
            {
                SetDisabledObject(bOD);
                return;
            }

            if (bOD != null && GameObjectsSet.IsBlockedObject(bOD.ID))
            {
                SetBlockedObject(bOD);
            }

            DynamicBlockerData dbOD = GOSet.GetDynamicBlockerObject(ID);
            if (dbOD != null)
            {
                SetDynamicBlockerObject(dbOD);
                return;
            }

            StaticBlockerData sbOD = GOSet.GetStaticBlockerObject(ID);
            if (sbOD != null)
            {
                SetStaticBlockerObject(sbOD);
                return;
            }

        }

        internal void SetDisabledObject(BaseObjectData bOD)
        {
            DestroyGridObjects();
            IsDisabled = true;
            if (gMode == GameMode.Play)
            {
                gameObject.SetActive(false);
            }
            else
            {
                sRenderer.sprite = bOD.ObjectImage;
            }
        }

        internal void SetBlockedObject(BaseObjectData bOD)
        {
            if (bOD == null || IsDisabled) { return; }
            DestroyGridObjects();
            Blocked = BlockedObject.Create(this, bOD, MBoard.TargetCollectEventHandler);  // sRenderer.sprite = bOD.ObjectImage;  Blocked = Creator.CreateSprite(transform, bOD.ObjectImage, transform.position, SortingOrder.Blocked).gameObject;
            Blocked.SetToFront(false);
        }

        internal void SetMatchObject(MatchObjectData mObjectData)
        {
            if (mObjectData == null || IsDisabled || Blocked) { return; }
            if (DynamicObject)
            {
                GameObject old = DynamicObject;
                DestroyImmediate(old);
            }
            if (StaticBlocker)
            {
                GameObject old = StaticBlocker.gameObject;
                DestroyImmediate(old);
            }
            MatchObject.Create(this, mObjectData, false, true, MBoard.TargetCollectEventHandler, MBoard.MatchScoreCollectHandler);
            Match.SetToFront(false);
        }

        internal void SetOverlay(OverlayObjectData oData)
        {
            if (oData == null || IsDisabled || Blocked) return;
            if (Overlay)
            {
                GameObject old = Overlay.gameObject;
                Destroy(old);
            }
            if (StaticBlocker)
            {
                GameObject old = StaticBlocker.gameObject;
                DestroyImmediate(old);
            }
            Overlay = OverlayObject.Create(this, oData, MBoard.TargetCollectEventHandler);
            Overlay.SetToFront(false);
        }

        internal void SetUnderlay(UnderlayObjectData mObjectData)
        {
            if (mObjectData == null || IsDisabled || Blocked) return;
            if (Underlay)
            {
                GameObject old = Underlay.gameObject;
                Destroy(old);
            }
            if (StaticBlocker)
            {
                GameObject old = StaticBlocker.gameObject;
                DestroyImmediate(old);
            }
            Underlay = UnderlayObject.Create(this, mObjectData, MBoard.TargetCollectEventHandler);
            Underlay.SetToFront(false);
        }

        internal void SetDynamicClickBomb(DynamicClickBombObjectData mObjectData)
        {
            if (mObjectData == null || IsDisabled || Blocked) { return; }
           // Debug.Log("set dynamic click bomb : " + mObjectData.ID);
            if (DynamicObject)
            {
                GameObject old = DynamicObject;
                DestroyImmediate(old);
            }
            if (StaticBlocker)
            {
                GameObject old = StaticBlocker.gameObject;
                DestroyImmediate(old);
            }
            DynamicClickBombObject.Create(this, mObjectData, false, true, MBoard.TargetCollectEventHandler);
            DynamicClickBomb.SetToFront(false);
        }

        internal void SetFalling(FallingObjectData mObjectData)
        {
            if (mObjectData == null || IsDisabled || Blocked) { return; }
          //  Debug.Log("set falling: " + mObjectData.ID);
            if (DynamicObject)
            {
                GameObject old = DynamicObject;
                DestroyImmediate(old);
            }
            if (StaticBlocker)
            {
                GameObject old = StaticBlocker.gameObject;
                DestroyImmediate(old);
            }
            FallingObject.Create(this, mObjectData, false, true, MBoard.TargetCollectEventHandler);//.gameObject;
            Falling.SetToFront(false);
        }

        internal void SetDynamicBlockerObject(DynamicBlockerData mObjectData)
        {
            if (mObjectData == null || IsDisabled || Blocked) { return; }
           // Debug.Log("set dynamic blocker: " + mObjectData.ID);
            if (DynamicObject)
            {
                GameObject old = DynamicObject;
                DestroyImmediate(old);
            }
            if (StaticBlocker)
            {
                GameObject old = StaticBlocker.gameObject;
                DestroyImmediate(old);
            }
            DynamicBlockerObject.Create(this, mObjectData, false, true, MBoard.TargetCollectEventHandler);
            DynamicBlocker.SetToFront(false);
        }

        internal void SetStaticBlockerObject(StaticBlockerData mObjectData)
        {
            if (mObjectData == null || IsDisabled || Blocked) { return; }
           // Debug.Log("set static blocker: " + mObjectData.ID);
            if (DynamicObject)
            {
                GameObject old = DynamicObject;
                DestroyImmediate(old);
            }
            if (StaticBlocker)
            {
                GameObject old = StaticBlocker.gameObject;
                DestroyImmediate(old);
            }

            StaticBlockerObject.Create(this, mObjectData, false, true, MBoard.TargetCollectEventHandler);
            StaticBlocker.SetToFront(false);
        }

        internal void MixJump(Vector3 pos, Action completeCallBack)
        {
            PhysStep = true;
            SimpleTween.Move(DynamicObject, transform.position, pos, 0.5f).AddCompleteCallBack(() =>
            {
                PhysStep = false;
                completeCallBack?.Invoke();
            }).SetEase(EaseAnim.EaseInSine);
        }

        internal void GrabDynamicObject(GameObject dObject, bool fast, Action completeCallBack)
        {
            if (dObject)
            {
                dObject.transform.parent = transform;
                if (!fast)
                    MoveTween(dObject, completeCallBack);
                else
                   FastMoveTween(dObject, completeCallBack);
                GridObject gO = dObject.GetComponent<GridObject>();
                if (gO) gO.SetToFront(false);
            }
            else
            {
                completeCallBack?.Invoke();
            }
        }

        private IEnumerator EndFill(bool isVert)
        {
            if (!PhysStep)
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
            }
            if (!PhysStep && DynamicObject)
            {
                AnimationCurve scaleCurve = MatchBoard.Instance.arcCurve;
                MSound.PlayClip(0.05f, fillSound);
                SimpleTween.Value(DynamicObject, 0.0f, 1f, 0.1f).SetEase(EaseAnim.EaseInSine).SetOnUpdate((float val) =>
                {
                    float t_scale = 1.0f + scaleCurve.Evaluate(val) * 0.1f;
                    if(DynamicObject) DynamicObject.transform.localScale = (isVert) ? new Vector3(t_scale, 2.0f - t_scale, 1) : new Vector3(2.0f - t_scale, t_scale, 1); //  mObject.SetLocalScaleX(t_scale); //  mObject.SetLocalScaleY(2.0f - t_scale);

                }).AddCompleteCallBack(() =>
                {
                    PhysStep = false;
                  //  completeCallBack?.Invoke();
                });
            }

        }

        /// <summary>
        /// Try to grab match object from fill path
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void FillGrab(Action completeCallBack)
        {
            GameObject mObject = null;
            GridCell gCell = null;

            if (spawner)
            {
                MatchObject mo = spawner.Get();
                mObject =(mo) ? mo.gameObject : null;
            }
            else
            {
                if (fillPath != null && fillPath.Count > 0)
                {
                    gCell = fillPath[0];
                }
                else // try to get at top
                {
                    gCell = GColumn.GetTopCell(Row);
                }
                mObject = gCell.DynamicObject; 
            }
            if (mObject && gCell && (gCell.PhysStep)) return;

            GrabDynamicObject(mObject, (MBoard.fillType == FillType.Fast), completeCallBack);
        }

        /// <summary>
        ///  mainObject = null;
        /// </summary>
        public void UnparentDynamicObject()
        {
           if(DynamicObject)  DynamicObject.transform.parent = null;
        }
        #endregion set mix grab

        #region grid objects behavior
        private void FastMoveTween(GameObject mObject, Action completeCallBack)
        {
            PhysStep = true;
            tS = new TweenSeq();
            Vector3 scale = transform.localScale;
            float tweenTime = 0.07f;
            float distK = Vector3.Distance(mObject.transform.position, transform.position) / MatchBoard.MaxDragDistance;

            Vector2 dPos = mObject.transform.position - transform.position;
            bool isVert = (Mathf.Abs(dPos.y) > Mathf.Abs(dPos.x));

            //move
            tS.Add((callBack) =>
            {
                SimpleTween.Move(mObject, mObject.transform.position, transform.position, tweenTime * distK).AddCompleteCallBack(() =>
                {
                    mObject.transform.position = transform.position;
                    PhysStep = false;
                    completeCallBack?.Invoke();
                    StartCoroutine(EndFill(isVert));
                    callBack();
                });
            });
            tS.Start();
        }

        private void MoveTween(GameObject mObject, Action completeCallBack)
        {
            PhysStep = true;
            tS = new TweenSeq();
            Vector3 scale = transform.localScale;
            float tweenTime = 0.07f;
            float distK = Vector3.Distance(mObject.transform.position, transform.position) / MatchBoard.MaxDragDistance;
            AnimationCurve scaleCurve = MatchBoard.Instance.arcCurve;

            Vector2 dPos = mObject.transform.position - transform.position;
            bool isVert = (Mathf.Abs(dPos.y) > Mathf.Abs(dPos.x));

            //move
            tS.Add((callBack) =>
            {
                SimpleTween.Move(mObject.gameObject, mObject.gameObject.transform.position, transform.position, tweenTime * distK).AddCompleteCallBack(() =>
                {
                    mObject.transform.position = transform.position;
                    callBack();
                }).SetEase(EaseAnim.EaseInSine);
            });

            //curve deform
            tS.Add((callBack) =>
            {
                SimpleTween.Value(mObject, 0.0f, 1f, 0.1f).SetEase(EaseAnim.EaseInSine).SetOnUpdate((float val) =>
                {
                    float t_scale = 1.0f + scaleCurve.Evaluate(val) * 0.1f;
                    mObject.transform.localScale = (isVert) ? new Vector3(t_scale, 2.0f - t_scale, 1) : new Vector3(2.0f - t_scale, t_scale, 1) ; //  mObject.SetLocalScaleX(t_scale); //  mObject.SetLocalScaleY(2.0f - t_scale);

                }).AddCompleteCallBack(() =>
                {
                    PhysStep = false;
                    completeCallBack?.Invoke();
                    callBack();
                });
            });

            tS.Start();
        }

        /// <summary>
        /// Show simple zoom sequence of main object
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void ZoomMatch(Action completeCallBack)
        {
            if (!Match)
            {
                completeCallBack?.Invoke();
                return;
            }

            Match.Zoom(completeCallBack);
        }

        /// <summary>
        /// Colect match object
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void CollectMatch(float delay, bool showPrefab, bool flyToGuiTarget, bool hitProtection, bool sideHitProtection, Action completeCallBack)
        {
            if (!Match)
            {
                completeCallBack?.Invoke();
                return;
            }

            #region match3
            /* match3
            if (HasBomb)
            {
                ExplodeBomb(delay, showBombExplode,  true,  true, false, completeCallBack);
            }
            else
            {
                Match.Collect(this, delay, showPrefab, flyToGuiTarget, hitProtection, sideHitProtection, completeCallBack);
            }
            */
            #endregion match3

            Match.Collect(this, delay, showPrefab, flyToGuiTarget, hitProtection, sideHitProtection, completeCallBack);
        }

        /// <summary>
        /// Play explode animation and explode area
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void ExplodeBomb(float delay,
            bool playExplodeAnimation,
            bool sequenced,
            bool showCollectPrefab, 
            bool collectFly,
            Action completeCallBack)
        {
            BombObject bomb = GetBomb();
            if (!bomb || MatchProtected)
            {
                completeCallBack?.Invoke();
                return;
            }

            if (Overlay && !Overlay.BlockMatch) DirectHitOverlay(null);

            bomb.transform.parent = null;

            if(playExplodeAnimation)
            bomb.PlayExplodeAnimation( this, delay, () =>
                {
                    bomb.ExplodeArea(this, 0, sequenced, showCollectPrefab, collectFly, true, completeCallBack);
                });
            else
            {
               bomb.ExplodeArea(this, 0, sequenced,  showCollectPrefab, collectFly, true, completeCallBack);
            }

        }
      
        /// <summary>
        /// Move match to gridcell and collect
        /// </summary>
        /// <param name="bombCell"></param>
        /// <param name="completeCallBack"></param>
        internal void MoveMatchAndCollect(GridCell toCell, float delay, bool showPrefab, bool fly, bool hitProtection, bool showBombExplode, Action completeCallBack)
        {
            if (!Match)
            {
                completeCallBack?.Invoke();
                return;
            }

          Match.MoveMatchToBomb(this, toCell, delay, hitProtection , ()=> {
                  CollectMatch(0, showPrefab, fly, false, false,  completeCallBack);
          });
        }

        public static void ExplodeCell(GridCell gCell, float delay, bool showPrefab, bool fly, bool hitProtection, Action completeCallBack)
        {
            if (gCell.GetBomb() && !gCell.MatchProtected)
            {
                gCell.ExplodeBomb(delay, true, true, true, false, completeCallBack);
                return;
            }

            if (gCell.Overlay && gCell.Overlay.BlockMatch)
            {
                delayAction(gCell.gameObject, delay, ()=> { gCell.DirectHitOverlay(null); completeCallBack?.Invoke(); });
                return;
            }

            if (gCell.DynamicBlocker)
            {
                delayAction(gCell.gameObject, delay, () =>
                {
                    gCell.DirectHitBlocker(null);
                    if (gCell.Overlay) gCell.DirectHitOverlay(null);
                    completeCallBack?.Invoke();
                });
                return;
            }

            if (gCell.StaticBlocker)
            {
                delayAction(gCell.gameObject, delay, () =>
                {
                    gCell.DirectHitBlocker(null);
                    if (gCell.Overlay) gCell.DirectHitOverlay(null);
                    completeCallBack?.Invoke();
                });
                return;
            }

            if (gCell.Match && gCell.IsMatchable)
            {
                gCell.Match.Explode(gCell, showPrefab, fly, hitProtection, hitProtection, delay, completeCallBack);
                return;
            }

            if (gCell.Underlay)
            {
                delayAction(gCell.gameObject, delay, () => { gCell.DirectHitUnderlay(null); completeCallBack?.Invoke(); });
                return;
            }

            completeCallBack?.Invoke();
        }

        ///// <summary>
        ///// play donuts aroma
        ///// </summary>
        ///// <param name="completeCallBack"></param>
        //internal void PlayIddle()
        //{
        //    if (!Match)
        //    {
        //        return;
        //    }
        //    Creator.InstantiateAnimPrefab(Match.OData.iddleAnimPrefab, Match.transform, Match.transform.position,  SortingOrder.MainIddle, true, null);
        //}

        /// <summary>
        /// Side hit neighbourn from collected
        /// </summary>
        internal void DirectHitOverlay(Action completeCallBack)
        {
            if (Overlay && !Overlay.BlockMatch)
            {
                Overlay.Hit(this,completeCallBack);
                return;
            }
            completeCallBack?.Invoke();
        }

        /// <summary>
        /// Side hit neighbourn from collected
        /// </summary>
        internal void SideHit(Action completeCallBack)
        {
            if (Overlay && Overlay.BlockMatch)
            {
                Overlay.Hit(this, completeCallBack);
                return;
            }

            if (DynamicBlocker && DynamicBlocker.OData.sideHit)
            {
                DynamicBlocker.Hit(this, completeCallBack);
                return;
            }

            if (StaticBlocker && StaticBlocker.OData.sideHit)
            {
                StaticBlocker.Hit(this, completeCallBack);
                return;
            }

            completeCallBack?.Invoke();
        }

        /// <summary>
        /// Side hit neighbourn from collected
        /// </summary>
        internal void DirectHitUnderlay(Action completeCallBack)
        {
            if (Underlay)
            {
                Underlay.Hit(this, completeCallBack);
                return;
            }
            completeCallBack?.Invoke();
        }

        /// <summary>
        /// Direct explode hit 
        /// </summary>
        private void DirectHitBlocker(Action completeCallBack)
        {
            if (DynamicBlocker && DynamicBlocker.OData.directHit)
            {
                DynamicBlocker.Hit(this, completeCallBack);
                return;
            }

            if (StaticBlocker && StaticBlocker.OData.directHit)
            {
                StaticBlocker.Hit(this, completeCallBack);
                return;
            }
            completeCallBack?.Invoke();
        }
        #endregion matchobject behavior

        /// <summary>
        ///  used by instancing for cache data
        /// </summary>
        internal void Init(int cellRow, int cellColumn, Column<GridCell> column, Row<GridCell> row, GameMode gMode)
        {
            IsDisabled = false;
            Row = cellRow;
            Column = cellColumn;
            GColumn = column;
            GRow = row;
            this.gMode = gMode;
#if UNITY_EDITOR
            name = ToString();
#endif
            sRenderer = GetComponent<SpriteRenderer>();
            sRenderer.sortingOrder = SortingOrder.Base;
            Neighbors = new NeighBors(this, false);
        }

        /// <summary>
        ///  return true if  match objects of two cells are equal
        /// </summary>
        internal bool IsMatchObjectEquals(GridCell other)
        {
            if (other == null) return false; 
            if (Match == null) return false;
            return Match.Equals(other.Match);
        }

        /// <summary>
        ///  cancel any tween on main MainObject object
        /// </summary>
        internal void CancelTween()
        {
         //   Debug.Log("Cancel tween");
            if (DynamicObject)
            {
                SimpleTween.Cancel(DynamicObject, false);
                DynamicObject.transform.localScale = Vector3.one;
                DynamicObject.transform.position = transform.position;
            }
            if (Match)
            {
                Match.CancellTweensAndSequences();
                Match.ResetTween();
            }
            if (mObjectOld)
            {
                mObjectOld.CancellTweensAndSequences();
            }
        }

        /// <summary>
        /// DestroyImeediate MainObject, OverlayProtector, UnderlayProtector
        /// </summary>
        internal void DestroyGridObjects()
        {
            if (DynamicObject)
            {
                DestroyImmediate(DynamicObject);
            } 
            if (Overlay) { DestroyImmediate(Overlay.gameObject); Overlay = null; }
            if (Underlay) { DestroyImmediate(Underlay.gameObject); Underlay = null; }
            if(Blocked) {  DestroyImmediate(Blocked.gameObject); Blocked = null;  }
            if (StaticBlocker) { DestroyImmediate(StaticBlocker.gameObject); }
        }

        public BombObject GetBomb()
        {
            if (DynamicClickBomb) return DynamicClickBomb;
            return null;
        } 

        public override string ToString()
        {
            return "cell : [ row: " + Row + " , col: " + Column + "]";
        }

        public bool HaveObjectWithID(int id)
        {
            if (Match && Match.GetID() == id) return true;
            if (Falling && Falling.GetID() == id) return true;
            if (Overlay && Overlay.GetID() == id) return true;
            if (Underlay && Underlay.GetID() == id) return true;
            if (DynamicClickBomb && DynamicClickBomb.GetID() == id) return true;
            return false;
        }

        public void CreateBorder()
        {
            if(Left && LeftBotCorner)
            {
                if (!Neighbors.Left || Neighbors.Left.IsDisabled)
                {
                    SpriteRenderer srL = Creator.CreateSprite(transform, Left, new Vector3(LeftBotCorner.position.x, transform.position.y, transform.position.z), 1);
                    srL.name = "Left border: " + ToString();
                }
            }
            if (Right && RightBotCorner)
            {
                if (!Neighbors.Right || Neighbors.Right.IsDisabled)
                {
                    SpriteRenderer srR = Creator.CreateSprite(transform, Right, new Vector3(RightBotCorner.position.x, transform.position.y, transform.position.z), 1);
                    srR.name = "Right border: " + ToString();
                }
            }
            if (Top && RightTopCorner)
            {
                if (!Neighbors.Top || Neighbors.Top.IsDisabled)
                {
                    SpriteRenderer srT = Creator.CreateSprite(transform, Top, new Vector3(transform.position.x, RightTopCorner.position.y, transform.position.z), 1);
                    srT.name = "Top border: " + ToString();
                }
            }
            if (Bottom && RightBotCorner)
            {
                if (!Neighbors.Bottom || Neighbors.Bottom.IsDisabled)
                {
                    SpriteRenderer srB = Creator.CreateSprite(transform, Bottom, new Vector3(transform.position.x, RightBotCorner.position.y, transform.position.z), 1);
                    srB.name = "Bottom border: " + ToString();
                }
            }

            if(OutTopLeft && LeftTopCorner)
            {
                if ((!Neighbors.Left || Neighbors.Left.IsDisabled) && (!Neighbors.Top || Neighbors.Top.IsDisabled))
                {
                    SpriteRenderer srTL = Creator.CreateSprite(transform, OutTopLeft, LeftTopCorner.position, 1);
                    srTL.name = "OutTopLeft border: " + ToString(); 
                }
            }

            if (OutBotLeft && LeftBotCorner)
            {
                if ((!Neighbors.Left || Neighbors.Left.IsDisabled) && (!Neighbors.Bottom || Neighbors.Bottom.IsDisabled))
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, OutBotLeft, LeftBotCorner.position, 1);
                    sr.name = "OutBotLeft border: " + ToString();
                }
            }

            if (OutBotRight && RightBotCorner)
            {
                if ((!Neighbors.Right || Neighbors.Right.IsDisabled) && (!Neighbors.Bottom || Neighbors.Bottom.IsDisabled))
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, OutBotRight, RightBotCorner.position, 1);
                    sr.name = "OutBotLeft border: " + ToString();
                }
            }
            if (OutTopRight && RightTopCorner)
            {
                if ((!Neighbors.Right || Neighbors.Right.IsDisabled) && (!Neighbors.Top || Neighbors.Top.IsDisabled))
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, OutTopRight, RightTopCorner.position, 1);
                    sr.name = "OutBotLeft border: " + ToString();
                }
            }

            NeighBors n = new NeighBors(this, true);
            if (InTopLeft && LeftTopCorner)
            {
                if ((!Neighbors.Left || Neighbors.Left.IsDisabled) && n.TopLeft && !n.TopLeft.IsDisabled)
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, InTopLeft, LeftTopCorner.position, 2);
                    sr.name = "InTopLeft border: " + ToString();
                }
            }

            if (InBotLeft && LeftBotCorner)
            {
                if ((!Neighbors.Left || Neighbors.Left.IsDisabled) && n.BottomLeft && !n.BottomLeft.IsDisabled)
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, InBotLeft, LeftBotCorner.position, 2);
                    sr.name = "InBotLeft border: " + ToString();
                }
            }

            if (InTopRight && RightTopCorner)
            {
                if ((!Neighbors.Right || Neighbors.Right.IsDisabled) && n.TopRight && !n.TopRight.IsDisabled)
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, InTopRight, RightTopCorner.position, 2);
                    sr.name = "InTopRight border: " + ToString();
                }
            }

            if (InBotRight && RightBotCorner)
            {
                if ((!Neighbors.Right || Neighbors.Right.IsDisabled) && n.BottomRight && !n.BottomRight.IsDisabled)
                {
                    SpriteRenderer sr = Creator.CreateSprite(transform, InBotRight, RightBotCorner.position, 2);
                    sr.name = "InBotRight border: " + ToString();
                }
            }

        }

        /// <summary>
        /// return true if have fillpath to spawner or (if fillpath == null) have dynamic object at top 
        /// </summary>
        /// <returns></returns>
        public bool HaveFillPath()
        {
            GridCell top =  GColumn.GetTopCell(Row);
            return ((!Blocked && !IsDisabled && !StaticBlocker && spawner) 
                || (!Blocked && !IsDisabled && !StaticBlocker && fillPath != null && fillPath.Count > 0)) 
                || (!Blocked && !IsDisabled && !StaticBlocker  && !top.Blocked && !top.IsDisabled && top.DynamicObject);
        }

        private bool PathIsFull()
        {
            if (fillPath == null) return false;
            foreach (var item in fillPath)
            {
                if (!item.DynamicObject) return false;
            }
            return true;
        }
    }
}