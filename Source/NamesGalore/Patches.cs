using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;
using RimWorld;
using Harmony;

namespace NamesGalore
{
    class Patcher : GameComponent
    {
        private static float curSolidNameProbability = 0.5f;
        private static float curNicknameProbability = 0.15f;

        public Patcher() { }
        public Patcher(Game g) { }

        public override void FinalizeInit()
        {
#if DEBUG
            HarmonyInstance.DEBUG = true;
#endif
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.whyisthat.namesgalore.patcher");

            harmony.Patch(AccessTools.Method(typeof(PawnBioAndNameGenerator), nameof(PawnBioAndNameGenerator.GeneratePawnName)), null, null, new HarmonyMethod(typeof(Patches), nameof(ReduceSolidNameChance)));
            curSolidNameProbability = NamesGaloreMod.settings.solidNameProbability;

            harmony.Patch(AccessTools.Method(typeof(PawnBioAndNameGenerator), "GeneratePawnName_Shuffled"), null, null, new HarmonyMethod(typeof(Patches), nameof(AdjustNicknameChance)));
            curNicknameProbability = NamesGaloreMod.settings.nicknameProbability;
        }

        private static IEnumerable<CodeInstruction> ReduceSolidNameChance(IEnumerable<CodeInstruction> instructions)
        {
            return FloatTranspileroo(instructions, curSolidNameProbability, NamesGaloreMod.settings.solidNameProbability);
        }

        private static IEnumerable<CodeInstruction> AdjustNicknameChance(IEnumerable<CodeInstruction> instructions)
        {
            return FloatTranspileroo(instructions, curNicknameProbability, NamesGaloreMod.settings.nicknameProbability);
        }

        private static IEnumerable<CodeInstruction> FloatTranspileroo(IEnumerable<CodeInstruction> instructions, float origVal, float newVal)
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
