using MonoGame.Extended.Screens;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Microsoft.Xna.Framework.Graphics;

namespace Amplis
{
    class Screen : GameScreen
    {
        private Game1 _myGame; // pour récupérer le jeu en cours
        private int _map;
        String[] scene = new String[] { "map", "maptest" };
        Vector2[] spawn = new Vector2[] { new Vector2(100, 1200), new Vector2(100, 650) };


        public int Map { get => _map; set => _map = value; }

        public Screen(Game1 game, int map) : base(game)
        {
            _myGame = game;
            Map = map;
            _myGame.p.Position = spawn[this.Map];
        }

        public override void LoadContent()
        {
            _myGame.TiledMap = Content.Load<TiledMap>(scene[this.Map]);
            _myGame.TiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _myGame.TiledMap);
            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
        }


        public override void Draw(GameTime gameTime)
        {

        }
    }
}