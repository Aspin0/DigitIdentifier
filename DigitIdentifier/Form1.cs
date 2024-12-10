using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DigitIdentifier
{
    public partial class Form1 : Form
    {
        Bitmap bmap;
        Boolean cursorMoving = false;
        Pen cursorPen;
        int cursorX = -1;
        int cursorY = -1;

        public Form1()
        {
            InitializeComponent();
            label3.Visible = false;
            bmap = new Bitmap(canvas.Width, canvas.Height);
            // start with bitmap as white otherwise first save will be transparent
            using (Graphics g = Graphics.FromImage(bmap))
            {
                g.Clear(Color.White);
            }

            cursorPen = new Pen(Color.Black, 40);
            cursorPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            cursorPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }

        // Clear button
        private void button1_Click(object sender, EventArgs e)
        {
            using (Graphics g = Graphics.FromImage(bmap)) 
            { 
                g.Clear(Color.White); 
            }
            canvas.Invalidate();
            label2.Text = "";
        }

        // Predict button
        private async void button2_Click(object sender, EventArgs e)
        {
            label3.Visible = true;

            String result = await RunOnSeperateThread();

            label3.Visible = false;
            label2.Text = result;
        }

        private async Task<String> RunOnSeperateThread()
        {
            return await Task.Run(() => predict());
        }

        private string predict()
        {
            // this is where the drawn image will be saved
            bmap.Save("C:/Users/Lucas/Documents/DigitsCNNproject/drawing.png");

            var psi = new ProcessStartInfo();
            // change this to whereever your python.exe is stored 
            psi.FileName = @"C:\Users\Lucas\AppData\Local\Programs\Python\Python312\python.exe";

            // change this to whereever the CNNScript.py is saved
            var script = @"C:\Users\Lucas\Documents\DigitsCNNproject\CNNscript.py";
            psi.Arguments = script;

            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            var errors = "";
            var results = "";

            using (var process = Process.Start(psi))
            {
                errors = process.StandardError.ReadToEnd();
                results = process.StandardOutput.ReadToEnd();
            }

            Console.WriteLine("Result:");
            Console.WriteLine(results);
            return results;

            //label2.Text = results;
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            cursorMoving = true;
            cursorX = e.X;
            cursorY = e.Y;
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            cursorMoving = false;
            cursorX = -1;
            cursorY = -1;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (cursorX != -1 && cursorY != -1 && cursorMoving == true)
            {
                // this draws to the bitmap that will be saved, but is not visible on screen
                using (Graphics g = Graphics.FromImage(bmap))
                {
                    g.DrawLine(cursorPen, new Point(cursorX, cursorY), e.Location);
                }
                // this draws the graphics on screen, seperate but identical to the bitmap
                canvas.CreateGraphics().DrawLine(cursorPen, new Point(cursorX, cursorY), e.Location);
                cursorX = e.X;
                cursorY = e.Y;
            }
        }
    }
}
