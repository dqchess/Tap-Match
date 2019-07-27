using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class SpawnController : MonoBehaviour
    {
        private bool useStack = true;
        public Stack<int> mStack;

        private MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private GameObjectsSet GOSet { get { return MPlayer.MatchSet; } }
        public Action EmptyStackEvent;
        private bool empty = false;
        public static SpawnController Instance;

        private Dictionary<int, int> fieldObjects;

        #region regular
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (useStack)
            {
                CreateStack(MPlayer.LcSet);
            }
        }
        #endregion regular;

        public MatchObjectData GetObject()
        {
            if (useStack)
            {
                if (mStack.Count > 0) return GOSet.GetMainObject(mStack.Pop());
                else
                {
                    if (!empty) EmptyStackEvent?.Invoke();
                    empty = true;
                    CreateStack(MPlayer.LcSet);
                    return GOSet.GetMainObject(mStack.Pop());
                }
            }
            else
            {
                return GOSet.GetMainRandomObjects(1)[0];
            }
        }

        public int GetID()
        {
            if (useStack)
            {
                if (mStack.Count > 0) return mStack.Pop();
                else
                {
                    if (!empty) EmptyStackEvent?.Invoke();
                    empty = true;
                    CreateStack(MPlayer.LcSet); // to avoid error
                    return mStack.Pop();
                }
            }
            else
            {
                return GOSet.GetMainRandomObjects(1)[0].ID;
            }
        }

        private void CreateStack(LevelConstructSet lcSet)
        {
            mStack = new Stack<int>();
            fieldObjects =  new Dictionary<int, int>();
            foreach (var item in GOSet.MatchObjects)
            {
                if (lcSet.matchObjects != null && lcSet.matchObjects.Count > 0 && lcSet.matchObjects.Contains(item.ID)) // add only objects from list
                {
                    fieldObjects.Add(item.ID, 1); // get count
                }
                else if ((lcSet.matchObjects == null || lcSet.matchObjects.Count == 0))
                {
                    fieldObjects.Add(item.ID, 1); // get count
                }
            }

            List<int> tS = new List<int>();

            Debug.Log("Spawn controller field objects count: " + fieldObjects.Count);
            foreach (var item in fieldObjects) // create stack
            {
                for (int i = 0; i < item.Value + 30; i++) // increase value 30 
                {
                    tS.Add(item.Key);
                }
            }
            tS.Shuffle();
            foreach (var item in tS)
            {
                mStack.Push(item);
            }
        }

        /// <summary>
        /// Only for debug
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetNextmatchID(int id)
        {
            List<int> keyList = new List<int>(fieldObjects.Keys);
            int pos = 0;
            if (keyList.Contains(id))
            {
                for (int i = 0; i < keyList.Count; i++)
                {
                    if(id == keyList[i])
                    {
                        pos = i;
                        break;
                    }
                }
            }

            pos++;
            pos = (int) Mathf.Repeat(pos, keyList.Count);
            return keyList[pos];
        }
    }
}
