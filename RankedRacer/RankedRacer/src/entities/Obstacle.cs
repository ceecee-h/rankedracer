using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace RankedRacer.src.entities
{
    public class Obstacle : Entity
    {
        private int spacing = (int) constants.baseScreenSize.X / 8;
        private Rectangle bounds;
        public Rectangle gBounds;
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);
        }

        public override void Load(Texture2D texture, Vector2 pos) { }

        public void Load(Texture2D texture, Vector2 pos, List<Obstacle> obstacles)
        {
            sprite = texture;
            choosePosition(pos);
            foreach (Obstacle o in obstacles)
            {
                while (((o.bounds.X + o.bounds.Width) < -(this.bounds.X + this.bounds.Width)) && o.bounds.Intersects(this.bounds))
                {
                    choosePosition(pos);
                }
             }    

        }

        public bool isColliding(GameTime gt, Player p)
        {
            var kstate = Keyboard.GetState();
            Vector2 changeP = position;

            if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D))
            {
                changeP -= constants.bkgVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            }
            if (kstate.IsKeyDown(Keys.Left))
            {
                changeP += constants.bkgVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            }
            Rectangle test = new Rectangle((int)changeP.X, (int)changeP.Y, bounds.Width, bounds.Height);
            return test.Intersects(p.sprite.Bounds);
        }

        public override void Update(GameTime gt)
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
            gBounds = new Rectangle((int)position.X, (int)position.Y-1, gBounds.Width, gBounds.Height);
        }

        public void choosePosition(Vector2 pos)
        {
            Random r = new Random();
            float y = pos.Y + r.Next(-spacing, spacing); // TODO: constrain btw y range on screen
            while (y < constants.baseScreenSize.Y/4 || y > (constants.baseScreenSize.Y-constants.baseScreenSize.Y/4))
            {
                y = pos.Y + r.Next(-spacing, spacing);
            }
            position = new Vector2(pos.X+spacing, y);
            bounds = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
            gBounds = new Rectangle((int)position.X, (int)position.Y - 1, sprite.Width-2, 2);
        }

    }
}
