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
using System.IO;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Amplis
{
    public class Game1 : Game
    {
        public const int PHEIGHT = 52;
        public const int PWIDTH = 72;
        public const int FHEIGHT = 64;
        public const int FWIDTH = 64;
        public const int DHEIGHT = 70;
        public const int DWIDTH = 100;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Sprite dragon;
        private Sprite bdf;
        private string bdfanimation;
        private Rectangle rp;
        private Rectangle rpprojectil;
        private Rectangle rbdf;
        private Rectangle rdragon;
        private Vector2 bossCharge;
        private float bossChargeX;
        private float bossChargeY;
        private bool _isBossAlive;
        private bool _bossCanBeTouched;

        public Personnage p;
        private Personnage d;
        private Personnage f;
        public TiledMap TiledMap { get; set; }
        private TiledMapTileLayer _mapLayer;
        private Sprite _perso;
        private bool _charge;
        private int _dragonhealth;
        public TiledMapRenderer TiledMapRenderer { get; set; }
        private int _currentMap;
        private readonly ScreenManager _screenManager;
        public enum State { Waiting = 0, Playing = 1 };
        private State state;

        private int _nbMort;
        private SpriteFont _texte;
        private Vector2 _positionTexte;

        private Song _musiqueFond;
        private SoundEffect _sonCrie;
        private SoundEffect _sonGrincement;
        private SoundEffect _sonClic;
        private SoundEffect _sonObjet;
        private SoundEffect _sonSaut;
        private SoundEffect _sonFeu;
        private SoundEffect _sonAie;



        //camera
        private OrthographicCamera _camera;
        private Vector2 _cameraPosition;





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
            if (!File.Exists("Stats.txt"))
                ResData();

            RecupData(out _nbMort, out _currentMap);
            _positionTexte = new Vector2(1920 / 2 - 50, 1072 - 70);

            state = State.Waiting;

            //choix niveau
            _currentMap = 1;

            

            //camera
            var viewportadapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1920, 1072);
            _camera = new OrthographicCamera(viewportadapter);
            _cameraPosition = new Vector2(1920 / 2, 1072 / 2);

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
            
            _texte = Content.Load<SpriteFont>("file");

            //_musiqueFond = Content.Load<Song>("lolo");
            //MediaPlayer.Play(_musiqueFond);
           // MediaPlayer.IsRepeating = true;

            _sonCrie = Content.Load<SoundEffect>("crie");
            _sonGrincement = Content.Load<SoundEffect>("grincement");
            _sonClic = Content.Load<SoundEffect>("clic");
            _sonObjet = Content.Load<SoundEffect>("chop");
            _sonSaut = Content.Load<SoundEffect>("saut");
            _sonFeu = Content.Load<SoundEffect>("feu");
            _sonAie = Content.Load<SoundEffect>("aie");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                SaveData(_nbMort, _currentMap);
                Exit();
            }
            float deltaSecondes = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState k = Keyboard.GetState();
            MouseState m = Mouse.GetState();
            if (TiledMap.Name != "accueil")
                state = State.Playing;
            if (state == State.Playing)
            {
                string animation = p.Anim[p.Pers, 0];


                ushort tx = (ushort)(p.Position.X / TiledMap.TileWidth);
                ushort txright = (ushort)(p.Position.X / TiledMap.TileWidth + 1);
                ushort txleft = (ushort)(p.Position.X / TiledMap.TileWidth - 1);
                ushort ty = (ushort)(p.Position.Y / TiledMap.TileHeight + 3);
                ushort tyfeet = (ushort)(p.Position.Y / TiledMap.TileHeight + 2);
                ushort tyarm = (ushort)(p.Position.Y / TiledMap.TileHeight + 1);
                ushort tychest = (ushort)(p.Position.Y / TiledMap.TileHeight);
                ushort tyhead = (ushort)(p.Position.Y / TiledMap.TileHeight - 1);
                ushort tyoverhead = (ushort)(p.Position.Y / TiledMap.TileHeight - 2);

                //camera
                if (_currentMap == 5)
                {
                    _camera.LookAt(_cameraPosition);
                    //TiledMapRenderer.Update(gameTime);
                    MoveCamera(gameTime);
                }

                //Boss
                if (_currentMap == 2 && _isBossAlive)
                {
                    Console.WriteLine($"Charge : {_charge}\nCango : {f.CanGo}\nBossCanBeTouched : {_bossCanBeTouched}\nVie Boss : {_dragonhealth}");
                    if(d.X > 1500 || d.X < 200)
                    {
                        d.XVelocity = -d.XVelocity;
                    }
                    IsMouseVisible = true;
                    if ((_charge && f.CanGo)|| f.Position == new Vector2(-100,-100))
                    {
                        _sonFeu.Play();
                        f.Position = d.Position;
                        bossCharge = p.Position;
                        bossChargeX = (f.X - bossCharge.X) / 100;
                        bossChargeY = (f.Y - bossCharge.Y) / 100;
                        _charge = false;
                        f.CanGo = false;
                        _bossCanBeTouched = false;
                        
                        bdfanimation = Animation(bossCharge, f.Position);

                    }
                    else if (!_charge)
                    {
                        if (Math.Round(bossCharge.X, 0) == Math.Round(f.X, 0) && Math.Round(bossCharge.Y, 0) == Math.Round(f.Y, 0))
                        {
                            _charge = true;
                            Console.WriteLine("oui");
                        }
                        else
                        {
                            f.X -= bossChargeX;
                            f.Y -= bossChargeY;
                        }
                    }
                    else if (_charge && !f.CanGo)
                        f.Position = new Vector2(-100, -100);

                    rp.X = (int)p.X;
                    rp.Y = (int)p.Y;
                    rbdf.X = (int)f.X;
                    rbdf.Y = (int)f.Y;
                    rpprojectil.X = (int)p.X - 80;
                    rpprojectil.Y = (int)p.Y - 80;
                    rdragon.X = (int)d.X;
                    rdragon.Y = (int)d.Y;
                    if (k.IsKeyDown(Keys.R))
                    {
                        if (rpprojectil.Intersects(rbdf) && p.Pers == 1)
                        {
                            _sonFeu.Play();
                            bossCharge = new Vector2(m.X,m.Y);
                            bossChargeX = (f.X - bossCharge.X) / 100;
                            bossChargeY = (f.Y - bossCharge.Y) / 100;
                            f.CanGo = true;
                            _charge = false;
                            _bossCanBeTouched = true;
                            bdfanimation = Animation(bossCharge, f.Position);
                        }
                        if (p.PersDelay == 0)
                        {
                            p.ChangePers();
                            p.PersDelay = 1;
                        }



                    }
                    if (rdragon.Intersects(rbdf) && _bossCanBeTouched)
                    {
                        _sonAie.Play();
                        _dragonhealth--;
                        f.Position = new Vector2(-100, -100);
                        if (_dragonhealth == 0) {
                            _sonAie.Play();
                            _sonGrincement.Play();                           
                            _isBossAlive = false;
                            d.Position = new Vector2(-100, -100);
                            f.Position = new Vector2(-100, -100);
                            p.CanGo = true;
                        }                   
                    }
                    if (rp.Intersects(rbdf))
                        Mort();


                    bdf.Perso.Play(bdfanimation);
                    bdf.Perso.Update(deltaSecondes);
                    d.X += d.XVelocity;
                }
                else
                {
                    IsMouseVisible = false;
                }
                //gestion de l'escaslade des échelles
                if (IsCollision(tx, tyfeet, "Grimpe") && (k.IsKeyDown(Keys.S) || k.IsKeyDown(Keys.Z) || k.IsKeyDown(Keys.Down) || k.IsKeyDown(Keys.Up)) && TiledMap.GetLayer<TiledMapTileLayer>("Grimpe").IsVisible)
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
                else if (!p.Grounded && !p.Climbing)
                {
                    if (p.YVelocity < 8)
                        p.YVelocity += 1;
                    if (p.YVelocity < 0 && IsCollision(tx, tyoverhead, "Collision"))
                        p.YVelocity = 0;
                }
                else
                    p.YVelocity = 0;


                //saut
                if (k.IsKeyDown(Keys.Space) && p.Grounded)
                {
                    _sonSaut.Play(0.2f, 0.0f, 0.0f);
                    p.XVelocity = 4;
                    if (TiledMap.GetLayer<TiledMapTileLayer>("Seum").IsVisible)
                    {
                        if (!IsCollision(tx, tyoverhead, "Collision") && !IsCollision(tx, tyoverhead, "Seum"))
                            p.YVelocity = -13;
                    }
                    else
                    {
                        if (!IsCollision(tx, tyoverhead, "Collision"))
                            p.YVelocity = -13;
                    }

                }

                //collision objet à récupérer
                if (IsCollision(txleft, tyfeet, "Objet") || IsCollision(tx, tyfeet, "Objet") || IsCollision(txright, tyfeet, "Objet"))
                {
                    if (k.IsKeyDown(Keys.R))
                    {
                        if (TiledMap.GetLayer<TiledMapTileLayer>("Objet").IsVisible)
                        {
                            _sonGrincement.Play();
                            _sonObjet.Play();
                        }

                        TiledMap.GetLayer<TiledMapTileLayer>("Objet").IsVisible = false;
                        p.CanGo = true;
                        if (_currentMap == 3)
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
                        InitBoss();
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
                    if (TiledMap.GetLayer<TiledMapTileLayer>("Seum").IsVisible)
                    {
                        if (!(IsCollision(txright, tyfeet, "Collision") || IsCollision(txright, tyhead, "Collision") || IsCollision(txright, tychest, "Collision") || IsCollision(txright, tyarm, "Collision") || p.Climbing || IsCollision(txright, tyfeet, "Seum") || IsCollision(txright, tyhead, "Seum") || IsCollision(txright, tychest, "Seum") || IsCollision(txright, tyarm, "Seum")) || IsCollision(txright, tyarm, "Grimpe"))
                        {
                            p.X += p.XVelocity;
                            animation = p.Anim[p.Pers, 3];
                        }
                    }
                    else
                    {
                        if (!(IsCollision(txright, tyfeet, "Collision") || IsCollision(txright, tyhead, "Collision") || IsCollision(txright, tychest, "Collision") || IsCollision(txright, tyarm, "Collision") || p.Climbing || IsCollision(txright, tyarm, "Grimpe")))
                        {
                            p.X += p.XVelocity;
                            animation = p.Anim[p.Pers, 3];
                        }
                    }


                }
                //déplacement vers la gauche
                else if ((k.IsKeyDown(Keys.Q) || k.IsKeyDown(Keys.Left)) && p.Position.X - _perso.Perso.TextureRegion.Width / 2 > 0)
                {

                    if (TiledMap.GetLayer<TiledMapTileLayer>("Seum").IsVisible)
                    {
                        if (!(IsCollision(txleft, tyfeet, "Collision") || IsCollision(txleft, tyhead, "Collision") || IsCollision(txleft, tychest, "Collision") || IsCollision(txleft, tyarm, "Collision") || p.Climbing || IsCollision(txleft, tyfeet, "Seum") || IsCollision(txleft, tyhead, "Seum") || IsCollision(txleft, tychest, "Seum") || IsCollision(txleft, tyarm, "Seum")) || IsCollision(txleft, tyarm, "Grimpe"))
                        {
                            p.X -= p.XVelocity;
                            animation = p.Anim[p.Pers, 2];
                        }
                    }
                    else
                    {
                        if (!(IsCollision(txleft, tyfeet, "Collision") || IsCollision(txleft, tyhead, "Collision") || IsCollision(txleft, tychest, "Collision") || IsCollision(txleft, tyarm, "Collision") || p.Climbing || IsCollision(txleft, tyarm, "Grimpe")))
                        {
                            p.X -= p.XVelocity;
                            animation = p.Anim[p.Pers, 2];
                        }
                    }



                }

                //détection de collision avec le sol

                if (TiledMap.GetLayer<TiledMapTileLayer>("Seum").IsVisible)
                {
                    if (!IsCollision(tx, ty, "Collision") && !IsCollision(tx, ty, "Seum") || IsCollision(tx, ty, "Grimpe"))
                        p.Grounded = false;

                    else
                        p.Grounded = true;
                }
                else
                {
                    if (!IsCollision(tx, ty, "Collision") || IsCollision(tx, ty, "Grimpe"))
                        p.Grounded = false;

                    else
                        p.Grounded = true;
                }


                //vérification de la possibilité de changer de personnage
                if (p.PersDelay > 0)
                    p.PersDelay -= deltaSecondes;
                else if (p.PersDelay < 0)
                {
                    p.PersDelay = 0;
                    p.ChangePers();
                }


                //gravité
                p.Y += p.YVelocity;


                if (k.IsKeyDown(Keys.L))
                {
                    _currentMap = 2;
                    LoadScreen(_currentMap);
                    InitBoss();
                }
                else if (k.IsKeyDown(Keys.M))
                {
                    _currentMap = 5;
                    LoadScreen(_currentMap);
                }

                //animation du personnage
                _perso.Perso.Play(animation);
                _perso.Perso.Update(deltaSecondes);

            }
            //menu
            else if (state == State.Waiting)
            {
                ushort mx = (ushort)(m.X / TiledMap.TileWidth);
                ushort my = (ushort)(m.Y / TiledMap.TileHeight);
                if (k.IsKeyDown(Keys.Up) && k.IsKeyDown(Keys.Down))
                    TiledMap.GetLayer<TiledMapTileLayer>("Logo").IsVisible = true;
                if (k.IsKeyDown(Keys.Right) && k.IsKeyDown(Keys.Left))
                    TiledMap.GetLayer<TiledMapTileLayer>("Logo").IsVisible = false;
                if (IsCollision(mx, my, "Jouer") && m.LeftButton == ButtonState.Pressed)
                {
                    _sonClic.Play();
                    p = new Personnage(new String[,] { { "idle", "walkSouth", "walkWest", "walkEast", "walkNorth" }, { "idle2", "walkSouth2", "walkWest2", "walkEast2", "walkNorth2" } });
                    LoadScreen(_currentMap);
                    IsMouseVisible = false;


                }
                else if (IsCollision(mx, my, "Quitter") && m.LeftButton == ButtonState.Pressed)
                {
                    _sonClic.Play();
                    Exit();
                }
                else if (IsCollision(mx, my, "Amplis") && m.LeftButton == ButtonState.Pressed)
                {

                    _sonClic.Play();
                    ResData();
                    RecupData(out _nbMort, out _currentMap);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            TiledMapRenderer.Draw(_camera.GetViewMatrix());
            if (state == State.Playing)
            {
                _spriteBatch.Draw(_perso.Perso, p.Position);
                if (_currentMap == 2)
                {
                    _spriteBatch.DrawString(_texte, $"Vie Boss : {_dragonhealth}", _positionTexte, Color.White);
                }
                else
                {
                    _spriteBatch.DrawString(_texte, $"Mort : {_nbMort}", _positionTexte, Color.White);
                }
            }


            if (_currentMap == 2)
            {
                _spriteBatch.Draw(bdf.Perso, f.Position);
                _spriteBatch.Draw(dragon.Perso, d.Position);
            }
                




            _spriteBatch.End();
            base.Draw(gameTime);
        }
        //camera
        private Vector2 GetMovementDirection()
        {
            var movementDirection = Vector2.Zero;
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Q) || state.IsKeyDown(Keys.Left))
            {
                movementDirection -= Vector2.UnitX;
            }
            if (state.IsKeyDown(Keys.D) || state.IsKeyDown(Keys.Right))
            {
                movementDirection += Vector2.UnitX;
            }

            if (movementDirection != Vector2.Zero)
            {
                movementDirection.Normalize();
            }

            return movementDirection;
        }

        private void MoveCamera(GameTime gameTime)
        {
            var speed = 200;
            var seconds = gameTime.GetElapsedSeconds();
            var movementDirection = GetMovementDirection();
            _cameraPosition = speed * movementDirection * seconds;
        }


        private bool IsCollision(ushort x, ushort y, String layer)
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
            _screenManager.LoadScreen(new Screen(this, scene), new FadeTransition(GraphicsDevice, Color.Black, 1));
        }
        private void Mort()
        {
            _nbMort++;
            TiledMap.GetLayer<TiledMapTileLayer>("Objet").IsVisible = true;
            p.CanGo = false;
            LoadScreen(_currentMap);
            p.YVelocity = 0;
            _sonCrie.Play();
        }
        private String Animation(Vector2 objectif, Vector2 position)
        {
            float diffX = objectif.X - position.X;
            float diffY = objectif.Y - position.Y;
            if (diffX > 400 || diffX < -400)
            {
                if (diffX <= 0)
                    return "left";
                else
                    return "right";
            }
            else if (diffX > 200 || diffX < -200)
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
                    if (diffX <= 0)
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
        private void SaveData(int morts, int lvl)
        {
            using (StreamWriter sw = new StreamWriter("Stats.txt"))
            {
                sw.WriteLine(morts);
                sw.WriteLine(lvl);
            }
        }
        private void ResData()
        {
            using (StreamWriter sw = new StreamWriter("Stats.txt"))
            {
                sw.WriteLine(0);
                sw.WriteLine(0);
            }
        }
        private void RecupData(out int morts, out int lvl)
        {
            using (StreamReader sr = new StreamReader("Stats.txt"))
            {
                morts = int.Parse(sr.ReadLine());
                lvl = int.Parse(sr.ReadLine());
            }
        }
        private void InitBoss()
        {
            bdf = new Sprite(this, "fireball.sf");
            dragon = new Sprite(this, "dragon.sf");
            f = new Personnage(new String[,] { { "left", "topleft", "top", "topright", "right", "botright", "bot", "botleft" } });
            d = new Personnage(new String[,] { { "face", "left", "right", "back" } });
            d.Position = new Vector2(960, 540);
            f.Position = d.Position;
            d.XVelocity = 5;
            _charge = true;
            rp = new Rectangle((int)p.X, (int)p.Y, PWIDTH, PHEIGHT);
            rpprojectil = new Rectangle((int)p.X - 80, (int)p.Y - 80, PWIDTH + 160, PHEIGHT + 160);
            rbdf = new Rectangle((int)f.X, (int)f.Y, FWIDTH, FHEIGHT);
            rdragon = new Rectangle((int)d.X, (int)d.Y, DWIDTH, DHEIGHT);
            bdfanimation = "left";
            _isBossAlive = true;
            _bossCanBeTouched = false;
            _dragonhealth = 4;
            p.ChangePers();

        }
    }
}
