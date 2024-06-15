using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameDataEditor;
using I2.Loc;
using DarkTonic.MasterAudio;
using ChronoArkMod;
using ChronoArkMod.Plugin;
using ChronoArkMod.Template;
using Debug = UnityEngine.Debug;
using ChronoArkMod.ModData;
using HarmonyLib;

namespace KamiyoModSkinHotfix
{
    public class KamiyoModSkinHotfix_Plugin: ChronoArkPlugin
    {
        private Harmony _harmony;

        public override void Dispose()
        {
            _harmony.UnpatchSelf();
        }

        public override void Initialize()
        {
            _harmony = new Harmony(GetGuid());
            _harmony.PatchAll();
        }

        [HarmonyPatch]
        public class MainHarmonyPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(CharacterSkinUI), nameof(CharacterSkinUI.CheckSkinUnlock))]
            public static void CharacterSkinUI_CheckSkinUnlock(SkinPrefab s, ref bool __result)
            {
                if (s._skinData?.Key == null) return;
                __result |= ModManager.IsModAddedGDE(s._skinData.Key) || ModManager.IsModAddedGDE(s._charData.Key);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(CharacterSkinData), nameof(CharacterSkinData.ChangeToBasicSkin))]
            public static void CharacterSkinData_ChangeToBasicSkin(ref List<SkinData> __state)
            {
                __state = new List<SkinData>();
                if (SaveManager.NowData.EnableSkins == null) return;
                __state.AddRange(SaveManager.NowData.EnableSkins);
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(CharacterSkinData), nameof(CharacterSkinData.ChangeToBasicSkin))]
            public static void CharacterSkinData_ChangeToBasicSkin_Post(ref List<SkinData> __state)
            {
                if (SaveManager.NowData.EnableSkins == null) return;
                foreach (var skinData in __state.Where(skinData =>
                             ModManager.IsModAddedGDE(skinData.skinKey) || ModManager.IsModAddedGDE(skinData.charKey)))
                    SaveManager.NowData.EnableSkins.Add(skinData);
            }
        }
    }
}