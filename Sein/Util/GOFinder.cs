using UnityEngine;

namespace Sein.Util
{
    static class GOFinder
    {
        public static GameObject Knight() => GameManager.instance?.hero_ctrl?.gameObject ?? GameObject.Find("Knight");

        public static HeroController HeroController() => GameManager.instance?.hero_ctrl ?? GameObject.Find("Knight").GetComponent<HeroController>();
    }
}
