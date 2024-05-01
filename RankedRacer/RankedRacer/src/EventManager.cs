using RankedRacer.src.entities;
using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RankedRacer.src
{

    public delegate Entity CollisionHandler(Entity e);

    public delegate void AssetHandler(Asset a);

    public delegate Ballot BallotHandler(Ballot b);

    public delegate void StateHandler(GameState state);

    public delegate void PlayerHandler(Player player);

    public delegate void TextDrawn(GameState toState);

    public delegate void EndHandler();

    public delegate void MatrixHandler(List<Ballot> new_ballots);

    public class EventManager
    {
        private IServiceProvider serviceProvider;
        private ContentManager content;

        private Logic gameLogic;
        private PlatformLevel level;
        private BallotManager ballotManager;
        private Player player;
        private GameState gameState;
        public GameState prevState;
        private Scene currentScene;
        private Prediction currentPrediction;
        private Puzzle puzzle;
        private static Matrix globalTransform;

        private Texture2D largePlayer;
        public bool first;

        public event StateHandler statePing;
        public event EndHandler end;

        public EventManager(IServiceProvider services, Logic l, Player p, PlatformLevel lvl, BallotManager ballotManager)
        {
            this.serviceProvider = services;
            content = new ContentManager(serviceProvider, "Content");
            largePlayer = content.Load<Texture2D>("player_large");
            this.gameLogic = l;
            this.player = p;
            this.level = lvl;
            this.ballotManager = ballotManager;
            this.gameState = GameState.Start;
            this.prevState = GameState.Start;
            this.puzzle = null;
            this.currentPrediction = null;
            first = true;
            level.foundBallot += HandleFoundBallot;
            level.timer.timeUp += HandleTimeUp;
            player.onAction += HandlePlayer;
            gameLogic.changeState += HandleStateChange;
        }

        public static void setTransform(Matrix t)
        {
            globalTransform = t;
        }

        public static Matrix getTransform() { return globalTransform; }

        public void addScene(Scene s)
        {
            this.currentScene = s;
            currentScene.endScene += HandleEndScene;
        }

        public void addPuzzle()
        {
            this.puzzle = new Puzzle(serviceProvider, gameLogic.player_ballots, gameLogic.candidates);
            puzzle.matrixUpdated += HandleMatrixUpdated;
            puzzle.submitMatrix += HandleSubmitMatrix;
        }

        public void addPrediction(string predictedWinner)
        {
            if (predictedWinner == constants.UNKNOWN) { predictedWinner = "AMBIGUOUS"; }
            this.currentPrediction = new Prediction(predictedWinner, content.Load<Texture2D>("prediction"), content.Load<SpriteFont>("RetroGaming"));
        }

        public Prediction GetPrediction()
        {
            return this.currentPrediction;
        }

        public Scene GetScene()
        {
            if (this.currentScene == null)
            {
                return null;
            }
            else return currentScene;
        }

        public Puzzle GetPuzzle()
        {
            if (this.puzzle == null)
            {
                return null;
            }
            else return puzzle;
        }

        private void HandleTimeUp(GameState state)
        {
            if (!first) {
                addScene(new Scene(serviceProvider, constants.success, GameState.End));
            } else addScene(new Scene(serviceProvider, constants.timesUp, GameState.Puzzle));
            HandleStateChange(GameState.Scene);
        }

        private void HandleSubmitMatrix()
        {
            this.puzzle = null;
            HandleStateChange(GameState.Plot);
        }

        private void HandleMatrixUpdated(List<Ballot> ballots)
        {
            EMatrix m = new EMatrix(ballots.ToArray(), gameLogic.candidates, gameLogic.numVoters());
            puzzle.setMatrix(m);
        }

        private void HandleEndScene(GameState toState)
        {
             currentScene = null;
             HandleStateChange(toState);
        }

        private void HandlePlayer(Player player)
        {
            if (player.bounds.Y > constants.baseScreenSize.Y)
            {
                HandleStateChange(GameState.GameOver);
            }
        }

        private void HandleFoundBallot(Asset a)
        {
            Ballot b = gameLogic.foundBallot();
            ballotManager.addBallot(b);
        }

        public void HandleStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.Start:
                    
                    break;
                case GameState.Menu:

                    break;
                case GameState.Racing:
                    if (!first)
                    {
                        level = new PlatformLevel(serviceProvider, false);
                        level.started = true;
                    }
                    break;
                case GameState.Ballots:
                    if (gameLogic.player_ballots.Count < 1)
                    {
                        return; 
                    }
                    ballotManager.resetCounter();
                    break;
                case GameState.GameOver:
                    gameLogic.reset();
                    ballotManager.reset();
                    break;
                case GameState.Puzzle:
                    addPuzzle();
                    break;
                case GameState.Plot:
                    player.Load(largePlayer, new Vector2(31, 154));
                    string winner = IRV.runIRV(new List<Ballot>(gameLogic.ballots), new List<String>(gameLogic.candidates), gameLogic.numVoters());
                    string predictWinner = IRV.runIRV(new List<Ballot>(gameLogic.player_ballots), new List<String>(gameLogic.candidates), gameLogic.numVoters());
                    first = false;
                    addPrediction(predictWinner);
                    break;
                default:
                    break;
            }
            prevState = gameState;
            gameState = state;
            System.Diagnostics.Debug.WriteLine("state changed to: " + gameState);
            statePing(gameState);
        }

        public void handleInput(GameTime gt)
        {
            KeyBoard.GetState();
            MyMouse.GetState();

            switch (gameState)
            {
                case GameState.Start:
                    if (KeyBoard.HasBeenPressed(Keys.Enter)) {
                        addScene(new Scene(serviceProvider, constants.intro, GameState.Racing));
                        HandleStateChange(GameState.Scene);
                    }
                    break;
                case GameState.GameOver:
                    if (KeyBoard.HasBeenPressed(Keys.Enter)) {
                        gameLogic.reset();
                        level.reset();
                        player.reset();
                        HandleStateChange(GameState.Racing);
                    }
                    if (MyMouse.Clicked(new Rectangle(112, 265, 181,61)))
                    {
                        HandleStateChange(GameState.Start);
                    }
                    break;
                case GameState.Scene:
                    if (currentScene != null) { currentScene.Update(gt); }
                    break;
                case GameState.Racing:
                    if (level.started) { player.Update(gt, level.obstacles); };
                    level.Update(gt, player);
                    break;
                case GameState.Ballots:
                    ballotManager.Update(gt);
                    break;
                case GameState.Paused:
                    if (KeyBoard.HasBeenPressed(Keys.D9)) {
                    end();
                    }
                    break;
                case GameState.Puzzle:
                    if (puzzle != null)
                    {
                        puzzle.Update(gt);
                    }
                    break;
                case GameState.Plot:
                    player.Update(gt);
                    if (KeyBoard.HasBeenPressed(Keys.Enter))
                    {
                        currentPrediction = null;
                    }
                    if (currentPrediction == null && MyMouse.Clicked(new Rectangle(190, 207, 65, 65)))
                    {
                        player.Load(content.Load<Texture2D>("player"), new Vector2(60, 51));
                        addScene(new Scene(serviceProvider, constants.rig, GameState.Racing));
                        HandleStateChange(GameState.Scene);
                    }

                    break;
            }
        

            if (KeyBoard.HasBeenPressed(Keys.E))
            {
                if (gameState == GameState.Menu)
                {
                    HandleStateChange(prevState);
                }
                else if (!constants.staticStates.Contains(gameState)) HandleStateChange(GameState.Menu);
            }
            if (KeyBoard.HasBeenPressed(Keys.Q))
            {
                if (gameState == GameState.Ballots)
                {
                    HandleStateChange(prevState);
                }
                else if (!constants.staticStates.Contains(gameState)) HandleStateChange(GameState.Ballots);
            }
            if (KeyBoard.HasBeenPressed(Keys.Escape))
            {
                if (gameState == GameState.Paused)
                {
                    HandleStateChange(prevState);
                }
                else if (!constants.staticStates.Contains(gameState)) HandleStateChange(GameState.Paused);
            }

        }

    }

}
