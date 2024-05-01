using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RankedRacer.src
{

    public class MyMouse
    {
        static MouseState currentKeyState;
        static MouseState previousKeyState;

        public static MouseState GetState()
        {
            previousKeyState = currentKeyState;
            currentKeyState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            return currentKeyState;
        }

        public static bool IsPressed()
        {
            return currentKeyState.LeftButton == ButtonState.Pressed;
        }

        public static bool HasBeenPressed()
        {
            return IsPressed() && previousKeyState.LeftButton != ButtonState.Pressed;
        }

        public static bool Clicked(Rectangle rect)
        {
            
            Vector2 point = new Vector2(currentKeyState.X, currentKeyState.Y);
            point = Vector2.Transform(point, Matrix.Invert(EventManager.getTransform())); ;
            return HasBeenPressed() && rect.Contains(point);
        }
    }
}
