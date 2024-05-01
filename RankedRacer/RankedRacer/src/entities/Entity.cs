using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RankedRacer.src.entities
{
    public abstract class Entity
    {
        public Texture2D sprite;
        public Vector2 position;
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Update(GameTime gt);
        public abstract void Load(Texture2D texture, Vector2 position);
    }
}
