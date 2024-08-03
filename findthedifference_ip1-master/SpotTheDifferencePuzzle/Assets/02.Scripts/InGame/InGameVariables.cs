using LunarConsolePlugin;

[CVarContainer]
public static class InGameVariables
{
    public static readonly CVar DISPLAY_DEBUG_TOKEN_MARKER = new CVar("Display debug Token Marker", false);
    public static readonly CVar PUZZLE_TOKEN_EXTRA_OFFSET = new CVar("Puzzle Token Extra Offset", 5.0f);

    public static readonly CVar MIN_SCORE_FOR_INTERSTITIAL_AD_DISPLAYED = new CVar("Min Score for Interstitial Ad", 20);
}