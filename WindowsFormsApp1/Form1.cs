using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp1


{
    public partial class Form1 : Form
    {
        class Bullet
        {
            int x, y, w, h;

        }

        int x, y, w, h;
        int direction, speed;
        int enemyX, enemyY, enemyDirection;
        float enemyFall, rate;
        bool hit = false, dead = false;
        int amoSize, next;
        int countdown;
        string str;
        Rectangle[] Amo;
        int[] goup;
        Rectangle box;
        Bitmap shipModel, enemyModel;
        Font hitFont = new Font("Arial", 10);

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            next = 0;
            direction = 0;
            speed = 3;
            x = gameField1.Width / 2;
            w = h = 10;
            y = gameField1.Height - h;
            Init_Amo();
            try
            {
                shipModel = (Bitmap)Image.FromFile(@"C:\Users\rafal\source\repos\WindowsFormsApp1\WindowsFormsApp1\ship.bmp", true);
                enemyModel = (Bitmap)Image.FromFile(@"C:\Users\rafal\source\repos\WindowsFormsApp1\WindowsFormsApp1\enemy.bmp", true);
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("Nie ma pliku");
            }

        }

        public void Collision()
        {
            for (int i=0; i < amoSize; i++)
            {
                if(Amo[i].Y < enemyY + enemyModel.Height && Amo[i].X > enemyX && Amo[i].X < enemyX + enemyModel.Width)
                {
                    hit = true;
                    enemyDirection = 0;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }


        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Space)
            {
                e.IsInputKey = true;
            }
        }

        private void Move(Keys key)
        {
            switch (key)
            {
               case Keys.Right:
                    direction = speed;
                    break;

                case Keys.Left:
                    direction = -speed;
                    break;

                case Keys.Space:
                    if (next < amoSize)
                    {
                        goup[next] = -1;
                        next++;
                    }
                    else { next = 0; }
                    break;
            }
        }

        private void Form1_KeyDown_1(object sender, KeyEventArgs e)
        {
            Move(e.KeyCode);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                direction = 0;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form1 NewForm = new Form1();
            NewForm.Show();
            this.Dispose(false);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            enemyX += enemyDirection;
            enemyFall += 0.2f;
            enemyY = (int)enemyFall;
            if (enemyX >= gameField1.Width - enemyModel.Width)
            { enemyDirection = -1; }
            if (enemyX <= 0)
            { enemyDirection = 1; }
            gameField1.Refresh();
        }

        private void Panel_Load(object sender, EventArgs e)
        {

        }

        private void gameField1_MouseMove(object sender, MouseEventArgs e)
        {
            x = e.X;
        }

        private void gameField1_MouseClick(object sender, MouseEventArgs e)
        {
            if (next < amoSize)
            {
                goup[next] = -1;
                next++;
            }
            else { next = 0; }
        }

        private void gameField1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush p = new SolidBrush(Color.Black);
            Color backColor = Color.White;
            x += direction;
            shipModel.MakeTransparent(backColor);
            g.DrawImage(shipModel, x - 7, y - shipModel.Height + 15);
            enemyModel.MakeTransparent(backColor);
            Collision();
            if (!dead)
            {
                if (hit == false)
                {
                    g.DrawImage(enemyModel, enemyX, enemyY);
                }
                else
                {
                    g.DrawString("Trafiony", hitFont, p, enemyX, enemyY);
                    kill();
                }
            }
            setAmoXY();
            AmoRefresh(g);
        }

        public async void kill()
        {
            await Task.Delay(2000);
            dead = true;

        }

        public void setBox(int x)
        {
            box.X = x - w;
            box.Width = 2 * w;
            box.Height = 2 * h;
        }
     
        private void Init_Amo()
        {
            amoSize = 10;
            Amo = new Rectangle[amoSize];
            goup = new int[amoSize];
            for (int i = 0; i < amoSize; i++)
            {
                Amo[i] = new Rectangle(x-1, y, 2, 3);
                goup[i] = 0;

            }
        }

        public void setAmoXY()
        {
            for (int i = 0; i < amoSize; i++)
            {
                Amo[i].Y += goup[i];
                if (goup[i] == 0) { Amo[i].X = x -1; }
                if (Amo[i].Y <= 0) { Amo[i].X = x - (w - 5) / 2; Amo[i].Y = y - h; goup[i] = 0; }
            }
        }

        public void AmoRefresh(Graphics g)
        {
            SolidBrush pa = new SolidBrush(Color.Red);
            for (int i = 0; i < amoSize; i++)
            {
                g.FillRectangle(pa, Amo[i]);
                g.DrawRectangle(new Pen(pa), Amo[i]);
            }
        }

    }
}
