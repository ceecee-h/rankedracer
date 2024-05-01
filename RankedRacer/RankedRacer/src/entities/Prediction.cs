using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankedRacer.src.entities
{
    public class Prediction : Entity
    {

        string predictedWinner;
        SpriteFont font;

        public Prediction(string predictedWinner, Texture2D texture, SpriteFont font)
        {
            this.predictedWinner = predictedWinner;
            sprite = texture;
            position = new Vector2(0, 0);
            this.font = font;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);
            spriteBatch.DrawString(font, predictedWinner, new Vector2(157, 185), Color.White);
        }

        public override void Load(Texture2D texture, Vector2 position)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gt)
        {
            throw new NotImplementedException();
        }
    }
}
