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
        private Sprite boss;
        private Personnage b;
        public Personnage p;
        public TiledMap TiledMap { get; set; }
        private TiledMapTileLayer _mapLayer;
        private Sprite _perso;
        public TiledMapRenderer TiledMapRenderer { get; set; }
        private int _currentMap;
        private readonly ScreenManager _screenManager;
        public enum State { Waiting = 0, Playing = 1 };
        private State state;
        

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
            
            _currentMap = 0;
            state = State.Waiting;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _perso = new Sprite(this, "motw.sf");
            TiledMap = Content.Load<TiledMap>("accueil");
            TiledMap.GetLayer<TiledMapTileLayer>("Logo").IsVisible = false;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1072;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            TiledMapRenderer = new TiledMapRenderer(GraphicsDevice, TiledMap);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            float deltaSecondes = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState k = Keyboard.GetState();
            MouseState m = Mouse.GetState();
            if (TiledMap.Name != "accueil")
                state = State.Playing;
            if (state == State.Playing)
            {
            string animation = p.Anim[p.Pers, 0];


            ushort tx = (ushort)(p.Position.X / TiledMap.TileWidth);
            ushort txright= (ushort)(p.Position.X / TiledMap.TileWidth+1);
            ushort txleft = (ushort)(p.Position.X / TiledMap.TileWidth-1);
            ushort ty = (ushort)(p.Position.Y / TiledMap.TileHeight + 3);
            ushort tyfeet = (ushort)(p.Position.Y / TiledMap.TileHeight + 2);
            ushort tyarm = (ushort)(p.Position.Y / TiledMap.TileHeight + 1);
            ushort tychest = (ushort)(p.Position.Y / TiledMap.TileHeight);
            ushort tyhead = (ushort)(p.Position.Y / TiledMap.TileHeight -1);
            ushort tyoverhead = (ushort)(p.Position.Y / TiledMap.TileHeight - 2);

            //gestion de l'escaslade des échelles
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


            //saut
            if (k.IsKeyDown(Keys.Space)&&p.Grounded)
            {
                p.XVelocity=4;
                if (!IsCollision(tx,tyoverhead, "Collision"))
                    p.YVelocity = -13;
            }
            
            //collision objet à récupérer
            if(IsCollision(txleft,tyfeet, "Objet")|| IsCollision(tx, tyfeet, "Objet")|| IsCollision(txright, tyfeet, "Objet"))
            {
                if (k.IsKeyDown(Keys.R))
                {
                    TiledMap.GetLayer<TiledMapTileLayer>("Objet").IsVisible = false;
                    p.CanGo = true;
                }
            }
            if (!p.CanGo)
                TiledMap.GetLayer<TiledMapTileLayer>("Porte ouverte").IsVisible = false;
            else
            {
                TiledMap.GetLayer<TiledMapTileLayer>("Porte ouverte").IsVisible = true;
                TiledMap.GetLayer<TiledMapTileLayer>("Porte ferme").IsVisible = false;
            }

            //collision porte
            if (IsCollision(tx, tyfeet, "Porte ouverte") && p.CanGo)
            {
                _currentMap++;
                    if (_currentMap == 2)
                    {
                        boss = new Sprite(this, "dragon.sf");
                        b = new Personnage(new String[,] { { "face", "left", "right", "back" } });
                        b.Position = p.Position;
                    }
                new FadeTransition(GraphicsDevice, Color.Black, 1);
                LoadScreen(_currentMap);
            }

                //changement de personnage
                if (k.IsKeyDown(Keys.J))
            {
                Console.WriteLine(p.PersDelay);
                if (p.PersDelay == 0)
                {
                    p.ChangePers();
                    p.PersDelay = 1;
                }
                    
            }
            //détection de la mort
            if (IsCollision(tx, tyfeet, "Mort"))
            {
                TiledMap.GetLayer<TiledMapTileLayer>("Objet").IsVisible = true;
                p.CanGo = false;
                
                LoadScreen(_currentMap);
            }
                
                

            //déplacement vers la droite
            if ((k.IsKeyDown(Keys.D) || k.IsKeyDown(Keys.Right)) && p.Position.X + _perso.Perso.TextureRegion.Width / 2 < GraphicsDevice.Viewport.Width - p.XVelocity)
            {
                if (!(IsCollision(txright, tyfeet, "Collision") || IsCollision(txright, tyhead, "Collision") || IsCollision(txright, tychest, "Collision") || IsCollision(txright, tyarm, "Collision") || p.Climbing))
                {
                    p.X += p.XVelocity;
                    animation = p.Anim[p.Pers, 3];
                }
                
            }
            //déplacement vers la gauche
            else if ((k.IsKeyDown(Keys.Q) || k.IsKeyDown(Keys.Left)) && p.Position.X - _perso.Perso.TextureRegion.Width / 2 > 0)
            {
                if (!(IsCollision(txleft, tyfeet, "Collision") ||IsCollision(txleft,tyhead, "Collision") || IsCollision(txleft, tychest, "Collision") || IsCollision(txleft, tyarm, "Collision")|| p.Climbing))
                {
                    p.X -= p.XVelocity;
                    animation = p.Anim[p.Pers, 2];
                }
            }

            //détection de collision avec le sol
            if (!IsCollision(tx, ty, "Collision")||IsCollision(tx,ty,"Grimpe"))
                p.Grounded = false;

            else
                p.Grounded = true;
            
            //vérification de la possibilité de changer de personnage
            if (p.PersDelay > 0)
                p.PersDelay -= deltaSecondes;
            else
                p.PersDelay = 0;

            //gravité
            p.Y += p.YVelocity;


            if (k.IsKeyDown(Keys.L))
                {
                    //boss = new Sprite(this, "dragon.sf");
                    //b = new Personnage(new String[,] { { "face", "left", "right", "back" } });
                    //b.Position = p.Position;
                    _currentMap = 2;
                    LoadScreen(_currentMap);
                }

                

            //animation du personnage
            _perso.Perso.Play(animation);
            _perso.Perso.Update(deltaSecondes);
            }
            else if (state == State.Waiting)
            {
                ushort mx = (ushort)(m.X / TiledMap.TileWidth);
                ushort my = (ushort)(m.Y / TiledMap.TileHeight);
                if (k.IsKeyDown(Keys.Up) && k.IsKeyDown(Keys.Down))
                    TiledMap.GetLayer<TiledMapTileLayer>("Logo").IsVisible = true;
                if(k.IsKeyDown(Keys.Right)&&k.IsKeyDown(Keys.Left))
                    TiledMap.GetLayer<TiledMapTileLayer>("Logo").IsVisible = false;
                if (IsCollision(mx, my, "Jouer") && m.LeftButton == ButtonState.Pressed)
                {
                    p = new Personnage(new String[,] { { "idle", "walkSouth", "walkWest", "walkEast", "walkNorth" }, { "idle2", "walkSouth2", "walkWest2", "walkEast2", "walkNorth2" } });
                    LoadScreen(_currentMap);
                    IsMouseVisible = false;


                }
                else if (IsCollision(mx, my, "Quitter") && m.LeftButton == ButtonState.Pressed)
                    Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            this.TiledMapRenderer.Draw();
            if(state==State.Playing)
                _spriteBatch.Draw(_perso.Perso, p.Position);
            if (_currentMap == 2)
                _spriteBatch.Draw(boss.Perso, b.Position);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private bool IsCollision(ushort x, ushort y,String layer)
        {
            TiledMapTile? tile;
            _mapLayer = TiledMap.GetLayer<TiledMapTileLayer>(layer);
            if (_mapLayer.TryGetTile(x, y, out tile) == false)
                return false;
            if (!tile.Value.IsBlank)
                return true;
            return false;
        }
        private void LoadScreen(int scene)
        {
            _screenManager.LoadScreen(new Screen(this, scene), new FadeTransition(GraphicsDevice, Color.Black,1));
        }
    }
}
