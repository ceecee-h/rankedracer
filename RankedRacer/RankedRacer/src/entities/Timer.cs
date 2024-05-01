using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace RankedRacer.src.entities
{
    public class Timer : Entity
    {
        SpriteFont font;
        public Timer(float seconds, Vector2 position, Texture2D texture, SpriteFont font)
        {
            TotalTime = seconds;
            this.position = position;
            this.font = font;
            sprite = texture;
        }
        public float ElapsedTime { get; set; }
        private float TotalTime { get; }

        public event StateHandler timeUp;

        public bool isDone = false;
        public override void Update(GameTime gameTime)
        {
            ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (ElapsedTime >= TotalTime)
            {
                timeUp(GameState.Scene);
                isDone = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float timePast = TotalTime - ElapsedTime;
            int minutes = (int) timePast / 60;
            int seconds = (int)timePast - (minutes * 60);
            string secs = seconds.ToString().Length < 2 ? "0" + seconds.ToString() : seconds.ToString();
            spriteBatch.Draw(sprite, position, Color.White);
            spriteBatch.DrawString(font, minutes.ToString() + ":" + secs, new Vector2(330, 45), Color.Black);
        }

        public override void Load(Texture2D texture, Vector2 position)
        {
            throw new NotImplementedException();
        }
    }
}