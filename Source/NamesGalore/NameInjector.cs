using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using RimWorld;
using HarmonyLib;

namespace NamesGalore
{
    [StaticConstructorOnStartup]
    public static class NameInjector
    {
        static FieldInfo FI_banks = AccessTools.Field(typeof(PawnNameDatabaseShuffled), "banks");
        static int counter;
        static string curPath;
        static StreamReader stream;

        static NameInjector()
        {
            Log.Message("NamesGalore: Injecting Names");
            //Dictionary<PawnNameCategory,NameBank> banks = (Dictionary<PawnNameCategory,NameBank>)FI_banks.GetValue(null);

            /*if (NamesGaloreMod.settings.removeDefaultNames)
                banks[PawnNameCategory.HumanStandard] = new NameBank(PawnNameCategory.HumanStandard);*/

            string modPath = Path.Combine(NamesGaloreMod.settings.rootDir, "Languages");
            HashSet<string> supportedLanguages = new HashSet<string>();
            string[] languagePaths = Directory.GetDirectories(modPath);
            for(int i = 0; i < languagePaths.Length; i++)
                supportedLanguages.Add(Path.GetFileName(languagePaths[i]));
            Log.Message($"{supportedLanguages.ToList<string>().First<string>()}");

            if (NamesGaloreMod.settings.international)
            {
                foreach(LoadedLanguage language in LanguageDatabase.AllLoadedLanguages)
                    if (supportedLanguages.Contains(language.folderName))
                        LoadNamesForLangauge(language.folderName);
            }
            else
            {
                string curLanguage = LanguageDatabase.activeLanguage.folderName;
                Log.Message($"NamesGalore: Curent Lang {curLanguage}");
                if (supportedLanguages.Contains(curLanguage))
                    LoadNamesForLangauge(curLanguage);
                else
                    Log.Error(string.Format("NG_LanguageNotFound".Translate(), curLanguage));
            }
#if DEBUG
            //nameBank.ErrorCheck();
#endif
        }

        // REFERENCE: from PawnNameDatabaseShuffled
        private static void LoadNamesForLangauge(string languageFolderName)
        {
            if (NamesGaloreMod.settings.logging)
                Log.Message($"Importing names for {languageFolderName} language.");

            string relativeLanguagePath = Path.Combine("Languages", languageFolderName);

#if DEBUG
            Log.Message($"Importing from {relativeLanguagePath}.");
#endif

            AddNamesFromFile(PawnNameSlot.First, Gender.Male, "First_Male.txt", relativeLanguagePath, languageFolderName);
            AddNamesFromFile(PawnNameSlot.First, Gender.Female, "First_Female.txt", relativeLanguagePath, languageFolderName);
            AddNamesFromFile(PawnNameSlot.Nick, Gender.Male, "Nick_Male.txt", relativeLanguagePath, languageFolderName);
            AddNamesFromFile(PawnNameSlot.Nick, Gender.Female, "Nick_Female.txt", relativeLanguagePath, languageFolderName);
            AddNamesFromFile(PawnNameSlot.Nick, Gender.None, "Nick_Unisex.txt", relativeLanguagePath, languageFolderName);
            AddNamesFromFile(PawnNameSlot.Last, Gender.None, "Last.txt", relativeLanguagePath, languageFolderName);
        }

        private static void AddNamesFromFile(PawnNameSlot slot, Gender gender, string fileName, string relativeLanguagePath, string culture)
        {
            bool count = NamesGaloreMod.settings.logging;
            curPath = ComposePath(fileName, relativeLanguagePath);
            if (count)
            {
                counter = 0;
                if (File.Exists(curPath))
                    CustomNameBank.AddNames(slot, gender, LinesFromFileWithCount_Fast(curPath), culture);
                else
                    Log.Error($"Path not found: {curPath}");
                Log.Message($"Imported {counter} {gender} {slot}s.");
            }
            else
            {
                if (File.Exists(curPath))
                    CustomNameBank.AddNames(slot, gender, LineFromFile_Fast(curPath), culture);
                else
                    Log.Error($"Path not found: {curPath}");
            }
        }

        private static string ComposePath(string fileName, string relativeLanguagePath)
        {
            return Path.Combine(Path.Combine(NamesGaloreMod.settings.rootDir, relativeLanguagePath), Path.Combine("Names", fileName));
        }

        // NOTE: no longer any validation. Consider implementing such a `admin` feature.
        private static IEnumerable<string> LineFromFile_Fast(string filePath)
        {
            stream = new StreamReader(filePath);
            while (stream.Peek() >= 0) yield return stream.ReadLine();
        }

        private static IEnumerable<string> LinesFromFileWithCount_Fast(string filePath)
        {
            stream = new StreamReader(filePath);
            while (stream.Peek() >= 0)
            {
                yield return stream.ReadLine();
                counter++;
            }
        }

    }
}
