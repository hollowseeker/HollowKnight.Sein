using ItemChanger;

namespace Sein.Effects
{
    static class SceneHooks
    {
        public delegate void ConditionalSceneHook(bool oriEnabled);
        public delegate void SceneHook();

        public static void Hook(ConditionalSceneHook hook)
        {
            Events.OnSceneChange += _ => hook(SeinMod.OriActive());
        }

        public static void HookWhenEnabled(SceneHook hook)
        {
            Events.OnSceneChange += _ =>
            {
                if (!SeinMod.OriActive()) return;
                hook();
            };
        }
    }
}
