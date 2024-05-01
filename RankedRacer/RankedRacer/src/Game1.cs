using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RankedRacer.src.entities;
using System.Collections.Generic;

namespace RankedRacer.src
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private Player player;
        private Texture2D playerTexture;
        private PlatformLevel platLvl;
        private BallotManager ballotManager;
        private EventManager eventManager;
        private Logic logic;
        public Vector2 scale;

        private int backbufferWidth;
        private int backbufferHeight;
        Matrix globalTransformation;

        private GameState state;
        private Texture2D menuTexture, mIcon, bIcon, homeScreen, gameOverScreen1, gameOverScreen2, hallScreen, pauseScreen, prediction;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            logic = new Logic();
            logic.genELection();
            state = GameState.Start;
            base.Initialize();
        }

        public void ScalePresentationArea()
        {
            //Work out how much we need to scale our graphics to fill the screen
            backbufferWidth = _graphics.PreferredBackBufferWidth;
            backbufferHeight = _graphics.PreferredBackBufferHeight;
            float horScaling = backbufferWidth / constants.baseScreenSize.X;
            float verScaling = backbufferHeight / constants.baseScreenSize.Y;
            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);
            EventManager.setTransform(globalTransformation);
            System.Diagnostics.Debug.WriteLine("Screen Size - Width[" + GraphicsDevice.PresentationParameters.BackBufferWidth + "] Height [" + GraphicsDevice.PresentationParameters.BackBufferHeight + "]");
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = this.Content.Load<SpriteFont>("RetroGaming");
            platLvl = new PlatformLevel(Services);
            ballotManager = new BallotManager(Services);

            playerTexture = this.Content.Load<Texture2D>("player");
            player = new Player();
            player.Load(playerTexture, new Vector2(constants.baseScreenSize.X/4, 0));

            menuTexture = this.Content.Load<Texture2D>("menu");
            mIcon = this.Content.Load<Texture2D>("menu_icon");
            bIcon = this.Content.Load<Texture2D>("bcon");
            homeScreen = this.Content.Load<Texture2D>("home");
            gameOverScreen1 = this.Content.Load<Texture2D>("game_over_fell");
            gameOverScreen2 = this.Content.Load<Texture2D>("game_over_caught");
            hallScreen = this.Content.Load<Texture2D>("hall");
            pauseScreen = this.Content.Load<Texture2D>("paused_screen");
            prediction = this.Content.Load<Texture2D>("prediction");

            eventManager = new EventManager(Services, logic, player, platLvl, ballotManager);
            eventManager.statePing += stateUpdate;
            eventManager.end += endGame;

        }

        protected void stateUpdate(GameState s)
        {
            state = s;
        }

        protected void endGame()
        {
            Exit();
        }

        protected override void Update(GameTime gameTime)
        {

            //Confirm the screen has not been resized by the user
            if (backbufferHeight != GraphicsDevice.Viewport.Height ||
                backbufferWidth != GraphicsDevice.Viewport.Width)
            {
                ScalePresentationArea();
            }

            eventManager.handleInput(gameTime);
            //System.Diagnostics.Debug.WriteLine(state);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, globalTransformation);

            drawOverlay(state);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void drawOverlay(GameState targetState)
        {
            // menu top corner
            switch (targetState)
            {
                case GameState.Racing:
                    platLvl.Draw(_spriteBatch);
                    player.Draw(_spriteBatch);
                    _spriteBatch.Draw(bIcon, new Vector2(constants.baseScreenSize.X - (bIcon.Width * 2), 10), Color.White);
                    _spriteBatch.Draw(mIcon, new Vector2(constants.baseScreenSize.X-(mIcon.Width), 10), Color.White);
                    break;
                case GameState.Plot:
                    _spriteBatch.Draw(hallScreen, new Vector2(0, 0), Color.White);
                    player.Draw(_spriteBatch);
                    Prediction pd = eventManager.GetPrediction();
                    if (pd != null)
                    {
                        pd.Draw(_spriteBatch);
                    }
                    break;
                case GameState.Scene:
                    Scene s = eventManager.GetScene();
                    if (s != null) { s.Draw(_spriteBatch); }
                    break;
                case GameState.Puzzle:
                    Puzzle p = eventManager.GetPuzzle();
                    if (p != null) { p.Draw(_spriteBatch); }
                    break;
                default:
                    break;
            }
            switch (targetState)
            {
                case GameState.Start:
                    _spriteBatch.Draw(homeScreen, new Vector2(0, 0), Color.White);
                    break;
                case GameState.Menu:
                    drawOverlay(eventManager.prevState);
                    _spriteBatch.Draw(menuTexture, new Vector2(0, 0), Color.White);
                    break;
                case GameState.Ballots:
                    drawOverlay(eventManager.prevState);
                    ballotManager.Draw(_spriteBatch);
                    break;
                case GameState.GameOver:
                    drawOverlay(eventManager.prevState);
                    _spriteBatch.Draw(eventManager.first ? gameOverScreen1 : gameOverScreen2, new Vector2(0, 0), Color.White);
                    break;
                case GameState.Paused:
                    drawOverlay(eventManager.prevState);
                    _spriteBatch.Draw(pauseScreen, new Vector2(0, 0), Color.White);
                    break;
                default:
                    break;
            }
        }
    }
}
