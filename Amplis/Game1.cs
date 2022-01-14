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
        public const int PHEIGHT = 52;
        public const int PWIDTH = 72;
        public const int BHEIGHT = 64;
        public const int BWIDTH = 64;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //private Sprite boss;
        private Sprite bdf;
        private string banimation;
        private Rectangle rp;
        private Rectangle rpprojectil;
        private Rectangle rb;
        private Vector2 bossCharge;
        private float bossChargeX;
        private float bossChargeY;

        public Personnage p;
        //private Personnage b;
        private Personnage f;
        public TiledMap TiledMap { get; set; }
        private TiledMapTileLayer _mapLayer;
        private Sprite _perso;
        private bool _charge;
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
            if(_currentMap == 2)
                {
                    if(_charge && f.CanGo)
                    {
                        bossCharge = p.Position;
                        bossChargeX = (f.X - bossCharge.X) / 100;
                        bossChargeY = (f.Y - bossCharge.Y) / 100;
                        _charge = false;
                        f.CanGo = false;
                        banimation = Animation(bossCharge, f.Position);
                    }
                    else if(!_charge)
                    {
                        if (Math.Round(bossCharge.X,0) == Math.Round(f.X,0) && Math.Round(bossCharge.Y, 0) == Math.Round(f.Y, 0))
                        {
                            _charge = true;
                        }
                        else
                        {
                            f.X -= bossChargeX;
                            f.Y -= bossChargeY;
                        }
                    }
                    else if(_charge && !f.CanGo)
                    {
                        bossCharge = new Vector2(960, 540);
                        bossChargeX = (f.X - bossCharge.X) / 100;
                        bossChargeY = (f.Y - bossCharge.Y) / 100;
                        f.CanGo = true;
                        _charge = false;
                        banimation = Animation(bossCharge, f.Position);

                    }

                    rp.X = (int)p.X;
                    rp.Y = (int)p.Y;
                    rb.X = (int)f.X;
                    rb.Y = (int)f.Y;
                    rpprojectil.X = (int)p.X - 20;
                    rpprojectil.Y = (int)p.Y - 20;
                    if (k.IsKeyDown(Keys.R))
                    {
                        if (rpprojectil.Intersects(rb) && p.Pers == 1)
                        {
                            bossCharge = new Vector2(960, 540);
                            bossChargeX = (f.X - bossCharge.X) / 100;
                            bossChargeY = (f.Y - bossCharge.Y) / 100;
                            f.CanGo = true;
                            _charge = false;
                            banimation = Animation(bossCharge, f.Position);
                        }
                        if (p.PersDelay == 0)
                        {
                            p.ChangePers();
                            p.PersDelay = 1;
                        }



                    }
                    if (rp.Intersects(rb))
                        Mort();


                    bdf.Perso.Play(banimation);
                    bdf.Perso.Update(deltaSecondes);

                }
            //gestion de l'escaslade des échelles
            if (IsCollision(tx, tyfeet, "Grimpe") &&(k.IsKeyDown(Keys.S)||k.IsKeyDown(Keys.Z) || k.IsKeyDown(Keys.Down) || k.IsKeyDown(Keys.Up)) && TiledMap.GetLayer<TiledMapTileLayer>("Grimpe").IsVisible)
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
                    if(_currentMap == 3)
                        {
                            TiledMap.GetLayer<TiledMapTileLayer>("Mort").IsVisible = true;
                            TiledMap.GetLayer<TiledMapTileLayer>("Grimpe").IsVisible = true;
                            TiledMap.GetLayer<TiledMapTileLayer>("Decor").IsVisible = true;
                            TiledMap.GetLayer<TiledMapTileLayer>("Seum").IsVisible = true;
                        }
                }
            }
            if (!p.CanGo)
                {
                    TiledMap.GetLayer<TiledMapTileLayer>("Porte ouverte").IsVisible = false;
                    TiledMap.GetLayer<TiledMapTileLayer>("Porte ferme").IsVisible = true;
                }
                
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
                        bdf = new Sprite(this, "fireball.sf");
                        f = new Personnage(new String[,] { { "left", "topleft", "top", "topright", "right", "botright", "bot", "botleft" } });
                        f.Position = new Vector2(960,540);
                        _charge = true;
                        rp = new Rectangle((int)p.X, (int)p.Y, PWIDTH, PHEIGHT);
                        rpprojectil = new Rectangle((int)p.X-20, (int)p.Y-20, PWIDTH+40, PHEIGHT+40);
                        rb = new Rectangle((int)f.X, (int)f.Y, BWIDTH, BHEIGHT);
                        p.ChangePers();
                    }
                    p.CanGo = false;
                LoadScreen(_currentMap);
            }

           
                    
            
            //détection de la mort
            if (IsCollision(tx, tyfeet, "Mort") && TiledMap.GetLayer<TiledMapTileLayer>("Mort").IsVisible)
            {
                Mort();
            }
                
                

            //déplacement vers la droite
            if ((k.IsKeyDown(Keys.D) || k.IsKeyDown(Keys.Right)) && p.Position.X + _perso.Perso.TextureRegion.Width / 2 < GraphicsDevice.Viewport.Width - p.XVelocity)
            {
                    if (_currentMap == 3 && TiledMap.GetLayer<TiledMapTileLayer>("Seum").IsVisible)
                    {
                        if (!(IsCollision(txright, tyfeet, "Collision") || IsCollision(txright, tyhead, "Collision") || IsCollision(txright, tychest, "Collision") || IsCollision(txright, tyarm, "Collision") || p.Climbing || IsCollision(txright, tyfeet, "Seum") || IsCollision(txright, tyhead, "Seum") || IsCollision(txright, tychest, "Seum") || IsCollision(txright, tyarm, "Seum")) || IsCollision(txright, tyarm, "Grimpe"))
                        {
                            p.X += p.XVelocity;
                            animation = p.Anim[p.Pers, 2];
                        }
                    }
                    else
                    {
                        if (!(IsCollision(txright, tyfeet, "Collision") || IsCollision(txright, tyhead, "Collision") || IsCollision(txright, tychest, "Collision") || IsCollision(txright, tyarm, "Collision") || p.Climbing) || IsCollision(txright, tyarm, "Grimpe"))
                        {
                            p.X += p.XVelocity;
                            animation = p.Anim[p.Pers, 3];
                        }
                    }
            }
            //déplacement vers la gauche
            else if ((k.IsKeyDown(Keys.Q) || k.IsKeyDown(Keys.Left)) && p.Position.X - _perso.Perso.TextureRegion.Width / 2 > 0)
            {
                if(_currentMap == 3 && TiledMap.GetLayer<TiledMapTileLayer>("Seum").IsVisible)
                {
                    if (!(IsCollision(txleft, tyfeet, "Collision") || IsCollision(txleft, tyhead, "Collision") || IsCollision(txleft, tychest, "Collision") || IsCollision(txleft, tyarm, "Collision") || p.Climbing || IsCollision(txleft, tyfeet, "Seum") || IsCollision(txleft, tyhead, "Seum") || IsCollision(txleft, tychest, "Seum") || IsCollision(txleft, tyarm, "Seum")) || IsCollision(txleft, tyarm, "Grimpe"))
                        {
                            p.X -= p.XVelocity;
                            animation = p.Anim[p.Pers, 2];
                        }
                }
                else
                {
                    if (!(IsCollision(txleft, tyfeet, "Collision") ||IsCollision(txleft,tyhead, "Collision") || IsCollision(txleft, tychest, "Collision") || IsCollision(txleft, tyarm, "Collision")|| p.Climbing) || IsCollision(txleft,tyarm,"Grimpe"))
                        {
                            p.X -= p.XVelocity;
                            animation = p.Anim[p.Pers, 2];
                        }
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
                else if(p.PersDelay<0)
                {
                    p.PersDelay = 0;
                    p.ChangePers();
                }
                

            //gravité
            p.Y += p.YVelocity;


            if (k.IsKeyDown(Keys.L))
            {
                bdf = new Sprite(this, "fireball.sf");
                f = new Personnage(new String[,] { { "left","topleft","top","topright","right","botright","bot","botleft"} } /*{ { "face", "left", "right", "back" } }*/);
                f.Position = new Vector2(960,540);
                _charge = true;
                rp = new Rectangle((int)p.X, (int)p.Y, PWIDTH, PHEIGHT);
                rb = new Rectangle((int)f.X, (int)f.Y, BWIDTH, BHEIGHT);
                rpprojectil = new Rectangle((int)p.X - 20, (int)p.Y - 20, PWIDTH + 40, PHEIGHT + 40);
                p.Pers = 1;
                _currentMap = 2;
                banimation = "left";
                LoadScreen(_currentMap);                    
            }
            if (k.IsKeyDown(Keys.M))
            {
                _currentMap = 3;
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
            TiledMapRenderer.Draw();
            if(state==State.Playing)
                _spriteBatch.Draw(_perso.Perso, p.Position);

            if (_currentMap == 2)
                _spriteBatch.Draw(bdf.Perso, f.Position);                
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
        private void Mort()
        {
            TiledMap.GetLayer<TiledMapTileLayer>("Objet").IsVisible = true;
            p.CanGo = false;
            LoadScreen(_currentMap);
            p.YVelocity = 0;
        }
        private String Animation(Vector2 objectif,Vector2 position)
        {
            float diffX = objectif.X - position.X;
            float diffY = objectif.Y - position.Y;
            if (diffX>400 || diffX < -400)
            {
                if (diffX <= 0)
                    return  "left";
                else
                    return  "right";
            }
            else if(diffX > 200 || diffX < -200)
            {
                if (diffY <= 0)
                {
                    if (diffX <= 0)
                        return "topleft";
                    else
                        return "topright";
                }
                else
                {
                    if(diffX <= 0)
                        return "botleft";
                    else
                        return "botright";
                }
                    
            }
            else
            {
                if (diffY <= 0)
                    return "top";
                else
                    return "bot";

            }
        }
    }   
}
