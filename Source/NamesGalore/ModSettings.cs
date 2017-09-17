using Verse;
using UnityEngine;
using ModSettingsHelper;

namespace NamesGalore
{
    public class NamesGaloreSettings : ModSettings
    {
        private const float nicknameProbability_default = 0.3f;

        public string rootDir; // NOTE: no need to expose
        public bool international = false;
        public bool logging = false;
        public float nicknameProbability = nicknameProbability_default;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.international, "international", false);
            Scribe_Values.Look(ref this.logging, "logging", false);
            Scribe_Values.Look(ref this.nicknameProbability, "nicknameProbability", nicknameProbability_default);
        }
    }

    class NamesGaloreMod : Mod
    {
        public static NamesGaloreSettings settings;

        public NamesGaloreMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<NamesGaloreSettings>();
            settings.rootDir = content.RootDir;
        }

        public override string SettingsCategory() => "NG_SettingsCategoryLabel".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            ModWindowHelper.Reset();
            ModWindowHelper.MakeLabeledCheckbox(inRect, $"{"NG_EnableInternationalLabel".Translate()}: ", ref settings.international);
            ModWindowHelper.MakeLabel(inRect, "NG_InternationalNote".Translate());
            ModWindowHelper.MakeLabeledCheckbox(inRect, $"{"NG_EnableLogging".Translate()}: ", ref settings.logging);
            settings.nicknameProbability = ModWindowHelper.HorizontalSlider(inRect, settings.nicknameProbability, 0.0f, 1.0f, leftAlignedLabel: $"{"NG_NicknameProbability".Translate()}: ");
            settings.Write();
        }
    }
}
