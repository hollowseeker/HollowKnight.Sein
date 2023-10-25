using System.Collections.Generic;
using UnityEngine;

namespace Sein.AnimationOverride;

internal class HeroAnimationTemplate
{
    public record State
    {
        public int frame;
        public float frameOverflowSeconds;
    }

    public record FrameData
    {
        public Vector3 scale = new(1, 1, 1);
        public Vector3 offset;
    }

    public readonly string Name;
    private readonly float spf;
    private readonly int loopFrame;
    private List<FrameData> frameData;
    private List<IC.EmbeddedSprite> sprites;

    public HeroAnimationTemplate(string name, float fps, int loopFrame, List<FrameData> frameData)
    {
        this.Name = name;
        this.loopFrame = loopFrame;
        this.spf = 1.0f / fps;
        this.frameData = frameData;
        this.sprites = new();

        for (int i = 0; i < frameData.Count; i++) sprites.Add(new IC.EmbeddedSprite($"{name}.{i}"));
    }

    public int NumFrames => frameData.Count;

    public void UpdateState(ref State state, float time)
    {
        state.frameOverflowSeconds += time;
        while (state.frameOverflowSeconds >= spf)
        {
            if (++state.frame == NumFrames) state.frame = loopFrame;
            state.frameOverflowSeconds -= spf;
        }
    }

    public void ApplyState(HeroControllerStates hcs, State state, SpriteRenderer renderer)
    {
        renderer.sprite = sprites[state.frame].Value;

        var data = frameData[state.frame];
        renderer.transform.localPosition = data.offset;
        renderer.transform.localScale = data.scale;
    }
}
