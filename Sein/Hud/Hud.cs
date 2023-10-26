using Sein.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sein.Hud;

internal class HudAttacher : PersistentAttacher<HudAttacher, Hud>
{
    protected override GameObject? FindParent() => GOFinder.HudCanvas();
}

internal class Hud : PersistentMonoBehaviour
{
    private static Vector3 LIVE_OFFSET = new(0, 6.1f, 0);
    private static Vector3 HIDE_OFFSET = new(-100, 0, 0);
    private List<GameObject> origChildren;
    private GameObject oriHud;

    protected void Awake()
    {
        origChildren = gameObject.AllChildren().ToList();

        oriHud = new("OriHud");
        oriHud.transform.SetParent(transform);
        oriHud.transform.position = LIVE_OFFSET + HIDE_OFFSET;
        oriHud.layer = gameObject.layer;

        SkinWatcher.OnSkinToggled += UpdateState;

        // Local position center
        GameObject spiritLightHud = new("SpiritLightHud");
        spiritLightHud.transform.SetParent(oriHud.transform);
        spiritLightHud.transform.localPosition = Vector3.zero;
        spiritLightHud.AddComponent<SpiritLightHud>();
    }

    private bool isOriActive = false;

    private void UpdateState(bool oriActive)
    {
        var hudX = origChildren[0].transform.position.x;
        Vector3 offset = Vector3.zero;
        if (oriActive && hudX > -50) offset = HIDE_OFFSET;
        else if (!oriActive && hudX < -50) offset = -HIDE_OFFSET;
        foreach (var go in origChildren) go.transform.localPosition += offset;

        if (oriActive != isOriActive)
        {
            isOriActive = oriActive;
            transform.localPosition += isOriActive ? -HIDE_OFFSET : HIDE_OFFSET;
        }
    }

    protected void Update() => UpdateState(SkinWatcher.OriActive());
}
