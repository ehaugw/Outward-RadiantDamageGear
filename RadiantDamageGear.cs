namespace RadiantDamageGear
{
    using InstanceIDs;
    using BepInEx;
    using HarmonyLib;
    using HolyDamageManager;
    using UnityEngine;
    using System.Linq;

    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(HolyDamageManager.GUID, HolyDamageManager.VERSION)]
    public class RadiantDamageGear : BaseUnityPlugin
    {
        public const string GUID = "com.ehaugw.radiantdamagegear";
        public const string VERSION = "3.2.0";
        public const string NAME = "Radiant Damage Gear";

        internal void Awake()
        {
            var harmony = new Harmony(GUID);
            harmony.PatchAll();
        }

        //It's a harmony patch, but still placed here because it's a setup function
        [HarmonyPatch(typeof(ResourcesPrefabManager), "Load")]
        public class ResourcesPrefabManager_Load
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (ResourcesPrefabManager.Instance?.Loaded ?? false)
                {
                    OnPackLoaded();
                }
            }
        }

        private static void OnPackLoaded()
        {
            //WHITE PRIEST SET
            SetDamageBonusModifier(HolyDamageManager.GetDamageType(), IDs.whitePriestRobesID, 10);
            //ReplaceDamageBonusModifier(HolyDamageManager.HolyDamageType(), IDs.whitePriestHoodID, 10);

            //NOVICE SET
            SetDamageBonusModifier(HolyDamageManager.GetDamageType(), IDs.noviceArmorID, 5); //Novice Armor
            SetDamageBonusModifier(HolyDamageManager.GetDamageType(), IDs.noviceHatID, 5); //Novice Hat

            //WOLF SET
            SetDamageBonusModifier(HolyDamageManager.GetDamageType(), IDs.wolfHelmOpenID, 5); //Wolf HelmOpen
            SetDamageBonusModifier(HolyDamageManager.GetDamageType(), IDs.wolfHelmClosedID, 5); //Wolf Helm
            SetDamageBonusModifier(HolyDamageManager.GetDamageType(), IDs.wolfPlateArmorID, 5); //Wolf Chest
            //ReplaceDamageBonusModifier(HolyDamageManager.HolyDamageType(), IDs.wolfPlateBootsID, 5); //Wolf Boots
            SetDamageBonusModifier(HolyDamageManager.GetDamageType(), IDs.wolfShieldID, 5); //Wolf Shield

            //KRYPTEIA SET
            ConvertDamageBonusModifier(HolyDamageManager.GetDamageType(), IDs.krypteiaArmorID, DamageType.Types.Electric); //Krypteia Armor
            ConvertDamageBonusModifier(HolyDamageManager.GetDamageType(), IDs.krypteiaBootsID, DamageType.Types.Electric); //Krypteia Boots
            ConvertDamageBonusModifier(HolyDamageManager.GetDamageType(), IDs.krypteiaHoodID, DamageType.Types.Electric); //Krypteia Hood

            //WHITE PRIEST MITRE
            ConvertDamageBonusModifier(HolyDamageManager.GetDamageType(), IDs.whitePriestMitreID, DamageType.Types.Electric); //Krypteia Hood

            //RED CLANSAGE ROBE
            ConvertDamageBonusModifier(HolyDamageManager.GetDamageType(), IDs.redClansageRobeID, DamageType.Types.Electric); //Krypteia Hood

            //Palladium Sword
            ConvertDamageSource(HolyDamageManager.GetDamageType(), IDs.palladiumAxeID, DamageType.Types.Electric);
            ConvertDamageSource(HolyDamageManager.GetDamageType(), IDs.palladiumClaymoreID, DamageType.Types.Electric);
            ConvertDamageSource(HolyDamageManager.GetDamageType(), IDs.palladiumGreataxeID, DamageType.Types.Electric);
            ConvertDamageSource(HolyDamageManager.GetDamageType(), IDs.palladiumGreathammerID, DamageType.Types.Electric);
            ConvertDamageSource(HolyDamageManager.GetDamageType(), IDs.palladiumHalberdID, DamageType.Types.Electric);
            ConvertDamageSource(HolyDamageManager.GetDamageType(), IDs.palladiumKnucklesID, DamageType.Types.Electric);
            ConvertDamageSource(HolyDamageManager.GetDamageType(), IDs.palladiumMaceID, DamageType.Types.Electric);
            ConvertDamageSource(HolyDamageManager.GetDamageType(), IDs.palladiumShieldID, DamageType.Types.Electric);
            ConvertDamageSource(HolyDamageManager.GetDamageType(), IDs.palladiumSpearID, DamageType.Types.Electric);
            ConvertDamageSource(HolyDamageManager.GetDamageType(), IDs.palladiumSwordID, DamageType.Types.Electric);

        }

        public static void ConvertDamageSource(DamageType.Types typeTo, int itemID, DamageType.Types typeFrom)
        {
            if (GetItem(itemID) is Weapon item)
            {
                var damage = item.Stats.BaseDamage;
                
                if (damage.Contains(typeTo))
                {
                    damage[damage.IndexOf(typeTo)].Damage += damage[damage.IndexOf(typeFrom)].Damage;
                    damage.Remove(typeFrom);
                } else
                {
                    damage.Replace(damage.IndexOf(typeFrom), typeTo);
                }
            }
        }

        public static void ConvertDamageBonusModifier(DamageType.Types typeTo, int itemID, DamageType.Types typeFrom)
        {
            if (GetItem(itemID) is Equipment equipment && equipment.Stats is EquipmentStats stats)
            {
                float[] damageBonuses = (float[])TinyHelper.At.GetValue(typeof(EquipmentStats), stats, "m_damageAttack");
                float value = damageBonuses[(int)typeFrom];
                damageBonuses[(int)typeFrom] = 0;
                damageBonuses[(int)typeTo] += value;
            }
        }

        public static void SetDamageBonusModifier(DamageType.Types type, int itemID, float value)
        {
            EquipmentStats stats = (GetItem(itemID) as Equipment).Stats;
            float[] damageBonuses = (float[])TinyHelper.At.GetValue(typeof(EquipmentStats), stats, "m_damageAttack");
            damageBonuses[(int)type] = value;
        }

        public static Item GetItem(int itemID)
        {
            return ResourcesPrefabManager.Instance.GetItemPrefab(itemID);
        }
    }
}
