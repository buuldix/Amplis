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
        private bool _climbing;
        private AnimatedSprite _perso;
        private TiledMap TiledMap { get; set; }
        private TiledMapTileLayer _mapLayer;
        private TiledMapRenderer _tiledMapRenderer;
        private Personnage p = new Personnage();
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
            _climbing = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteSheet spriteSheet = Content.Load<SpriteSheet>("motw.sf", new JsonContentLoader());
            _perso = new AnimatedSprite(spriteSheet);
            this.TiledMap = Content.Load<TiledMap>("map");
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            //_graphics.ToggleFullScreen();
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, this.TiledMap);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            float deltaSecondes = (float)gameTime.ElapsedGameTime.TotalSeconds;
            string animation = p.Anim[p.Pers, 0];
            KeyboardState k = Keyboard.GetState();

            ushort tx = (ushort)(_persoPosition.X / this.TiledMap.TileWidth);
            ushort txright= (ushort)(_persoPosition.X / this.TiledMap.TileWidth+1);
            ushort txleft = (ushort)(_persoPosition.X / this.TiledMap.TileWidth-1);
            ushort ty = (ushort)(_persoPosition.Y / this.TiledMap.TileHeight + 3);
            ushort tyfeet = (ushort)(_persoPosition.Y / this.TiledMap.TileHeight + 2);
            ushort tyarm = (ushort)(_persoPosition.Y / this.TiledMap.TileHeight + 1);
            ushort tychest = (ushort)(_persoPosition.Y / this.TiledMap.TileHeight);
            ushort tyhead = (ushort)(_persoPosition.Y / this.TiledMap.TileHeight -1);
            ushort tyoverhead = (ushort)(_persoPosition.Y / this.TiledMap.TileHeight - 2);


            if (IsCollision(tx, tychest, "obstacles")&&(k.IsKeyDown(Keys.S)||k.IsKeyDown(Keys.Z)))
                _climbing = true;
            else
                _climbing = false;
            if (_climbing)
            {
                if (k.IsKeyDown(Keys.Z))
                    _yVelocity = -5;

            }
            else if (!_grounded&&!_climbing)
            {
                if (_yVelocity < 5)
                    _yVelocity += 1;
                if (_yVelocity < 0 && IsCollision(tx, tyoverhead, "sol"))
                    _yVelocity = 0;
            }
            else
                _yVelocity = 0;
                
            if (k.IsKeyDown(Keys.Space)&&_grounded)
            {
                if(!IsCollision(tx,tyoverhead, "sol"))
                    _yVelocity = -11;
            }



            if (k.IsKeyDown(Keys.J))
            {
                Console.WriteLine(p.PersDelay);
                if (p.PersDelay == 0)
                {
                    p.ChangePers();
                    p.PersDelay = 1;
                }
                    
            }

            if (k.IsKeyDown(Keys.D) && _persoPosition.X + _perso.TextureRegion.Width / 2 < GraphicsDevice.Viewport.Width - _xVelocity)
            {
                if (!(IsCollision(txright, tyfeet, "sol") || IsCollision(txright, tyhead, "sol") || IsCollision(txright, tychest, "sol") || IsCollision(txright, tyarm, "sol")))
                {
                    _persoPosition.X += _xVelocity;
                    animation = p.Anim[p.Pers, 3];
                }
                
            }

            else if (k.IsKeyDown(Keys.Q) && _persoPosition.X - _perso.TextureRegion.Width / 2 > 0)
            {
                if (!(IsCollision(txleft, tyfeet, "sol") ||IsCollision(txleft,tyhead, "sol") || IsCollision(txleft, tychest, "sol") || IsCollision(txleft, tyarm, "sol")))
                {
                    _persoPosition.X -= _xVelocity;
                    animation = p.Anim[p.Pers, 2];
                }
            }
            if (!IsCollision(tx, ty, "sol"))
                _grounded = false;

            else
                _grounded = true;
            
            if (p.PersDelay > 0)
                p.PersDelay -= deltaSecondes;
            else
                p.PersDelay = 0;
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
        private bool IsCollision(ushort x, ushort y,String layer)
        {
            TiledMapTile? tile;
            _mapLayer = this.TiledMap.GetLayer<TiledMapTileLayer>(layer);
            if (_mapLayer.TryGetTile(x, y, out tile) == false)
                return false;
            if (!tile.Value.IsBlank)
                return true;
            return false;
        }
    }
}
