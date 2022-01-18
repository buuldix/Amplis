using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;

namespace Amplis
{
    class Sprite
    {
        public AnimatedSprite Perso { get; set; }


        public Sprite(Game1 game, string animation)
        {
            SpriteSheet spriteSheet = game.Content.Load<SpriteSheet>(animation, new JsonContentLoader());
            Perso = new AnimatedSprite(spriteSheet);
        }
    }
}
