using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Windows;

namespace Amplis
{
    public class Game1 : Game
    {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Personnage p;
        private AnimatedSprite _perso;
        public TiledMap TiledMap { get; set; }
        private TiledMapTileLayer _mapLayer;
        public TiledMapRenderer TiledMapRenderer { get; set; }
        private readonly ScreenManager _screenManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _screenManager = new ScreenManager();
            Components.Add(_screenManager);
        }

        protected override void Initialize()
        {
            p = new Personnage();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteSheet spriteSheet = Content.Load<SpriteSheet>("motw.sf", new JsonContentLoader());
            _perso = new AnimatedSprite(spriteSheet);
            this.TiledMap = Content.Load<TiledMap>("map");
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1072;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            //_graphics.ToggleFullScreen();
            this.TiledMapRenderer = new TiledMapRenderer(GraphicsDevice, this.TiledMap);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            float deltaSecondes = (float)gameTime.ElapsedGameTime.TotalSeconds;
            string animation = p.Anim[p.Pers, 0];
            KeyboardState k = Keyboard.GetState();
            //const float movementSpeed = 200;
            //_camera.Move(GetMovementDirection() * movementSpeed * gameTime.GetElapsedSeconds());

            ushort tx = (ushort)(p.Position.X / this.TiledMap.TileWidth);
            ushort txright= (ushort)(p.Position.X / this.TiledMap.TileWidth+1);
            ushort txleft = (ushort)(p.Position.X / this.TiledMap.TileWidth-1);
            ushort ty = (ushort)(p.Position.Y / this.TiledMap.TileHeight + 3);
            ushort tyfeet = (ushort)(p.Position.Y / this.TiledMap.TileHeight + 2);
            ushort tyarm = (ushort)(p.Position.Y / this.TiledMap.TileHeight + 1);
            ushort tychest = (ushort)(p.Position.Y / this.TiledMap.TileHeight);
            ushort tyhead = (ushort)(p.Position.Y / this.TiledMap.TileHeight -1);
            ushort tyoverhead = (ushort)(p.Position.Y / this.TiledMap.TileHeight - 2);


            if (IsCollision(tx, tyfeet, "Grimpe") &&(k.IsKeyDown(Keys.S)||k.IsKeyDown(Keys.Z) || k.IsKeyDown(Keys.Down) || k.IsKeyDown(Keys.Up)))
                p.Climbing = true;
            else
                p.Climbing = false;
            if (p.Climbing)
            {

                if (k.IsKeyDown(Keys.Up) || k.IsKeyDown(Keys.Z))
                    p.YVelocity = -5;
                else if ((k.IsKeyDown(Keys.S) || k.IsKeyDown(Keys.Down)) && IsCollision(tx, ty, "Grimpe"))
                    p.YVelocity = 5;
            }
            else if (!p. Grounded&&!p.Climbing)
            {
                if (p.YVelocity < 8)
                    p.YVelocity += 1;
                if (p.YVelocity < 0 && IsCollision(tx, tyoverhead, "Collision"))
                    p.YVelocity = 0;
            }
            else
                p.YVelocity = 0;
                
            if (k.IsKeyDown(Keys.Space)&&p.Grounded)
            {
                if(!IsCollision(tx,tyoverhead, "Collision"))
                    p.YVelocity = -13;
            }
            p.XVelocity = 4;



            if (k.IsKeyDown(Keys.J))
            {
                Console.WriteLine(p.PersDelay);
                if (p.PersDelay == 0)
                {
                    p.ChangePers();
                    p.PersDelay = 1;
                }
                    
            }
            if (IsCollision(tx, tyfeet, "Mort"))
                p.Position = new Vector2(100, 500);


            if ((k.IsKeyDown(Keys.D) || k.IsKeyDown(Keys.Right)) && p.Position.X + _perso.TextureRegion.Width / 2 < GraphicsDevice.Viewport.Width - p.XVelocity)
            {
                if (!(IsCollision(txright, tyfeet, "Collision") || IsCollision(txright, tyhead, "Collision") || IsCollision(txright, tychest, "Collision") || IsCollision(txright, tyarm, "Collision")))
                {
                    p.X += p.XVelocity;
                    animation = p.Anim[p.Pers, 3];
                }
                
            }

            else if ((k.IsKeyDown(Keys.Q) || k.IsKeyDown(Keys.Left)) && p.Position.X - _perso.TextureRegion.Width / 2 > 0)
            {
                if (!(IsCollision(txleft, tyfeet, "Collision") ||IsCollision(txleft,tyhead, "Collision") || IsCollision(txleft, tychest, "Collision") || IsCollision(txleft, tyarm, "Collision")))
                {
                    p.X -= p.XVelocity;
                    animation = p.Anim[p.Pers, 2];
                }
            }
            if (!IsCollision(tx, ty, "Collision"))
                p.Grounded = false;

            else
                p.Grounded = true;
            
            if (p.PersDelay > 0)
                p.PersDelay -= deltaSecondes;
            else
                p.PersDelay = 0;

            if (k.IsKeyDown(Keys.L))
                LoadScreen(1);
            p.Y += p.YVelocity;
            _perso.Play(animation);
            _perso.Update(deltaSecondes);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            this.TiledMapRenderer.Draw();
            _spriteBatch.Draw(_perso, p.Position);
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
        private void LoadScreen(int scene)
        {
            _screenManager.LoadScreen(new Screen(this, scene), new FadeTransition(GraphicsDevice, Color.Black));
        }
    }
}
