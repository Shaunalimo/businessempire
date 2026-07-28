using System;
using System.Collections.Generic;
using BusinessEmpire.Monetization;
using UnityEngine;

namespace BusinessEmpire.Data
{
    /// <summary>
    /// IAP product catalog for shop UI. IDs must match Google Play Console.
    /// </summary>
    [CreateAssetMenu(fileName = "IapProductCatalog", menuName = "Business Empire/IAP Catalog")]
    public class IapProductCatalog : ScriptableObject
    {
        [Serializable]
        public struct ProductEntry
        {
            public string productId;
            public string displayName;
            public string description;
            public ProductType productType;
            public int gemAmount;
            public double cashAmount;
            public bool isNonConsumable;
        }

        public List<ProductEntry> products = new List<ProductEntry>
        {
            new ProductEntry
            {
                productId = MonetizationConstants.ProductRemoveAds,
                displayName = "Remove Ads",
                description = "Removes banner and interstitial ads permanently.",
                productType = ProductType.NonConsumable,
                isNonConsumable = true
            },
            new ProductEntry
            {
                productId = MonetizationConstants.ProductStarterPack,
                displayName = "CEO Starter Pack",
                description = "25,000 cash + 250 gems — one-time bundle.",
                productType = ProductType.NonConsumable,
                gemAmount = MonetizationConstants.StarterPackGems,
                cashAmount = MonetizationConstants.StarterPackCash,
                isNonConsumable = true
            },
            new ProductEntry
            {
                productId = MonetizationConstants.ProductGems100,
                displayName = "100 Gems",
                description = "Premium currency pack.",
                productType = ProductType.Consumable,
                gemAmount = 100
            },
            new ProductEntry
            {
                productId = MonetizationConstants.ProductGems500,
                displayName = "500 Gems",
                description = "Best value gem pack.",
                productType = ProductType.Consumable,
                gemAmount = 500
            },
            new ProductEntry
            {
                productId = MonetizationConstants.ProductGems1200,
                displayName = "1200 Gems",
                description = "Mega gem bundle.",
                productType = ProductType.Consumable,
                gemAmount = 1200
            }
        };

        public enum ProductType
        {
            Consumable,
            NonConsumable
        }
    }
}
