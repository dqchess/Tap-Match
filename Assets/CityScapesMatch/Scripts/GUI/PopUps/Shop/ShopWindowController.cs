using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class ShopWindowController : PopUpsController
    {
        [SerializeField]
        private RectTransform ThingsParent;
        [SerializeField]
        private RealShopType shopType = RealShopType.Coins;
        [SerializeField]
        private GameObject scrollFlag;
        private List<ShopThingHelper> shopThings;

        void Start()
        {
            CreateThingTab();
        }

        public override void RefreshWindow()
        {

            base.RefreshWindow();
        }

        private void CreateThingTab()
        {
            ShopThingHelper[] sT = ThingsParent.GetComponentsInChildren<ShopThingHelper>();
            foreach (var item in sT)
            {
                DestroyImmediate(item.gameObject);
            }

            // Purchaser p = Purchaser.Instance;
            // if (p == null) return;

            // List<ShopThingDataReal> products = new List<ShopThingDataReal>();

            // VideoPurchaser vP = VideoPurchaser.Instance;
            // if (vP && vP.gameProducts!=null &&  vP.gameProducts.Length > 0)
            // {
            //     products.AddRange(vP.gameProducts);
            // }

            // if (p.consumable != null && p.consumable.Length > 0) products.AddRange(p.consumable);
            // if (p.nonConsumable != null && p.nonConsumable.Length > 0) products.AddRange(p.nonConsumable);
            // if (p.subscriptions != null && p.subscriptions.Length > 0) products.AddRange(p.subscriptions);

            // Debug.Log("products count: " + products.Count);
            // if (products.Count==0) return;

            // shopThings = new List<ShopThingHelper>();
            // for (int i = 0; i < products.Count; i++)
            // {
            //   if(products[i]!=null && (products[i].shopType == shopType) && products[i].prefab)  shopThings.Add(ShopThingHelper.CreateShopThingsHelper(products[i].prefab, ThingsParent, products[i]));
            // }

            // if (scrollFlag) scrollFlag.SetActive(products.Count > 4);
        }
    }
}