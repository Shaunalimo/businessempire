# Mobile UI Prototype — Business Empire: CEO Simulator

## Build (one click)

In Unity menu:
```
Business Empire → Build Complete UI
```

Open scene: **`Assets/Scenes/UIMobilePrototype.unity`** and press **Play**.

## 20 Screens

| # | Screen | Navigation |
|---|--------|------------|
| 1 | Splash | Auto → Main Menu |
| 2 | Main Menu | Play → Save Select |
| 3 | Save Select | New/Continue → Dashboard |
| 4 | Dashboard | Hub + quick actions |
| 5 | Business List | Bottom tab "Biz" |
| 6 | Business Detail | Tap owned business |
| 7 | Buy Business | Tap unowned business |
| 8 | Upgrade Business | From business detail |
| 9 | Employee Hiring | Bottom tab "Staff" |
| 10 | Marketing Campaign | Dashboard button |
| 11 | Bank Loan | Dashboard button |
| 12 | Investment | Dashboard button |
| 13 | Reports | Bottom tab "Reports" |
| 14 | Daily Rewards | Dashboard button |
| 15 | Achievements | Dashboard button |
| 16 | Settings | Top bar ⚙ or Main Menu |
| 17 | Shop | Bottom tab "Shop" |
| 18 | Game Over | Settings/Reports preview |
| 19 | Level Up Popup | Buy/upgrade/claim |
| 20 | Offline Earnings | Dashboard preview (auto on start) |

## Architecture

```
UI/
├── Core/          UINavigationManager, UIScreenBase, UITheme
├── Components/    UITopBar, UIBottomNav, UICardView, UIListRowView
├── Data/          UIViewModels, UIDummyDataProvider, UIGameDataBridge
└── Screens/       20 screen controllers (UI logic only)
```

- **UIDummyDataProvider** — sample data for preview
- **UIGameDataBridge** — connects to live save when `UseDummyData = false`
- Economy logic stays in `Managers/` — screens never modify save directly in prototype mode

## Top Bar
Cash · Gems · Company Level · Reputation slider · Settings

## Bottom Nav
Home · Biz · Staff · Reports · Shop

## Reusable Prefabs (`Assets/Prefabs/UI/`)
- `UIListRow.prefab`
- `UICard.prefab`

## Theme
Premium blue-green corporate — see `UITheme.cs`

## Responsive
- Reference resolution 1080×1920
- `UISafeAreaFitter` for notched Android devices
- CanvasScaler match 0.5
