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
        // note: handle languages better...
        static string relativeLanguagePath = Path.Combine("Languages", LanguageDatabase.activeLanguage.folderName);

        static NameInjector()
        {
            Log.Message("NamesGalore: Injecting Names");
            Dictionary<PawnNameCategory, NameBank> banks = (Dictionary < PawnNameCategory, NameBank > )FI_banks.GetValue(null);
            NameBank nameBank = banks[PawnNameCategory.HumanStandard];
            AddNamesFromFile(nameBank, PawnNameSlot.First, Gender.Male, "First_Male.txt");
            AddNamesFromFile(nameBank, PawnNameSlot.First, Gender.Female, "First_Female.txt");
            AddNamesFromFile(nameBank, PawnNameSlot.Nick, Gender.Male, "Nick_Male.txt");
            AddNamesFromFile(nameBank, PawnNameSlot.Nick, Gender.Female, "Nick_Female.txt");
            AddNamesFromFile(nameBank, PawnNameSlot.Nick, Gender.None, "Nick_Unisex.txt");
            AddNamesFromFile(nameBank, PawnNameSlot.Last, Gender.None, "Last.txt");
        }

        private static string ComposePath(string fileName)
        {
            return Path.Combine(Path.Combine(NamesGaloreMod.settings.rootDir, relativeLanguagePath), Path.Combine("Names", fileName));
        }

        private static void AddNamesFromFile(NameBank nameBank, PawnNameSlot slot, Gender gender, string fileName)
        {
            nameBank.AddNames(slot, gender, LinesFromFile(ComposePath(fileName)));
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
