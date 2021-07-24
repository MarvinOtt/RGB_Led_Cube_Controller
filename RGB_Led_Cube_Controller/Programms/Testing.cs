using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace RGB_Led_Cube_Controller.Programms
{
    class Testing : Programm_Interface
    {
        public override string name { get; set; }
        public override bool IsActiveted { get; set; }

        public Testing(string name)
        {
            this.name = name;
            IsActiveted = false;
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
                Game1.main_cube.SetAll2Col(new Vector3(1.0f, 1.0f, 1.0f));
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
