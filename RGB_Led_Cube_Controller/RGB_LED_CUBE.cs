using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RGB_Led_Cube_Controller
{
    public class RGB_LED_CUBE
    {
        public bool IsWaiting, IsUpdated;
        public Vector3[,,] color_data, send_data;
        public Thread SendThread;

        private GraphicsDevice graphicsdevice;
        private static Effect effect;
        private static VertexBuffer led_vertexbuffer;
        private static IndexBuffer led_indexbuffer;
        private static Model led_model;
        private SerialPort port;
        private bool IsConnected;
        public int size;

        public RGB_LED_CUBE(int size, GraphicsDevice device)
        {
            IsWaiting = true;
            this.size = size;
            this.graphicsdevice = device;
            if (effect == null)
                effect = Game1.contentmanager.Load<Effect>("led_effect");
            if (led_model == null)
            {
                led_model = Game1.contentmanager.Load<Model>("5mmLED");
                led_vertexbuffer = led_model.Meshes[0].MeshParts[0].VertexBuffer;
                led_indexbuffer = led_model.Meshes[0].MeshParts[0].IndexBuffer;
            }
            color_data = new Vector3[size, size, size];
            send_data = new Vector3[size, size, size];
            SendThread = new Thread(SenderFunction);
            SendThread.IsBackground = true;
            SendThread.Start();

        }

        public bool ConnectwithCube()
        {
            IsConnected = false;
            string[] portnames = SerialPort.GetPortNames();
            if (portnames.Length == 0)
                return false;
            port = new SerialPort(portnames[0], 1500000, Parity.None, 8, StopBits.One);
            port.Open();
            IsConnected = true;
            return true;
        }
        public bool DisconnectwithCube()
        {
            IsConnected = false;
            if (port != null && port.IsOpen)
            {
                port.Close();
                port.Dispose();
            }
            return true;
        }

        public void SenderFunction()
        {
            while (true)
            {
                bool state = SendFrame();
                Thread.Sleep(1);
            }
        }

        public bool SendFrame()
        {
            //if (!IsUpdated)
            //return false;
            byte[] DATA1 = new byte[(size * size * size / 2)];
            byte[] DATA2 = new byte[(size * size * size / 2)];
            byte[] DATA3 = new byte[(size * size * size / 2)]; // Number of Leds * RGB * Bits for Brightness Levels
            lock (color_data)
            {
                int offset = size * (size / 2);
                // RED
                for (int i = 0; i < DATA1.Length; ++i)
                {
                    byte strengthdata1 = (byte)(send_data[((i / 8) % 4), 7 - i / 32, i % 8].X * 15.9f);
                    byte strengthdata2 = (byte)(send_data[((i / 8) % 4 + 4), 7 - i / 32, i % 8].X * 15.9f);
                    DATA1[255 - i] = (byte)(strengthdata1 + (strengthdata2 << 4));
                }

                // GREEN
                for (int i = 0; i < DATA2.Length; ++i)
                {
                    byte strengthdata1 = (byte)(send_data[((i / 8) % 4), 7 - i / 32, i % 8].Y * 15.9f);
                    byte strengthdata2 = (byte)(send_data[((i / 8) % 4 + 4), 7 - i / 32, i % 8].Y * 15.9f);
                    DATA2[255 - i] = (byte)(strengthdata1 + (strengthdata2 << 4));
                }

                // BLUE
                for (int i = 0; i < DATA3.Length; ++i)
                {
                    byte strengthdata1 = (byte)(send_data[((i / 8) % 4), 7 - i / 32, i % 8].Z * 15.9f);
                    byte strengthdata2 = (byte)(send_data[((i / 8) % 4 + 4), 7 - i / 32, i % 8].Z * 15.9f);
                    DATA3[255 - i] = (byte)(strengthdata1 + (strengthdata2 << 4));
                }
            }

            if (port == null || !port.IsOpen)
                return false;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int counter = 0;
            while (true)
            {
                if (port == null || !port.IsOpen)
                    return false;
                if (port.BytesToRead > 0)
                {
                    char data = (char)port.ReadChar();
                    //port.DiscardInBuffer();
                    if (data == 'R')
                        port.Write(DATA1, 0, 128);
                    if (data == 'X')
                        port.Write(DATA1, 128, 128);
                    if (data == 'G')
                        port.Write(DATA2, 0, 128);
                    if (data == 'Y')
                        port.Write(DATA2, 128, 128);
                    if (data == 'B')
                        port.Write(DATA3, 0, 128);
                    if (data == 'Z')
                    {
                        port.Write(DATA3, 128, 128);
                        break;
                    }
                }
            }

            IsUpdated = false;
            IsWaiting = true;
            return true;
        }

        public void SetAll2Col(Vector3 col)
        {
            float R = col.X;//0.01f * Game1.r.Next(0, 100);
            float G = col.Y;//0.01f * Game1.r.Next(0, 100);
            float B = col.Z;//0.01f * Game1.r.Next(0, 100);
            for (int x = 0; x < 8; ++x)
            {
                for (int y = 0; y < size; ++y)
                {
                    for (int z = 0; z < 8; ++z)
                    {
                        color_data[x, y, z] = col;
                    }
                }
            }
        }

        public void Draw(Matrix View, Matrix Projection)
        {
            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);
            effect.Parameters["lightdir"].SetValue(new Vector3(0, 0, 1));
            graphicsdevice.SetVertexBuffer(led_vertexbuffer);
            graphicsdevice.Indices = led_indexbuffer;
            for (int x = 0; x < size; ++x)
            {
                for (int y = 0; y < size; ++y)
                {
                    for (int z = 0; z < size; ++z)
                    {
                        effect.Parameters["World"].SetValue(Matrix.CreateRotationX(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(new Vector3(x, y, z) * 30));
                        effect.Parameters["ledcol"].SetValue(color_data[x, y, z]);
                        effect.CurrentTechnique.Passes[0].Apply();
                        graphicsdevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, led_indexbuffer.IndexCount / 3);
                    }
                }
            }
        }
    }
}
