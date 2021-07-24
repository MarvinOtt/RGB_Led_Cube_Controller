using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace RGB_Led_Cube_Controller.Programms
{
    public abstract class Programm_Interface
    {
        public abstract string name { get; set; }
        public abstract bool IsActiveted { get; set; }

        public abstract void Activate();
        public abstract void Deactivate();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}
