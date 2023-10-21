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
    private List<GameObject> origChildren;

    protected bool Init()
    {
        if (realHud != null) return true;
        realHud = GOFinder.HudCanvas();
        if (realHud == null) return false;

        origChildren = realHud.AllChildren().ToList();

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
        var hudX = origChildren[0].transform.position.x;
        Vector3 offset = Vector3.zero;
        if (oriActive && hudX > -50)
            offset = HIDE_OFFSET;
        else if (!oriActive && hudX < -50)
            offset = -HIDE_OFFSET;
        foreach (var go in origChildren) go.transform.localPosition += offset;

        if ((oriActive && realHud.activeSelf) != isOriAndHudActive)
        {
            isOriAndHudActive = oriActive && realHud.activeSelf;
            transform.localPosition += isOriAndHudActive ? -HIDE_OFFSET : HIDE_OFFSET;
        }
    }

    protected void Update()
    {
        if (!Init()) return;
        UpdateState(SkinWatcher.OriActive());
    }
}
