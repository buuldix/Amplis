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
using System.Windows;

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
            SpriteSheet spriteSheet = Content.Load<SpriteSheet>("motw.sf", new JsonContentLoader());
            _perso = new AnimatedSprite(spriteSheet);
            _tiledMap = Content.Load<TiledMap>("map");
            //_graphics.PreferredBackBufferHeight = 1920;
            //_graphics.PreferredBackBufferWidth = 1080;
            //_graphics.IsFullScreen = true;
            //_graphics.ApplyChanges();
            //_graphics.ToggleFullScreen();
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

            ushort tx = (ushort)(_persoPosition.X / _tiledMap.TileWidth);
            ushort txright= (ushort)(_persoPosition.X / _tiledMap.TileWidth+1);
            ushort txleft = (ushort)(_persoPosition.X / _tiledMap.TileWidth-1);
            ushort ty = (ushort)(_persoPosition.Y / _tiledMap.TileHeight + 3);
            ushort tyfeet = (ushort)(_persoPosition.Y / _tiledMap.TileHeight + 2);
            ushort tyarm = (ushort)(_persoPosition.Y / _tiledMap.TileHeight + 1);
            ushort tychest = (ushort)(_persoPosition.Y / _tiledMap.TileHeight);
            ushort tyhead = (ushort)(_persoPosition.Y / _tiledMap.TileHeight -1);
            ushort tyoverhead = (ushort)(_persoPosition.Y / _tiledMap.TileHeight - 2);



            if (!_grounded)
            {
                if (_yVelocity < 5)
                    _yVelocity += 1;
                else if (_yVelocity < 0 && IsCollision(tx, tyoverhead))
                    _yVelocity = 0;
            }
            else
                _yVelocity = 0;
                
            if (k.IsKeyDown(Keys.Space)&&_grounded)
            {
                if(!IsCollision(tx,tyoverhead))
                    _yVelocity = -10;
            }
            
            
                


            if (k.IsKeyDown(Keys.D) && _persoPosition.X + _perso.TextureRegion.Width / 2 < GraphicsDevice.Viewport.Width - _xVelocity)
            {
                if (!(IsCollision(txright, tyfeet) || IsCollision(txright, tyhead) || IsCollision(txright, tychest) || IsCollision(txright, tyarm)))
                {
                    _persoPosition.X += _xVelocity;
                    animation = "walkEast";
                }
                
            }

            else if (k.IsKeyDown(Keys.Q) && _persoPosition.X - _perso.TextureRegion.Width / 2 > 0)
            {
                if (!(IsCollision(txleft, tyfeet)||IsCollision(txleft,tyhead) || IsCollision(txleft, tychest) || IsCollision(txleft, tyarm)))
                {
                    _persoPosition.X -= _xVelocity;
                    animation = "walkWest";
                }
            } 
            if (!IsCollision(tx, ty))
            {
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
