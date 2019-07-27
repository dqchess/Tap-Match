using System;
using UnityEngine;

namespace Mkey
{
    public class InGamePurchaser : MonoBehaviour
    {
        [Header("Products array: ", order = 1)]
        public ShopThingDataInGame[] gameProducts;

        private MatchGUIController MGui { get { return MatchGUIController.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }

        public static InGamePurchaser Instance;

        #region regular
        void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            InitializePurchasing();
        }
        #endregion regular 

        /// <summary>
        /// Add for each button product clickEevnt
        /// </summary>
        private void InitializePurchasing()
        {
            if (gameProducts != null && gameProducts.Length > 0)
            {
                for (int i = 0; i < gameProducts.Length; i++)
                {
                    if (gameProducts[i] != null && !string.IsNullOrEmpty(gameProducts[i].kProductID))
                    {
                        string prodID = gameProducts[i].kProductID;
                        string prodName = gameProducts[i].name;
                        int count = gameProducts[i].thingCount;
                        int price = (int)gameProducts[i].thingPrice;

                        gameProducts[i].clickEvent.RemoveAllListeners();
                        gameProducts[i].clickEvent.AddListener(() => { BuyBoosterID(prodID, prodName, count, price); });
                    }
                }
            }
        }

        /// <summary>
        /// Buy booster in ingameshop, increase boosters count, decrease game coins, show result message
        /// </summary>
        /// <param name="prodID"></param>
        /// <param name="prodName"></param>
        /// <param name="count"></param>
        /// <param name="price"></param>
        public void BuyBoosterID(string prodID, string prodName, int count, int price)
        {
            int id;
            bool result = false;
            if (MPlayer?.Coins >= price)
            {
                if (int.TryParse(prodID, out id))
                {
                    if (id > 0)
                    {
                        Booster b = MPlayer?.BoostHolder.GetBoosterById(id);
                        if (b != null)
                        {
                            b.AddCount(count);
                            result = true;
                        }
                    }
                }
            }

            if (result)
            {
                MPlayer.AddCoins(-price);
                GoodPurchaseMessage(prodID, prodName);
            }
            else
            {
                FailedPurchaseMessage(prodID, prodName);
            }
        }

        /// <summary>
        /// Show good purchase message
        /// </summary>
        /// <param name="prodId"></param>
        /// <param name="prodName"></param>
        private void GoodPurchaseMessage(string prodId, string prodName)
        {
           MGui.ShowMessage("Succesfull!!!", prodName + " purchased successfull.", 3, null);
        }

        /// <summary>
        /// Show failed purchase message
        /// </summary>
        /// <param name="prodId"></param>
        /// <param name="prodName"></param>
        private void FailedPurchaseMessage(string prodId, string prodName)
        {
            MGui.ShowMessage("Sorry.", prodName + " - purchase failed.", 3, null);
        }

        /// <summary>
        /// Search in array gameProducts appropriate product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ShopThingDataInGame GetProductById(string id)
        {
            if (gameProducts != null && gameProducts.Length > 0)
                for (int i = 0; i < gameProducts.Length; i++)
                {
                    if (gameProducts[i] != null)
                        if (String.Equals(id, gameProducts[i].kProductID, StringComparison.Ordinal))
                            return gameProducts[i];
                }
            return null;
        }
    }
}