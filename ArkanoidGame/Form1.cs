using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ArkanoidGame
{
    public partial class Form1 : Form
    {
        private const int BlockWidth = 70;
        private const int BlockHeight = 30;
        private const int BlockPadding = 5;
        private const int GridRows = 5;
        private const int GridCols = 10;

        private const int GridOffsetX = 40;
        private const int GridOffsetY = 60;
        private const int PaddleOffsetDivisor = 7;
        private const string PowerUpTag = "buff";

        private const int BallStartSpeedX = 4;
        private const int BallStartSpeedY = -4;
        private const int MaxBallSpeedX = 7;
        private const int MinBallSpeedY = 3;
        private const int BonusSize = 15;
        private const int BonusFallSpeed = 4;
        private const int BonusChancePercent = 20;
        private const int MaxBallDamage = 3;
        private const int TimerInterval = 20;

        /// <summary> Текущая скорость перемещения мяча по горизонтали </summary>
        public int BallSpeedX = BallStartSpeedX;

        /// <summary> Текущая скорость перемещения мяча по вертикали </summary>
        public int BallSpeedY = BallStartSpeedY;

        /// <summary> Количество урона, наносимого мячом за одно столкновение </summary>
        public int BallDamage = 1;

        /// <summary> Коллекция всех активных кирпичей на игровом поле </summary>
        public List<PictureBox> Blocks = new List<PictureBox>();

        /// <summary> Коллекция активных бонусных объектов, падающих вниз </summary>
        public List<PictureBox> PowerUps = new List<PictureBox>();

        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        private Random rnd = new Random();

        /// <summary> Инициализирует новый экземпляр формы и запускает игровые механизмы </summary>
        public Form1()
        {
            InitializeComponent();

            this.Text = "Arkanoid | Damage: 1";
            this.BackColor = Color.Black;
            this.DoubleBuffered = false;

            this.MouseMove += Form1_MouseMove;
            gameTimer.Interval = TimerInterval;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            this.Load += (s, e) => CreateLevel();
        }

        /// <summary> Выполняет генерацию сетки блоков и их размещение на форме </summary>
        public void CreateLevel()
        {
            for (int r = 0; r < GridRows; r++)
            {
                for (int c = 0; c < GridCols; c++)
                {
                    int hp = GridRows - r;

                    PictureBox block = new PictureBox
                    {
                        Size = new Size(BlockWidth, BlockHeight),
                        Left = c * (BlockWidth + BlockPadding) + GridOffsetX,
                        Top = r * (BlockHeight + BlockPadding) + GridOffsetY,
                        BorderStyle = BorderStyle.FixedSingle,
                        Tag = hp
                    };

                    UpdateBlockVisuals(block, hp);
                    Blocks.Add(block);
                    this.Controls.Add(block);
                }
            }
        }

        /// <summary> Устанавливает цвет фона блока в соответствии с его уровнем здоровья </summary>
        public void UpdateBlockVisuals(PictureBox block, int hp)
        {
            if (hp >= 5) block.BackColor = Color.Purple;
            else if (hp == 4) block.BackColor = Color.Red;
            else if (hp == 3) block.BackColor = Color.Orange;
            else if (hp == 2) block.BackColor = Color.Yellow;
            else block.BackColor = Color.Green;
        }

        /// <summary> Создает визуальный объект бонуса в месте уничтожения блока </summary>
        public void SpawnPowerUp(Point startPos)
        {
            PictureBox bonus = new PictureBox
            {
                Size = new Size(BonusSize, BonusSize),
                BackColor = Color.Cyan,
                Left = startPos.X + (BlockWidth / 2) - (BonusSize / 2),
                Top = startPos.Y,
                Tag = PowerUpTag
            };

            PowerUps.Add(bonus);
            this.Controls.Add(bonus);
            bonus.BringToFront();
        }

        /// <summary> Прекращает игровой процесс и уведомляет пользователя о результате </summary>
        public void EndGame(string msg)
        {
            gameTimer.Stop();
            MessageBox.Show(msg, "Результат игры");
            Application.Restart();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            pbBall.Left += BallSpeedX;
            pbBall.Top += BallSpeedY;

            if (pbBall.Left <= 0 || pbBall.Right >= this.ClientSize.Width) BallSpeedX = -BallSpeedX;
            if (pbBall.Top <= 0) BallSpeedY = -BallSpeedY;

            if (pbBall.Bottom >= this.ClientSize.Height)
            {
                EndGame("Вы проиграли!");
            }

            if (pbBall.Bounds.IntersectsWith(pbPaddle.Bounds))
            {
                pbBall.Top = pbPaddle.Top - pbBall.Height;
                BallSpeedY = -Math.Abs(BallSpeedY);

                int offset = (pbBall.Left + pbBall.Width / 2) - (pbPaddle.Left + pbPaddle.Width / 2);
                BallSpeedX = offset / PaddleOffsetDivisor;

                if (BallSpeedX > MaxBallSpeedX) BallSpeedX = MaxBallSpeedX;
                if (BallSpeedX < -MaxBallSpeedX) BallSpeedX = -MaxBallSpeedX;
                if (BallSpeedY > -MinBallSpeedY) BallSpeedY = -MinBallSpeedY;
            }

            for (int i = PowerUps.Count - 1; i >= 0; i--)
            {
                var p = PowerUps[i];
                p.Top += BonusFallSpeed;

                if (p.Bounds.IntersectsWith(pbPaddle.Bounds))
                {
                    if (BallDamage < MaxBallDamage)
                    {
                        BallDamage++;
                        this.Text = $"Arkanoid | Damage: {BallDamage}";
                    }
                    RemovePowerUp(i);
                }
                else if (p.Top > this.ClientSize.Height)
                {
                    RemovePowerUp(i);
                }
            }

            for (int i = Blocks.Count - 1; i >= 0; i--)
            {
                var b = Blocks[i];
                if (pbBall.Bounds.IntersectsWith(b.Bounds))
                {
                    BallSpeedY = -BallSpeedY;
                    int hp = (int)b.Tag - BallDamage;

                    if (hp <= 0)
                    {
                        if (rnd.Next(100) < BonusChancePercent) SpawnPowerUp(b.Location);
                        this.Controls.Remove(b);
                        Blocks.RemoveAt(i);

                        if (Blocks.Count == 0) EndGame("ПОБЕДА!");
                    }
                    else
                    {
                        b.Tag = hp;
                        UpdateBlockVisuals(b, hp);
                    }
                    break;
                }
            }
        }

        private void RemovePowerUp(int index)
        {
            this.Controls.Remove(PowerUps[index]);
            PowerUps.RemoveAt(index);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            int nextX = e.X - (pbPaddle.Width / 2);
            if (nextX >= 0 && nextX <= this.ClientSize.Width - pbPaddle.Width)
            {
                pbPaddle.Left = nextX;
            }
        }
    }
}