using System;
using UnityEngine;

namespace Mkey
{
    public class Booster
    {
        public BoosterObjectData bData;
        public static Booster ActiveBooster { get; private set; }
        public Action  FooterClickEvent;          
        public Action  ChangeCountEvent;
        public Action <Booster> ChangeUseEvent;
        public Action  ActivateEvent;
        public Action <Booster> DeActivateEvent;
        private string saveKey; // = savePrefix + "booster" + bData.ID.ToString();
        private bool saveData;

        public GameObject SceneObject
        {
            get;private set;
        }

        private FooterBoosterHelper footerBooster;
        private SoundMaster Sound { get { return SoundMaster.Instance; } }

        #region properties
        /// <summary>
        /// Show in footer or no
        /// </summary>
        public bool Use
        {
            get; private set;
        }

        public int Count
        {
            get; private set;
        }

        /// <summary>
        /// return true if this booster == activebooster
        /// </summary>
        public bool IsActive
        {
            get { return this == ActiveBooster; }
        } 
        #endregion properties

        /// <summary>
        /// Create new booster object and set saved count (if (saveData ==true))
        /// </summary>
        /// <param name="bData"></param>
        /// <param name="saveData"></param>
        public Booster(BoosterObjectData bData, bool saveData, string savePrefix)
        {
            this.bData = bData;
            this.saveData = saveData;

            Count = 0;
            if (string.IsNullOrEmpty(savePrefix)) savePrefix = "";

            saveKey = savePrefix + "booster" + bData.ID.ToString();
            if (saveData)
            {
                if (!PlayerPrefs.HasKey(saveKey) || PlayerPrefs.GetInt(saveKey) < 0)
                {
                    PlayerPrefs.SetInt(saveKey, 0);
                }
                SetCount(PlayerPrefs.GetInt(saveKey));
            }
            else
            {
                SetCount(0);
            }
            Debug.Log("Create - " + ToString());

         //   Use = true;
        }

        /// <summary>
        /// Return field image
        /// </summary>
        /// <returns></returns>
        public Sprite GetImage()
        {
            return bData.ObjectImage;
        }

        /// <summary>
        /// Retun gui sprite
        /// </summary>
        /// <returns></returns>
        public Sprite GetGUIImage()
        {
            return bData.GuiImage;
        }

        #region change count
        /// <summary>
        /// Increase count and refresh gui
        /// </summary>
        public void AddCount(int count)
        {
           SetCount(Count + count);
        }

        /// <summary>
        /// Increase count and refresh gui
        /// </summary>
        public void SetCount(int count)
        {
            count = Mathf.Max(0,  count);
            bool changed = (count != Count);
            Count = count;

            if (changed)
            {
                Save();
                ChangeCountEvent?.Invoke();
            }
        }
        #endregion change count

        public override string ToString()
        {
            return (bData.Name + " : " + Count.ToString() + " items" );
        }

        /// <summary>
        /// Use PlayerPrefs to save count.
        /// </summary>
        private void Save()
        {
            if (saveData)
            {
                PlayerPrefs.SetInt(saveKey, Count);
            }
        }

        #region handlers
        private void FooterClickEventHandler()
        {
            if (!IsActive && Count > 0)     // activate booster
            {
                ActiveBooster?.DeActivateBooster();
                ActivateBooster();
            }
            else if (IsActive)              // deactivate booster
            {
                DeActivateBooster();
            }
            else if (!IsActive && Count == 0 && ActiveBooster != null) // open shop  
            {
                ActiveBooster.DeActivateBooster();
            }
        }
        #endregion handlers

        public void ActivateBooster()
        {
            CreateSceneObject(null, footerBooster.instantiatePosition ? footerBooster.instantiatePosition : footerBooster.GetComponent<RectTransform>());
            ActiveBooster = this;
            Debug.Log(ToString() + " activated   ");
            if (Apply()) { AddCount(-1); DeActivateBooster(); return; }
            ActivateEvent?.Invoke();
        }

        /// <summary>
        /// Set  ActiveBooster = null, raise DeActivateEvent
        /// </summary>
        public void DeActivateBooster()
        {
            if (SceneObject) UnityEngine.Object.Destroy(SceneObject);
            ActiveBooster = null;
            Debug.Log(ToString() + " deactivated ");
            DeActivateEvent?.Invoke(this);
        }

        /// <summary>
        /// Create new footer booster instance 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="prefab"></param>
        /// <param name="boost"></param>
        /// <param name="GotoShopHandler"></param>
        /// <returns></returns>
        public FooterBoosterHelper CreateFooterBooster(RectTransform parent, FooterBoosterHelper prefab, Action GotoShopHandler)
        {
            Debug.Log("create footer booster");
            footerBooster = UnityEngine.Object.Instantiate(prefab);
            footerBooster.transform.localScale = parent.transform.lossyScale;
            footerBooster.transform.SetParent(parent);
            footerBooster.boosterImage.sprite = bData.GuiImage;
            footerBooster.boosterCounter.text = Count.ToString();

            // add footer click handlers
            FooterClickEvent = () => { if (GotoShopHandler != null && Count == 0) GotoShopHandler(); };
            FooterClickEvent += FooterClickEventHandler;
            footerBooster.boosterButton.onClick.AddListener(FooterClickEvent.Invoke);

            footerBooster.booster = this;
            footerBooster.InitStart();
            return footerBooster;
        }

        private void CreateSceneObject( Transform parent, RectTransform guiObject)
        {
            SceneObject = UnityEngine.Object.Instantiate(bData.prefab).gameObject;
            Vector3 wPos = Vector3.zero;

            if (guiObject != null)
            {
                wPos = guiObject.transform.position; //Coordinats.CanvasToWorld(guiObject.gameObject);
            }

            SceneObject.transform.position = wPos;
            SceneObject.transform.parent = parent;
            SpriteRenderer sr = SceneObject.GetOrAddComponent<SpriteRenderer>();
            SceneObject.GetComponent<BoosterFunc>().SetToFront(false);
        }

        public void ChangeUse()
        {
            Use = !Use;
            Debug.Log("change: " + ToString() + " : " + Use);
            ChangeUseEvent?.Invoke(this);
        }

        public void Apply(GridCell gCell, Action completeCallBack)
        {
            if (!gCell) { completeCallBack?.Invoke(); return; }
            if (!footerBooster) { completeCallBack?.Invoke(); return; }
            if (gCell.IsDisabled) { completeCallBack?.Invoke(); return; }
            if (gCell.Blocked) { completeCallBack?.Invoke(); return; }
          //  if (!gCell.Match) { completeCallBack?.Invoke(); return; }

            BoosterFunc bF = SceneObject.GetComponent<BoosterFunc>();
            bF.ApplyToGrid(gCell, bData, completeCallBack);
        }

        public bool Apply()
        {
            BoosterFunc bF = SceneObject.GetComponent<BoosterFunc>();
            return bF.ActivateApply(this);
           // return false;
        }
    }
}



