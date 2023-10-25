using Sein.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Sein.AnimationOverride;

internal class AnimationOverriderAttacher : PersistentAttacher<AnimationOverriderAttacher, HeroAnimationOverrider>
{
    protected override GameObject? FindParent() => GOFinder.Knight();
}

internal class HeroAnimationOverrider : PersistentMonoBehaviour
{
    private class OverrideState
    {
        public string clipName;
        public HeroAnimationTemplate template;
        public HeroAnimationTemplate.State state;
    }

    private delegate void OrigFn();

    private tk2dSpriteAnimator animator;
    private HeroController heroController;
    private MeshRenderer renderer;

    private Dictionary<string, HeroAnimationTemplate> animTemplates = new();
    private OverrideState? overrideState;
    private SpriteRenderer spriteOverrideRenderer;

    private void AddTemplate(HeroAnimationTemplate template) => animTemplates.Add(template.Name, template);

    protected void Awake()
    {
        AddTemplate(RunTemplate.Instance);

        animator = GetComponent<HeroAnimationController>().animator;
        heroController = GetComponent<HeroController>();
        renderer = GetComponent<MeshRenderer>();

        GameObject spriteOverride = new("OriSpriteOverride");
        spriteOverride.transform.SetParent(gameObject.transform);
        spriteOverride.transform.localPosition = Vector3.zero;
        spriteOverrideRenderer = spriteOverride.AddComponent<SpriteRenderer>();
        spriteOverrideRenderer.sortingLayerID = renderer.sortingLayerID;

        On.tk2dSpriteAnimator.IsPlaying_string += OverrideAnimatorIsPlaying;
        On.tk2dSpriteAnimator.Play_string += OverrideAnimatorPlay;
        On.tk2dSpriteAnimator.PlayFromFrame_string_int += OverrideAnimatorPlayFromFrame;
    }

    protected override void OnDestroy()
    {
        On.tk2dSpriteAnimator.IsPlaying_string -= OverrideAnimatorIsPlaying;
        On.tk2dSpriteAnimator.Play_string -= OverrideAnimatorPlay;
        On.tk2dSpriteAnimator.PlayFromFrame_string_int -= OverrideAnimatorPlayFromFrame;
        base.OnDestroy();
    }

    private bool OverrideAnimatorIsPlaying(On.tk2dSpriteAnimator.orig_IsPlaying_string orig, tk2dSpriteAnimator self, string clipName)
    {
        if (self != animator) return orig(self, clipName);

        if (overrideState != null) return overrideState.clipName == clipName;
        else return orig(self, clipName);
    }

    private void OverrideAnimatorPlay(On.tk2dSpriteAnimator.orig_Play_string orig, tk2dSpriteAnimator self, string clipName)
    {
        if (self != animator)
        {
            orig(self, clipName);
            return;
        }

        PlayFromFrameImpl(() => orig(self, clipName), clipName, 0);
    }

    private void OverrideAnimatorPlayFromFrame(On.tk2dSpriteAnimator.orig_PlayFromFrame_string_int orig, tk2dSpriteAnimator self, string clipName, int frame)
    {
        if (self != animator)
        {
            orig(self, clipName, frame);
            return;
        }

        PlayFromFrameImpl(() => orig(self, clipName, frame), clipName, frame);
    }

    private void PlayFromFrameImpl(OrigFn orig_fn, string clipName, int frame)
    {
        if (overrideState == null || overrideState.clipName != clipName)
        {
            overrideState = null;
            if (animTemplates.TryGetValue(clipName, out var template))
            {
                overrideState = new()
                {
                    clipName = clipName,
                    template = template,
                    state = new() { frame = frame, frameOverflowSeconds = 0 },
                };
            }
        }

        // Always update the animator anyway, to keep its CurrentClip accurate.
        UpdateImpl(0);
        orig_fn();
    }

    private void UpdateImpl(float time)
    {
        if (overrideState == null)
        {
            if (!renderer.enabled) renderer.enabled = true;
            if (spriteOverrideRenderer.enabled) spriteOverrideRenderer.enabled = false;
            return;
        }

        if (renderer.enabled) renderer.enabled = false;
        if (!spriteOverrideRenderer.enabled) spriteOverrideRenderer.enabled = true;
        overrideState.template.UpdateState(ref overrideState.state, time);
        overrideState.template.ApplyState(heroController.cState, overrideState.state, spriteOverrideRenderer);
    }

    protected void Update() => UpdateImpl(Time.deltaTime);
}
