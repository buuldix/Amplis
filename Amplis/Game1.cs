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
using System;

namespace Amplis
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Vector2 _persoPosition;
        private int _yVelocity;
        private int _xVelocity;
        private int _jumpStart;
        private bool _grounded;
        private AnimatedSprite _perso;
        private TiledMap _tiledMap;
        private TiledMapTileLayer _mapLayer;
        private TiledMapRenderer _tiledMapRenderer;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _persoPosition = new Vector2(100, 100);
            _jumpStart = (int)_persoPosition.Y;
            _yVelocity = 0;
            _xVelocity = 2;
            _grounded = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // spritesheet
            SpriteSheet spriteSheet = Content.Load<SpriteSheet>("motw.sf", new JsonContentLoader());
            _perso = new AnimatedSprite(spriteSheet);
            // TODO: use this.Content to load your game content here
            _tiledMap = Content.Load<TiledMap>("map");
            _graphics.PreferredBackBufferHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            _graphics.PreferredBackBufferWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            _graphics.ApplyChanges();
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);
            _mapLayer = _tiledMap.GetLayer<TiledMapTileLayer>("sol");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            float deltaSecondes = (float)gameTime.ElapsedGameTime.TotalSeconds;
            string animation = "idle";
            KeyboardState k = Keyboard.GetState();

            if (!_grounded)
            {
                _yVelocity += 1;
            }
            else
            {
                _yVelocity = 0;
            }
                
            if (k.IsKeyDown(Keys.Space)&&_grounded)
            {
                _yVelocity = -10;
            }
            
            
                

            ushort tx = (ushort)(_persoPosition.X / _tiledMap.TileWidth);
            ushort ty = (ushort)(_persoPosition.Y / _tiledMap.TileHeight + 3);
            if (k.IsKeyDown(Keys.D) && _persoPosition.X + _perso.TextureRegion.Width / 2 < GraphicsDevice.Viewport.Width - _xVelocity)
            {
                _persoPosition.X += _xVelocity;
                animation = "walkEast";
            }

            else if (k.IsKeyDown(Keys.Q) && _persoPosition.X - _perso.TextureRegion.Width / 2 > 0)
            {
                _persoPosition.X -= _xVelocity;
                animation = "walkWest";
            } 
            if (!IsCollision(tx, ty))
            {
                //_persoPosition.Y += _yVelocity;
                _grounded = false;
            }
            else
                _grounded = true;
            _persoPosition.Y += _yVelocity;
            _perso.Play(animation);
            _perso.Update(deltaSecondes);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _tiledMapRenderer.Draw();
            _spriteBatch.Draw(_perso, _persoPosition);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private bool IsCollision(ushort x, ushort y)
        {
            TiledMapTile? tile;
            if (_mapLayer.TryGetTile(x, y, out tile) == false)
                return false;
            if (!tile.Value.IsBlank)
                return true;
            return false;
        }
    }
}
