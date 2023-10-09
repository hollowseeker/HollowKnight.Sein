using ItemChanger.Internal;

namespace Sein.IC;

public class EmbeddedSprite : ItemChanger.EmbeddedSprite
{
    private static readonly SpriteManager manager = new(typeof(EmbeddedSprite).Assembly, "Sein.Resources.Sprites.");

    public EmbeddedSprite(string key) => this.key = key.Replace("/", ".");

    public override SpriteManager SpriteManager => manager;
}
