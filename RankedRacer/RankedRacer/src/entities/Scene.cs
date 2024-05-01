using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace RankedRacer.src.entities
{
    public class Scene : Entity
    {
        ContentManager content;
        sequence sequence;
        Dialog currentDialog;
        SpriteFont font;
        public GameState toState;

        public event TextDrawn endScene;

        public Scene(IServiceProvider serviceProvider, sequence sequence, GameState toState)
        {
            content = new ContentManager(serviceProvider, "Content");
            font = content.Load<SpriteFont>("RetroGaming");
            sprite = content.Load<Texture2D>("skyline");
            this.toState = toState;
            this.sequence = sequence;
            addDialog();
        }

        public void addDialog()
        {
            string name, text;
            (name, text) = sequence.Next();
            if (name == null || text == null)
            {
                return;
            }
            Texture2D dTexture = content.Load<Texture2D>(name + "_dialog");
            currentDialog = new Dialog(name, text, dTexture, font);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Vector2(0, 0), Color.White);
            currentDialog.Draw(spriteBatch);
        }

        public override void Load(Texture2D texture, Vector2 position)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gt)
        {

            if (KeyBoard.HasBeenPressed(Keys.Enter))
            {
                if (sequence.isIterated())
                {
                    System.Diagnostics.Debug.WriteLine("end scene");
                    endScene(toState);
                } else addDialog();
            }
            currentDialog.Update(gt);
        }
    }
}
