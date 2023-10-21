using GlobalEnums;
using ItemChanger.Extensions;
using Sein.IC;
using Sein.Util;
using UnityEngine;

namespace Sein.Hud;

internal class SpiritLightHud : MonoBehaviour
{
    private static readonly EmbeddedSprite hudSprite = new("SpiritLightHud");
    private static readonly EmbeddedSprite lightSprite = new("SpiritLightOrb");
    private static readonly Vector3 TEXT_OFFSET = new(0, -0.5f, -0.01f);

    private TextMesh realGeoText;
    private TextMesh realGeoAddText;
    private TextMesh realGeoSubtractText;

    private GameObject container;
    private GameObject light;
    private TextMesh spiritLightText;
    private TextMesh spiritLightAddText;
    private TextMesh spiritLightSubtractText;

    protected void Awake()
    {
        var geoCounter = GOFinder.HudCanvas().FindChild("Geo Counter");
        realGeoText = geoCounter.FindChild("Geo Text").GetComponent<TextMesh>();
        realGeoAddText = geoCounter.FindChild("Add Text").GetComponent<TextMesh>();
        realGeoSubtractText = geoCounter.FindChild("Subtract Text").GetComponent<TextMesh>();

        container = AddSprite("Container", hudSprite.Value, 0);
        light = AddSprite("Light", lightSprite.Value, 1);

        spiritLightText = CloneTextMesh("Counter", realGeoText, new(0, 0, 0), Color.black);
        spiritLightAddText = CloneTextMesh("Adder", realGeoAddText, TEXT_OFFSET, Color.white);
        spiritLightSubtractText = CloneTextMesh("Subtractor", realGeoSubtractText, TEXT_OFFSET, Color.red);
    }

    private GameObject AddSprite(string name, Sprite sprite, int sortOrder)
    {
        GameObject obj = new(name);
        obj.layer = (int)PhysLayers.UI;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingLayerName = "Over";
        renderer.sortingOrder = sortOrder;
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        return obj;
    }

    private TextMesh CloneTextMesh(string name, TextMesh prefab, Vector3 offset, Color color)
    {
        GameObject obj = Instantiate(prefab.gameObject);
        foreach (var fsm in obj.GetComponents<PlayMakerFSM>()) Destroy(fsm);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = offset;

        obj.GetComponent<MeshRenderer>().sortingOrder = 2;

        var text = obj.GetComponent<TextMesh>();
        text.alignment = TextAlignment.Center;
        text.anchor = TextAnchor.MiddleCenter;
        text.color = color;
        text.fontSize = 24;

        return text;
    }

    private const int MAX_GEO = 20000;
    private static float MIN_SCALE = 0.1f;
    private static float ROT_SPEED = 30;

    private float GetGeoScale(int counter)
    {
        if (counter >= MAX_GEO) return 1;
        if (counter <= 1) return 0.1f;

        float log = Mathf.Log(counter) / Mathf.Log(MAX_GEO);
        float p = log * log;
        return MIN_SCALE + p * (1 - MIN_SCALE);
    }

    private float angle = 0;

    protected void Update()
    {
        spiritLightText.text = realGeoText.text;
        spiritLightAddText.text = realGeoAddText.text;
        spiritLightSubtractText.text = realGeoSubtractText.text;

        if (int.TryParse(spiritLightText.text, out int counter))
        {
            float scale = GetGeoScale(counter);
            light.transform.localScale = new(scale, scale, 1);
        }
        else light.transform.localScale = new(MIN_SCALE, MIN_SCALE, 1);

        angle += ROT_SPEED * Time.deltaTime;
        light.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
