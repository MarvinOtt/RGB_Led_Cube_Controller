using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RGB_Led_Cube_Controller.Programms
{
    public class Smooth_RGB : Programm_Interface
    {
        public override string name { get; set; }
        public override bool IsActiveted { get; set; }

        float speed = 1.5f;
        float rstrength = 0.0f, gstrength = 1.0f, bstrength = 0.0f;
        int state = 0, counter = 0;
        private Slider speedslider;

        public Smooth_RGB(string name)
        {
            this.name = name;
            IsActiveted = false;
            speedslider = new Slider(new Vector2(Game1.Screenwidth - 80, Game1.Screenheight - 40), new Vector2(Game1.Screenwidth - 80, Game1.Screenheight - 140), 0.2f, 5.0f, 1.5f);
            speedslider.Enabled = false;
            speedslider.Visible = false;
            Game1.components.Add(speedslider);

        }

        public override void Activate()
        {
            IsActiveted = true;
            speedslider.Enabled = true;
            speedslider.Visible = true;
        }

        public override void Deactivate()
        {
            IsActiveted = false;
            speedslider.Enabled = false;
            speedslider.Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsActiveted)
            {
                speed = speedslider.currentvalue;
                if (state == 0)
                {
                    gstrength -= 0.01f * speed;
                    rstrength += 0.01f * speed;
                    if (gstrength <= 0)
                    {
                        gstrength = 0;
                        rstrength = 1.0f;
                        state = 1;
                    }
                }
                else if (state == 1)
                {
                    rstrength -= 0.01f * speed;
                    bstrength += 0.01f * speed;
                    if (rstrength <= 0)
                    {
                        rstrength = 0;
                        bstrength = 1.0f;
                        state = 2;
                    }
                }
                else if (state == 2)
                {
                    bstrength -= 0.01f * speed;
                    gstrength += 0.01f * speed;
                    if (bstrength <= 0)
                    {
                        bstrength = 0;
                        gstrength = 1.0f;
                        state = 0;
                    }
                }
                Game1.main_cube.SetAll2Col(new Vector3(rstrength, gstrength, bstrength));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsActiveted)
            {
                Game1.spriteBatch.Begin();

                Game1.spriteBatch.DrawString(Game1.font, "Speed: ", new Vector2(Game1.Screenwidth - 110, Game1.Screenheight - 195), Color.WhiteSmoke);
                Vector2 size = Game1.font.MeasureString(speedslider.currentvalue.ToString());
                Game1.spriteBatch.DrawString(Game1.font, speedslider.currentvalue.ToString(), new Vector2(Game1.Screenwidth - 80 - size.X / 2, Game1.Screenheight - 175), Color.WhiteSmoke);
                Game1.spriteBatch.End();
            }
        }

    }
}
