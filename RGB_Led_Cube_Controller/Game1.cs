using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using RGB_Led_Cube_Controller.Programms;
using FormClosingEventArgs = System.Windows.Forms.FormClosingEventArgs;
using Form = System.Windows.Forms.Form;

namespace RGB_Led_Cube_Controller
{
    public class ExtendedDatatypes
    {
        public struct int3
        {
            public int X, Y, Z;

            public int3(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }
            public static int3 operator -(int3 i1, int3 i2)
            {
                return new int3(i1.X - i2.X, i1.Y - i2.Y, i1.Z - i2.Z);
            }
            public static int3 operator +(int3 i1, int3 i2)
            {
                return new int3(i1.X + i2.X, i1.Y + i2.Y, i1.Z + i2.Z);
            }
            public static int3 operator *(int3 i1, int3 i2)
            {
                return new int3(i1.X * i2.X, i1.Y * i2.Y, i1.Z * i2.Z);
            }
            public static int3 operator /(int3 i1, int3 i2)
            {
                return new int3(i1.X / i2.X, i1.Y / i2.Y, i1.Z / i2.Z);
            }
            public static int3 operator %(int3 i1, int3 i2)
            {
                return new int3(i1.X % i2.X, i1.Y % i2.Y, i1.Z % i2.Z);
            }
            public static bool operator ==(int3 i1, int3 i2)
            {
                return (i1.X == i2.X && i1.Y == i2.Y && i1.Z == i2.Z);
            }
            public static bool operator !=(int3 i1, int3 i2)
            {
                return !(i1.X == i2.X && i1.Y == i2.Y && i1.Z == i2.Z);
            }

            public override string ToString()
            {
                return "{" + X + ", " + Y + ", " + Z + "}";
            }
        }
        public struct int2
        {
            public int X, Y;

            public int2(int x, int y)
            {
                X = x;
                Y = y;
            }
            public static int2 operator -(int2 i1, int2 i2)
            {
                return new int2(i1.X - i2.X, i1.Y - i2.Y);
            }
            public static int2 operator +(int2 i1, int2 i2)
            {
                return new int2(i1.X + i2.X, i1.Y + i2.Y);
            }
            public static int2 operator *(int2 i1, int2 i2)
            {
                return new int2(i1.X * i2.X, i1.Y * i2.Y);
            }
            public static int2 operator /(int2 i1, int2 i2)
            {
                return new int2(i1.X / i2.X, i1.Y / i2.Y);
            }
            public static int2 operator /(int2 i1, int i2)
            {
                return new int2(i1.X / i2, i1.Y / i2);
            }
            public static int2 operator %(int2 i1, int2 i2)
            {
                return new int2(i1.X % i2.X, i1.Y % i2.Y);
            }
            public static bool operator ==(int2 i1, int2 i2)
            {
                return (i1.X == i2.X && i1.Y == i2.Y);
            }
            public static bool operator !=(int2 i1, int2 i2)
            {
                return !(i1.X == i2.X && i1.Y == i2.Y);
            }

            public override string ToString()
            {
                return "{" + X + ", " + Y + "}";
            }
        }
        public struct short3
        {
            public short X, Y, Z;

            public short3(short x, short y, short z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }
    }

    public class States
    {
        public bool IsDraw, IsUpdate, DrawConnectButton, IsCamera;
        public States()
        {
            IsDraw = IsUpdate = DrawConnectButton = IsCamera = true;
        }
    }

    public class Game1 : Game
    {
        #region VertexDeclarations

        public struct VertexPositionColorNormal_noTexCoo
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Color Color;

            public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(sizeof(float) * 6, VertexElementFormat.Color, VertexElementUsage.Color, 0)
            );

            public VertexPositionColorNormal_noTexCoo(Vector3 pos, Vector3 normal, Color color)
            {
                Position = pos;
                Normal = normal;
                Color = color;
            }
        }

        public struct VertexPositionNormaltexCoo_weights
        {
            public Vector3 Position;
            public Vector3 Normal;
            private Vector2 TexCoos;
            public Vector4 TexWeights;

            public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.Color, 0)

            );

            public VertexPositionNormaltexCoo_weights(Vector3 pos, Vector3 normal, Vector2 texcoo, Vector4 Texweights)
            {
                Position = pos;
                Normal = normal;
                TexCoos = texcoo;
                TexWeights = Texweights;
            }
        }

        #endregion

        GraphicsDeviceManager graphics;
        public static Game maingame;
        public static SpriteBatch spriteBatch;
        public static GraphicsDevice gdevice;
        public static GameComponentCollection components;
        public static SpriteFont font;
        public static Random r = new Random();
        public static ContentManager contentmanager;
        private Model led_model;
        private Effect led_effect;

        private DropDown programm_dropdown;
        private Button Cube_Connect_Button;
        private Stopwatch connectiontimer;


        #region Input

        public static KeyboardState oldkeyboardstate, keyboardstate;
        public static MouseState oldmousestate, mousestate;
        private Vector2 cameramousepos, mouserotationbuffer;
        private System.Drawing.Point mousepointpos;

        #endregion

        #region Camera

        public static Vector3 camerapos = new Vector3(0, 10, 0), camerarichtung;
        public static Matrix camview, camworld, camprojection;
        private BasicEffect cameraeffect;
        private Vector3 rotation;
        private bool camerabewegen;
        private float cameraspeed = 0.1f;

        #endregion

        #region UI

        #endregion

        public static States mainstates;
        public static RGB_LED_CUBE main_cube;
        public static Vector3[,,] led_colors;
        public List<Programm_Interface> programms;
        private int currentprogramm = 0;

        public static int Screenwidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        public static int Screenheight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

        public Game1()
        {
            maingame = this;
            graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = Screenwidth,
                PreferredBackBufferHeight = Screenheight,
                IsFullScreen = false,
                SynchronizeWithVerticalRetrace = false

            };
            IsFixedTimeStep = false;
            Window.IsBorderless = true;
            IsMouseVisible = true;
            graphics.PreparingDeviceSettings += (object s, PreparingDeviceSettingsEventArgs args) =>
            {
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Form f = Form.FromHandle(Window.Handle) as Form;
            f.Location = new System.Drawing.Point(0, 0);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            contentmanager = Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gdevice = GraphicsDevice;
            components = Components;
            mainstates = new States();
            CreateThePixel(spriteBatch);
            font = Content.Load<SpriteFont>("font");
            cameraeffect = new BasicEffect(GraphicsDevice);
            cameraeffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(80), GraphicsDevice.Viewport.AspectRatio, 0.1f, 10000);
            cameraeffect.World = Matrix.Identity;
            cameraeffect.LightingEnabled = true;
            cameraeffect.EmissiveColor = new Vector3(1, 0, 0);

            led_model = Content.Load<Model>("5mmLED");
            led_effect = Content.Load<Effect>("led_effect");
            main_cube = new RGB_LED_CUBE(8, GraphicsDevice);
            programms = new List<Programm_Interface>();

            programms.Add(new Smooth_RGB("Smooth RGB"));
            programms.Add(new Rain("Rain"));
            programms.Add(new FluidSim("Fluid Sim"));
            programms.Add(new Painter("Painter"));
            programms.Add(new Animator("Animator"));
            programms.Add(new Snake("Snake"));
            programms.Add(new Testing("Testing"));
            programms[0].Activate();

            programm_dropdown = new DropDown(font, new Vector2(Screenwidth - 275, 20), new Vector2(125, 24), 200);
            programm_dropdown.event_selectionchange += SelectionChange;
            for (int i = 0; i < programms.Count; ++i)
            {
                programm_dropdown.AddItem(programms[i].name);
            }

            Cube_Connect_Button = new Button(font, new Vector2(50, Screenheight - 50 - 50), new Vector2(160, 50), "Connect");
            Cube_Connect_Button.event_pressed += ConnectButtonPressed;
            Components.Add(programm_dropdown);
            Components.Add(Cube_Connect_Button);

            //main_cube.ConnectwithCube();
        }

        protected override void UnloadContent()
        {
        }

        #region FUNKTIONS
        public static Effect line_effect;
        public static void DrawLine3d(GraphicsDevice graphicsdevice, Vector3 pos1, Vector3 pos2)
        {
            DrawLine3d(graphicsdevice, pos1, pos2, Color.White, Color.White);
        }
        public static void DrawLine3d(GraphicsDevice graphicsdevice, Vector3 pos1, Vector3 pos2, Color color1, Color color2)
        {
            var vertices = new[] { new VertexPositionColor(pos1, color1), new VertexPositionColor(pos2, color2) };
            line_effect.CurrentTechnique.Passes[0].Apply();
            graphicsdevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
        public static void DrawLine3d(GraphicsDevice graphicsdevice, Vector3 pos1, Vector3 pos2, Color color)
        {
            DrawLine3d(graphicsdevice, pos1, pos2, color, color);
        }
        public static Vector3 getDirectionAtPixel(Vector2 pixelpos, Matrix projection, Matrix view)
        {
            Vector3 nearSource = new Vector3((float)pixelpos.X, (float)pixelpos.Y, 0f);
            Vector3 farSource = new Vector3((float)pixelpos.X, (float)pixelpos.Y, 1f);
            Vector3 nearPoint = gdevice.Viewport.Unproject(nearSource, projection, view, Matrix.Identity);
            Vector3 farPoint = gdevice.Viewport.Unproject(farSource, projection, view, Matrix.Identity);
            return Vector3.Normalize(farPoint - nearPoint);
        }

        public static Texture2D pixel;
        private static void CreateThePixel(SpriteBatch spriteBatch)
        {
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }

        public static void DrawRectangle_Filled(Vector2 pos, Vector2 size, Color col)
        {
            Game1.spriteBatch.Draw(Game1.pixel, pos, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), col, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }
        public static void DrawRectangle_Filled(Vector2 pos, Vector2 size, Color col, float angle)
        {
            Game1.spriteBatch.Draw(Game1.pixel, pos, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), col, angle, size / 2, Vector2.One, SpriteEffects.None, 0);
        }
        public static void DrawRectangle(Vector2 pos, Vector2 size, Color col, int strokewidth)
        {
            size -= new Vector2(strokewidth, strokewidth);
            DrawLine(Game1.spriteBatch, pos, pos + new Vector2(size.X + strokewidth, 0), col, strokewidth);
            DrawLine(Game1.spriteBatch, pos + new Vector2(strokewidth, 0), pos + new Vector2(strokewidth, size.Y + strokewidth), col, strokewidth);
            DrawLine(Game1.spriteBatch, pos + new Vector2(size.X + strokewidth, strokewidth), pos + size + new Vector2(strokewidth, strokewidth), col, strokewidth);
            DrawLine(Game1.spriteBatch, pos + new Vector2(strokewidth, size.Y), pos + size + new Vector2(strokewidth, 0), col, strokewidth);
            //Game1.spriteBatch.Draw(Game1.pixel, pos, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), col, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }
        public static void DrawLine(SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color)
        {
            DrawLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, 1.0f);
        }
        public static void DrawLine(SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, float thickness)
        {
            DrawLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);
        }
        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color)
        {
            DrawLine(spriteBatch, point1, point2, color, 1.0f);
        }
        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness)
        {
            // calculate the distance between the two vectors
            float distance = Vector2.Distance(point1, point2);

            // calculate the angle between the two vectors
            float angle = (float)System.Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }
        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color)
        {
            DrawLine(spriteBatch, point, length, angle, color, 1.0f);
        }
        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness)
        {
            if (pixel == null)
            {
                CreateThePixel(spriteBatch);
            }
            spriteBatch.Draw(pixel, point, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        public void BeginRender3D()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            line_effect.Parameters["World"].SetValue(camworld);
            line_effect.Parameters["View"].SetValue(camview);
            line_effect.Parameters["Projection"].SetValue(camprojection);
        }

        public static void DrawMesh(BasicEffect basiceffect, Model model, Effect shader)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    shader.Parameters["World"].SetValue(basiceffect.World * mesh.ParentBone.Transform);
                    shader.Parameters["View"].SetValue(basiceffect.View);
                    shader.Parameters["Projection"].SetValue(basiceffect.Projection);
                    shader.Parameters["EyePosition"].SetValue(camerapos);
                    shader.Parameters["LightDirection"].SetValue(new Vector3(-1, 0, 0));
                    part.Effect = shader;
                }

                mesh.Draw();

            }
        }

        public static void DrawMesh(BasicEffect basiceffect, Model model, Matrix matrix)
        {
            Matrix[] transformations = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transformations);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.EmissiveColor = basiceffect.EmissiveColor;
                    effect.LightingEnabled = basiceffect.LightingEnabled;
                    if (effect.LightingEnabled == true)
                    {
                        effect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
                        effect.DirectionalLight0.Enabled = true;
                        effect.DirectionalLight0.Direction = new Vector3(1, -1, 0);
                        effect.DirectionalLight0.DiffuseColor = Color.LightGoldenrodYellow.ToVector3() * 0.2f;
                        effect.DirectionalLight0.SpecularColor = Color.LightGoldenrodYellow.ToVector3() * 1;
                    }

                    effect.World = transformations[mesh.ParentBone.Index] * matrix;
                    effect.View = basiceffect.View;
                    effect.Projection = basiceffect.Projection;
                }

                mesh.Draw();
            }
        }

        public static void MeshPos(ref Model model, Matrix original, int ID, Vector3 pos)
        {
            model.Bones[ID].Transform = original;
            Vector3 oldpos = model.Bones[ID].Transform.Translation;
            model.Bones[ID].Transform *= Matrix.CreateTranslation(pos - oldpos);
        }

        public static void MeshMatrix(ref Model model, Matrix original, int ID, Matrix matrix)
        {
            model.Bones[ID].Transform = original;
            Vector3 oldpos = model.Bones[ID].Transform.Translation;
            model.Bones[ID].Transform *= Matrix.CreateTranslation(Vector3.Zero - oldpos) * matrix;
        }

        public static void MeshMatrix(ref Model model, Matrix original, int ID, Matrix matrix, Vector3 pos)
        {
            model.Bones[ID].Transform = original;
            Vector3 oldpos = model.Bones[ID].Transform.Translation;
            model.Bones[ID].Transform *= matrix * Matrix.CreateTranslation(pos - oldpos);
        }

        #endregion
        public void SelectionChange(int id)
        {
            programms[currentprogramm].Deactivate();
            currentprogramm = id;
            programms[id].Activate();
        }

        public void ConnectButtonPressed(object sender, EventArgs e)
        {
            if (Cube_Connect_Button.text == "Connect" || Cube_Connect_Button.text[0] == 'F')
            {
                bool state = main_cube.ConnectwithCube();
                if (!state)
                {
                    Cube_Connect_Button.text = "Failed to Connect\nPress to Retry";
                    if (connectiontimer == null)
                        connectiontimer = new Stopwatch();
                    connectiontimer.Restart();
                }
                else
                {
                    Cube_Connect_Button.text = "Disconnect";
                }
            }
            else if (Cube_Connect_Button.text == "Disconnect")
            {
                Cube_Connect_Button.text = "Connect";
                main_cube.DisconnectwithCube();
            }
        }

        private int count = 0;
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keyboardstate = Keyboard.GetState();
            mousestate = Mouse.GetState();
            var mousePosition = new Vector2(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);

            Cube_Connect_Button.Enabled = Cube_Connect_Button.Visible = mainstates.DrawConnectButton;

            programms[currentprogramm].Update(gameTime);

            if (mainstates.IsUpdate)
            {
                #region Updating Camera

                if (mainstates.IsCamera)
                {
                    if (keyboardstate.IsKeyUp(Keys.LeftAlt))
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.A))
                        {
                            camerapos.Z -= (float)Math.Sin(rotation.X) * 2 * cameraspeed;
                            camerapos.X += (float)Math.Cos(rotation.X) * 2 * cameraspeed;
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.D))
                        {
                            camerapos.Z += (float)Math.Sin(rotation.X) * 2 * cameraspeed;
                            camerapos.X -= (float)Math.Cos(rotation.X) * 2 * cameraspeed;
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        {
                            camerapos.Y += 2 * cameraspeed;
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                        {
                            camerapos.Y -= 2 * cameraspeed;
                        }
                    }

                    cameraspeed = Keyboard.GetState().IsKeyDown(Keys.LeftShift) ? 1.5f : 0.3f;

                    if (camerabewegen == true && this.IsActive)
                    {
                        int changed = 0;
                        float deltax, deltay;
                        deltax = System.Windows.Forms.Cursor.Position.X - cameramousepos.X;
                        deltay = System.Windows.Forms.Cursor.Position.Y - cameramousepos.Y;
                        mouserotationbuffer.X += 0.004f * deltax;
                        mouserotationbuffer.Y += 0.004f * deltay;
                        if (mouserotationbuffer.Y < MathHelper.ToRadians(-88))
                        {
                            mouserotationbuffer.Y = mouserotationbuffer.Y - (mouserotationbuffer.Y - MathHelper.ToRadians(-88));
                        }

                        if (mouserotationbuffer.Y > MathHelper.ToRadians(88))
                        {
                            mouserotationbuffer.Y = mouserotationbuffer.Y - (mouserotationbuffer.Y - MathHelper.ToRadians(88));
                        }

                        if (cameramousepos != mousePosition)
                            changed = 1;
                        rotation = new Vector3(-mouserotationbuffer.X, -mouserotationbuffer.Y, 0);
                        if (changed == 1)
                        {
                            System.Windows.Forms.Cursor.Position = mousepointpos;
                        }
                    }

                    if (Mouse.GetState().RightButton == ButtonState.Pressed && IsActive)
                    {
                        if (camerabewegen == false)
                        {
                            camerabewegen = true;
                            cameramousepos = mousePosition;
                            mousepointpos.X = (int)mousePosition.X;
                            mousepointpos.Y = (int)mousePosition.Y;
                        }
                    }

                    if (Mouse.GetState().RightButton == ButtonState.Released && camerabewegen == true)
                    {
                        camerabewegen = false;
                    }


                    Matrix rotationMatrix = Matrix.CreateRotationY(rotation.X); // * Matrix.CreateRotationX(rotationY);
                    Vector3 transformedReference = Vector3.TransformNormal(new Vector3(0, 0, 1000), rotationMatrix);
                    Vector3 cameraLookat = camerapos + transformedReference;
                    camerarichtung.Y = cameraLookat.Y - (float)Math.Sin(-rotation.Y) * Vector3.Distance(camerapos, cameraLookat);
                    camerarichtung.X = cameraLookat.X - (cameraLookat.X - camerapos.X) * (float)(1 - Math.Cos(rotation.Y));
                    camerarichtung.Z = cameraLookat.Z - (cameraLookat.Z - camerapos.Z) * (float)(1 - Math.Cos(rotation.Y));
                    if (keyboardstate.IsKeyUp(Keys.LeftAlt))
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.W))
                        {
                            var camerablickrichtung = camerapos - camerarichtung;
                            camerablickrichtung = camerablickrichtung / camerablickrichtung.Length();
                            camerapos -= camerablickrichtung * 2 * cameraspeed;
                            camerarichtung -= camerablickrichtung * 2 * cameraspeed;
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.S))
                        {
                            var camerablickrichtung = camerapos - camerarichtung;
                            camerablickrichtung = camerablickrichtung / camerablickrichtung.Length();
                            camerapos += camerablickrichtung * 2 * cameraspeed;
                            camerarichtung += camerablickrichtung * 2 * cameraspeed;
                        }
                    }

                    cameraeffect.View = Matrix.CreateLookAt(camerapos, camerarichtung, Vector3.Up);
                    camworld = cameraeffect.World;
                    camview = cameraeffect.View;
                    camprojection = cameraeffect.Projection;
                }

                #endregion

                if (connectiontimer?.ElapsedMilliseconds > 3000)
                {
                    Cube_Connect_Button.text = "Connect";
                    connectiontimer.Reset();
                }

                /*count++;
		        if (count > 3)
		        {
		            count = 0;
		            
		        }
				if(count == 0)
					main_cube.SetAll2Col(new Vector3(1.0f, 0, 0));
			    if (count == 1)
				    main_cube.SetAll2Col(new Vector3(0, 1.0f, 0));
			    if (count == 2)
				    main_cube.SetAll2Col(new Vector3(0, 0, 1.0f));
				*/


            }
            if (main_cube.IsWaiting)
            {
                lock (main_cube.color_data)
                {
                    Array.Copy(main_cube.color_data, main_cube.send_data, main_cube.color_data.Length);
                }

                main_cube.IsUpdated = true;
            }

            //main_cube.SendFrame();
            base.Update(gameTime);
            oldkeyboardstate = keyboardstate;
            oldmousestate = mousestate;
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            BeginRender3D();
            GraphicsDevice.Clear(new Color(new Vector3(0.05f, 0.05f, 0.05f)));
            if (mainstates.IsDraw)
            {
                main_cube.Draw(camview, camprojection);


                spriteBatch.Begin();
                //spriteBatch.DrawString(font, camerapos.ToString(), new Vector2(100, 100), Color.Red);
                /*spriteBatch.DrawString(font, main_cube.color_data[0, 0, 0].X.ToString(), new Vector2(100, 100), Color.Red);
		        spriteBatch.DrawString(font, main_cube.color_data[0, 0, 0].Y.ToString(), new Vector2(100, 130), Color.Red);
		        spriteBatch.DrawString(font, main_cube.color_data[0, 0, 0].Z.ToString(), new Vector2(100, 160), Color.Red);*/

                spriteBatch.End();
            }
            programms[currentprogramm].Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
