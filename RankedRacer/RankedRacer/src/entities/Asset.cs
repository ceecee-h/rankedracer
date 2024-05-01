using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// for now using this for ballot icon
namespace RankedRacer.src.entities
{
    public class Asset : Entity
    {
        public Rectangle bounds;
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);
        }

        public override void Load(Texture2D texture, Vector2 position)
        {
            sprite = texture;
            this.position = position;
            bounds = new Rectangle((int)position.X, (int)position.Y, (int)(sprite.Width), (int)(sprite.Height));
        }

        public override void Update(GameTime gt) // change to higher order func specifying action if hit
        {
            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D))
            {
                position -= constants.bkgVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            }
            if (kstate.IsKeyDown(Keys.Left))
            {
                position += constants.bkgVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            }
            bounds = new Rectangle((int)position.X, (int)position.Y, bounds.Width, bounds.Height);
        }

    }
}
