using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using Il2CppInterop.Runtime;
using System.Runtime.CompilerServices;
using Unity.Collections;
using System.Diagnostics;

[assembly: MelonInfo(typeof(Better_Fusion_Cassidy.Core), "Better Pot & Pumpkin Fusion", "3.1.1", "cassidy, dynaslash, Dakosha, JustNull & Mamoru-kun", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]

namespace Better_Fusion_Cassidy
{
    public class Core : MelonMod
    {
        public static int GetMixData(PlantType firstPlant, PlantType secondPlant)
        {
            int fusionPlant = 0;

            Il2CppSystem.Array mixData = MixData.data.Cast<Il2CppSystem.Array>();
            int[] checks = [mixData.GetValue((long)firstPlant, (long)secondPlant).Unbox<int>(), mixData.GetValue((long)secondPlant, (long)firstPlant).Unbox<int>()];
            fusionPlant = (checks[0] != 0) ? checks[0] : checks[1];

            return fusionPlant;
        }

        public static void CustomEvents(Plant plant)
        {
            PlantType thePlantType = plant.thePlantType;
            int theColumn = plant.thePlantColumn;
            int theRow = plant.thePlantRow;

            switch(thePlantType)
            {
                case PlantType.MelonPot:
                    CreateItem.Instance.SetCoin(theColumn, theRow, 0, 0);
                    CreateItem.Instance.SetCoin(theColumn, theRow, 0, 0);
                    CreateItem.Instance.SetCoin(theColumn, theRow, 0, 0);
                    CreateItem.Instance.SetCoin(theColumn, theRow, 0, 0);
                    CreateItem.Instance.SetCoin(theColumn, theRow, 0, 0);
                    CreateItem.Instance.SetCoin(theColumn, theRow, 0, 0);
                    break;
                default:
                    break;
            }
        }

        public static bool IsPotPlant(Plant plant) => TypeMgr.IsPot(plant.thePlantType);
        public static bool IsPumpkinPlant(Plant plant) => TypeMgr.IsPumpkin(plant.thePlantType);
    }

    [HarmonyPatch(typeof(CreatePlant))]
    public static class CreatePlant_Patch
    {
        [HarmonyPatch(nameof(CreatePlant.CheckMix))]
        [HarmonyPrefix]
        public static bool Pre_CheckMix(CreatePlant __instance, ref int theColumn, ref int theRow, ref PlantType theUsedType, ref GameObject __result)
        {
            if(Input.GetKey(KeyCode.LeftAlt))
            {
                Il2CppSystem.Collections.Generic.List<Plant> gridPlants = Lawnf.Get1x1Plants(theColumn, theRow);
                if (gridPlants != null)
                {
                    foreach (Plant plant in gridPlants)
                    {
                        if (Core.IsPotPlant(plant))
                        {
                            PlantType fusionType = (PlantType)Core.GetMixData(plant.thePlantType, theUsedType);
                            
                            if (fusionType != (PlantType)0)
                            {
                                if (!__instance.Lim(fusionType) && !__instance.LimTravel(fusionType))
                                {
                                    plant.Die(reason: Plant.DieReason.ByMix); 
                                    __result = __instance.SetPlant(theColumn, theRow, fusionType, withEffect: true);
                                    __instance.MixEvent(theUsedType, __result.GetComponent<Plant>(), theRow);
                                    Plant resultPlant = __result.GetComponent<Plant>();
                                    CreateItem.Instance.SetCoin(theColumn, theRow, 0, 0);
                                    Core.CustomEvents(resultPlant);
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                Il2CppSystem.Collections.Generic.List<Plant> gridPlants = Lawnf.Get1x1Plants(theColumn, theRow);
                if (gridPlants != null)
                {
                    foreach (Plant plant in gridPlants)
                    {
                        if (Core.IsPumpkinPlant(plant))
                        {
                            PlantType fusionType = (PlantType)Core.GetMixData(plant.thePlantType, theUsedType);

                            if (fusionType != (PlantType)0)
                            {
                                if (!__instance.Lim(fusionType) && !__instance.LimTravel(fusionType))
                                {
                                    bool ifvStarException = false;

                                    if (fusionType == PlantType.IFVPumpkin && plant.thePlantType == PlantType.IFVPumpkin)
                                    {
                                        if(plant.gameObject.TryGetComponent(out IFVPumpkin ifvPumpkin))
                                        {
                                            ifvStarException = true;
                                            ifvPumpkin.temp = ifvStarException;
                                        }
                                    }

                                    plant.Die(reason: Plant.DieReason.ByMix);
                                    __result = __instance.SetPlant(theColumn, theRow, fusionType, withEffect: true);
                                    __instance.MixEvent(theUsedType, __result.GetComponent<Plant>(), theRow);
                                    Plant resultPlant = __result.GetComponent<Plant>();
                                    Core.CustomEvents(resultPlant);
                                    if (ifvStarException)
                                    {
                                        resultPlant.Die();
                                    }
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        [HarmonyPatch(nameof(CreatePlant.CheckBox))]
        [HarmonyPrefix]
        public static bool Pre_CheckBox(CreatePlant __instance, ref int theBoxColumn, ref int theBoxRow, ref PlantType theSeedType, ref bool __result)
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Il2CppSystem.Collections.Generic.List<Plant> gridPlants = Lawnf.Get1x1Plants(theBoxColumn, theBoxRow);
                if (gridPlants != null)
                {
                    foreach (Plant plant in gridPlants)
                    {
                        if (Core.IsPotPlant(plant))
                        {
                            int fusionType = Core.GetMixData(plant.thePlantType, theSeedType);
                            if (fusionType != 0)
                            {
                                __result = false;
                                return false;
                            }
                        }
                    }
                }
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                Il2CppSystem.Collections.Generic.List<Plant> gridPlants = Lawnf.Get1x1Plants(theBoxColumn, theBoxRow);
                if (gridPlants != null)
                {
                    foreach (Plant plant in gridPlants)
                    {
                        if (Core.IsPumpkinPlant(plant))
                        {
                            int fusionType = Core.GetMixData(plant.thePlantType, theSeedType);
                            if (fusionType != 0)
                            {
                                __result = false;
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(SuperMachineNut))]
    public static class SuperMachineNut_Patch
    {
        [HarmonyPatch(nameof(SuperMachineNut.Summon))]
        [HarmonyPrefix]
        public static bool Pre_Summon(SuperMachineNut __instance)
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                int column = __instance.thePlantColumn;
                int row = __instance.thePlantRow;

                Il2CppSystem.Collections.Generic.List<Plant> gridPlants = Lawnf.Get1x1Plants(column, row);
                if (gridPlants != null)
                {
                    foreach (Plant plant in gridPlants)
                    {
                        if (plant.thePlantType == PlantType.Pot)
                        {
                            if (Mouse.Instance.thePlantTypeOnMouse == PlantType.WallNut)
                            {
                                CreatePlant.Instance.CheckMix(column, row, PlantType.WallNut);
                                __instance.gameObject.AddComponent<PreventRecover>();
                                return false;
                            }
                        }
                    }
                }
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                int column = __instance.thePlantColumn;
                int row = __instance.thePlantRow;

                Il2CppSystem.Collections.Generic.List<Plant> gridPlants = Lawnf.Get1x1Plants(column, row);
                if (gridPlants != null)
                {
                    foreach (Plant plant in gridPlants)
                    {
                        if (plant.thePlantType == PlantType.Pumpkin)
                        {
                            if (Mouse.Instance.thePlantTypeOnMouse == PlantType.WallNut)
                            {
                                CreatePlant.Instance.CheckMix(column, row, PlantType.WallNut);
                                __instance.gameObject.AddComponent<PreventRecover>();
                                return false;
                            }
                        }
                    }
                }
            }
            return true;

        }
    }

    [HarmonyPatch(typeof(Plant))]
    public static class Plant_Patch
    {
        [HarmonyPatch(nameof(Plant.Recover))]
        [HarmonyPrefix]
        public static bool Pre_Recover(Plant __instance)
        {
            if((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftShift)) && __instance.thePlantType == PlantType.SuperMachineNut)
            {
                if(__instance.TryGetComponent<PreventRecover>(out PreventRecover preventRecover))
                {
                    UnityEngine.Object.Destroy(preventRecover);
                    return false;
                }
            }

            return true;
        }
    }

    [RegisterTypeInIl2Cpp]
    public class PreventRecover : MonoBehaviour { }
}