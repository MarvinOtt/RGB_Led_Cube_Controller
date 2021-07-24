using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static RGB_Led_Cube_Controller.ExtendedDatatypes;

namespace RGB_Led_Cube_Controller.Programms
{
    public class Snake : Programm_Interface
    {
        public override bool IsActiveted { get; set; }
        public override string name { get; set; }

        private List<int3> pos;
        private int dir;
        private float speed;
        private Stopwatch watch;
        private Vector3 snakecol;
        private int3 foodpos;
        private Random r;

        public Snake(string name)
        {
            this.name = name;
            pos = new List<int3>();
            pos.Add(new int3(4, 4, 4));
            dir = 0;
            speed = 675.0f;
            watch = new Stopwatch();
            snakecol = Color.Green.ToVector3();
            r = new Random();
            foodpos = new int3(r.Next(0, 8), r.Next(0, 8), r.Next(0, 8));
        }

        public override void Activate()
        {
            watch.Restart();
            IsActiveted = true;
            Game1.mainstates.IsCamera = false;
        }

        public override void Deactivate()
        {
            watch.Reset();
            IsActiveted = false;
            Game1.mainstates.IsCamera = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsActiveted)
            {

                if (Game1.keyboardstate.IsKeyDown(Keys.W) && dir != 3)
                    dir = 1;
                if (Game1.keyboardstate.IsKeyDown(Keys.A) && dir != 2)
                    dir = 0;
                if (Game1.keyboardstate.IsKeyDown(Keys.S) && dir != 1)
                    dir = 3;
                if (Game1.keyboardstate.IsKeyDown(Keys.D) && dir != 0)
                    dir = 2;
                if (Game1.keyboardstate.IsKeyDown(Keys.Space) && dir != 5)
                    dir = 4;
                if (Game1.keyboardstate.IsKeyDown(Keys.LeftControl) && dir != 4)
                    dir = 5;
                if (((watch.ElapsedTicks * 1000.0) / (double)Stopwatch.Frequency) >= speed)
                {
                    watch.Restart();
                    int3 offset = new int3(0, 0, 0);
                    int3 frontpos = pos[0];
                    if (dir == 0)
                        offset = new int3(1, 0, 0);
                    if (dir == 1)
                        offset = new int3(0, 0, 1);
                    if (dir == 2)
                        offset = new int3(-1, 0, 0);
                    if (dir == 3)
                        offset = new int3(0, 0, -1);
                    if (dir == 4)
                        offset = new int3(0, 1, 0);
                    if (dir == 5)
                        offset = new int3(0, -1, 0);
                    int3 newfrontpos = offset + frontpos;
                    if (newfrontpos.X >= 0 && newfrontpos.X < 8 && newfrontpos.Y >= 0 && newfrontpos.Y < 8 && newfrontpos.Z >= 0 && newfrontpos.Z < 8)
                    {
                        pos.Insert(0, newfrontpos);
                        bool IsEaten = false;
                        for (int i = 0; i < pos.Count - 1; ++i)
                        {
                            if (pos[i] == foodpos)
                            {
                                foodpos = new int3(r.Next(0, 8), r.Next(0, 8), r.Next(0, 8));
                                IsEaten = true;
                            }
                        }
                        if (!IsEaten)
                        {
                            pos.RemoveAt(pos.Count - 1);
                        }
                    }
                    else
                    {
                        pos.Clear();
                        pos.Add(new int3(4, 4, 4));
                    }

                    for (int i = 0; i < pos.Count; ++i)
                    {
                        for (int j = 0; j < pos.Count; ++j)
                        {
                            if (i != j)
                            {
                                if (pos[i] == pos[j])
                                {
                                    pos.Clear();
                                    pos.Add(new int3(4, 4, 4));
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsActiveted)
            {
                Game1.main_cube.SetAll2Col(Vector3.Zero);
                for (int i = 0; i < pos.Count; ++i)
                {
                    Game1.main_cube.color_data[pos[i].X, pos[i].Y, pos[i].Z] = snakecol;
                }
                Game1.main_cube.color_data[foodpos.X, foodpos.Y, foodpos.Z] = Color.Red.ToVector3();
                Game1.spriteBatch.Begin();
                Game1.spriteBatch.DrawString(Game1.font, "Current Length: " + pos.Count, new Vector2(100, 100), Color.Red);
                Game1.spriteBatch.End();

            }
        }
    }
}
