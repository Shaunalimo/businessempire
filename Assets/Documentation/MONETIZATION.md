# Monetization Guide — Business Empire: CEO Simulator

This document explains the monetization architecture, where to add production IDs, and how rewards flow back to the player.

---

## Architecture

```
MonetizationManager   ← Game/UI should call this
    ├── AdsManager         Banner, interstitial, rewarded (AdMob)
    └── IAPManager         Remove Ads, gems, Starter Pack (Unity IAP)
```

| Script | Responsibility |
|--------|----------------|
| **MonetizationManager** | Placement rules, reward grants, milestone interstitials |
| **AdsManager** | AdMob load/show, cooldowns, Remove Ads enforcement |
| **IAPManager** | Google Play billing, fulfillment, restore |
| **MonetizationConstants** | TEST ad unit IDs and product ID placeholders |

---

## Where to add real AdMob IDs (production)

**File:** `Assets/Scripts/Monetization/MonetizationConstants.cs`

Replace these TEST constants before release:

| Constant | Purpose |
|----------|---------|
| `AdMobAppIdAndroid` | App ID in AdMob console |
| `AdMobBannerAndroid` | Banner ad unit |
| `AdMobInterstitialAndroid` | Interstitial ad unit |
| `AdMobRewardedAndroid` | Rewarded ad unit |

Also update:

- `Assets/Plugins/Android/GoogleMobileAdsAppId.xml`
- Build-time injection: `Assets/Scripts/Editor/AdMobAndroidManifestProcessor.cs`

**Steps:**

1. Import [Google Mobile Ads Unity plugin](https://github.com/googleads/googleads-mobile-unity/releases)
2. Run **Business Empire → Setup Monetization SDKs** (enables `GOOGLE_MOBILE_ADS` define)
3. Replace TEST IDs in `MonetizationConstants.cs` with your AdMob console IDs
4. Build and test on a real Android device

> Until then, the project uses **Google's official test ad unit IDs** only.

---

## Where to add Google Play product IDs

**File:** `Assets/Scripts/Monetization/MonetizationConstants.cs`

| Constant | Product type | Description |
|----------|--------------|-------------|
| `ProductRemoveAds` | Non-consumable | Removes banner + interstitial |
| `ProductStarterPack` | Non-consumable | Cash + gems bundle |
| `ProductGems100` | Consumable | 100 gems |
| `ProductGems500` | Consumable | 500 gems |
| `ProductGems1200` | Consumable | 1200 gems |

**Also configure:** `Assets/Resources/IapProductCatalog.asset` (display names for shop UI)

**Google Play Console:**

1. Create in-app products with **exact matching** product IDs
2. Enable **Unity Purchasing** in Unity (Services)
3. Test with license testers before production

---

## How Remove Ads disables ads

1. Player purchases `ProductRemoveAds` via `IAPManager.PurchaseRemoveAds()`
2. `IAPManager.ApplyRemoveAds()` sets `SaveManager.CurrentSave.adsRemoved = true`
3. `AdsManager.DisableAllAds()` destroys banner, interstitial, and rewarded controllers
4. Future rewarded offers **grant rewards immediately** without showing an ad (optional reward still works)

To verify: use **Monetization Test Panel → Remove Ads** button.

---

## Rewarded ad callbacks → player rewards

All optional rewarded flows go through **MonetizationManager** (never call `AdsManager` directly from UI).

| Placement | UI entry | On success |
|-----------|----------|------------|
| `DoubleOfflineEarnings` | Offline popup "x2" button | `OfflineIncomeManager.CollectOfflineIncome(useAdBonus: true)` |
| `ExtraCash` | Test panel / future HUD button | `+5000` cash via `EconomyManager` |
| `InstantUpgradeSpeedUp` | Upgrade screen (optional) | Speed-up hook (extend when timers added) |
| `DailyRewardMultiplier` | Daily rewards "Watch ad for x2" | Duplicate daily cash + gems |

**Callback pattern:**

```csharp
MonetizationManager.Instance.OfferExtraCash(success =>
{
    if (success) { /* reward already applied */ }
    else { /* show "Ad not available" — no penalty */ }
});
```

`onSuccess` runs **only** after the user completes the rewarded ad (or immediately if Remove Ads is owned).

If the ad is not loaded, `onFailed` runs — **no reward, no punishment**.

---

## Interstitial rules (not every click)

Interstitials show only after **major milestones**, with a **180-second cooldown**:

| Milestone | Trigger |
|-----------|---------|
| Business purchased | First ownership of a new business |
| Business upgraded | Levels 5, 10, 15… |
| Achievement unlocked | Any new achievement |
| Weekly progress | Every 7 in-game days |

Interstitials **never** show on button clicks, navigation, or every day tick.

---

## Banner rules

- Shown on **main menu** via `MonetizationManager.EnterMainMenu()`
- Hidden during gameplay via `EnterGameplay()`
- Hidden permanently after Remove Ads purchase

---

## Test UI

**MonetizationTestPanel** (`Assets/Scripts/UI/MonetizationTestPanel.cs`)

Buttons:

- Test Rewarded / Interstitial / Banner toggle
- Purchase Remove Ads, Gems, Starter Pack
- Trigger each rewarded placement

Add to a scene via **Business Empire → Setup Monetization SDKs** (creates overlay in prototype scene).

---

## Editor setup menu

```
Business Empire → Setup Monetization SDKs
```

- Syncs `GOOGLE_MOBILE_ADS` scripting define when SDK detected
- Adds monetization test panel to UIMobilePrototype scene (if present)

---

## Files reference

```
Assets/Scripts/Monetization/
  MonetizationManager.cs
  AdsManager.cs
  IAPManager.cs
  MonetizationConstants.cs      ← TEST IDs & product IDs
  RewardedAdPlacement.cs
  BannerAdController.cs
  RewardedAdController.cs
  InterstitialAdController.cs
Assets/Scripts/UI/
  MonetizationTestPanel.cs
Assets/Documentation/
  MONETIZATION.md               ← this file
```
