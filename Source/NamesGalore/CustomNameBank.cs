using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using RimWorld;
using HarmonyLib;
using System;

namespace NamesGalore
{
    [StaticConstructorOnStartup]
    public static class CustomNameBank
    {
        // TODO: check all access levels...
        private static readonly int numGenders = Enum.GetValues(typeof(Gender)).Length;
        private static readonly int numSlots = Enum.GetValues(typeof(PawnNameSlot)).Length;
        //private static List<string>[,] names = new List<string>[numGenders, numSlots];
        private static Dictionary<string, List<string>[,]> names = new Dictionary<string, List<string>[,]>();
        private static List<string> activeCultures = new List<string>();
        private static string currentCulture;

        private static Harmony _harmony;
        private static Harmony harmony
        {
            get
            {
                if (_harmony == null)
                {
                    _harmony = new Harmony("rimworld.whyisthat.namesgalore.patcher");
                }
                return _harmony;
            }
            set { _harmony = value; }
        }

        static CustomNameBank()
        {
            ApplyPatches();

            if (!NamesGaloreMod.settings.removeDefaultNames)
            {
                NewCultureGrouping("Default");
                AddNamesFromFile(PawnNameSlot.First, Gender.Male, "First_Male");
                AddNamesFromFile(PawnNameSlot.First, Gender.Female, "First_Female");
                AddNamesFromFile(PawnNameSlot.Nick, Gender.Male, "Nick_Male");
                AddNamesFromFile(PawnNameSlot.Nick, Gender.Female, "Nick_Female");
                AddNamesFromFile(PawnNameSlot.Nick, Gender.None, "Nick_Unisex");
                AddNamesFromFile(PawnNameSlot.Last, Gender.None, "Last");
            }
        }
        
        private static void ApplyPatches()
        {
#if DEBUG
            Harmony.DEBUG = true;
#endif
            harmony.Patch(AccessTools.Method(typeof(NameBank), "GetName"), new HarmonyMethod(typeof(CustomNameBank), nameof(GetNamePrefix)));
            harmony.Patch(AccessTools.Constructor(typeof(NameTriple), new Type[] { typeof(string), typeof(string), typeof(string)}), new HarmonyMethod(typeof(CustomNameBank), nameof(FixNicknames)));
            harmony = null;
#if DEBUG
            Harmony.DEBUG = false;
#endif
        }

        public static bool GetNamePrefix(ref string __result, PawnNameSlot slot, Gender gender = Gender.None, bool checkIfAlreadyUsed = true)
        {
            __result = CustomNameBank.GetName(slot, gender, checkIfAlreadyUsed);
            return false;
        }

        public static void FixNicknames(string first, ref string nick, string last)
        {
            if (nick == "")
                nick = (Rand.Value < 0.5f) ? first : last;
        }

        private static void NewCultureGrouping(string culture)
        {
            // initialize lists
            if (!names.ContainsKey(culture))
            {
                names.Add(culture, new List<string>[numGenders, numSlots]);
                for (int i = 0; i < numGenders; i++)
                    for (int j = 0; j < numSlots; j++)
                        names[culture][i, j] = new List<string>();
                activeCultures.Add(culture);
            }
        }

        private static List<string> NamesFor(PawnNameSlot slot, Gender gender, string culture="Default")
        {
            return names[culture][(int)gender, (int)slot];
        }

        public static void AddNames(PawnNameSlot slot, Gender gender, IEnumerable<string> namesToAdd, string culture="Default")
        {
            NewCultureGrouping(culture);
            foreach (string current in namesToAdd)
            {
                NamesFor(slot, gender, culture).Add(current);
            }
        }

        public static void AddNamesFromFile(PawnNameSlot slot, Gender gender, string fileName, string culture="Default")
        {
            
            AddNames(slot, gender, GenFile.LinesFromFile("Names/" + fileName));
        }

        // remove cultures/languages which no longer have very many unused names
        // TODO: look for improvements here on repeating the process with first/nick/last
        private static void RemoveActiveCulture(string culture)
        {
            if (activeCultures.Contains(culture))
                activeCultures.Remove(culture);
            if (activeCultures.Count == 0) // reset
                activeCultures = names.Keys.ToList<string>();
        }

        public static string GetName(PawnNameSlot slot, Gender gender = Gender.None, bool checkIfAlreadyUsed = true)
        {
            //Log.Message($"GetName {slot}");
            // keeps first/nick/last together for a specific language
            if (slot == PawnNameSlot.First)
                currentCulture = activeCultures.RandomElement<string>();

            List<string> list = NamesFor(slot, gender, currentCulture);
            int num = 0;
            if (list.Count == 0)
            {
                // NOTE: some custom name sets have no nicknames... 
                // TODO: get community help to pad out nicknames
                if (slot == PawnNameSlot.Nick)
                    return "";

                Log.Error(string.Concat(new object[]
                {
                    "Name list for gender=",
                    gender,
                    " slot=",
                    slot,
                    " is empty."
                }));
                return "Errorname";
            }
            string text;
            while (true)
            {
                text = list.RandomElement<string>();
                if (checkIfAlreadyUsed && !NameUseChecker.NameWordIsUsed(text))
                {
                    break;
                }
                num++;
                if (num > 50)
                {
                    // remove the cultural names when too commonly used
                    RemoveActiveCulture(currentCulture);
                    return text;
                }
            }
            //Log.Message($"{currentCulture} {slot} {text}");

            return text;
        }
    }
}
