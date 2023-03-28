using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using modern_press_turns_06;

[assembly: MelonInfo(typeof(ModernPressTurns06), "Modern Press Turns [SMT IV] (0.6 ver.)", "1.0.0", "Matthiew Purple")]
[assembly: MelonGame("アトラス", "smt3hd")]

namespace modern_press_turns_06;
public class ModernPressTurns06 : MelonMod
{
    // PASS (same as in SMTIV)

    // DISMISS
    [HarmonyPatch(typeof(nbActionProcess), nameof(nbActionProcess.SetAction_RETURN))]
    private class Patch2
    {
        public static void Postfix()
        {
            // If there is at least one full press turn
            if (nbMainProcess.nbGetMainProcessData().press4_p != 0)
            {
                // If there are no blinking press turns
                if (nbMainProcess.nbGetMainProcessData().press4_p == nbMainProcess.nbGetMainProcessData().press4_ten)
                {
                    PressTurnsAdjustements.MainFullToBlinking(); // Changes the main press turn from full to blinking
                }
            }

            // If there are no full press turns or at least on blinking press turn, the default behavior is the same as in SMT IV
        }
    }

    // DISMISS (removes the 10 frames delay in press turn consumption for aesthetic reasons)
    [HarmonyPatch(typeof(nbMakePacket), nameof(nbMakePacket.nbMakeNewPressPacket))]
    private class Patch22
    {
        public static void Prefix(ref int startframe, ref int ptype)
        {
            if (ptype == 9) startframe = 0; // If using DISMISS then create the PressPacket immediately
        }
    }

    // SUMMON (also works when enemies do it)
    [HarmonyPatch(typeof(nbKoukaProcess), nameof(nbKoukaProcess.RunSummonKouka))]
    private class Patch3
    {
        public static void Prefix()
        {
            // If there is at least one full press turn
            if (nbMainProcess.nbGetMainProcessData().press4_p != 0)
            {
                // If there are no blinking press turns
                if (nbMainProcess.nbGetMainProcessData().press4_p == nbMainProcess.nbGetMainProcessData().press4_ten)
                {
                    PressTurnsAdjustements.MainFullToBlinking(); // Changes the main press turn from full to blinking
                }
            }

            // If there are no full press turns or at least on blinking press turn, the default behavior is the same as in SMT IV
        }
    }

    // ANALYZE
    [HarmonyPatch(typeof(nbPanelProcess), nameof(nbPanelProcess.nbPanelAnalyzeStart))]
    private class Patch4
    {
        public static void Postfix()
        {
            // If there is at least one full press turn
            if (nbMainProcess.nbGetMainProcessData().press4_p != 0)
            {
                // If there are no blinking press turns
                if (nbMainProcess.nbGetMainProcessData().press4_p == nbMainProcess.nbGetMainProcessData().press4_ten)
                {
                    PressTurnsAdjustements.SecondaryFullToBlinking(); // Only this line is required as the game consumes the press turn AFTER this is called
                }
            }
        }
    }

    
    public static class PressTurnsAdjustements
    {
        public static void MainFullToBlinking()
        {
            // The game automatically consumes the main full press turn
            nbMainProcess.nbGetMainProcessData().press4_ten++; // Adds a blinking press turn
        }

        public static void SecondaryFullToBlinking()
        {
            // The game automatically consumes the main blinking press turn
            nbMainProcess.nbGetMainProcessData().press4_ten++; // Adds the blinking press turn back
            nbMainProcess.nbGetMainProcessData().press4_p--; // Converts the first full press turn into a blinking press turn
        }
    }
}
