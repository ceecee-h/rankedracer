using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace RankedRacer.src.entities
{
    public class OBallot : Entity
    {
        private Ballot ballot;
        private SpriteFont font;
        public OBallot(Ballot ballot, Texture2D texture, SpriteFont font)
        {
            this.ballot = ballot;
            this.font = font;
            this.Load(texture, constants.baseScreenSize / 2);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, new Rectangle(0, 0, sprite.Width, sprite.Height), Color.White, 0, new Vector2(sprite.Width/2, sprite.Height/2), 1.0f, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, ballot.formatStr(), position, Color.Black);
        }

        public override void Load(Texture2D texture, Vector2 position)
        {
            sprite = texture;
            this.position = position;
        }

        public override void Update(GameTime gt)
        {
            
        }
    }
}
