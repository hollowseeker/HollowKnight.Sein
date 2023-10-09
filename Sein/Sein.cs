using Modding;
using PurenailCore.ModUtil;
using System;

namespace Sein
{
    public class SeinMod : Mod
    {
        private static SeinMod? _instance;

        internal static SeinMod Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException($"An instance of {nameof(SeinMod)} was never constructed");
                return _instance;
            }
        }

        private static readonly string _version = VersionUtil.ComputeVersion<SeinMod>();

        public override string GetVersion() => _version;

        public SeinMod() : base("Sein")
        {
            _instance = this;
        }

        // public static bool OriActive() => SkinManager.GetCurrentSkin().GetId() == "Ori";
        public static bool OriActive() => true;

        public override void Initialize()
        {
            Orb.Hook();
        }
    }
}
