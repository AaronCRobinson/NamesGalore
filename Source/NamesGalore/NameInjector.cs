using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using RimWorld;
using Harmony;

namespace NamesGalore
{
    [StaticConstructorOnStartup]
    public static class NameInjector
    {
        static FieldInfo FI_banks = AccessTools.Field(typeof(PawnNameDatabaseShuffled), "banks");

        static NameInjector()
        {
            Log.Message("NamesGalore: Injecting Names");
            Dictionary<PawnNameCategory,NameBank> banks = (Dictionary<PawnNameCategory,NameBank>)FI_banks.GetValue(null);
            NameBank nameBank = banks[PawnNameCategory.HumanStandard];

            if (NamesGaloreMod.settings.international)
            {
                foreach(LoadedLanguage language in LanguageDatabase.AllLoadedLanguages)
                    LoadNamesForLangauge(nameBank, language.folderName);
            }
            else
            {
                LoadNamesForLangauge(nameBank, LanguageDatabase.defaultLanguage.folderName);
            }
#if DEBUG
            nameBank.ErrorCheck();
#endif
        }

        private static void LoadNamesForLangauge(NameBank nameBank, string languageFolderName)
        {
            string relativeLanguagePath = Path.Combine("Languages", languageFolderName);
            // NOTE: from PawnNameDatabaseShuffled
            AddNamesFromFile(nameBank, PawnNameSlot.First, Gender.Male, "First_Male.txt", relativeLanguagePath);
            AddNamesFromFile(nameBank, PawnNameSlot.First, Gender.Female, "First_Female.txt", relativeLanguagePath);
            AddNamesFromFile(nameBank, PawnNameSlot.Nick, Gender.Male, "Nick_Male.txt", relativeLanguagePath);
            AddNamesFromFile(nameBank, PawnNameSlot.Nick, Gender.Female, "Nick_Female.txt", relativeLanguagePath);
            AddNamesFromFile(nameBank, PawnNameSlot.Nick, Gender.None, "Nick_Unisex.txt", relativeLanguagePath);
            AddNamesFromFile(nameBank, PawnNameSlot.Last, Gender.None, "Last.txt", relativeLanguagePath);
        }

        private static void AddNamesFromFile(NameBank nameBank, PawnNameSlot slot, Gender gender, string fileName, string relativeLanguagePath)
        {
            if (File.Exists(fileName))
                nameBank.AddNames(slot, gender, LinesFromFile(ComposePath(fileName, relativeLanguagePath)));
        }

        private static string ComposePath(string fileName, string relativeLanguagePath)
        {
            return Path.Combine(Path.Combine(NamesGaloreMod.settings.rootDir, relativeLanguagePath), Path.Combine("Names", fileName));
        }

        private static IEnumerable<string> LinesFromFile(string filePath)
        {
            string rawText = GenFile.TextFromRawFile(filePath);
            foreach (string line in GenText.LinesFromString(rawText))
            {
                yield return line;
            }
        }
    }
}
