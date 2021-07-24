using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RGB_Led_Cube_Controller
{
    public class Slider : DrawableGameComponent
    {
        private Vector2 startpos, endpos;
        private float startvalue, endvalue, thickness;
        public float currentvalue;
        public Color col_button;
        private Color col_frame;
        private bool IsSliding;

        public Slider(Vector2 startpos, Vector2 endpos, Color but_col, float startvalue, float endvalue, float currentvalue) : base(Game1.maingame)
        {
            this.startpos = startpos;
            this.endpos = endpos;
            this.startvalue = startvalue;
            this.endvalue = endvalue;
            this.currentvalue = currentvalue;
            thickness = 2;
            col_button = but_col;
            col_frame = Color.WhiteSmoke;
        }

        public Slider(Vector2 startpos, Vector2 endpos, float startvalue, float endvalue, float currentvalue) : base(Game1.maingame)
        {
            this.startpos = startpos;
            this.endpos = endpos;
            this.startvalue = startvalue;
            this.endvalue = endvalue;
            this.currentvalue = currentvalue;
            thickness = 2;
            col_button = Color.Green;
            col_frame = Color.WhiteSmoke;
        }

        private Vector2 ClosestPointtoLine(Vector2 start, Vector2 end, Vector2 p)
        {
            Vector2 dir = Vector2.Normalize(end - start);
            float length = (end - start).Length();
            return ClosestPointtoLine(start, dir, length, p);
        }
        private Vector2 ClosestPointtoLine(Vector2 start, Vector2 dir, float length, Vector2 p)
        {
            Vector2 p2end = start + dir * length - p;
            float dot = Vector2.Dot(dir, p2end);
            dot = MathHelper.Clamp(dot, 0, length);
            return start + dir * (length - dot);
        }

        public override void Update(GameTime gameTime)
        {
            if (Game1.mousestate.LeftButton == ButtonState.Pressed)
            {
                Vector2 mousepos = Game1.mousestate.Position.ToVector2();
                Vector2 buttonpos = startpos + ((currentvalue - startvalue) / (endvalue - startvalue)) * (endpos - startpos);
                if ((buttonpos - mousepos).Length() < 12 || IsSliding)
                {
                    IsSliding = true;
                    Vector2 closestpoint = ClosestPointtoLine(startpos, endpos, mousepos);
                    float dist = (endpos - startpos).Length();
                    if ((closestpoint - startpos).Length() <= dist && (closestpoint - endpos).Length() <= dist)
                    {

                    }
                    else
                    {
                        if ((closestpoint - startpos).Length() > dist)
                            closestpoint = endpos;
                        else
                            closestpoint = startpos;
                    }

                    float strength = (closestpoint - startpos).Length() / (endpos - startpos).Length();


                    currentvalue = startvalue * (1 - strength) + endvalue * strength;

                }
            }
            else
                IsSliding = false;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game1.spriteBatch.Begin();
            Game1.DrawLine(Game1.spriteBatch, startpos, endpos, col_frame);
            Vector2 buttonpos = startpos + ((currentvalue - startvalue) / (endvalue - startvalue)) * (endpos - startpos);
            float angle = (float)(Math.Atan2(endpos.Y - startpos.Y, endpos.X - startpos.X) - Math.PI / 2);
            Game1.DrawRectangle_Filled(buttonpos, new Vector2(20, 12), col_button, angle);
            Game1.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
