using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RGB_Led_Cube_Controller
{
    public class IntBox : DrawableGameComponent
    {
        public int minvalue, maxvalue;
        private Vector2 pos, size;
        private bool IsActive;
        private string currenttext, newtext;
        public int currentvalue;
        private Keys[] pressed_old, pressed_new;

        public IntBox(int minvalue, int maxvalue, Vector2 pos, Vector2 size) : base(Game1.maingame)
        {
            this.pos = pos;
            this.size = size;
            this.minvalue = minvalue;
            this.maxvalue = maxvalue;
            currenttext = newtext = "0";
        }

        public void UpdateValue(int value)
        {
            currentvalue = MathHelper.Clamp(value, minvalue, maxvalue);
            currenttext = currentvalue.ToString();
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 mousepos = Game1.mousestate.Position.ToVector2();
            if (IsActive)
            {
                pressed_new = Game1.keyboardstate.GetPressedKeys();
                for (int i = 0; i < pressed_new.Length; ++i)
                {
                    bool IsInput = true;
                    if ((int)pressed_new[i] >= 48 && (int)pressed_new[i] <= 57)
                    {
                        for (int j = 0; j < pressed_old.Length; ++j)
                        {
                            if ((int)pressed_new[i] == (int)pressed_old[j])
                                IsInput = false;
                        }
                    }
                    else
                        IsInput = false;

                    if (IsInput)
                        newtext += (char)pressed_new[i];

                }

                if (newtext.Length > 0 && Game1.keyboardstate.IsKeyDown(Keys.Back) && Game1.oldkeyboardstate.IsKeyUp(Keys.Back))
                    newtext = newtext.Remove(newtext.Length - 1);
                else if (newtext.Length > 0 && Game1.keyboardstate.IsKeyDown(Keys.Enter) && Game1.oldkeyboardstate.IsKeyUp(Keys.Enter))
                {
                    IsActive = false;
                    currenttext = newtext;
                    currentvalue = MathHelper.Clamp(int.Parse(currenttext), minvalue, maxvalue);
                    currenttext = newtext = currentvalue.ToString();
                }
                else if (Game1.keyboardstate.IsKeyDown(Keys.Escape) && Game1.oldkeyboardstate.IsKeyUp(Keys.Escape))
                    IsActive = false;

                pressed_old = pressed_new;
            }
            else
            {
                if (mousepos.X >= pos.X && mousepos.X <= pos.X + size.X && mousepos.Y >= pos.Y && mousepos.Y <= pos.Y + size.Y && Game1.mousestate.LeftButton == ButtonState.Pressed && Game1.oldmousestate.LeftButton == ButtonState.Released)
                {
                    IsActive = true;
                    newtext = "";
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 mousepos = Game1.mousestate.Position.ToVector2();
            Game1.spriteBatch.Begin();

            Game1.DrawRectangle_Filled(pos, size, Color.WhiteSmoke);
            if (IsActive)
                Game1.spriteBatch.DrawString(Game1.font, newtext, pos, new Color(new Vector3(0.4f)));
            else
                Game1.spriteBatch.DrawString(Game1.font, currenttext, pos, new Color(new Vector3(0.4f)));

            Vector3 finaledgecolor = new Vector3(0);
            bool IsHovered = mousepos.X >= pos.X && mousepos.X <= pos.X + size.X && mousepos.Y >= pos.Y && mousepos.Y <= pos.Y + size.Y;
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
