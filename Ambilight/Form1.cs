using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace Ambilight
{
    public partial class Form1 : Form
    {
        Bitmap bitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
        SerialPort port = new SerialPort();
        //int R = 255, G = 255, B = 255;
        int NUM_LEDS = 30;
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            try
            {

                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.DataBits = 8;
                port.Handshake = Handshake.None;
                port.RtsEnable = true;
                
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    comboBox1.Items.Add(port);
                }
                port.BaudRate = 500000;
                comboBox1.SelectedIndex = 1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        

        public int[] getFrame(int frame_num)
        {

            int[] frame = new int[3];

            if (frame_num == 1)                                             // Taking scrennshot every first "frame"
            {
                Graphics graphics = Graphics.FromImage(bitmap as Image); // Create a new graphics objects that can capture the screen
                try
                {
                    graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size); // Screenshot moment → screen content to graphics object
                }
                catch (Exception) { }
                
            }


            int sumR = 0;
            int sumG = 0;
            int sumB = 0;

            if (frame_num <= NUM_LEDS / 2)
            {
                for (int x = bitmap.Width / (NUM_LEDS / 2) * frame_num - bitmap.Width / (NUM_LEDS / 2); x < bitmap.Width / (NUM_LEDS / 2) * frame_num; x += 16)
                {

                    for (int y = 0; y < bitmap.Height / 6; y += 18)
                    {

                        byte rr = bitmap.GetPixel(x, y).R;
                        byte gg = bitmap.GetPixel(x, y).G;
                        byte bb = bitmap.GetPixel(x, y).B;

                        sumR += rr;
                        sumG += gg;
                        sumB += bb;
                    }
                }
            }

            if (frame_num > NUM_LEDS / 2)
            {
                frame_num -= NUM_LEDS / 2;

                for (int x = bitmap.Width / (NUM_LEDS / 2) * frame_num - bitmap.Width / (NUM_LEDS / 2); x < bitmap.Width / (NUM_LEDS / 2) * frame_num; x += 16)
                {
                    for (int y = bitmap.Height / 6 * 5; y < bitmap.Height; y += 18)
                    {
                        byte rr = bitmap.GetPixel(x, y).R;
                        byte gg = bitmap.GetPixel(x, y).G;
                        byte bb = bitmap.GetPixel(x, y).B;

                        sumR += rr;
                        sumG += gg;
                        sumB += bb;
                    }
                }
            }
            
            frame[0] = sumR / ((bitmap.Width / (NUM_LEDS / 2)) / 16 * ((bitmap.Height / 6)) / 18);
            frame[1] = sumG / ((bitmap.Width / (NUM_LEDS / 2)) / 16 * ((bitmap.Height / 6)) / 18);
            frame[2] = sumB / ((bitmap.Width / (NUM_LEDS / 2)) / 16 * ((bitmap.Height / 6)) / 18);
            /*
            frame[0] = sumR / ((bitmap.Width / (NUM_LEDS / 2)) / 4 * ((bitmap.Height / 6)) / 6);
            frame[1] = sumG / ((bitmap.Width / (NUM_LEDS / 2)) / 4 * ((bitmap.Height / 6)) / 6);
            frame[2] = sumB / ((bitmap.Width / (NUM_LEDS / 2)) / 4 * ((bitmap.Height / 6)) / 6);
            */

            return frame;

        }



        

        private void Connect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!port.IsOpen)
                {
                    port.PortName = comboBox1.Text;
                    port.Open();       
                    port.Write("c");
                    timer1.Enabled = true;
                    timer1.Interval = 50;
                }
            }


            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public int[] GetScreen()
        {
            int[] screen = new int[3 * NUM_LEDS];
            int[] screen_temp = new int[3];

            for (int i = 1; i <= NUM_LEDS; i++)
            {

                screen_temp = getFrame(i);
                screen[i * 3 - 3] = screen_temp[0];
                screen[i * 3 - 2] = screen_temp[1];
                screen[i * 3 - 1] = screen_temp[2];
            }


            return screen;
        }


        public void setAmbient()
        {
           
            int[] screen = GetScreen();

            try
            {
                port.Write(string.Join(" ", screen) + "A");
            }

            catch (Exception ex)
            {
               
                MessageBox.Show(ex.Message);
            }
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            try
            {
                port.Write("d");
                port.Close();
                timer1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
                setAmbient();
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (port.IsOpen == true)
            {
                Disconnect_Click(null, null);
            }
            
        }
    }
}

