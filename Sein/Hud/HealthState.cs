using System.Collections.Generic;

namespace Sein.Hud;

public enum MaskFillState
{
    Empty,
    Healing,
    Filled
}

public record MaskState
{
    public bool permanent;
    public MaskFillState fillState;
    public bool hiveblood;
    public bool lifeblood;
}

public class HealthState
{
    public List<MaskState> maskStates = new();
}
