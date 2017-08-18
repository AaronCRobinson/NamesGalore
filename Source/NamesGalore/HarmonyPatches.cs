using System.Collections.Generic;
using Verse;
using RimWorld;
using Harmony;
using System.Reflection.Emit;

namespace NamesGalore
{
    [StaticConstructorOnStartup]
    class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.whyisthat.namesgalore.main");
            harmony.Patch(AccessTools.Method(typeof(PawnBioAndNameGenerator), nameof(PawnBioAndNameGenerator.GeneratePawnName)), null, null, new HarmonyMethod(typeof(HarmonyPatches), nameof(ReduceSolidNameChance)));
        }

        public static IEnumerable<CodeInstruction> ReduceSolidNameChance(IEnumerable<CodeInstruction> instructions)
        {
            foreach(CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 0.5f)
                    yield return new CodeInstruction(OpCodes.Ldc_R4, 0.1f);
                else
                    yield return instruction;
            }
        }

    }
}
