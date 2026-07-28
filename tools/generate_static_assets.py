#!/usr/bin/env python3
"""Generate static Unity assets (icons, WAV, ScriptableObjects) without the Unity Editor."""

import math
import os
import struct
import uuid
import zlib
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RES = ROOT / "Assets" / "Resources"
ICON_DIR = RES / "BusinessIcons"
DEF_DIR = RES / "BusinessDefinitions"
SFX_DIR = ROOT / "Assets" / "Audio" / "SFX"

BUSINESS_DEF_GUID = "4a7f216ca6ca30b4eb0c52d2f6eb9cf4"
IAP_CATALOG_GUID = "cc4ee832aeecf6540a193de9d3596a6b"
SOUND_LIBRARY_GUID = "f53022e67631b56448103c1864365de0"

BUSINESSES = [
    ("GroceryShop", 0, "Grocery Shop", "Essential retail with steady foot traffic.", 8000, 420, 80, 1, 0, False, 0, (0.18, 0.72, 0.45), "GS"),
    ("CoffeeShop", 1, "Coffee Shop", "High-margin beverages and loyal morning customers.", 12000, 580, 90, 1, 0, False, 0, (0.55, 0.35, 0.20), "CF"),
    ("Restaurant", 2, "Restaurant", "Full-service dining with premium ticket sizes.", 25000, 950, 120, 2, 0, True, 0, (0.90, 0.35, 0.25), "RT"),
    ("ClothingStore", 3, "Clothing Store", "Fashion retail with seasonal demand spikes.", 35000, 1100, 110, 2, 1, True, 1, (0.75, 0.30, 0.65), "CL"),
    ("SoftwareCompany", 4, "Software Company", "Scalable digital products and subscriptions.", 75000, 2200, 200, 3, 2, True, 2, (0.25, 0.55, 0.95), "SW"),
    ("LogisticsCompany", 5, "Logistics Company", "Delivery fleet serving regional clients.", 120000, 3200, 180, 3, 2, True, 3, (0.95, 0.70, 0.15), "LG"),
    ("Factory", 6, "Factory", "Manufacturing at scale with high overhead.", 200000, 4800, 250, 4, 3, True, 4, (0.50, 0.52, 0.55), "FC"),
    ("RealEstateAgency", 7, "Real Estate Agency", "Property commissions and portfolio growth.", 350000, 7500, 300, 5, 3, True, 5, (0.20, 0.75, 0.80), "RE"),
]

SFX = {
    "button_click": ("tone", 880, 0.06, 0.35),
    "panel_open": ("sweep", 400, 700, 0.08, 0.3),
    "panel_close": ("sweep", 700, 350, 0.08, 0.25),
    "error": ("tone", 180, 0.15, 0.4),
    "purchase": ("chord", [523, 659, 784], 0.25, 0.45),
    "upgrade": ("sweep", 500, 1200, 0.2, 0.4),
    "coin": ("tone", 1318, 0.05, 0.25),
    "cash_register": ("chord", [880, 1108], 0.12, 0.35),
    "reward": ("chord", [523, 659, 784, 1046], 0.35, 0.5),
    "achievement": ("chord", [392, 523, 659, 784, 1046], 0.5, 0.55),
    "level_up": ("sweep", 300, 1500, 0.35, 0.45),
    "day_advance": ("tone", 440, 0.1, 0.3),
    "event_positive": ("chord", [659, 784], 0.2, 0.35),
    "event_negative": ("tone", 220, 0.2, 0.35),
    "music_menu": ("ambient", 220, 330, 4.0, 0.12),
    "music_game": ("ambient", 196, 294, 4.0, 0.10),
}


def new_guid() -> str:
    return uuid.uuid4().hex


def write_meta(path: Path, guid: str, importer: str = "DefaultImporter") -> None:
    meta = f"""fileFormatVersion: 2
guid: {guid}
{importer}:
  externalObjects: {{}}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {{instanceID: 0}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    if path.suffix.lower() == ".png":
        meta = f"""fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 12
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
  isReadable: 1
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMasterTextureLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: 100
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  platformSettings:
  - serializedVersion: 3
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 256
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: 5e97eb03825dee720800000000000000
    internalID: 0
    vertices: []
    indices: 
    edges: []
    weights: []
    secondaryTextures: []
    nameFileIdTable: {{}}
  spritePackingTag: 
  pSDRemoveMatte: 0
  pSDShowRemoveMatteOption: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    elif path.suffix.lower() == ".wav":
        meta = f"""fileFormatVersion: 2
guid: {guid}
AudioImporter:
  externalObjects: {{}}
  serializedVersion: 7
  defaultSettings:
    serializedVersion: 2
    loadType: 0
    sampleRateSetting: 0
    sampleRateOverride: 44100
    compressionFormat: 1
    quality: 1
    conversionMode: 0
  platformSettingOverrides: {{}}
  forceToMono: 1
  normalize: 1
  preloadAudioData: 1
  loadInBackground: 0
  ambisonic: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    elif path.suffix.lower() == ".asset":
        meta = f"""fileFormatVersion: 2
guid: {guid}
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    path.with_suffix(path.suffix + ".meta").write_text(meta, encoding="utf-8")


def png_chunk(tag: bytes, data: bytes) -> bytes:
    crc = zlib.crc32(tag + data) & 0xFFFFFFFF
    return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", crc)


def write_icon_png(path: Path, rgb, abbr: str) -> str:
    size = 256
    r, g, b = [int(c * 255) for c in rgb]
    rows = []
    for y in range(size):
        row = b"\x00"
        for x in range(size):
            nx = (x / size) * 2 - 1
            ny = (y / size) * 2 - 1
            inside = max(abs(nx), abs(ny)) < 0.82
            if inside:
                t = (ny + 1) * 0.5
                rr = int(r * (0.55 + 0.45 * t))
                gg = int(g * (0.55 + 0.45 * t))
                bb = int(b * (0.55 + 0.45 * t))
                row += bytes([rr, gg, bb, 255])
            else:
                row += b"\x00\x00\x00\x00"
        rows.append(row)
    raw = b"".join(rows)
    ihdr = struct.pack(">IIBBBBB", size, size, 8, 6, 0, 0, 0)
    png = b"\x89PNG\r\n\x1a\n" + png_chunk(b"IHDR", ihdr) + png_chunk(b"IDAT", zlib.compress(raw, 9)) + png_chunk(b"IEND", b"")
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(png)
    guid = new_guid()
    write_meta(path, guid, "TextureImporter")
    return guid


def synth_samples(spec):
    sr = 44100
    kind = spec[0]
    if kind == "tone":
        _, freq, dur, vol = spec
        n = int(sr * dur)
        return [math.sin(2 * math.pi * freq * (i / sr)) * vol * (1 - i / n) for i in range(n)]
    if kind == "sweep":
        _, f0, f1, dur, vol = spec
        n = int(sr * dur)
        return [math.sin(2 * math.pi * (f0 + (f1 - f0) * (i / n)) * (i / sr)) * vol * (1 - i / n) for i in range(n)]
    if kind == "chord":
        _, freqs, dur, vol = spec
        n = int(sr * dur)
        out = []
        for i in range(n):
            t = i / sr
            env = 1 - i / n
            out.append(sum(math.sin(2 * math.pi * f * t) for f in freqs) / len(freqs) * vol * env)
        return out
    if kind == "ambient":
        _, f1, f2, dur, vol = spec
        n = int(sr * dur)
        return [(math.sin(2 * math.pi * f1 * (i / sr)) * 0.6 + math.sin(2 * math.pi * f2 * (i / sr)) * 0.4) * vol for i in range(n)]
    return []


def write_wav(path: Path, samples) -> str:
    sr = 44100
    ints = [max(-32768, min(32767, int(max(-1, min(1, s)) * 32767))) for s in samples]
    data = b"".join(struct.pack("<h", s) for s in ints)
    header = struct.pack("<4sI4s4sIHHIIHH4sI", b"RIFF", 36 + len(data), b"WAVE", b"fmt ", 16, 1, 1, sr, sr * 2, 2, 16, b"data", len(data))
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(header + data)
    guid = new_guid()
    write_meta(path, guid, "AudioImporter")
    return guid


def sprite_ref(guid: str) -> str:
    return f"{{fileID: 21300000, guid: {guid}, type: 3}}"


def audio_ref(guid: str) -> str:
    return f"{{fileID: 8300000, guid: {guid}, type: 3}}"


def write_business_definition(name, data, icon_guid):
    _, btype, display, desc, cost, income, salary, level, max_emp, req, prereq, *_ = data
    path = DEF_DIR / f"{name}.asset"
    yaml = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {BUSINESS_DEF_GUID}, type: 3}}
  m_Name: {name}
  m_EditorClassIdentifier: 
  type: {btype}
  displayName: {display}
  description: {desc}
  icon: {sprite_ref(icon_guid)}
  purchaseCost: {cost}
  baseDailyIncome: {income}
  baseSalaryPerEmployee: {salary}
  upgradeCostMultiplier: 1.5
  incomeGrowthPerLevel: 0.15
  customerGrowthRate: 0.05
  startingEmployeeSlots: 2
  maxEmployees: {max_emp}
  requiredPlayerLevel: {level}
  prerequisiteBusiness: {prereq}
  requiresPrerequisite: {1 if req else 0}
"""
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(yaml, encoding="utf-8")
    write_meta(path, new_guid(), "NativeFormatImporter")


def write_iap_catalog():
    path = RES / "IapProductCatalog.asset"
    yaml = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {IAP_CATALOG_GUID}, type: 3}}
  m_Name: IapProductCatalog
  m_EditorClassIdentifier: 
  products:
  - productId: com.tekhgames.businessempire.removeads
    displayName: Remove Ads
    description: Removes banner and interstitial ads permanently.
    productType: 1
    gemAmount: 0
    cashAmount: 0
    isNonConsumable: 1
  - productId: com.tekhgames.businessempire.starterpack
    displayName: CEO Starter Pack
    description: 25,000 cash + 250 gems — one-time bundle.
    productType: 1
    gemAmount: 250
    cashAmount: 25000
    isNonConsumable: 1
  - productId: com.tekhgames.businessempire.gems100
    displayName: 100 Gems
    description: Premium currency pack.
    productType: 0
    gemAmount: 100
    cashAmount: 0
    isNonConsumable: 0
  - productId: com.tekhgames.businessempire.gems500
    displayName: 500 Gems
    description: Best value gem pack.
    productType: 0
    gemAmount: 500
    cashAmount: 0
    isNonConsumable: 0
  - productId: com.tekhgames.businessempire.gems1200
    displayName: 1200 Gems
    description: Mega gem bundle.
    productType: 0
    gemAmount: 1200
    cashAmount: 0
    isNonConsumable: 0
"""
    path.write_text(yaml, encoding="utf-8")
    write_meta(path, new_guid(), "NativeFormatImporter")


def write_sound_library(audio_guids):
    path = RES / "SoundLibrary.asset"
    fields = [
        "buttonClick", "panelOpen", "panelClose", "error", "purchase", "upgrade",
        "coinCollect", "cashRegister", "reward", "achievement", "levelUp", "dayAdvance",
        "positiveEvent", "negativeEvent", "mainMenuMusic", "gameplayMusic",
    ]
    keys = list(SFX.keys())
    lines = []
    for field, key in zip(fields, keys):
        lines.append(f"  {field}: {audio_ref(audio_guids[key])}")
    yaml = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {SOUND_LIBRARY_GUID}, type: 3}}
  m_Name: SoundLibrary
  m_EditorClassIdentifier: 
{chr(10).join(lines)}
"""
    path.write_text(yaml, encoding="utf-8")
    write_meta(path, new_guid(), "NativeFormatImporter")


def main():
    print("Generating business icons...")
    icon_guids = {}
    for row in BUSINESSES:
        name = row[0]
        icon_guids[name] = write_icon_png(ICON_DIR / f"{name}.png", row[11], row[12])

    print("Generating business definitions...")
    for row in BUSINESSES:
        write_business_definition(row[0], row, icon_guids[row[0]])

    print("Generating sound effects...")
    audio_guids = {}
    for key, spec in SFX.items():
        audio_guids[key] = write_wav(SFX_DIR / f"{key}.wav", synth_samples(spec))

    print("Generating ScriptableObject catalogs...")
    write_iap_catalog()
    write_sound_library(audio_guids)

    # Remove placeholder gitkeep files from populated folders
    for keep in [ICON_DIR / ".gitkeep", DEF_DIR / ".gitkeep", SFX_DIR / ".gitkeep"]:
        if keep.exists():
            keep.unlink()

    print("Static assets generated successfully.")


if __name__ == "__main__":
    main()
