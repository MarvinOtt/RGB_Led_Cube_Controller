using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RGB_Led_Cube_Controller.Programms
{
    public class Animation
    {
        public string name;
        public List<Vector3[,,]> frames;
        public float framerate;
        public bool IsRepeat, IsPlaying;
        private int currentframe, oldframe;

        Stopwatch timer = new Stopwatch();

        public Animation(string name, float framerate, bool IsRepeat)
        {
            this.name = name;
            this.framerate = framerate;
            this.IsRepeat = IsRepeat;
            frames = new List<Vector3[,,]>();
            currentframe = oldframe = -1;
        }

        public void StartPlaying()
        {
            oldframe = -1;
            IsPlaying = true;
            if (!timer.IsRunning)
                timer.Start();
        }
        public void StopPlaying()
        {
            IsPlaying = false;
            if (timer.IsRunning)
                timer.Stop();
        }
        public void Reset()
        {
            currentframe = oldframe = -1;
            if (timer.IsRunning)
                timer.Restart();
            else
                timer.Reset();
        }

        public void Update()
        {
            if (IsPlaying)
            {
                double elapsedseconds = (timer.ElapsedTicks) / (double)Stopwatch.Frequency;
                currentframe = (int)(framerate * elapsedseconds);
                if (currentframe != oldframe || currentframe == 0)
                {
                    if (currentframe < frames.Count)
                    {
                        Array.Copy(frames[currentframe], Game1.main_cube.color_data, frames[currentframe].Length);
                    }
                    else
                    {
                        if (IsRepeat)
                            Reset();
                        else
                            StopPlaying();
                    }
                }

                oldframe = currentframe;
            }
        }
    }
    public class Animator : Programm_Interface
    {
        public override string name { get; set; }
        public override bool IsActiveted { get; set; }
        private Effect animation_effect;
        private RenderTarget2D layeredittarget;
        private Texture2D tex_currentedit, useless, tex_play, tex_break;
        private Texture2D tex_open, tex_save, tex_edit, tex_minus, tex_plus;
        private Button but_open, but_save, but_edit, but_frame_minus, but_frame_plus, but_layer_minus, but_layer_plus, but_IsEdit;
        private IntBox framebox, layerbox;
        private FloatBox frameratebox;

        private Slider R_slider, G_slider, B_slider;
        private Vector3 curcol;
        private Texture2D tex_fill, tex_pencil, tex_pipette, tex_copie, tex_paste, tex_frame_add, tex_frame_delete;
        private Button but_fill, but_pencil, but_pipette, but_play, but_frame_copie, but_layer_copie, but_frame_paste, but_layer_paste, but_frame_add, but_frame_delete;
        private Button[] buttons;
        private Vector2 editorpos;

        //private DropDown animationdropper;
        private Animation currentanimation;
        private int IsSavedcounter;
        private bool IsEditing, IsEditOpen;
        private int edit_currentlayer, edit_currentframe;

        // Copying, Pasting, Adding, Deleting
        private Vector3[,,] buffer_frame;
        private Vector3[,] buffer_layer;

        public bool _IsPlaying;
        public bool IsPlaying
        {
            get { return _IsPlaying; }
            set
            {
                currentanimation.StopPlaying();
                currentanimation.Reset();
                currentanimation.IsPlaying = _IsPlaying = value;
                if (value)
                    currentanimation.StartPlaying();
                if (value)
                    but_play.tex = tex_break;
                else
                    but_play.tex = tex_play;
            }
        }

        public Animator(string name)
        {
            this.name = name;
            //animationdropper = new DropDown(Game1.font, new Vector2(Game1.Screenwidth - 250, 400), new Vector2(125, 24), 200);
            //animationdropper.Enabled = animationdropper.Visible = false;
            //Game1.components.Add(animationdropper);
            tex_copie = Game1.contentmanager.Load<Texture2D>("animator_copie");
            tex_paste = Game1.contentmanager.Load<Texture2D>("animator_paste");
            tex_frame_add = Game1.contentmanager.Load<Texture2D>("animator_frame_add");
            tex_frame_delete = Game1.contentmanager.Load<Texture2D>("animator_frame_delete");

            but_frame_copie = new Button(tex_copie, new Vector2(Game1.Screenwidth - 220, 355), new Vector2(24));
            but_frame_copie.Enabled = but_frame_copie.Visible = false;
            Game1.components.Add(but_frame_copie);
            but_layer_copie = new Button(tex_copie, new Vector2(Game1.Screenwidth - 220, 455), new Vector2(24));
            but_layer_copie.Enabled = but_layer_copie.Visible = false;
            Game1.components.Add(but_layer_copie);

            but_frame_paste = new Button(tex_paste, new Vector2(Game1.Screenwidth - 190, 355), new Vector2(24));
            but_frame_paste.Enabled = but_frame_paste.Visible = false;
            Game1.components.Add(but_frame_paste);
            but_layer_paste = new Button(tex_paste, new Vector2(Game1.Screenwidth - 190, 455), new Vector2(24));
            but_layer_paste.Enabled = but_layer_paste.Visible = false;
            Game1.components.Add(but_layer_paste);

            but_frame_add = new Button(tex_frame_add, new Vector2(Game1.Screenwidth - 130, 355), new Vector2(24));
            but_frame_add.Enabled = but_frame_add.Visible = false;
            Game1.components.Add(but_frame_add);
            but_frame_delete = new Button(tex_frame_delete, new Vector2(Game1.Screenwidth - 100, 355), new Vector2(24));
            but_frame_delete.Enabled = but_frame_delete.Visible = false;
            Game1.components.Add(but_frame_delete);

            but_frame_copie.event_pressed += frame_copie_pressed;
            but_frame_paste.event_pressed += frame_paste_pressed;
            but_layer_copie.event_pressed += layer_copie_pressed;
            but_layer_paste.event_pressed += layer_paste_pressed;
            but_frame_add.event_pressed += frame_add_pressed;
            but_frame_delete.event_pressed += frame_delete_pressed;

            frameratebox = new FloatBox(0.0f, 1000.0f, new Vector2(Game1.Screenwidth - 280, 430), new Vector2(80, 25));
            frameratebox.Enabled = frameratebox.Visible = false;
            Game1.components.Add(frameratebox);

            animation_effect = Game1.contentmanager.Load<Effect>("animator_effect");
            layeredittarget = new RenderTarget2D(Game1.gdevice, 600, 600, false, SurfaceFormat.Color, DepthFormat.None);
            tex_currentedit = new Texture2D(Game1.gdevice, 8, 8, false, SurfaceFormat.Color);
            useless = new Texture2D(Game1.gdevice, 600, 600, false, SurfaceFormat.Color);

            editorpos = new Vector2(Game1.Screenwidth / 2 - layeredittarget.Width / 2, Game1.Screenheight / 2 - layeredittarget.Height / 2);



            tex_play = Game1.contentmanager.Load<Texture2D>("animator_play");
            tex_break = Game1.contentmanager.Load<Texture2D>("animator_break");
            but_play = new Button(tex_play, new Vector2(Game1.Screenwidth - 280, 460), new Vector2(36));
            but_play.event_pressed += play_pressed;
            but_play.Enabled = but_play.Visible = false;
            Game1.components.Add(but_play);

            tex_open = Game1.contentmanager.Load<Texture2D>("animator_open");
            but_open = new Button(tex_open, new Vector2(Game1.Screenwidth - 280, 370), new Vector2(24));
            but_open.event_pressed += Load_GenAnimation;
            but_open.Enabled = but_open.Visible = false;
            Game1.components.Add(but_open);

            tex_save = Game1.contentmanager.Load<Texture2D>("animator_save");
            but_save = new Button(tex_save, new Vector2(Game1.Screenwidth - 280, 400), new Vector2(24));
            but_save.event_pressed += Save_Animation;
            but_save.Enabled = but_save.Visible = false;
            Game1.components.Add(but_save);

            currentanimation = new Animation("Empty", 2, false);
            currentanimation.frames.Add(new Vector3[8, 8, 8]);

            tex_edit = Game1.contentmanager.Load<Texture2D>("painter_pencil");
            but_edit = new Button(tex_edit, new Vector2(Game1.Screenwidth - 250 + Game1.font.MeasureString(currentanimation.name).X + 5, 400), new Vector2(24));
            but_edit.event_pressed += Edit_but_pressed;
            but_edit.Enabled = but_edit.Visible = false;
            Game1.components.Add(but_edit);

            framebox = new IntBox(0, currentanimation.frames.Count - 1, new Vector2(Game1.Screenwidth - 160, 325), new Vector2(100, 25));
            framebox.Enabled = framebox.Visible = false;
            Game1.components.Add(framebox);
            layerbox = new IntBox(0, 7, new Vector2(Game1.Screenwidth - 160, 425), new Vector2(100, 25));
            layerbox.Enabled = layerbox.Visible = false;
            Game1.components.Add(layerbox);

            tex_minus = Game1.contentmanager.Load<Texture2D>("minus");
            tex_plus = Game1.contentmanager.Load<Texture2D>("plus");
            but_frame_minus = new Button(tex_minus, new Vector2(Game1.Screenwidth - 220, 325), new Vector2(24));
            but_frame_plus = new Button(tex_plus, new Vector2(Game1.Screenwidth - 190, 325), new Vector2(24));
            but_frame_minus.Enabled = but_frame_minus.Visible = but_frame_plus.Enabled = but_frame_plus.Visible = false;
            Game1.components.Add(but_frame_minus);
            Game1.components.Add(but_frame_plus);
            but_layer_minus = new Button(tex_minus, new Vector2(Game1.Screenwidth - 220, 425), new Vector2(24));
            but_layer_plus = new Button(tex_plus, new Vector2(Game1.Screenwidth - 190, 425), new Vector2(24));
            but_layer_minus.Enabled = but_layer_minus.Visible = but_layer_plus.Enabled = but_layer_plus.Visible = false;
            Game1.components.Add(but_layer_plus);
            Game1.components.Add(but_layer_minus);
            but_frame_plus.event_pressed += frame_plus_pressed;
            but_frame_minus.event_pressed += frame_minus_pressed;
            but_layer_plus.event_pressed += layer_plus_pressed;
            but_layer_minus.event_pressed += layer_minus_pressed;

            but_IsEdit = new Button(tex_edit, new Vector2(Game1.Screenwidth - 220, 510), new Vector2(24));
            but_IsEdit.Enabled = but_IsEdit.Visible = false;
            but_IsEdit.event_pressed += but_IsEdit_pressed;
            Game1.components.Add(but_IsEdit);

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
            but_pencil.IsActive = true;
            for (int i = 0; i < buttons.Length; ++i)
            { buttons[i].Enabled = buttons[i].Visible = false; Game1.components.Add(buttons[i]); buttons[i].event_pressed += but_pressed; }

            GenNewTex();
        }

        private void play_pressed(object sender, EventArgs e)
        {
            IsPlaying ^= true;
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

        public override void Activate()
        {
            IsActiveted = true;
            IsPlaying = false;
            but_open.Enabled = but_open.Visible = true;
            but_save.Enabled = but_save.Visible = true;
            but_edit.Enabled = but_edit.Visible = true;
            but_play.Enabled = but_play.Visible = true;
            frameratebox.Enabled = frameratebox.Visible = true;
            //animationdropper.Enabled = animationdropper.Visible = true;
        }
        public override void Deactivate()
        {
            IsActiveted = false;
            IsEditing = false;
            IsPlaying = false;
            but_open.Enabled = but_open.Visible = false;
            but_save.Enabled = but_save.Visible = false;
            but_edit.Enabled = but_edit.Visible = false;
            framebox.Enabled = framebox.Visible = false;
            layerbox.Enabled = layerbox.Visible = false;
            but_IsEdit.Enabled = but_IsEdit.Visible = false;
            but_frame_minus.Enabled = but_frame_minus.Visible = but_frame_plus.Enabled = but_frame_plus.Visible = false;
            but_layer_minus.Enabled = but_layer_minus.Visible = but_layer_plus.Enabled = but_layer_plus.Visible = false;
            but_IsEdit.Enabled = but_IsEdit.Visible = false;
            R_slider.Enabled = G_slider.Enabled = B_slider.Enabled = false;
            R_slider.Visible = G_slider.Visible = B_slider.Visible = false;
            but_frame_add.Enabled = but_frame_add.Visible = false;
            but_frame_delete.Enabled = but_frame_delete.Visible = false;
            but_frame_copie.Enabled = but_frame_copie.Visible = but_frame_paste.Enabled = but_frame_paste.Visible = false;
            but_layer_copie.Enabled = but_layer_copie.Visible = but_layer_paste.Enabled = but_layer_paste.Visible = false;
            frameratebox.Enabled = frameratebox.Visible = false;
            but_play.Enabled = but_play.Visible = false;
            Game1.mainstates.IsCamera = true;
            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].Enabled = buttons[i].Visible = false;
            }
            //animationdropper.Enabled = animationdropper.Visible = false;
        }

        private void but_IsEdit_pressed(object sender, EventArgs e)
        {
            IsEditOpen ^= true;
        }

        private void GenNewTex()
        {
            Color[] cols = new Color[8 * 8];
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    cols[i + j * 8] = new Color(currentanimation.frames[edit_currentframe][i, edit_currentlayer, j]);
                }
            }
            tex_currentedit.SetData(cols);
        }

        private void frame_plus_pressed(object sender, EventArgs e)
        {
            if (edit_currentframe < framebox.maxvalue)
            { edit_currentframe++; framebox.UpdateValue(edit_currentframe); }
            GenNewTex();
        }
        private void frame_minus_pressed(object sender, EventArgs e)
        {
            if (edit_currentframe > 0)
            { edit_currentframe--; framebox.UpdateValue(edit_currentframe); }
            GenNewTex();
        }
        private void layer_plus_pressed(object sender, EventArgs e)
        {
            if (edit_currentlayer < 7)
            { edit_currentlayer++; layerbox.UpdateValue(edit_currentlayer); }
            GenNewTex();
        }
        private void layer_minus_pressed(object sender, EventArgs e)
        {
            if (edit_currentlayer > 0)
            { edit_currentlayer--; layerbox.UpdateValue(edit_currentlayer); }
            GenNewTex();
        }

        private void frame_copie_pressed(object sender, EventArgs e)
        {
            if (buffer_frame == null)
                buffer_frame = new Vector3[8, 8, 8];
            Array.Copy(currentanimation.frames[edit_currentframe], buffer_frame, buffer_frame.Length);
        }
        private void layer_copie_pressed(object sender, EventArgs e)
        {
            if (buffer_layer == null)
                buffer_layer = new Vector3[8, 8];
            for (int x = 0; x < 8; ++x)
            {
                for (int y = 0; y < 8; ++y)
                {
                    buffer_layer[x, y] = currentanimation.frames[edit_currentframe][x, edit_currentlayer, y];
                }
            }
        }
        private void frame_paste_pressed(object sender, EventArgs e)
        {
            if (buffer_frame != null)
                Array.Copy(buffer_frame, currentanimation.frames[edit_currentframe], buffer_frame.Length);
            GenNewTex();
        }
        private void layer_paste_pressed(object sender, EventArgs e)
        {
            if (buffer_layer != null)
            {
                for (int x = 0; x < 8; ++x)
                {
                    for (int y = 0; y < 8; ++y)
                    {
                        currentanimation.frames[edit_currentframe][x, edit_currentlayer, y] = buffer_layer[x, y];
                    }
                }
                GenNewTex();
            }
        }
        private void frame_add_pressed(object sender, EventArgs e)
        {
            currentanimation.frames.Insert(edit_currentframe + 1, new Vector3[8, 8, 8]);
            framebox.maxvalue++;
            GenNewTex();
        }
        private void frame_delete_pressed(object sender, EventArgs e)
        {
            if (currentanimation.frames.Count > 1)
            {
                currentanimation.frames.RemoveAt(edit_currentframe);
                if (edit_currentframe >= currentanimation.frames.Count)
                    edit_currentframe--;
                framebox.maxvalue--;
                framebox.UpdateValue(edit_currentframe);
                GenNewTex();
            }
        }

        private void Edit_but_pressed(object sender, EventArgs e)
        {
            if (!IsEditing)
            {
                IsEditing = true;
                IsPlaying = false;
                edit_currentlayer = 0;
                framebox.maxvalue = currentanimation.frames.Count - 1;
                framebox.Enabled = framebox.Visible = true;
                layerbox.Enabled = layerbox.Visible = true;
                but_frame_minus.Enabled = but_frame_minus.Visible = but_frame_plus.Enabled = but_frame_plus.Visible = true;
                but_layer_minus.Enabled = but_layer_minus.Visible = but_layer_plus.Enabled = but_layer_plus.Visible = true;
                but_frame_copie.Enabled = but_frame_copie.Visible = but_frame_paste.Enabled = but_frame_paste.Visible = true;
                but_layer_copie.Enabled = but_layer_copie.Visible = but_layer_paste.Enabled = but_layer_paste.Visible = true;
                but_frame_add.Enabled = but_frame_add.Visible = true;
                but_frame_delete.Enabled = but_frame_delete.Visible = true;
                but_IsEdit.Enabled = but_IsEdit.Visible = true;
                R_slider.Enabled = G_slider.Enabled = B_slider.Enabled = true;
                R_slider.Visible = G_slider.Visible = B_slider.Visible = true;
                for (int i = 0; i < buttons.Length; ++i)
                {
                    buttons[i].Enabled = buttons[i].Visible = true;
                }
                but_edit.Enabled = but_open.Enabled = but_save.Enabled = false;
                but_edit.Visible = but_open.Visible = but_save.Visible = false;
                frameratebox.Enabled = frameratebox.Visible = false;
                but_play.Enabled = but_play.Visible = false;
            }
            else
            {
                IsEditing = false;
                framebox.Enabled = framebox.Visible = false;
                layerbox.Enabled = layerbox.Visible = false;
                but_frame_minus.Enabled = but_frame_minus.Visible = but_frame_plus.Enabled = but_frame_plus.Visible = false;
                but_layer_minus.Enabled = but_layer_minus.Visible = but_layer_plus.Enabled = but_layer_plus.Visible = false;
                but_IsEdit.Enabled = but_IsEdit.Visible = false;
                R_slider.Enabled = G_slider.Enabled = B_slider.Enabled = false;
                R_slider.Visible = G_slider.Visible = B_slider.Visible = false;
                but_frame_copie.Enabled = but_frame_copie.Visible = but_frame_paste.Enabled = but_frame_paste.Visible = false;
                but_layer_copie.Enabled = but_layer_copie.Visible = but_layer_paste.Enabled = but_layer_paste.Visible = false;
                but_frame_add.Enabled = but_frame_add.Visible = false;
                but_frame_delete.Enabled = but_frame_delete.Visible = false;
                for (int i = 0; i < buttons.Length; ++i)
                {
                    buttons[i].Enabled = buttons[i].Visible = false;
                }
                but_edit.Enabled = but_open.Enabled = but_save.Enabled = true;
                but_edit.Visible = but_open.Visible = but_save.Visible = true;
                frameratebox.Enabled = frameratebox.Visible = true;
                but_play.Enabled = but_play.Visible = true;
            }
        }

        private void Save_Animation(object sender, EventArgs e)
        {
            string savepath = System.IO.Directory.GetCurrentDirectory() + "\\SAVES";
            try
            { System.IO.Directory.CreateDirectory(savepath); }
            catch (Exception exp)
            { Console.WriteLine("Error while trying to create Save folder: {0}", exp); }

            string filename = savepath + "\\" + currentanimation.name + ".bin";

            try
            {
                FileStream stream;
                bool FileExists = File.Exists(filename);
                if (FileExists)
                    stream = new FileStream(filename, FileMode.Open);
                else
                    stream = new FileStream(filename, FileMode.Create);
                List<byte> buffer = new List<byte>();
                buffer.AddRange(BitConverter.GetBytes(currentanimation.frames.Count));
                buffer.AddRange(BitConverter.GetBytes(currentanimation.framerate));
                buffer.AddRange(BitConverter.GetBytes(currentanimation.IsRepeat));
                for (int i = 0; i < currentanimation.frames.Count; ++i)
                {
                    for (int x = 0; x < 8; ++x)
                    {
                        for (int y = 0; y < 8; ++y)
                        {
                            for (int z = 0; z < 8; ++z)
                            {
                                buffer.AddRange(BitConverter.GetBytes(currentanimation.frames[i][x, y, z].X));
                                buffer.AddRange(BitConverter.GetBytes(currentanimation.frames[i][x, y, z].Y));
                                buffer.AddRange(BitConverter.GetBytes(currentanimation.frames[i][x, y, z].Z));
                            }
                        }
                    }
                }
                stream.Write(buffer.ToArray(), 0, buffer.Count);
                stream.Close();
                stream.Dispose();
                Console.WriteLine("Saving suceeded. Filename: {0}", filename);
                IsSavedcounter = 100;

            }
            catch (Exception exp)
            {
                Console.WriteLine("Failed: {0}", exp);
                System.Windows.Forms.MessageBox.Show("Failed", null, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

        }

        private void Load_GenAnimation(object sender, EventArgs e)
        {
            currentanimation.IsPlaying = false;
            using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
            {
                try
                {
                    string savepath = System.IO.Directory.GetCurrentDirectory() + "\\SAVES";
                    System.IO.Directory.CreateDirectory(savepath);
                    dialog.InitialDirectory = savepath;
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Error while trying to create Save folder: {0}", exp);
                }

                dialog.Multiselect = false;
                dialog.CheckPathExists = false;
                dialog.CheckFileExists = false;
                dialog.Title = "Select Animation or create a new Animation";
                dialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    string filename = dialog.FileName;
                    try
                    {
                        FileStream stream;
                        bool FileExists = File.Exists(filename);
                        if (FileExists)
                            stream = new FileStream(filename, FileMode.Open);
                        else
                            stream = new FileStream(filename, FileMode.Create);

                        if (!FileExists)
                        {
                            currentanimation = new Animation(dialog.SafeFileName.Split('.')[0], 2, true);
                            currentanimation.frames.Add(new Vector3[8, 8, 8]);
                        }
                        else
                        {
                            currentanimation.name = dialog.SafeFileName.Split('.')[0];
                            currentanimation.IsPlaying = false;
                            byte[] buffer4 = new byte[4];
                            stream.Read(buffer4, 0, 4);
                            int framecount = BitConverter.ToInt32(buffer4, 0);
                            stream.Read(buffer4, 0, 4);
                            currentanimation.framerate = BitConverter.ToSingle(buffer4, 0);
                            frameratebox.currentvalue = currentanimation.framerate;
                            stream.Read(buffer4, 0, 1);
                            currentanimation.IsRepeat = BitConverter.ToBoolean(buffer4, 0);
                            currentanimation.frames.Clear();
                            for (int i = 0; i < framecount; ++i)
                            {
                                currentanimation.frames.Add(new Vector3[8, 8, 8]);
                                for (int x = 0; x < 8; ++x)
                                {
                                    for (int y = 0; y < 8; ++y)
                                    {
                                        for (int z = 0; z < 8; ++z)
                                        {
                                            Vector3 data = Vector3.Zero;
                                            stream.Read(buffer4, 0, 4);
                                            data.X = BitConverter.ToSingle(buffer4, 0);
                                            stream.Read(buffer4, 0, 4);
                                            data.Y = BitConverter.ToSingle(buffer4, 0);
                                            stream.Read(buffer4, 0, 4);
                                            data.Z = BitConverter.ToSingle(buffer4, 0);
                                            currentanimation.frames[i][x, y, z] = data;
                                        }
                                    }
                                }
                            }

                        }
                        stream.Close();
                        stream.Dispose();
                        IsPlaying = false;
                        edit_currentframe = 0;
                        edit_currentlayer = 0;
                        framebox.UpdateValue(edit_currentframe);
                        framebox.maxvalue = currentanimation.frames.Count - 1;
                        GenNewTex();
                        Console.WriteLine("Loading suceeded. Filename: {0}", filename);

                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine("Failed: {0}", exp);
                        System.Windows.Forms.MessageBox.Show("Failed", null, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (IsActiveted)
            {
                Game1.mainstates.IsCamera = true;
                Vector2 mousepos = Game1.mousestate.Position.ToVector2();
                Vector2 oldmousepos = Game1.oldmousestate.Position.ToVector2();

                if (Game1.mousestate.LeftButton == ButtonState.Pressed)
                {
                    if (oldmousepos.X > editorpos.X && oldmousepos.X < editorpos.X + layeredittarget.Width && oldmousepos.Y > editorpos.Y - 25 && oldmousepos.Y < editorpos.Y)
                    {
                        editorpos += mousepos - oldmousepos;
                    }
                }

                curcol = new Vector3(R_slider.currentvalue, G_slider.currentvalue, B_slider.currentvalue);
                if (IsEditing)
                {
                    if (Game1.keyboardstate.IsKeyDown(Keys.Escape) && Game1.oldkeyboardstate.IsKeyUp(Keys.Escape))
                        Edit_but_pressed(null, null);

                    Array.Copy(currentanimation.frames[edit_currentframe], Game1.main_cube.color_data, 8 * 8 * 8);

                    edit_currentframe = framebox.currentvalue;
                    edit_currentlayer = layerbox.currentvalue;
                    if (IsEditOpen)
                    {
                        if (but_pencil.IsActive || but_pipette.IsActive)
                        {
                            if (but_pencil.IsActive)
                            {
                                if (Game1.mousestate.LeftButton == ButtonState.Pressed)
                                {
                                    Vector2 pos = (mousepos - editorpos) / new Vector2(layeredittarget.Width / 8);
                                    ExtendedDatatypes.int2 pos_int = new ExtendedDatatypes.int2((int)pos.X, (int)pos.Y);
                                    if (pos.X >= 0 && pos.X < 8 && pos.Y >= 0 && pos.Y < 8)
                                    {
                                        currentanimation.frames[edit_currentframe][pos_int.X, edit_currentlayer, pos_int.Y] = curcol;
                                        GenNewTex();
                                    }
                                }
                                if (Game1.mousestate.RightButton == ButtonState.Pressed)
                                {
                                    Vector2 pos = (mousepos - editorpos) / new Vector2(layeredittarget.Width / 8);
                                    ExtendedDatatypes.int2 pos_int = new ExtendedDatatypes.int2((int)pos.X, (int)pos.Y);
                                    if (pos.X >= 0 && pos.X < 8 && pos.Y >= 0 && pos.Y < 8)
                                    {
                                        Game1.mainstates.IsCamera = false;
                                        currentanimation.frames[edit_currentframe][pos_int.X, edit_currentlayer, pos_int.Y] = Vector3.Zero;
                                        GenNewTex();
                                    }
                                }
                            }
                            else
                            {
                                if (Game1.mousestate.LeftButton == ButtonState.Pressed)
                                {
                                    Vector2 pos = (mousepos - editorpos) / new Vector2(layeredittarget.Width / 8);
                                    ExtendedDatatypes.int2 pos_int = new ExtendedDatatypes.int2((int)pos.X, (int)pos.Y);
                                    if (pos.X >= 0 && pos.X < 8 && pos.Y >= 0 && pos.Y < 8)
                                    {
                                        curcol = currentanimation.frames[edit_currentframe][pos_int.X, edit_currentlayer, pos_int.Y];
                                        R_slider.currentvalue = curcol.X;
                                        G_slider.currentvalue = curcol.Y;
                                        B_slider.currentvalue = curcol.Z;
                                    }
                                }
                            }
                        }

                        if (but_fill.IsActive)
                        {
                            for (int i = 0; i < 8 * 8; ++i)
                            {
                                currentanimation.frames[edit_currentframe][i % 8, edit_currentlayer, i / 8] = curcol;
                            }
                            GenNewTex();
                        }
                    }
                }
                else
                {
                    currentanimation.framerate = frameratebox.currentvalue;
                }
                currentanimation.Update();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsActiveted)
            {
                if (!IsEditing)
                {
                    Game1.spriteBatch.Begin();
                    Game1.spriteBatch.DrawString(Game1.font, "Current Animation: ", new Vector2(Game1.Screenwidth - 250, 370), Color.Gray);
                    Game1.spriteBatch.DrawString(Game1.font, currentanimation.name, new Vector2(Game1.Screenwidth - 250, 400), Color.Gray);
                    Game1.spriteBatch.DrawString(Game1.font, "Hz", new Vector2(Game1.Screenwidth - 195, 430), Color.Gray);
                    if (IsSavedcounter > 0)
                    {
                        IsSavedcounter--;
                        Game1.spriteBatch.DrawString(Game1.font, "Saved", new Vector2(Game1.Screenwidth - 340, 400), Color.Red);
                    }
                    Game1.spriteBatch.End();
                }
                else
                {
                    if (IsEditOpen)
                    {
                        Game1.gdevice.SetRenderTarget(layeredittarget);
                        animation_effect.Techniques[0].Passes[0].Apply();
                        animation_effect.Parameters["layertex"].SetValue(tex_currentedit);
                        Game1.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, animation_effect, Matrix.Identity);
                        Game1.spriteBatch.Draw(useless, Vector2.Zero, Color.White);
                        Game1.spriteBatch.End();
                        Game1.gdevice.SetRenderTarget(null);
                        Game1.spriteBatch.Begin();
                        Game1.DrawRectangle_Filled(editorpos - new Vector2(0, 25), new Vector2(layeredittarget.Width, 25), new Color(new Vector3(0.1f)));
                        Game1.DrawRectangle(editorpos - new Vector2(4, 4 + 25), new Vector2(layeredittarget.Width + 8, 25 + 8 + layeredittarget.Height), Color.Black, 4);
                        Game1.spriteBatch.Draw(layeredittarget, editorpos, Color.White);
                        Game1.spriteBatch.Draw(tex_currentedit, Vector2.Zero, Color.White);
                        Game1.spriteBatch.End();
                    }
                    Game1.spriteBatch.Begin();
                    Game1.spriteBatch.DrawString(Game1.font, "Current Frame<0-" + framebox.maxvalue.ToString() + ">:", new Vector2(Game1.Screenwidth - 220, 300), Color.Gray);
                    Game1.spriteBatch.DrawString(Game1.font, "Current Layer<0-7>", new Vector2(Game1.Screenwidth - 220, 400), Color.Gray);
                    Game1.DrawRectangle_Filled(new Vector2(Game1.Screenwidth - 120, Game1.Screenheight - 180), new Vector2(80, 30), new Color(curcol));
                    Game1.spriteBatch.End();

                }
            }

        }
    }
}
