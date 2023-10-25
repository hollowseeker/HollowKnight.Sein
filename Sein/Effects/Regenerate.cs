using ItemChanger.Extensions;
using Sein.Util;

namespace Sein.Effects;

internal class Regenerate
{
    public static void Hook() => SceneHooks.Hook(SetFocusLines);

    private static void SetFocusLines(bool oriEnabled)
    {
        GOFinder.Knight().FindChild("Focus Effects").FindChild("Lines Anim").active = !oriEnabled;
    }
}
