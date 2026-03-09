using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ArkanoidGame
{
    /// <summary> Главное окно игры Арканоид, управляющее логикой отрисовки и столкновений </summary>
    public partial class ArkanoidForm : Form
    {
        /// <summary> Текущая скорость перемещения мяча по горизонтали </summary>
        public int BallSpeedX = GameSettings.BallStartSpeedX;

        /// <summary> Текущая скорость перемещения мяча по вертикали </summary>
        public int BallSpeedY = GameSettings.BallStartSpeedY;

        /// <summary> Количество урона, наносимого мячом </summary>
        public int BallDamage = 1;

        /// <summary> Список активных блоков на поле </summary>
        public List<PictureBox> Blocks = new List<PictureBox>();

        /// <summary> Список падающих бонусов </summary>
        public List<PictureBox> PowerUps = new List<PictureBox>();

        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        private Random rnd = new Random();

        public ArkanoidForm()
        {
            InitializeComponent();
            this.Text = "Arkanoid | Damage: 1";
            this.BackColor = Color.Black;
            // DoubleBuffered убрал, так как ревьювер сказал, что оно и так в дефолте

            this.MouseMove += Form1_MouseMove;
            gameTimer.Interval = GameSettings.TimerInterval;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            this.Load += (s, e) => CreateLevel();
        }

        public void CreateLevel()
        {
            for (var r = 0; r < GameSettings.GridRows; r++)
            {
                for (var c = 0; c < GameSettings.GridCols; c++)
                {
                    var hp = GameSettings.GridRows - r;
                    var block = new PictureBox
                    {
                        Size = new Size(GameSettings.BlockWidth, GameSettings.BlockHeight),
                        Left = c * (GameSettings.BlockWidth + GameSettings.BlockPadding) + GameSettings.GridOffsetX,
                        Top = r * (GameSettings.BlockHeight + GameSettings.BlockPadding) + GameSettings.GridOffsetY,
                        BorderStyle = BorderStyle.FixedSingle,
                        Tag = hp
                    };

                    UpdateBlockVisuals(block, hp);
                    Blocks.Add(block);
                    this.Controls.Add(block);
                }
            }
        }

        public void UpdateBlockVisuals(PictureBox block, int hp)
        {
            if (hp >= 5)
            {
                block.BackColor = Color.Purple;
            }
            else if (hp == 4)
            {
                block.BackColor = Color.Red;
            }
            else if (hp == 3)
            {
                block.BackColor = Color.Orange;
            }
            else if (hp == 2)
            {
                block.BackColor = Color.Yellow;
            }
            else
            {
                block.BackColor = Color.Green;
            }
        }

        public void SpawnPowerUp(Point startPos)
        {
            var bonus = new PictureBox
            {
                Size = new Size(GameSettings.BonusSize, GameSettings.BonusSize),
                BackColor = Color.Cyan,
                Left = startPos.X + (GameSettings.BlockWidth / 2) - (GameSettings.BonusSize / 2),
                Top = startPos.Y,
                Tag = GameSettings.PowerUpTag
            };

            PowerUps.Add(bonus);
            this.Controls.Add(bonus);
            bonus.BringToFront();
        }

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

            if (pbBall.Left <= 0 || pbBall.Right >= this.ClientSize.Width)
            {
                BallSpeedX = -BallSpeedX;
            }

            if (pbBall.Top <= 0)
            {
                BallSpeedY = -BallSpeedY;
            }

            if (pbBall.Bottom >= this.ClientSize.Height)
            {
                EndGame("Вы проиграли!");
            }

            if (pbBall.Bounds.IntersectsWith(pbPaddle.Bounds))
            {
                pbBall.Top = pbPaddle.Top - pbBall.Height;
                BallSpeedY = -Math.Abs(BallSpeedY);

                var offset = (pbBall.Left + pbBall.Width / 2) - (pbPaddle.Left + pbPaddle.Width / 2);
                BallSpeedX = offset / GameSettings.PaddleOffsetDivisor;

                if (BallSpeedX > GameSettings.MaxBallSpeedX) { BallSpeedX = GameSettings.MaxBallSpeedX; }
                if (BallSpeedX < -GameSettings.MaxBallSpeedX) { BallSpeedX = -GameSettings.MaxBallSpeedX; }
                if (BallSpeedY > -GameSettings.MinBallSpeedY) { BallSpeedY = -GameSettings.MinBallSpeedY; }
            }

            for (var i = PowerUps.Count - 1; i >= 0; i--)
            {
                var p = PowerUps[i];
                p.Top += GameSettings.BonusFallSpeed;

                if (p.Bounds.IntersectsWith(pbPaddle.Bounds))
                {
                    if (BallDamage < GameSettings.MaxBallDamage)
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

            for (var i = Blocks.Count - 1; i >= 0; i--)
            {
                var b = Blocks[i];
                if (pbBall.Bounds.IntersectsWith(b.Bounds))
                {
                    BallSpeedY = -BallSpeedY;
                    var hp = (int)b.Tag - BallDamage;

                    if (hp <= 0)
                    {
                        if (rnd.Next(100) < GameSettings.BonusChancePercent)
                        {
                            SpawnPowerUp(b.Location);
                        }
                        this.Controls.Remove(b);
                        Blocks.RemoveAt(i);

                        if (Blocks.Count == 0)
                        {
                            EndGame("ПОБЕДА!");
                        }
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
            var nextX = e.X - (pbPaddle.Width / 2);
            if (nextX >= 0 && nextX <= this.ClientSize.Width - pbPaddle.Width)
            {
                pbPaddle.Left = nextX;
            }
        }
    }
}