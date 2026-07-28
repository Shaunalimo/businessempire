# Play Guide — Business Empire: CEO Simulator

## Open this scene first

**`Assets/Scenes/MainMenu.unity`**

Press **Play**, then choose **New Game** or **Continue**.

---

## Scene flow

| Scene | Purpose |
|-------|---------|
| **MainMenu.unity** | Start here — New Game / Continue |
| **Gameplay.unity** | Full mobile UI + live economy loop |
| **Bootstrap.unity** | Optional loading scene → auto-loads MainMenu |

---

## Gameplay controls

| UI | Action |
|----|--------|
| **Top bar** | Cash, gems, level, reputation |
| **Home** | Dashboard overview |
| **Biz** | Business list → Buy / Upgrade |
| **Staff** | Hire employees (owned businesses) |
| **Shop** | IAP products (simulation mode) |
| **More (Settings)** | Reset save, return to menu |

Passive income runs automatically via the **60-second day loop** (GameManager). Save/load is automatic every 30 seconds.

---

## Data

- Business ScriptableObjects: `Assets/Resources/BusinessDefinitions/`
- Employee sample JSON: `Assets/Resources/employee_sample.json`
- UI built at runtime — no external art required
