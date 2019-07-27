using UnityEngine;
using System;
using System.Collections.Generic;

namespace Mkey
{
    [CreateAssetMenu]
    public class LevelConstructSet : BaseScriptable
    {
        [SerializeField]
        private int vertSize = 8;
        [SerializeField]
        private int horSize = 8;
        [SerializeField]
        private float distX = 0.0f;
        [SerializeField]
        private float distY = 0.0f;

        [SerializeField]
        private float scale = 0.9f;
        [SerializeField]
        private int backGroundNumber = 0;


        public int BackGround
        {
            get { return backGroundNumber; }

        }

        public MissionConstruct levelMission;

        public int VertSize
        {
            get { return vertSize; }
            set
            {
                if (value < 1) value = 1;
                vertSize = value;
                SetAsDirty();
            }
        }

        public int HorSize
        {
            get { return horSize; }
            set
            {
                if (value < 1) value = 1;
                horSize = value;
                SetAsDirty();
            }
        }

        public float DistX
        {
            get { return distX; }
            set
            {
                distX = RoundToFloat(value, 0.05f);
                SetAsDirty();
            }
        }

        public float DistY
        {
            get { return distY; }
            set
            {
                distY = RoundToFloat(value, 0.05f);
                SetAsDirty();
            }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                if (value < 0) value = 0;
                scale = RoundToFloat(value, 0.05f);
                SetAsDirty();
            }
        }

        [SerializeField]
        public List<int> matchObjects;
        [HideInInspector]
        [SerializeField]
        public List<CellData> blockedCells;
        [SerializeField]
        public List<CellData> disabledCells;
        [HideInInspector]
        [SerializeField]
        public List<CellData> featuredCells;
        [SerializeField]
        public List<CellData> fallingCells;
        [SerializeField]
        public List<CellData> overlayCells;
        [HideInInspector]
        [SerializeField]
        public List<CellData> underlayCells;
        [SerializeField]
        public List<CellData> dynamicBlockerCells;
        [SerializeField]
        public List<CellData> staticBlockerCells;

        #region regular
        void OnEnable()
        {
            // Debug.Log("onenable " + ToString());
            if (levelMission == null) levelMission = new MissionConstruct();
            levelMission.SaveEvent = SetAsDirty;

        }

        void Awake()
        {
            // Debug.Log("awake " + ToString());
            //if (levelMission == null) levelMission = new MissionConstruct();
            //levelMission.SaveEvent = SetAsDirty;

        }
        #endregion regular

        public void AddFeaturedCell(CellData cd)
        {
            if (featuredCells == null) featuredCells = new List<CellData>();
            RemoveCellData(featuredCells, cd);
            RemoveCellData(blockedCells, cd);
            RemoveCellData(disabledCells, cd);
            RemoveCellData(fallingCells, cd);
            RemoveCellData(dynamicBlockerCells, cd);
            RemoveCellData(staticBlockerCells, cd);
            featuredCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveFeaturedCell(CellData cd)
        {
            if (featuredCells == null) featuredCells = new List<CellData>();
            RemoveCellData(featuredCells, cd);
            SetAsDirty();
        }

        public void AddFallingCell(CellData cd)
        {
            if (fallingCells == null) fallingCells = new List<CellData>();
            RemoveCellData(featuredCells, cd);
            RemoveCellData(blockedCells, cd);
            RemoveCellData(disabledCells, cd);
            RemoveCellData(fallingCells, cd);
            RemoveCellData(dynamicBlockerCells, cd);
            RemoveCellData(staticBlockerCells, cd);
            fallingCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveFallingCell(CellData cd)
        {
            if (fallingCells == null) fallingCells = new List<CellData>();
            RemoveCellData(fallingCells, cd);
            SetAsDirty();
        }

        public void AddDynamicBlockerCell(CellData cd)
        {
            if (dynamicBlockerCells == null) dynamicBlockerCells = new List<CellData>();
            RemoveCellData(featuredCells, cd);
            RemoveCellData(blockedCells, cd);
            RemoveCellData(disabledCells, cd);
            RemoveCellData(fallingCells, cd);
            RemoveCellData(dynamicBlockerCells, cd);
            RemoveCellData(staticBlockerCells, cd);
            dynamicBlockerCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveDynamicBlockerCell(CellData cd)
        {
            if (dynamicBlockerCells == null) dynamicBlockerCells = new List<CellData>();
            RemoveCellData(dynamicBlockerCells, cd);
            SetAsDirty();
        }

        public void AddStaticBlockerCell(CellData cd)
        {
            if (staticBlockerCells == null) staticBlockerCells = new List<CellData>();
            RemoveCellData(featuredCells, cd);
            RemoveCellData(blockedCells, cd);
            RemoveCellData(disabledCells, cd);
            RemoveCellData(fallingCells, cd);
            RemoveCellData(dynamicBlockerCells, cd);
            RemoveCellData(staticBlockerCells, cd);
            staticBlockerCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveStaticBlockerCell(CellData cd)
        {
            if (staticBlockerCells == null) staticBlockerCells = new List<CellData>();
            RemoveCellData(staticBlockerCells, cd);
            SetAsDirty();
        }

        public void AddDisabledCell(CellData cd)
        {
            if (disabledCells == null) disabledCells = new List<CellData>();
            RemoveCellData(cd);
            disabledCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveDisabledCell(CellData cd)
        {
            if (disabledCells == null) disabledCells = new List<CellData>();
            RemoveCellData(disabledCells, cd);
            SetAsDirty();
        }

        public void AddBlockedCell(CellData cd)
        {
            if (blockedCells == null) blockedCells = new List<CellData>();
            RemoveCellData(cd);
            blockedCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveBlockedCell(CellData cd)
        {
            if (blockedCells == null) blockedCells = new List<CellData>();
            RemoveCellData(blockedCells, cd);
            SetAsDirty();
        }

        public void AddOverlayCell(CellData cd)
        {
            if (overlayCells == null) overlayCells = new List<CellData>();
            RemoveCellData(overlayCells, cd);
            RemoveCellData(blockedCells, cd);
            RemoveCellData(disabledCells, cd);
            overlayCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveOverlayCell(CellData cd)
        {
            if (overlayCells == null) overlayCells = new List<CellData>();
            RemoveCellData(overlayCells, cd);
            SetAsDirty();
        }

        public void AddUnderlayCell(CellData cd)
        {
            if (underlayCells == null) underlayCells = new List<CellData>();
            RemoveCellData(underlayCells, cd);
            RemoveCellData(blockedCells, cd);
            RemoveCellData(disabledCells, cd);
            underlayCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveUnderlayCell(CellData cd)
        {
            if (underlayCells == null) underlayCells = new List<CellData>();
            RemoveCellData(underlayCells, cd);
            SetAsDirty();
        }

        public void AddMatch(int id)
        {
            if (matchObjects == null) matchObjects = new List<int>();
            if (matchObjects.Contains(id))
            {
                matchObjects.Remove(id);
            }
            else
            {
                matchObjects.Add(id);
            }
            SetAsDirty();
        }

        public bool ContainMatch(int id)
        {
            if (matchObjects!=null && matchObjects.Contains(id))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove all non-existent cells data from board
        /// </summary>
        /// <param name="gOS"></param>
        public void Clean(GameObjectsSet gOS)
        {
            Action<List<CellData>> cAction = (arr) => 
            {
                if (arr != null)
                {
                    arr.RemoveAll((c) =>
                    {
                        return ((c.Column >= horSize) && (c.Row >= vertSize));
                    });

                    if(gOS)
                    arr.RemoveAll((c) =>
                    {
                        return (!gOS.ContainID(c.ID));
                    });
                }
            };
            cAction(featuredCells);
            cAction(blockedCells);
            cAction(overlayCells);
            cAction(disabledCells);
            cAction(underlayCells);
            cAction(dynamicBlockerCells);
            cAction(staticBlockerCells);
           if(matchObjects!=null) matchObjects.RemoveAll((m)=> { return !gOS.ContainMatchID(m); });
            SetAsDirty();
        }

        public void IncBackGround()
        {
            backGroundNumber++;
            //  if (backGroundNumber >= mSet.BackGroundsCount || backGroundNumber < 0) backGroundNumber = 0;
            Save();
        }

        public void DecBackGround()
        {
            backGroundNumber--;
            //   if (backGroundNumber >= mSet.BackGroundsCount) backGroundNumber = 0;
            //   else if (backGroundNumber < 0) backGroundNumber = mSet.BackGroundsCount - 1;
            Save();
        }

        private float RoundToFloat(float val, float delta)
        {
            int vi = Mathf.RoundToInt(val / delta);
            return (float)vi * delta;
        }

        private void RemoveCellData(List<CellData> cdl, CellData cd)
        {
            if (cdl != null) cdl.RemoveAll((c) => { return ((cd.Column == c.Column) && (cd.Row == c.Row)); });
        }

        /// <summary>
        /// Remove celldata overlay -> disabled
        /// </summary>
        /// <param name="cd"></param>
        public void RemoveCellData(CellData cd)
        {
            RemoveCellData(overlayCells, cd);
            RemoveCellData(featuredCells, cd);
            RemoveCellData(underlayCells, cd);
            RemoveCellData(blockedCells, cd);
            RemoveCellData(disabledCells, cd);
            RemoveCellData(fallingCells, cd);
            RemoveCellData(staticBlockerCells, cd);
            RemoveCellData(dynamicBlockerCells, cd);
        }

        private bool ContainCellData(List<CellData> lcd, CellData cd)
        {
            if (lcd == null || cd == null) return false;
            foreach (var item in lcd)
            {
                if ((item.Row == cd.Row) && (item.Column == cd.Column)) return true; 
            }
            return false;
        }
    }
}



