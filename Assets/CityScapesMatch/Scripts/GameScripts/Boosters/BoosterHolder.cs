using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class BoostersHolder
    {
        private List<Booster> boosters;
        private bool saveData;
        private string savePrefix;

        #region properties
        public IList<Booster> Boosters { get { return boosters.AsReadOnly(); } }
        #endregion properties

        public BoostersHolder(GameObjectsSet goSet, bool saveData, string savePrefix)
        {
            this.saveData = saveData;
            this.savePrefix = savePrefix;

            IList<BoosterObjectData> bDataList = goSet.BoosterObjects;
            boosters = new List<Booster>();
            foreach (var item in bDataList)
            {
                boosters.Add(new Booster(item, saveData, savePrefix));
            }
            Debug.Log("Boosters count: " + boosters.Count);
        }

        public Booster GetBoosterById(int id)
        {
            foreach (var item in boosters)
            {
                if (item.bData.ID == id) return item;
            }
            return null;
        }
    }
}