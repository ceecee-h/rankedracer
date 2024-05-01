using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace RankedRacer.src.entities
{
    public class Player : Entity
    {
        private float velocity;
        private bool isGrounded;
        private bool isJumping;
        public static int jumpHeight = (int)constants.baseScreenSize.Y / 6;
        private float jump;
        public Rectangle bounds;

        public event PlayerHandler onAction;

        public void reset()
        {
            position = new Vector2(constants.baseScreenSize.X / 4, 0);
            isJumping = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);
        }

        public override void Load(Texture2D texture, Vector2 position)
        {
            sprite = texture;
            this.position = position;
            velocity = constants.bkgVelocity.X;
            bounds = new Rectangle((int)position.X, (int)position.Y, (int)(sprite.Width), (int)(sprite.Height));
            isJumping = false;
        }

        public override void Update(GameTime gt){
            float changeY = velocity * (float)gt.ElapsedGameTime.TotalSeconds;

            isGrounded = true;

            if (isGrounded && KeyBoard.HasBeenPressed(Keys.Space) && !isJumping)
            {
                isJumping = true;
                jump = 0;

            }
            if (isJumping)
            {
                if (jump >= jumpHeight)
                {
                    isJumping = false;
                }
                else
                {
                    position.Y -= changeY;
                    jump += changeY;
                }

            }
            else if (!isGrounded)
            {
                position.Y += changeY;
            }

            if (KeyBoard.IsPressed(Keys.Right) || KeyBoard.IsPressed(Keys.D))
            {
                position += constants.bkgVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            }
            if (KeyBoard.IsPressed(Keys.Left))
            {
                position -= constants.bkgVelocity * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            bounds = new Rectangle((int)position.X, (int)position.Y, (int)(sprite.Width), (int)(sprite.Height));
            onAction(this);
        }

        public void Update(GameTime gt, List<Obstacle> obstacles)
        {
            float changeY = velocity * (float)gt.ElapsedGameTime.TotalSeconds;

            isGrounded = false;
            foreach (Obstacle entity in obstacles)
            {
                if (entity.gBounds.Intersects(this.bounds)) {
                    isGrounded = true;
                    isJumping = false;
                }; 
            }
            if (isGrounded && KeyBoard.HasBeenPressed(Keys.Space) && !isJumping)
            {
                isJumping = true;
                jump = 0;

            }
            if (isJumping)
            {
                if (jump >= jumpHeight) 
                {
                    isJumping = false;
                } else
                {
                    position.Y -= changeY;
                    jump += changeY;
                }

            } else if (!isGrounded)
            {
                position.Y += changeY;
            }
            bounds = new Rectangle((int)position.X, (int)position.Y, (int)(sprite.Width), (int)(sprite.Height));
            onAction(this);
        }

    }
}
