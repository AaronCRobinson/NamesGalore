using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;
using RimWorld;
using HarmonyLib;

namespace NamesGalore
{
    class Patcher : GameComponent
    {
        private static float curSolidNameProbability = 0.5f;
        private static float curNicknameProbability = 0.15f;
        private static Harmony _harmony;
        private static Harmony harmony
        {
            get {
                if (_harmony==null)
                {
                    _harmony = new Harmony("rimworld.whyisthat.namesgalore.patcher");
                }
                return _harmony;
            }
            set { _harmony = value; }
        }

        public Patcher() => ApplyPatches();

        public Patcher(Game g) => ApplyPatches();

        //public override void FinalizeInit()
        private void ApplyPatches()
        {
#if DEBUG
            Harmony.DEBUG = true;
#endif
            if (curSolidNameProbability != NamesGaloreMod.settings.solidNameProbability)
            {
#if DEBUG
                Log.Message("NamesGalore: Patching Solid Name Probability");
#endif
                harmony.Patch(AccessTools.Method(typeof(PawnBioAndNameGenerator), nameof(PawnBioAndNameGenerator.GeneratePawnName)), null, null, new HarmonyMethod(typeof(Patcher), nameof(ReduceSolidNameChance)));
                curSolidNameProbability = NamesGaloreMod.settings.solidNameProbability;
            }

            if (curNicknameProbability != NamesGaloreMod.settings.nicknameProbability)
            {
#if DEBUG
                Log.Message("NamesGalore: Patching Nickname Probability");
#endif
                harmony.Patch(AccessTools.Method(typeof(PawnBioAndNameGenerator), "GeneratePawnName_Shuffled"), null, null, new HarmonyMethod(typeof(Patcher), nameof(AdjustNicknameChance)));
                curNicknameProbability = NamesGaloreMod.settings.nicknameProbability;
            }

            harmony = null;
#if DEBUG
            Harmony.DEBUG = false;
#endif
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
