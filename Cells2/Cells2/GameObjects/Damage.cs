using System;
using Cells.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Cells.Geometry.Rectangle;

namespace Cells.GameObjects
{
    public class FloatingNumber : GameObject
    {
        public override Rectangle Bounds => new Rectangle(Position - Size * 0.5f, Size);
        private Vector2 Size;

        private float _drawPriority = 20000;
        public override float DrawPriority => _drawPriority;

        private string _value;
        private float _maxAge = 100;
        public float AgePercent = 0.1f;
        public Color Color { get; set; }

        private Texture2D Texture = null;

        public static FloatingNumber ArmorRegen(Vector2 position, float armor, float maxAge = 10)
        {
            return new FloatingNumber(position, armor, Color.Blue, maxAge);
        }
        public static FloatingNumber Damage(Vector2 position, float damage, float maxAge = 10)
        {
            return new FloatingNumber(position, damage, Color.Red, maxAge);
        }
        public FloatingNumber(Vector2 position, float value, Color color, float maxAge = 10)
        {
            Color = color;
            Position = position;
            _value = $"{value:0.00}";
            _maxAge = maxAge;
            Velocity = new Vector2(0f, -50f);
            Friction = 0.2f;
            var textureSize = Game1.Arial.MeasureString(_value);
            var rt = new RenderTarget2D(Game1.PublicGraphicsDevice, (int)textureSize.X + 10, (int)textureSize.Y + 10);
            var device = Game1.PublicGraphicsDevice;
            device.SetRenderTarget(rt);
            device.Clear(Color.Transparent);
            var sb = new SpriteBatch(device);
            sb.Begin();
            sb.DrawString(Game1.Arial, _value, Vector2.One * 5f, Color.White);
            sb.End();
            var textureData = new Color[rt.Width * rt.Height];
            rt.GetData(textureData);
            Texture = new Texture2D(device, rt.Width, rt.Height);
            Texture.SetData(textureData);
            device.SetRenderTarget(null);
            sb.Dispose();
            rt.Dispose();
            Size = new Vector2(Texture.Width * 0.1f, Texture.Height * 0.1f);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            AgePercent = Math.Clamp(Age / _maxAge, 0.1f, 1f);
            Size = new Vector2(Texture.Width * (AgePercent + 0.3f) * 2, Texture.Height * (AgePercent + 0.3f) * 2);

            if (Age > _maxAge)
                Die(true);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.View.Contains(Bounds) || Game1.View.Intersects(Bounds)) {
                spriteBatch.Draw(Texture, Bounds.Translate(Game1.View, Game1.ViewZoom).ToRectangle(), Color.Lerp(Color, Color.Transparent, Math.Clamp(AgePercent + 0.2f, 0f,1f)));
            }
        }
        public override void HandleCollision(GameObject other, float deltaTime)
        {
            // No collision handling
        }
    }
}