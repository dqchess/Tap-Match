using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class ShopBoosterWindowController : PopUpsController
    {
        [SerializeField]
        private RectTransform ThingsParent;
        [SerializeField]
        private InGameShopType shopType;
        [SerializeField]
        private GameObject scrollFlag;
        private List<ShopThingHelper> shopThings;
        [SerializeField]
        private Text caption;

        public static string ID;
        public static string ShopCaption;

        public override void RefreshWindow()
        {
            CreateThingTab();
            base.RefreshWindow();
        }

        private void CreateThingTab()
        {
            ShopThingHelper[] sT = ThingsParent.GetComponentsInChildren<ShopThingHelper>();
            foreach (var item in sT)
            {
                DestroyImmediate(item.gameObject);
            }

            InGamePurchaser p = InGamePurchaser.Instance;
            if (p == null) return;

            List<ShopThingDataInGame> products = new List<ShopThingDataInGame>();
            if (p.gameProducts != null && p.gameProducts.Length > 0) products.AddRange(p.gameProducts);


            if (products.Count == 0) return;

            shopThings = new List<ShopThingHelper>();
            for (int i = 0; i < products.Count; i++)
            {
                Debug.Log(ID + " : " + products[i].kProductID);

                if (string.IsNullOrEmpty(ID) ||  ID.Contains(products[i].kProductID))
                    if (products[i] != null && (products[i].shopType == shopType) && products[i].prefab) shopThings.Add (ShopThingHelper.CreateShopThingsHelper(products[i].prefab, ThingsParent, products[i]));
            }
            if (caption && !string.IsNullOrEmpty(ShopCaption)) caption.text = ShopCaption;
            if (scrollFlag)scrollFlag.SetActive(products.Count > 2);
            ID = null;
            ShopCaption = null;
        }
    }
}
