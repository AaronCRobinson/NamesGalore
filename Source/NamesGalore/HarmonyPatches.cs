using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using RimWorld;
using Harmony;

namespace NamesGalore
{
    [StaticConstructorOnStartup]
    class HarmonyPatches
    {
        static HarmonyPatches()
        {
#if DEBUG
            HarmonyInstance.DEBUG = true;
#endif
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.whyisthat.namesgalore.main");
            harmony.Patch(AccessTools.Method(typeof(PawnBioAndNameGenerator), nameof(PawnBioAndNameGenerator.GeneratePawnName)), null, null, new HarmonyMethod(typeof(HarmonyPatches), nameof(ReduceSolidNameChance)));
            harmony.Patch(AccessTools.Method(typeof(PawnBioAndNameGenerator), "GeneratePawnName_Shuffled"), null, null, new HarmonyMethod(typeof(HarmonyPatches), nameof(AdjustNicknameChance)));
        }

        // NOTE: consider exposing this as a setting but not right meow.
        public static IEnumerable<CodeInstruction> ReduceSolidNameChance(IEnumerable<CodeInstruction> instructions)
        {
            return FloatTranspileroo(instructions, 0.5f, 0.1f);
        }

        // NOTE: consider exposing this as a setting but not right meow.
        public static IEnumerable<CodeInstruction> AdjustNicknameChance(IEnumerable<CodeInstruction> instructions)
        {
            return FloatTranspileroo(instructions, 0.15f, NamesGaloreMod.settings.nicknameProbability);
        }

        public static IEnumerable<CodeInstruction> FloatTranspileroo(IEnumerable<CodeInstruction> instructions, float origVal, float newVal)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == origVal)
                    yield return new CodeInstruction(OpCodes.Ldc_R4, newVal);
                else
                    yield return instruction;
            }
        }

    }
}
