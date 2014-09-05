using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace SudokuDogan
{
    public partial class Form1 : Form
    {

        const int CellWidth = 32;

        const int cellHeight = 32;
        const int xOffset = -20;
        const int yOffset = 25;

        private Color DEFAULT_BACKCOLOR = Color.White;

        private Color FIXED_FORECOLOR = Color.Blue;
        private Color FIXED_BACKCOLOR = Color.LightSteelBlue;

        private Color USER_FORECOLOR = Color.Black;

        private Color USER_BACKCOLOR = Color.LightYellow;
        private int SelectedNumber;

        private Stack<string> Moves;
        private Stack<string> RedoMoves;

        private string saveFileName = string.Empty;


        private int[,] actual = new int[10, 10];
        private int seconds = 0;

        private bool GameStarted = false;
        public Form1()
        {
            InitializeComponent();
        }

        public void DrawBoard()
        {
            toolStripButton1.Checked = true;
            SelectedNumber = 1;
            Point location = new Point();

            for (int row = 1; row <= 9; row++)
            {
                for (int col = 1; col <= 9; col++)
                {
                    location.X = col * (CellWidth + 1) + xOffset;
                    location.Y = row * (cellHeight + 1) + yOffset;
                    Label lbl = new Label();

                    var _with1 = lbl;
                    _with1.Name = col.ToString() + row.ToString();
                    _with1.BorderStyle = BorderStyle.Fixed3D;
                    _with1.Location = location;
                    _with1.Width = CellWidth;
                    _with1.Height = cellHeight;
                    _with1.TextAlign = ContentAlignment.MiddleCenter;
                    _with1.BackColor = DEFAULT_BACKCOLOR;
                    _with1.Font = new Font(_with1.Font, _with1.Font.Style | FontStyle.Bold);
                    _with1.Tag = "1";
                    lbl.Click += Cell_Click;
                    this.Controls.Add(lbl);
                }
            }
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            if (!GameStarted)
            {
                DisplayActivity("Click File->New to start a new" + " game or File->Open to load an existing game", true);
                return;
            }
            Label cellLabel = (Label)sender;
            if (cellLabel.Tag.ToString() == "0")
            {
                DisplayActivity("Selected cell is not empty", false);
                return;
            }

            int col = int.Parse(cellLabel.Name.Substring(0, 1)); 
            int row = int.Parse(cellLabel.Name.Substring(1, 1));

            if (SelectedNumber == 0)
            {
                if (actual[col, row] == 0)
                    return;

                SetCell(col, row, SelectedNumber, 1);
                DisplayActivity("Number erased at (" + col + "," + row + ")", false);
            }
            else if (cellLabel.Text == string.Empty)
            {
                if (!IsMoveValid(col, row, SelectedNumber))
                {
                   DisplayActivity("Invalid move at (" + col + "," + row + ")", false);
                    return;
                }

                SetCell(col, row, SelectedNumber, 1);
                DisplayActivity("Number placed at (" + col + "," + row + ")", false);
                Moves.Push(cellLabel.Name.ToString() + SelectedNumber);
                if (IsPuzzleSolved())
                {
                    timer1.Enabled = false;
                    //Interaction.Beep();
                    SystemSounds.Beep.Play();
                    toolStripStatusLabel1.Text = "*****Puzzle Solved*****";
                }
            }
        }

        private void DisplayActivity(string p1, bool p2)
        {
            if (p2)
            {
                SystemSounds.Beep.Play();
                
            }
            txtActivities.Text += p1 + Environment.NewLine;
        }

        private void SetCell(int col, int row, int SelectedNumber, int erasable)
        {
            Control[] lbl = this.Controls.Find(col.ToString() + row.ToString(), true);
            Label cellLabel = (Label)lbl[0];

            actual[col, row] = SelectedNumber;
            if (SelectedNumber == 0)
            {
                cellLabel.Text = string.Empty;
                cellLabel.Tag = erasable;
                cellLabel.BackColor = DEFAULT_BACKCOLOR;

            }
            else
            {
                if (erasable == 0)
                {
                    cellLabel.BackColor = FIXED_BACKCOLOR;
                    cellLabel.ForeColor = FIXED_FORECOLOR;
                }
                else
                {
                    cellLabel.BackColor = USER_BACKCOLOR;
                    cellLabel.ForeColor = USER_FORECOLOR;
                }
                cellLabel.Text = SelectedNumber.ToString();
                cellLabel.Tag = erasable;
            }
        }

        private bool IsPuzzleSolved()
        {
            string pattern = null;
            int r = 0;
            int c = 0;
            for (r = 1; r <= 9; r++)
            {
                pattern = "123456789";
                for (c = 1; c <= 9; c++)
                {
                    pattern = pattern.Replace(actual[c, r].ToString(), string.Empty);
                }
                if (pattern.Length > 0)
                {
                    return false;
                }
            }

            for (c = 1; c <= 9; c++)
            {
                pattern = "123456789";
                for (r = 1; r <= 9; r++)
                {
                    pattern = pattern.Replace(actual[c, r].ToString(), string.Empty);
                }
                if (pattern.Length > 0)
                {
                    return false;
                }
            }

            for (c = 1; c <= 9; c += 3)
            {
                pattern = "123456789";
                for (r = 1; r <= 9; r += 3)
                {
                    for (int cc = 0; cc <= 2; cc++)
                    {
                        for (int rr = 0; rr <= 2; rr++)
                        {
                            pattern = pattern.Replace(actual[c + cc, r + rr].ToString(), string.Empty);

                        }
                    }
                }
                if (pattern.Length > 0)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsMoveValid(int col, int row, int SelectedNumber)
        {
            bool puzzleSolved = true;
            for (int r = 1; r <= 9; r++)
            {
                if (actual[col, r] == SelectedNumber)
                {
                    return false;
                }
            }
            for (int c = 1; c <= 9; c++)
            {
                if (actual[c, row] == SelectedNumber)
                {
                    return false;
                }
            }

            int startC = 0;
            int startR = 0;
            startC = col - ((col - 1) % 3);
            startR = row - ((row - 1) % 3);
            for (int rr = 0; rr <= 2; rr++)
            {
                for (int cc = 0; cc <= 2; cc++)
                {
                    if (actual[startC + cc, startR + rr] == SelectedNumber)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = string.Empty;
            toolStripStatusLabel2.Text = string.Empty;
            DrawBoard();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            int x1 = 0;
            int y1 = 0;
            int x2 = 0;
            int y2 = 0;
            x1 = 1 * (CellWidth + 1) + xOffset - 1;
            x2 = 9 * (CellWidth + 1) + xOffset + CellWidth;
            for (int r = 1; r <= 10; r += 3)
            {
                y1 = r * (cellHeight + 1) + yOffset - 1;
                y2 = y1;
                e.Graphics.DrawLine(Pens.Black, x1, y1, x2, y2);
            }
            y1 = 1 * (cellHeight + 1) + yOffset - 1;
            y2 = 9 * (cellHeight + 1) + yOffset + cellHeight;
            for (int c = 1; c <= 10; c += 3)
            {
                x1 = c * (CellWidth + 1) + xOffset - 1;
                x2 = x1;
                e.Graphics.DrawLine(Pens.Black, x1, y1, x2, y2);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SelectedNumber = 1;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 1" + Environment.NewLine;
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SelectedNumber = 2;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 2" + Environment.NewLine;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SelectedNumber = 3;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 3" + Environment.NewLine;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SelectedNumber = 4;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 4" + Environment.NewLine;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            SelectedNumber = 5;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 5" + Environment.NewLine;
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            SelectedNumber = 6;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 6" + Environment.NewLine;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            SelectedNumber = 7;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 7" + Environment.NewLine;
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            SelectedNumber = 8;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 8" + Environment.NewLine;
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            SelectedNumber = 9;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 9" + Environment.NewLine;
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            SelectedNumber = 0;
            txtActivities.Text = string.Empty;
            txtActivities.Text = "Selected number has been erased." + Environment.NewLine;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            DialogResult result= MessageBox.Show("Do you want to save current game?", "Save current game", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            switch (result)
            {
                case DialogResult.Yes: SaveGameToDisk(false);
                    break;
                case DialogResult.No: MessageBox.Show("You have chosen not to save the current game!");
                    return;
            }

            StartNewGame();
        }

        private void SaveGameToDisk(bool p)
        {
            MessageBox.Show("The game is saved!");
            return;
        }

        private void StartNewGame()
        {
            saveFileName = string.Empty;
            txtActivities.Text = string.Empty;
            seconds = 0;
            ClearBoard();

            GameStarted = true;
            timer1.Enabled = true;
            toolStripStatusLabel1.Text = "New game started";
        }

        private void ClearBoard()
        {
            Moves = new Stack<string>();
            RedoMoves = new Stack<string>();
            for (int row = 1; row <= 9; row++)
            {
                for (int col = 1; col <= 9; col++)
                {
                    SetCell(col, row, 0, 1);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text = "Elapsed time: " + seconds + " second(s)";
            seconds += 1;
        }
    }
}
