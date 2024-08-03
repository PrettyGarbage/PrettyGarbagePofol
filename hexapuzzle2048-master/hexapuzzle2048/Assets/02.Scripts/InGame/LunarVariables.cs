using LunarConsolePlugin;

[CVarContainer]
public static class LunarVariables
{
	// Game Manager
	public static readonly CVar dragSensitivity = new CVar("Drag Sensitivity", 100f);
	public static readonly CVar debug = new CVar("Debug", false);

	// Board
	public static readonly CVar combineDuration = new CVar("Cell Merge Duration", 0.3f);
	public static readonly CVar combineInterval = new CVar("Cell Marge Interval", 0.2f);

	// Block
	public static readonly CVar dragBlockOffset = new CVar("Drag Block Offset", 2.0f);
}
