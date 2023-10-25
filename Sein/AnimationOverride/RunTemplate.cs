using System.Linq;
using UnityEngine;

namespace Sein.AnimationOverride;

internal static class RunTemplate
{
    private static readonly Vector3 SCALE = new(1.55f, 1.55f, 1);
    private static readonly Vector3 OFFSET = new(0.6f, -0.55f, 0);

    public static HeroAnimationTemplate Instance =>
        new("Run", 12.0f, 6, Enumerable.Repeat(new HeroAnimationTemplate.FrameData() { scale = SCALE, offset = OFFSET }, 13).ToList());
}
