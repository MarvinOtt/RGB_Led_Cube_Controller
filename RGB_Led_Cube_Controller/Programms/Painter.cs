using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RGB_Led_Cube_Controller.Programms
{
    public class Painter : Programm_Interface
    {
        public override string name { get; set; }
        public override bool IsActiveted { get; set; }
        private Slider R_slider, G_slider, B_slider;
        private Vector3 curcol;
        private Texture2D tex_fill, tex_pencil, tex_pipette;
        private Button but_fill, but_pencil, but_pipette;
        private Button[] buttons;

        public Painter(string name)
        {
            this.name = name;
            IsActiveted = false;
            R_slider = new Slider(new Vector2(Game1.Screenwidth - 110, Game1.Screenheight - 40), new Vector2(Game1.Screenwidth - 110, Game1.Screenheight - 140), Color.Red, 0.0f, 1.0f, 0);
            G_slider = new Slider(new Vector2(Game1.Screenwidth - 80, Game1.Screenheight - 40), new Vector2(Game1.Screenwidth - 80, Game1.Screenheight - 140), new Color(new Vector3(0, 1.0f, 0)), 0.0f, 1.0f, 0);
            B_slider = new Slider(new Vector2(Game1.Screenwidth - 50, Game1.Screenheight - 40), new Vector2(Game1.Screenwidth - 50, Game1.Screenheight - 140), Color.Blue, 0.0f, 1.0f, 0);
            R_slider.Enabled = G_slider.Enabled = B_slider.Enabled = false;
            R_slider.Visible = G_slider.Visible = B_slider.Visible = false;
            Game1.components.Add(R_slider);
            Game1.components.Add(G_slider);
            Game1.components.Add(B_slider);
            buttons = new Button[3];
            tex_fill = Game1.contentmanager.Load<Texture2D>("painter_fill");
            but_fill = new Button(tex_fill, new Vector2(Game1.Screenwidth - 160, Game1.Screenheight - 60), new Vector2(24));
            buttons[0] = but_fill;
            tex_pencil = Game1.contentmanager.Load<Texture2D>("painter_pencil");
            but_pencil = new Button(tex_pencil, new Vector2(Game1.Screenwidth - 160, Game1.Screenheight - 140), new Vector2(24));
            buttons[1] = but_pencil;
            tex_pipette = Game1.contentmanager.Load<Texture2D>("painter_pipette");
            but_pipette = new Button(tex_pipette, new Vector2(Game1.Screenwidth - 160, Game1.Screenheight - 100), new Vector2(24));
            buttons[2] = but_pipette;
            but_pipette.IsActive = true;
            for (int i = 0; i < buttons.Length; ++i)
            { buttons[i].Enabled = buttons[i].Visible = false; Game1.components.Add(buttons[i]); buttons[i].event_pressed += but_pressed; }
        }

        public override void Activate()
        {
            IsActiveted = true;
            R_slider.Enabled = G_slider.Enabled = B_slider.Enabled = true;
            R_slider.Visible = G_slider.Visible = B_slider.Visible = true;
            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].Enabled = buttons[i].Visible = true;
            }
            //Game1.mainstates.DrawConnectButton = false;
        }

        public override void Deactivate()
        {
            IsActiveted = false;
            R_slider.Enabled = G_slider.Enabled = B_slider.Enabled = false;
            R_slider.Visible = G_slider.Visible = B_slider.Visible = false;
            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].Enabled = buttons[i].Visible = false;
            }
            //Game1.mainstates.DrawConnectButton = true;
        }

        private void but_pressed(object o, EventArgs e)
        {
            Button curbutton = o as Button;
            if (curbutton.IsActive != true)
            {
                for (int i = 0; i < buttons.Length; ++i)
                { buttons[i].IsActive = false; }
                curbutton.IsActive = true;
            }
        }

        private bool RaySpereCol(Vector3 pos, Vector3 dir, Vector3 sp, float sr, out float dist)
        {
            float det, b;
            Vector3 p = pos - sp;

            b = -Vector3.Dot(p, dir);

            det = b * b - Vector3.Dot(p, p) + sr * sr;

            if (det < 0)
            {
                dist = 0;
                return false;
            }

            det = (float)Math.Sqrt(det);

            dist = b - det;

            // intersecting with ray?

            if (b + det < 0) return false;

            if (dist < 0) dist = 0;

            return true;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsActiveted)
            {
                curcol = new Vector3(R_slider.currentvalue, G_slider.currentvalue, B_slider.currentvalue);
                Vector2 mousepos = Game1.mousestate.Position.ToVector2();
                if (but_pencil.IsActive || but_pipette.IsActive)
                {
                    if (!(mousepos.X > Game1.Screenwidth - 150 && mousepos.Y > Game1.Screenheight - 170))
                    {
                        if (Game1.mousestate.LeftButton == ButtonState.Pressed)
                        {
                            SortedSet<float> distances = null;
                            Vector3 dir = Game1.getDirectionAtPixel(mousepos, Game1.camprojection, Game1.camview);
                            float dist;
                            for (int x = 0; x < Game1.main_cube.size; ++x)
                            {
                                for (int y = 0; y < Game1.main_cube.size; ++y)
                                {
                                    for (int z = 0; z < Game1.main_cube.size; ++z)
                                    {
                                        bool state = RaySpereCol(Game1.camerapos, dir, new Vector3(x, y, z) * 30 + new Vector3(0, 4.5f, 0), 6, out dist);
                                        if (state)
                                        {
                                            if (distances == null)
                                                distances = new SortedSet<float>();
                                            distances.Add(dist);
                                        }
                                    }
                                }
                            }

                            if (distances != null && distances.Count > 0)
                            {
                                if (Game1.keyboardstate.IsKeyDown(Keys.LeftAlt) && but_pencil.IsActive)
                                {
                                    float[] dists = distances.ToArray();
                                    for (int i = 0; i < dists.Length; ++i)
                                    {
                                        Vector3 pos = (Game1.camerapos + dir * dists[i]) / 30.0f;
                                        ExtendedDatatypes.int3 ledindex = new ExtendedDatatypes.int3((int)Math.Round(pos.X), (int)Math.Round(pos.Y), (int)Math.Round(pos.Z));
                                        Game1.main_cube.color_data[ledindex.X, ledindex.Y, ledindex.Z] = curcol;
                                    }
                                }
                                else if (but_pencil.IsActive)
                                {
                                    Vector3 pos = (Game1.camerapos + dir * distances.Min) / 30.0f;
                                    ExtendedDatatypes.int3 ledindex = new ExtendedDatatypes.int3((int)Math.Round(pos.X), (int)Math.Round(pos.Y), (int)Math.Round(pos.Z));
                                    Game1.main_cube.color_data[ledindex.X, ledindex.Y, ledindex.Z] = curcol;
                                }
                                else
                                {
                                    Vector3 pos = (Game1.camerapos + dir * distances.Min) / 30.0f;
                                    ExtendedDatatypes.int3 ledindex = new ExtendedDatatypes.int3((int)Math.Round(pos.X), (int)Math.Round(pos.Y), (int)Math.Round(pos.Z));
                                    curcol = Game1.main_cube.color_data[ledindex.X, ledindex.Y, ledindex.Z];
                                    R_slider.currentvalue = curcol.X;
                                    G_slider.currentvalue = curcol.Y;
                                    B_slider.currentvalue = curcol.Z;
                                }

                                distances.Clear();
                                distances = null;
                            }
                        }
                    }
                }
                else if (but_fill.IsActive)
                {
                    Game1.main_cube.SetAll2Col(curcol);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsActiveted)
            {
                Game1.spriteBatch.Begin();
                Game1.DrawRectangle_Filled(new Vector2(Game1.Screenwidth - 120, Game1.Screenheight - 180), new Vector2(80, 30), new Color(curcol));
                Game1.spriteBatch.End();
            }
        }
    }
}
