using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Content;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace Amplis
{
    class Sprite
    {
        private Game1 _myGame;
        private AnimatedSprite _perso;

        public AnimatedSprite Perso { get => _perso; set => _perso = value; }


        public Sprite(Game1 game, string animation)
        {
            SpriteSheet spriteSheet = game.Content.Load<SpriteSheet>(animation, new JsonContentLoader());
            Perso = new AnimatedSprite(spriteSheet);
        }
    }
}
