using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Data.SqlClient;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // Initialization of Graphics
        Graphics g;
        // Initialize pen with color black as default color
        Pen p = new Pen(Color.Black);
        // Flag used to mark the chosen shape
        int flag;
        // Boolean used for free hand drawing
        bool freestyle = false;
        // Variables used for mouse points
        int x, y, w, h, fsX1, fsY1;
        // Variable needed for the drawing effect
        int tickCount;
        
        String timeStamp = "";     
       
        public Form1()
        {
            InitializeComponent();
        }

        // Simple color selection with colorDialog
        private void colorSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                p.Color = colorDialog1.Color;
            }
        }

        // Selection of shapes/lines
        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flag = 1;
        }
        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flag = 2;
        }
        private void ellipseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flag = 3;
        }

        private void freeLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flag = 4;
        }

        // Clearing of canvas
        private void clearCanvasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            g = this.CreateGraphics();
            g.Clear(Color.White);
        }

        // Width selection
        private void button2_Click(object sender, EventArgs e)
        {
            int w = Int32.Parse(numericUpDown1.Text);
            p.Width = w;
        }

        // Setting up some key variables used for drawing
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Cross;
            if(flag == 4)
            {
                freestyle = true;
            }
            x = e.X;
            y = e.Y;
            fsX1 = e.X;
            fsY1 = e.Y;
        }

        // Free hand drawing
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            g = this.CreateGraphics();
            if (freestyle)
            {
                g.DrawLine(p, fsX1, fsY1, e.X, e.Y);
                timeStamp = GetTimestamp(DateTime.Now);
                writeToDb("Free line", timeStamp);
                fsX1 = e.X;
                fsY1 = e.Y;
            }
        }

        // Shape drawing
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
            freestyle = false;
            h = e.X - x;
            w = e.Y - y;
            g = this.CreateGraphics();
            Rectangle shape = new Rectangle(x, y, h, w);
            if (flag == 1)
            {
                g.DrawLine(p, x, y, e.X, e.Y);
                timeStamp = GetTimestamp(DateTime.Now);
                writeToDb("Straight line", timeStamp);
            }
            else if(flag == 2)
            {
                g.DrawRectangle(p, shape);
                timeStamp = GetTimestamp(DateTime.Now);
                writeToDb("Rectangle", timeStamp);
            }
            else if (flag == 3)
            {
                g.DrawEllipse(p, shape);
                timeStamp = GetTimestamp(DateTime.Now);
                writeToDb("Ellipse", timeStamp);
            }
        }

        // Initiation of the drawing of a simple house
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            tickCount = 0;
        }

        // Handling of the drawing of a simple house
        private void timer1_Tick(object sender, EventArgs e)
        {
            g = this.CreateGraphics();

            switch (tickCount)
            {
                case 0:
                    g.DrawLine(p, 150, 330, 300, 330);
                    break;
                case 1:
                    g.DrawLine(p, 300, 330, 300, 180);
                    break;
                case 2:
                    g.DrawLine(p, 300, 180, 225, 100);
                    break;
                case 3:
                    g.DrawLine(p, 225, 100, 150, 180);
                    break;
                case 4:
                    g.DrawLine(p, 150, 180, 150, 330);
                    break;
                case 5:
                    g.DrawLine(p, 150, 180, 300, 180);
                    break;
                case 6:
                    g.DrawLine(p, 210, 330, 210, 280);
                    break;
                case 7:
                    g.DrawLine(p, 210, 280, 240, 280);
                    break;
                case 8:
                    g.DrawLine(p, 240, 280, 240, 330);
                    timer1.Enabled = false;
                    break;
            }
            tickCount++;
        }

        // Database updater
        private void writeToDb(String shape, String timeStamp)
        {
            // You may need to change the DB's Data Source 
            SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\dimol\OneDrive\Documents\Πανεπιστήμιο\Paint 101\WindowsFormsApp1\DB\ShapeDb.mdf;Integrated Security = True; Connect Timeout = 30;");
            String insertQuery = "INSERT INTO Shapes(Shape, Timestamp) VALUES('" + shape + "','" + timeStamp + "');";
            SqlDataAdapter sda = new SqlDataAdapter(insertQuery, connection);
            DataTable dtbl = new DataTable();
            sda.Fill(dtbl);
        }

        // Timestamp format, we get the timestamp each time the user draws a shape
        private static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy-MM-dd-HH-mm-ss-ffff");
        }
    }
}
