using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using static RGB_Led_Cube_Controller.ExtendedDatatypes;

namespace RGB_Led_Cube_Controller
{
    public class Button : DrawableGameComponent
    {
        public bool IsTexture, IsActive, IsHovered;
        private SpriteFont font;
        public Texture2D tex;
        public Color fontcolor, buttoncolor;
        private Vector2 pos;
        private Vector2 size;
        public string text;
        public event EventHandler event_pressed;

        public Button(SpriteFont font, Color fontcolor, Color buttoncolor, Vector2 pos, Vector2 size, string text) : base(Game1.maingame)
        {
            this.font = font;
            this.fontcolor = fontcolor;
            this.buttoncolor = buttoncolor;
            this.pos = pos;
            this.size = size;
            this.text = text;
        }
        public Button(SpriteFont font, Vector2 pos, Vector2 size, string text) : base(Game1.maingame)
        {
            this.font = font;
            this.fontcolor = new Color(new Vector3(0.4f));
            this.buttoncolor = Color.WhiteSmoke;
            this.pos = pos;
            this.size = size;
            this.text = text;
        }
        public Button(Texture2D tex, Vector2 pos, Vector2 size) : base(Game1.maingame)
        {
            IsTexture = true;
            this.tex = tex;
            this.buttoncolor = Color.WhiteSmoke;
            this.pos = pos;
            this.size = size;

        }

        public override void Update(GameTime gameTime)
        {
            Vector2 mousepos = Game1.mousestate.Position.ToVector2();
            if (mousepos.X >= pos.X && mousepos.X < pos.X + size.X && mousepos.Y >= pos.Y && mousepos.Y < pos.Y + size.Y)
            {
                if (Game1.mousestate.LeftButton == ButtonState.Pressed && Game1.oldmousestate.LeftButton == ButtonState.Released)
                {
                    event_pressed?.Invoke(this, EventArgs.Empty);
                }
                else
                    IsHovered = true;
            }
            else
                IsHovered = false;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game1.spriteBatch.Begin();
            Game1.DrawRectangle_Filled(pos, size, buttoncolor);
            if (!IsTexture)
            {
                string[] lines = text.Split('\n');
                Vector2 firstsize = font.MeasureString(lines[0]);
                float textheight = (firstsize.Y) * lines.Length + 2 * (lines.Length - 1);
                for (int i = 0; i < lines.Length; ++i)
                {
                    Vector2 textsize = font.MeasureString(lines[i]);
                    Game1.spriteBatch.DrawString(font, lines[i], pos + size / 2 - new Vector2(0, textheight / 2) + new Vector2(-textsize.X / 2, (firstsize.Y) * i + 2 * (i)), fontcolor);
                }
            }
            else
            {
                Game1.spriteBatch.Draw(tex, pos + new Vector2(2), Color.White);
            }

            Vector3 finaledgecolor = new Vector3(0);
            if (IsHovered)
                finaledgecolor = Color.MonoGameOrange.ToVector3() * 0.5f;
            if (IsActive)
                finaledgecolor = finaledgecolor + Color.MonoGameOrange.ToVector3() * 0.5f;
            if (!(IsActive || IsHovered))
                finaledgecolor = Color.Gray.ToVector3();
            Game1.DrawRectangle(pos, size, new Color(finaledgecolor), 2);
            Game1.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
