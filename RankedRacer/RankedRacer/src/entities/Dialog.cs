using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RankedRacer.src.entities
{
    public class Dialog : Entity
    {
        string name;
        string text;
        string drawnText;
        int index;
        SpriteFont font;

        private float prev;

        public Dialog(string name, string text, Texture2D texture, SpriteFont font)
        {
            this.name = name;
            this.text = text;
            drawnText = "";
            index = 0;
            this.font = font;
            prev = 0;
            Load(texture, new Vector2(0, constants.baseScreenSize.Y - texture.Height));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);
            spriteBatch.DrawString(font, name, new Vector2(position.X + constants.baseScreenSize.X/4, position.Y), Color.White);
            spriteBatch.DrawString(font, text, new Vector2(position.X + constants.baseScreenSize.X / 4, position.Y + 20), Color.White);
        }

        public override void Load(Texture2D texture, Vector2 position)
        {
            sprite = texture;
            this.position = position;
        }

        public override void Update(GameTime gt)
        {
            if ((gt.ElapsedGameTime.TotalSeconds - prev) >= constants.textSpeed)
            {
                if (drawnText.Length >= text.Length) return;
                else
                {
                    drawnText += text[index];
                    index += 1;
                }
                prev = (float)gt.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
