using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace WAV2SBWAV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class data
        {
            public static byte[] input;
            public static byte[] output = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Oops?
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                WaveFileReader tmp = new WaveFileReader(openFileDialog1.FileName);
                if ((tmp.WaveFormat.SampleRate == 8180) || checkBox1.Checked)
                {
                    if (tmp.WaveFormat.BitsPerSample == 8)
                    {
                        if (tmp.SampleCount <= 245400 || checkBox1.Checked)
                        {
                            data.input = new byte[tmp.Length];
                            tmp.Read(data.input, 0, Convert.ToInt32(tmp.Length - 1));
                            data.output = new byte[(tmp.Length*4)+128];
                            setbinaryvalues(ref data.output, 0, "00 00 01 00 00 00 00 00 00 00 00 00 00 D7 01 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 50 43 42 4E 30 30 30 31 04 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00"); //DAT flag, date
                            setbinaryvalues(ref data.output, 92, (tmp.Length % 256).ToString("X2"));
                            setbinaryvalues(ref data.output, 93, ((tmp.Length >> 8) % 256).ToString("X2"));
                            setbinaryvalues(ref data.output, 94, ((tmp.Length >> 16) % 256).ToString("X2"));
                            setbinaryvalues(ref data.output, 95, ((tmp.Length >> 24) % 256).ToString("X2"));
                            setbinaryvalues(ref data.output, 108, (Convert.ToInt32(tmp.Length / 8180.0f * 60.0f) % 256).ToString("X2"));
                            setbinaryvalues(ref data.output, 109, (((Convert.ToInt32(tmp.Length / 8180.0f * 60.0f) >> 8) % 256)).ToString("X2"));
                            setbinaryvalues(ref data.output, 110, (((Convert.ToInt32(tmp.Length / 8180.0f * 60.0f) >> 16) % 256)).ToString("X2"));
                            setbinaryvalues(ref data.output, 111, (((Convert.ToInt32(tmp.Length / 8180.0f * 60.0f) >> 24) % 256)).ToString("X2"));
                            setbinaryvalues(ref data.output, data.output.Length - 19, "57 41 56 32 53 42 57 41 56 20 46 6F 6F 74 65 72 00 00");
                            progressBar1.Maximum = data.input.Length;
                            for (int i = 0; i < data.input.Length; i++)
                            {
                                progressBar1.Value = i+1;
                                setbinaryvalues(ref data.output, 112+i*4, data.input[i].ToString("X2"));
                            }
                        }
                        else
                        {
                            MessageBox.Show("ERROR: Too many samples.\nSplit your audio file?");
                        }
                    }
                    else
                    {
                        MessageBox.Show("ERROR: The amount of bits per sample is not 8.");
                    }
                }
                else
                {
                    MessageBox.Show("ERROR: Sample rate is not 8180.");
                }
            }
            button1.Enabled = true;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (data.output == null)
            {
                MessageBox.Show("ERROR: NO DATA HAS BEEN WRITTEN\nDid you really load a WAV file?");
            }
            else
            {
                DialogResult res = saveFileDialog1.ShowDialog();
                if (res==DialogResult.OK)
                {
                    System.IO.File.WriteAllBytes(saveFileDialog1.FileName,data.output);
                }
            }
        }

        private void setbinaryvalues(ref byte[] temporary, int start, string data)
        {
            string[] datas = data.Split(' ');
            for (int i = 0; i < datas.Length; i++)
            {
                temporary[start + i] = Convert.ToByte(datas[i],16);
            }
        }
    }
}
