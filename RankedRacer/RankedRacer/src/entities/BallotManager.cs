using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace RankedRacer.src.entities
{
    public class BallotManager : Entity
    {
        private ContentManager content;
        private Texture2D arrowTexture;
        private SpriteFont font;
        private List<OBallot> ballots;
        private int currentBallot;

        public BallotManager(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");
            sprite = content.Load<Texture2D>("ballot");
            arrowTexture = content.Load<Texture2D>("arrow");
            font = content.Load<SpriteFont>("RetroGaming");
            currentBallot = 0;
            ballots = new List<OBallot>();
        }

        public void addBallot(Ballot b)
        {
            ballots.Add(new OBallot(b, sprite, font));
        }
        public void resetCounter() { currentBallot = 0; }

        public void reset() { ballots = new List<OBallot>(); }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D rect = new Texture2D(spriteBatch.GraphicsDevice, (int)constants.baseScreenSize.X, (int)constants.baseScreenSize.Y);
            spriteBatch.Draw(rect, new Rectangle(0, 0, (int)constants.baseScreenSize.X, (int)constants.baseScreenSize.Y), Color.Black * 0.5f);

            Vector2 rightPos = new Vector2(3 *constants.baseScreenSize.X / 4, constants.baseScreenSize.Y / 2);
            Vector2 leftPos = new Vector2(constants.baseScreenSize.X / 4, constants.baseScreenSize.Y / 2);
            ballots[currentBallot].Draw(spriteBatch);

            if (currentBallot == 0 && ballots.Count != 1)
            {
                spriteBatch.Draw(arrowTexture, rightPos, Color.White);
            } else if (currentBallot == ballots.Count-1)
            {
                spriteBatch.Draw(arrowTexture, leftPos, new Rectangle(0,0, arrowTexture.Width, arrowTexture.Height), Color.White, 0, leftPos, 1.0f, SpriteEffects.FlipHorizontally, 1);
            } else
            {
                spriteBatch.Draw(arrowTexture, rightPos, Color.White);
                spriteBatch.Draw(arrowTexture, leftPos, new Rectangle(0, 0, arrowTexture.Width, arrowTexture.Height), Color.White, 0, leftPos, 1.0f, SpriteEffects.FlipHorizontally, 1);
            }
        }

        public override void Load(Texture2D texture, Vector2 position)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gt)
        {
            if (KeyBoard.HasBeenPressed(Keys.Right) || KeyBoard.HasBeenPressed(Keys.D)) {
                if (currentBallot < ballots.Count-1){
                    currentBallot++;
                }
            }
            if (KeyBoard.HasBeenPressed(Keys.Left) || KeyBoard.HasBeenPressed(Keys.A))
            {
                if (currentBallot > 0){
                    currentBallot--;
                }
            }
        }
    }
}
