using Sein.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sein.Hud;

internal class Hud : MonoBehaviour
{
    public static void Hook()
    {
        GameObject hud = new("OriHud");
        hud.AddComponent<Hud>();
        DontDestroyOnLoad(hud);
    }

    private static Vector3 LIVE_OFFSET = new(0, 6.1f, 0);
    private static Vector3 HIDE_OFFSET = new(-100, 0, 0);
    private GameObject realHud;

    protected bool Init()
    {
        if (realHud != null) return true;
        realHud = GOFinder.HudCanvas();
        if (realHud == null) return false;

        transform.SetParent(realHud.transform.parent);
        transform.position = LIVE_OFFSET + HIDE_OFFSET;
        gameObject.layer = realHud.layer;

        SkinWatcher.OnSkinToggled += UpdateState;

        // Local position center
        GameObject spiritLightHud = new("SpiritLightHud");
        spiritLightHud.transform.SetParent(transform);
        spiritLightHud.transform.localPosition = Vector3.zero;
        spiritLightHud.AddComponent<SpiritLightHud>();

        // TODO: Init other children

        return true;
    }

    private bool isOriAndHudActive = false;

    private void UpdateState(bool oriActive)
    {
        var hudX = realHud.transform.position.x;
        if (oriActive && hudX > -50)
            realHud.transform.localPosition += HIDE_OFFSET;
        else if (!oriActive && hudX < -50)
            realHud.transform.localPosition -= HIDE_OFFSET;

        if ((oriActive && realHud.activeSelf) != isOriAndHudActive)
        {
            isOriAndHudActive = oriActive && realHud.activeSelf;
            var offset = isOriAndHudActive ? -HIDE_OFFSET : HIDE_OFFSET;
            transform.localPosition += offset;
        }
    }

    protected void Update()
    {
        if (!Init()) return;
        UpdateState(SkinWatcher.OriActive());
    }
}
