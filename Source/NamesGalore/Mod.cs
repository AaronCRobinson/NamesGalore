using Verse;
using UnityEngine;

namespace NamesGalore
{
    class NamesGaloreMod : Mod
    {
        public static NamesGaloreSettings settings;

        public NamesGaloreMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<NamesGaloreSettings>();
            settings.rootDir = content.RootDir;
        }

        public override string SettingsCategory() => "Names Galore";

        /*public override void DoSettingsWindowContents(Rect inRect)
        {
            ModWindowHelper.Reset();
            ModWindowHelper.MakeLabeledCheckbox(inRect, "EnableFirestarterAbilityLabel".Translate() + ": ", ref settings.enableFirestarterAbility);
            ModWindowHelper.MakeLabeledCheckbox(inRect, "OnlyPyroLabel".Translate() + ": ", ref settings.onlyPyro);
            ModWindowHelper.MakeLabeledCheckbox(inRect, "EnableResearch".Translate() + ": ", ref settings.enableResearch);
            settings.Write();
        }*/
    }

    public class NamesGaloreSettings : ModSettings
    {
        public string rootDir;

        /*public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.enableFirestarterAbility, "enableFirestarterAbility", true);
            Scribe_Values.Look(ref this.onlyPyro, "onlyPyro", false);
            Scribe_Values.Look(ref this.enableResearch, "enableResearch", true);
        }*/
    }
}
