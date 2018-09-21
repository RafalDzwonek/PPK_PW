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
        class Enemy
        {
            public int x, y, direction;
            public float fall;
            public bool hit = false, dead = true;

            public Enemy(GameField gameField)
            {
                Random rnd = new Random();
                x = rnd.Next(gameField.Width);
                y = 0;
                direction = 1;
                int to = rnd.Next(2);
                for(int i=0; i< to; i++)
                {
                    direction *= -1;
                }
                fall = 0;
            }

            public async void kill()
            {
                await Task.Delay(2000);
                dead = true;

            }

            public void Collision(Rectangle[] Amo, int amoSize, Bitmap enemyModel)
            {
                for (int i = 0; i < amoSize; i++)
                {
                    if (Amo[i].Y < y + enemyModel.Height && Amo[i].X > x && Amo[i].X < x + enemyModel.Width)
                    {
                        hit = true;
                        direction = 0;
                    }
                }
            }

            public void isOnBottom(Bitmap shipModel, Font font, GameField gameField, PaintEventArgs e, int Y, Timer timer)
            {
                    if (Y - shipModel.Height <= y && !dead)
                {
                    Graphics g = e.Graphics;
                    SolidBrush p = new SolidBrush(Color.Black);
                    PointF point = new PointF();
                    point.X = gameField.Width / 2 - 50;
                    point.Y = gameField.Height / 2;
                    g.DrawString("You Lose!", font, p, point);
                    timer.Stop();
                }
            }

            public async void Delay()
            {
                await Task.Delay(2000);

            }

        }

        int x, y, w, h;
        int direction, speed;
        int enemyX, enemyY, enemyDirection;
        float enemyFall, rate;
        bool hit = false, dead = false;
        int amoSize, next;
        Rectangle[] Amo;
        Enemy[] enemies;
        int[] goup;
        Bitmap shipModel, enemyModel, bulletModel;
        Font font = new Font("Arial", 16);

        public Form1()
        {
            InitializeComponent();
            enemies = new Enemy[100];
            for (int i=0; i < enemies.Length; i++)
            {
                enemies[i] = new Enemy(gameField1);
            }
            spawn();
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
                bulletModel = (Bitmap)Image.FromFile(@"C:\Users\rafal\source\repos\WindowsFormsApp1\WindowsFormsApp1\bullet.bmp", true);
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("Nie ma pliku");
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

        public async void spawn()
        {
            int delay = 10000;
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].dead = false;
                await Task.Delay(delay);
                delay -= 100;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i=0; i < enemies.Length; i++)
            {
                if (enemies[i].dead == false)
                {
                    enemies[i].x += enemies[i].direction;
                    enemies[i].fall += 0.2f;
                    enemies[i].y = (int)enemies[i].fall;
                    if (enemies[i].x >= gameField1.Width - enemyModel.Width)
                    { enemies[i].direction = -1; }
                    if (enemies[i].x <= 0)
                    { enemies[i].direction = 1; }
                }
            }
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
        int count=0;
        private void gameField1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush p = new SolidBrush(Color.Black);
            Color backColor = Color.White;
            x += direction;
            shipModel.MakeTransparent(backColor);
            g.DrawImage(shipModel, x - 7, y - shipModel.Height + 15);
            enemyModel.MakeTransparent(backColor);
            count++;
                for (int i = 0; i < enemies.Length; i++)
                {                
                    if (enemies[i].dead == false)
                    {
                    enemies[i].Collision(Amo, 10, enemyModel);
                    enemies[i].isOnBottom(shipModel, font, gameField1, e, y, timer1);
                    if (enemies[i].hit == false)
                        {
                            g.DrawImage(enemyModel, enemies[i].x, enemies[i].y);
                        }
                        else
                        {
                            g.DrawString("Trafiony", Font, p, enemies[i].x, enemies[i].y);
                            enemies[i].kill();
                    }
                    }
                }
            setAmoXY();
            AmoRefresh(g);
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
                g.DrawImage(bulletModel , Amo[i].X , Amo[i].Y);
            }
        }

    }
}
