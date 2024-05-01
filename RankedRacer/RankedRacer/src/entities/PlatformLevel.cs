using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RankedRacer.src.entities
{
    public class PlatformLevel : Entity
    {
        private ContentManager content;
        private Vector2 position2;
        public List<Obstacle> obstacles;
        private bool genBallots;
        private Texture2D ballotTexture, howToTexture;
        private List<Asset> ballots;
        public Timer timer;

        public bool started;

        public event AssetHandler foundBallot;

        public void reset()
        {
            obstacles = new List<Obstacle>();
            ballots = new List<Asset>();
            this.Load(content.Load<Texture2D>("skyline"), new Vector2(0, 0));
            loadObstacle();
            obstacles[0].position = new Vector2(constants.baseScreenSize.X / 5, constants.baseScreenSize.Y / 2);
            loadObstacle();
            loadObstacle();
            timer.ElapsedTime = 0;
            started = false;
        }

        public PlatformLevel(IServiceProvider serviceProvider, bool genBallots = true)
        {
            obstacles = new List<Obstacle>();
            ballots = new List<Asset>();
            content = new ContentManager(serviceProvider, "Content");
            this.Load(content.Load<Texture2D>("skyline"), new Vector2(0, 0));
            this.genBallots = genBallots;
            ballotTexture = content.Load<Texture2D>("bcon");
            howToTexture = content.Load<Texture2D>("how_to_race");
            loadObstacle();
            obstacles[0].position = new Vector2(constants.baseScreenSize.X / 5, constants.baseScreenSize.Y / 2);
            loadObstacle();
            loadObstacle();            
            timer = new Timer(genBallots ? 90 : 60, new Vector2(305, 26), content.Load<Texture2D>("game_clock"), content.Load<SpriteFont>("RetroGaming"));
            started = false;
        }

        private void loadObstacle()
        {
            Random r = new Random();

            Texture2D texture = content.Load<Texture2D>("plat" + r.Next(1, constants.numObstacles + 1));
            Obstacle o = new Obstacle();
            Vector2 pos = new Vector2(constants.baseScreenSize.X / 4, constants.baseScreenSize.Y / 5);
            try
            {
                Obstacle last = obstacles.Last<Obstacle>();
                pos = new Vector2(last.position.X +last.sprite.Width, last.position.Y);
            } catch (Exception e) {}
            o.Load(texture, pos, obstacles);
            if (genBallots)// && r.Next() % 10 == 0)
            {
                Vector2 newPos = new Vector2((o.position.X + o.gBounds.Width) / 2, o.position.Y - Player.jumpHeight);
                Asset a = new Asset();
                a.Load(ballotTexture, newPos);
                ballots.Add(a);
            }
            obstacles.Add(o);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);

            spriteBatch.Draw(sprite, position2, Color.White);

            foreach (Obstacle o in obstacles)
            {
                o.Draw(spriteBatch);
            }

            foreach (Asset b in ballots)
            {
                b.Draw(spriteBatch);
            }
            if (started  == true)
            {
                timer.Draw(spriteBatch);
            }
            else
            {
                spriteBatch.Draw(howToTexture, new Vector2(0,0), Color.White);
            }
        }

        public override void Load(Texture2D texture, Vector2 position)
        {
            this.position = position;
            sprite = texture;
            position2 = new Vector2(position.X+sprite.Width, position.Y);
        }

        public override void Update(GameTime gt) { }

        public void Update(GameTime gt, Player p)
        {
            if (!started)
            {
                if (KeyBoard.HasBeenPressed(Keys.Enter))
                {
                    started = true;
                }
                return;
            }
            bool isCollision = false;
            if (position.X < -sprite.Width)
            {
                position.X = position2.X + sprite.Width;
            }
            if (position2.X < -sprite.Width)
            {
                position2.X = position.X + sprite.Width;
            }
            for (int i = 0; i < obstacles.Count; i++)
            {
                if (obstacles[i].position.X < -obstacles[i].gBounds.Width)
                {
                    loadObstacle();
                    obstacles.RemoveAt(i);
                }
            }
            for (int i = 0; i < ballots.Count; i++)
            {
                if (ballots[i].position.X < -ballots[i].sprite.Width)
                {
                    ballots.RemoveAt(i);
                }
                else if (ballots[i].bounds.Intersects(p.bounds)) {
                    foundBallot(ballots[i]);
                    ballots.RemoveAt(i);
                };
            }



            foreach (Asset b in ballots) { b.Update(gt); }

            foreach (Obstacle o in obstacles)
            {
                if (o.isColliding(gt, p)) {
                    isCollision = true;

                };
            }

            if (!isCollision)
            {
                foreach (Obstacle o in obstacles)
                {
                    o.Update(gt);
                }

                if (KeyBoard.IsPressed(Keys.Right) || KeyBoard.IsPressed(Keys.D))
                {
                    position -= constants.bkgVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
                    position2 -= constants.bkgVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
                }
                if (KeyBoard.IsPressed(Keys.Left))
                {
                    position += constants.bkgVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
                    position2 += constants.bkgVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
                }
            }
            timer.Update(gt);
        }
    }
}
