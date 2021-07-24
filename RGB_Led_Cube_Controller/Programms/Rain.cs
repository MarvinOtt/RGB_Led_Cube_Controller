using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static RGB_Led_Cube_Controller.ExtendedDatatypes;

namespace RGB_Led_Cube_Controller.Programms
{
    public class Rain : Programm_Interface
    {
        public class Droplet
        {
            public int3 pos;
            public int timer;
            public float health;

            public Droplet(int3 pos)
            {
                this.pos = pos;
                health = 1.0f;
                timer = 0;
            }
        }

        public override string name { get; set; }
        public override bool IsActiveted { get; set; }
        private Random r;
        private List<Droplet> drops;

        public Rain(string name)
        {
            this.name = name;
            drops = new List<Droplet>();
            r = new Random();
        }

        public override void Activate()
        {
            IsActiveted = true;
        }

        public override void Deactivate()
        {
            IsActiveted = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsActiveted)
            {
                Game1.main_cube.SetAll2Col(Vector3.Zero);
                for (int i = 0; i < drops.Count; ++i)
                {
                    drops[i].timer++;
                    if (drops[i].pos.Y > 0 && drops[i].timer % 8 == 0)
                        drops[i].pos.Y--;
                    else if (drops[i].pos.Y == 0)
                    {
                        drops[i].health -= 0.002f;
                        if (drops[i].health <= 0)
                            drops.RemoveAt(i);
                    }

                    //Game1.main_cube.color_data[drops[i].pos.X, drops[i].pos.Y, drops[i].pos.Z] += Color.Blue.ToVector3() * drops[i].health;
                }

                int spawndrop = r.Next(0, 6);
                if (spawndrop == 0)
                {
                    for (int i = 0; i < 1; ++i)
                    {
                        drops.Add(new Droplet(new int3(r.Next(0, Game1.main_cube.size), Game1.main_cube.size - 1, r.Next(0, Game1.main_cube.size))));
                    }
                }

                for (int i = 0; i < drops.Count; ++i)
                {
                    Vector3 val = Game1.main_cube.color_data[drops[i].pos.X, drops[i].pos.Y, drops[i].pos.Z] + Color.Blue.ToVector3() * drops[i].health;
                    Game1.main_cube.color_data[drops[i].pos.X, drops[i].pos.Y, drops[i].pos.Z] = new Vector3(MathHelper.Clamp(val.X, 0, 1), MathHelper.Clamp(val.Y, 0, 1), MathHelper.Clamp(val.Z, 0, 1));
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsActiveted)
            {

            }
        }
    }
}
