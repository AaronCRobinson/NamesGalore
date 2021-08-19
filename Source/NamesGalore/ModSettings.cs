using Verse;
using UnityEngine;
using SettingsHelper;

namespace NamesGalore
{
    public class NamesGaloreSettings : ModSettings
    {
        private const float nicknameProbability_default = 1.0f;
        private const float solidNameProbability_default = 1.0f;

        public string rootDir; // NOTE: no need to expose
        public bool international = false;
        public bool logging = false;
        public float nicknameProbability = nicknameProbability_default;
        public float solidNameProbability = solidNameProbability_default;

      
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.international, "international", false);
            Scribe_Values.Look(ref this.logging, "logging", false);
            Scribe_Values.Look(ref this.nicknameProbability, "nicknameProbability", nicknameProbability_default);
            Scribe_Values.Look(ref this.solidNameProbability, "solidNameProbability", solidNameProbability_default);
        }
    }

    class NamesGaloreMod : Mod
    {
        public static NamesGaloreSettings settings;

        public NamesGaloreMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<NamesGaloreSettings>();
            settings.rootDir = content.RootDir;
            ListingStandardHelper.Gap = 10f;
        }

        public override string SettingsCategory() => "NG_SettingsCategoryLabel".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.AddLabelLine("NG_RestartNote".Translate(), 2f * Text.LineHeight);
            listing.AddHorizontalLine();
            listing.AddLabeledSlider($"{"NG_NicknameProbability".Translate()}: ", ref settings.nicknameProbability, 0.0f, 1.0f);
            listing.AddLabeledSlider($"{"NG_SolidNameProbability".Translate()}: ", ref settings.solidNameProbability, 0.0f, 1.0f);
            listing.AddHorizontalLine();
            listing.AddLabelLine("NG_InternationalNote".Translate());
            listing.AddLabeledCheckbox($"{"NG_EnableLogging".Translate()}: ", ref settings.logging);
            listing.AddHorizontalLine();
            listing.AddLabeledCheckbox($"{"NG_EnableInternationalLabel".Translate()}: ", ref settings.international);
            listing.End();
            settings.Write();
        }
    }
    
}
