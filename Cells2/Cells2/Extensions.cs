using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace Cells
{
    public static class Extensions
    {
        public static bool IsKeyPressed(this KeyboardState currentState, KeyboardState previousState, Keys keys)
        {
            return currentState.IsKeyDown(keys) && previousState.IsKeyUp(keys);
        }

        public static bool IsClicked(this MouseState currentState, MouseState previousState, MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return currentState.LeftButton == ButtonState.Pressed && previousState.LeftButton == ButtonState.Released;
                case MouseButton.Right:
                    return currentState.RightButton == ButtonState.Pressed && previousState.RightButton == ButtonState.Released;
            }
            return false;
        }

        private static Dictionary<SpriteBatch, Vector2> linePosition = new Dictionary<SpriteBatch, Vector2>();
        public static void LinePosition(this SpriteBatch spriteBatch, Vector2 position)
        {
            if (!linePosition.ContainsKey(spriteBatch))
                linePosition.Add(spriteBatch, position);
            else
                linePosition[spriteBatch] = position;
        }

        public static void LinePosition(this SpriteBatch spriteBatch, float x, float y)
        {
            spriteBatch.LinePosition(new Vector2(x,y));
        }
        public static void DrawLine(this SpriteBatch spriteBatch, SpriteFont font, string text, Color color, float yShift = 0)
        {
            var linePos = linePosition.ContainsKey(spriteBatch) ? linePosition[spriteBatch] : new Vector2(10,10);
            spriteBatch.DrawString(font, text, linePos, color);
            linePos.Y += font.LineSpacing + yShift;
            if (!linePosition.ContainsKey(spriteBatch))
                linePosition.Add(spriteBatch, linePos);
            else
                linePosition[spriteBatch] = linePos;
        }
        public static void DrawCenteredLine(this SpriteBatch spriteBatch, SpriteFont font, string text, Color color, float yShift = 0)
        {
            var linePos = linePosition.ContainsKey(spriteBatch) ? linePosition[spriteBatch] : new Vector2(10,10);
            spriteBatch.DrawString(font, text, linePos - new Vector2(font.MeasureString(text).X / 2, 0), color);
            linePos.Y += font.LineSpacing + yShift;
            if (!linePosition.ContainsKey(spriteBatch))
                linePosition.Add(spriteBatch, linePos);
            else
                linePosition[spriteBatch] = linePos;
        }
    }

    public enum MouseButton
    {
        Left,
        Right
    }
}