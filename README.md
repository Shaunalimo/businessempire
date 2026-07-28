# Business Empire: CEO Simulator

A production-ready **Unity C#** business simulation / tycoon game for **Android** (portrait-first, iOS-ready).

**Studio:** Tekh Games Studio  
**Starting cash:** $5,000  
**Engine:** Unity 2022.3 LTS (recommended)

---

## Features

| System | Description |
|--------|-------------|
| **8 Businesses** | Grocery, Coffee, Restaurant, Clothing, Software, Logistics, Factory, Real Estate |
| **Economy** | Purchase cost, daily income, salaries, upgrades, profit reports |
| **Finance** | Bank loans, timed investments |
| **Growth** | Marketing campaigns, R&D tree, employee hiring |
| **Events** | Random business events with income/cash effects |
| **Progression** | Achievements, daily rewards, player XP/levels |
| **Offline** | Earn income while away (50% efficiency, 24h cap) |
| **Save** | Auto-save every 30s + on pause/focus loss |
| **Monetization** | AdMob rewarded/interstitial, Remove Ads IAP, premium gems |
| **Future** | Cloud save stub, Google Play leaderboard hook |

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # Bootstrap, events, constants, singleton base
│   ├── Data/           # ScriptableObjects, enums, save models
│   ├── Managers/       # Game systems (business, economy, save, etc.)
│   ├── Monetization/   # AdMob + Unity IAP
│   ├── UI/             # HUD, panels, animations
│   ├── Utils/          # Formatting, JSON, time helpers
│   └── Editor/         # One-click setup tools
├── UI/                 # Sprites, fonts, UI assets
├── Prefabs/            # Business cards, panels, popups
├── Scenes/             # Bootstrap + GameScene
├── Animations/         # UI animation clips
├── Audio/              # SFX and music
└── Resources/          # Business definition assets
```

---

## Quick Start

### 1. Open in Unity

1. Install **Unity 2022.3 LTS** with **Android Build Support**
2. Open this folder as a Unity project
3. Wait for packages to resolve (TextMeshPro, Unity Purchasing, Unity Ads)

### 2. Run Editor Setup (one click)

In Unity menu bar:

```
Business Empire → Build Complete UI
Business Empire → Configure Android Portrait
Business Empire → Generate Business Assets
Business Empire → Setup Monetization SDKs
```

**Build Complete UI** creates:
- All UI prefabs in `Assets/Prefabs/UI/`
- IAP catalog in `Assets/Resources/IapProductCatalog.asset`
- Three wired scenes: Bootstrap → MainMenu → GameScene
- Build Settings configured automatically

### 3. Build Settings

Scenes (auto-configured by Build Complete UI):

1. `Assets/Scenes/Bootstrap.unity` — initializes managers, loads MainMenu
2. `Assets/Scenes/MainMenu.unity` — play, shop, settings
3. `Assets/Scenes/GameScene.unity` — full game HUD and panels

Switch platform to **Android**, set **Minimum API Level** to 24+.

### 4. Preview

Open **UIMobilePrototype** scene and press Play to preview all 20 connected screens.

See [Assets/UI/MOBILE_UI_README.md](Assets/UI/MOBILE_UI_README.md) for screen map and architecture.

---

## Architecture

Clean **object-oriented** design with **singleton managers** and an **event bus** (`GameEvents`) for decoupled UI updates.

```
GameBootstrap
    └── SaveManager (loads first)
    └── EconomyManager
    └── BusinessManager
    └── GameManager (day loop)
    └── [Finance, Marketing, Events, Achievements, etc.]
    └── MonetizationManager
        ├── AdsManager (banner, interstitial, rewarded)
        └── IAPManager (Remove Ads, gems, Starter Pack)
```

### Day Loop (every 60 seconds by default)

1. Calculate business income − salaries  
2. Process loans, investments, marketing timers  
3. Apply random events  
4. Check achievements  
5. Optionally show interstitial ad after major milestones (not every day tick)

### Save Format

JSON at `Application.persistentDataPath/business_empire_save.json`

---

## Business Stats (Default)

| Business | Cost | Base Income/Day | Salary/Employee |
|----------|------|-----------------|-----------------|
| Grocery Shop | $8,000 | $420 | $80 |
| Coffee Shop | $12,000 | $580 | $90 |
| Restaurant | $25,000 | $950 | $120 |
| Clothing Store | $35,000 | $1,100 | $110 |
| Software Company | $75,000 | $2,200 | $200 |
| Logistics Company | $120,000 | $3,200 | $180 |
| Factory | $200,000 | $4,800 | $250 |
| Real Estate Agency | $350,000 | $7,500 | $300 |

Edit via **ScriptableObjects** in `Assets/Resources/BusinessDefinitions/` or regenerate from the menu.

---

## Monetization Setup

Full documentation: **`Assets/Documentation/MONETIZATION.md`**

### Architecture

```
MonetizationManager   ← Game/UI should call this
    ├── AdsManager         Banner, interstitial, rewarded (AdMob TEST IDs)
    └── IAPManager         Remove Ads, gems, Starter Pack (Unity IAP)
```

### Google AdMob

1. Run **Business Empire → Install Google Mobile Ads (Instructions)**
2. Import the [Google Mobile Ads Unity plugin](https://github.com/googleads/googleads-mobile-unity/releases)
3. Run **Business Empire → Setup Monetization SDKs** (enables `GOOGLE_MOBILE_ADS` define)
4. Replace **TEST** IDs in `Assets/Scripts/Monetization/MonetizationConstants.cs` with production Ad Unit IDs
5. AdMob App ID is injected at build time via `AdMobAndroidManifestProcessor`

**Ad types:**
| Type | Usage |
|------|-------|
| Banner | Main menu only (hidden during gameplay) |
| Rewarded | Optional: offline x2, extra cash, upgrade speed-up, daily bonus x2 |
| Interstitial | Major milestones only (business purchase, upgrade levels 5/10/…, achievements, weekly) — 180s cooldown |

Without the SDK, ads run in **simulation mode** for testing.

### Unity IAP

Products (configured in `IapProductCatalog` and `MonetizationConstants`):

| Product ID | Type | Reward |
|------------|------|--------|
| `com.tekhgames.businessempire.removeads` | Non-consumable | Removes banner + interstitial ads |
| `com.tekhgames.businessempire.starterpack` | Non-consumable | 25,000 cash + 250 gems |
| `com.tekhgames.businessempire.gems100` | Consumable | 100 gems |
| `com.tekhgames.businessempire.gems500` | Consumable | 500 gems |
| `com.tekhgames.businessempire.gems1200` | Consumable | 1200 gems |

1. Enable **Unity Purchasing** in Project Settings → Services
2. Create matching products in Google Play Console
3. Shop UI loads prices from the store via `MonetizationManager.GetLocalizedPrice()`
4. **Restore Purchases** available in Settings (iOS/macOS)

### Test UI

Run **Business Empire → Setup Monetization SDKs** to add **MonetizationTestPanel** to the prototype scene, or open it from **Settings → Monetization Test Panel**.

---

## Business Icons

Each business has a unique colored icon with abbreviation letters (GS, CF, RT, etc.).

```
Business Empire → Generate Business Icons
```

Icons are saved to `Assets/Resources/BusinessIcons/` and auto-assigned to BusinessDefinition assets. Replace with custom art anytime — keep the same filenames.

| Business | Color | Abbr |
|----------|-------|------|
| Grocery Shop | Green | GS |
| Coffee Shop | Brown | CF |
| Restaurant | Red-orange | RT |
| Clothing Store | Purple | CL |
| Software Company | Blue | SW |
| Logistics Company | Gold | LG |
| Factory | Gray | FC |
| Real Estate Agency | Teal | RE |

---

## Audio System

**SoundLibrary** (`Assets/Resources/SoundLibrary.asset`) holds all clips.

```
Business Empire → Generate Sound Effects
```

| Hook | Sound |
|------|-------|
| Button tap | `buttonClick` |
| Panel open/close | `panelOpen` / `panelClose` |
| Purchase / upgrade | `purchase` / `upgrade` |
| Cash increase | `coinCollect` |
| Daily reward | `reward` |
| Achievement | `achievement` |
| Day advance | `dayAdvance` |
| Business events | `positiveEvent` / `negativeEvent` |
| Main menu / gameplay | Background music loops |

`AudioEventBridge` listens to `GameEvents` and plays sounds automatically. Swap WAV files in `Assets/Audio/SFX/` with your own recordings.

---

## Google Play Games Leaderboards

```
Business Empire → Install Google Play Games (Instructions)
Business Empire → Open Play Games Constants
Business Empire → Setup Monetization SDKs
```

**Features:**
- Auto sign-in on game start
- Submit total profit score daily and on demand
- Load top 25 global scores in-app
- Open native Google Play leaderboard UI
- Falls back to local cache when offline or SDK not installed

**Setup:**
1. Create leaderboards in Google Play Console (Total Profit recommended)
2. Paste leaderboard IDs into `PlayGamesConstants.cs`
3. Install [Play Games Plugin for Unity](https://github.com/playgameservices/play-games-plugin-for-unity)
4. Run **Setup Monetization SDKs** to enable `GOOGLE_PLAY_GAMES` define

---

## Mobile Optimization

- Target frame rate: **60 FPS** (`GameBootstrap`)
- Portrait lock via **Configure Android Portrait** menu
- Canvas scaler: **1080×1920** reference resolution
- Object pooling ready (instantiate/destroy on panel refresh — pool in production)
- Auto-save on app pause (Android background)

---

## Cloud Save (Future)

`CloudSaveManager` provides upload/download stubs. Connect Firebase or Play Games Saved Games when ready.

---

## Extending to iOS

1. Switch build target to iOS  
2. Update ad unit IDs for iOS in `MonetizationConstants.cs`  
3. Configure App Store IAP products  
4. Portrait orientation is already enforced  

---

## License

Proprietary — Tekh Games Studio. All rights reserved.
