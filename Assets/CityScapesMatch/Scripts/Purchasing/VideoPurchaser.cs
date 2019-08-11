// using System;
// using UnityEngine;

// namespace Mkey
// {
//     public class VideoPurchaser : MonoBehaviour
//     {
//         public ShopThingDataReal[] gameProducts;

//         public static VideoPurchaser Instance;

//         private MatchGUIController MGui { get { return MatchGUIController.Instance; } }
//         private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }

//         void Awake()
//         {
//             if (Instance) Destroy(gameObject);
//             else
//             {
//                 Instance = this;
//             }
//         }

//         void Start()
//         {
//             InitializePurchasing();
//         }

//         /// <summary>
//         /// Add for each button product clickEevnt
//         /// </summary>
//         private void InitializePurchasing()
//         {
//             if (gameProducts != null && gameProducts.Length > 0)
//             {
//                 for (int i = 0; i < gameProducts.Length; i++)
//                 {
//                     if (gameProducts[i] != null && !string.IsNullOrEmpty(gameProducts[i].kProductID))
//                     {
//                         string prodID = gameProducts[i].kProductID;
//                         string prodName = gameProducts[i].name;
//                         int count = gameProducts[i].thingCount;
//                         int price = (int)gameProducts[i].thingPrice;

//                         gameProducts[i].clickEvent.RemoveAllListeners();
//                         gameProducts[i].clickEvent.AddListener(() => { ShowVideo(prodID, prodName, count, price); });
//                     }
//                 }
//             }
//         }

//         /// <summary>
//         /// Buy product, increase product count
//         /// </summary>
//         /// <param name="prodID"></param>
//         /// <param name="prodName"></param>
//         /// <param name="count"></param>
//         /// <param name="price"></param>
//         public void ShowVideo(string prodID, string prodName, int count, int price)
//         {
//             int id;
//             bool result = false;

//             Debug.Log("----insert video start code---");

//             if (result)
//             {
//                 GoodPurchaseMessage(prodID, prodName);
//             }
//             else
//             {
//                 FailedPurchaseMessage(prodID, prodName);
//             }
//         }

//         public void AddCoins(int count)
//         {
//             MPlayer.AddCoins(count);
//         }

//         public void SetInfiniteLife(int hours)
//         {
//             MPlayer.StartInfiniteLife(hours);
//         }

//         public void AddLife(int count)
//         {
//             MPlayer.AddLifes(count);
//         }

//         /// <summary>
//         /// Show good purchase message
//         /// </summary>
//         /// <param name="prodId"></param>
//         /// <param name="prodName"></param>
//         private void GoodPurchaseMessage(string prodId, string prodName)
//         {
//             MGui?.ShowMessage("Succesfull!!!", prodName + " received successfull.", 3, null);
//         }

//         /// <summary>
//         /// Show failed purchase message
//         /// </summary>
//         /// <param name="prodId"></param>
//         /// <param name="prodName"></param>
//         private void FailedPurchaseMessage(string prodId, string prodName)
//         {
//             MGui?.ShowMessage("Sorry.", prodName + " - not received.", 3, null);
//         }

//         /// <summary>
//         /// Search in array gameProducts appropriate product
//         /// </summary>
//         /// <param name="id"></param>
//         /// <returns></returns>
//         public ShopThingDataReal GetProductById(string id)
//         {
//             if (gameProducts != null && gameProducts.Length > 0)
//                 for (int i = 0; i < gameProducts.Length; i++)
//                 {
//                     if (gameProducts[i] != null)
//                         if (String.Equals(id, gameProducts[i].kProductID, StringComparison.Ordinal))
//                             return gameProducts[i];
//                 }
//             return null;
//         }
//     }
// }