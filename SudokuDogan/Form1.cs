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
using System.IO;

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
        private Stack<int[,]> ActualStack = new Stack<int[,]>();
        private Stack<string[,]> PossibleStack = new Stack<string[,]>();

        private bool BruteForceStop = false;

        private string saveFileName = string.Empty;
        private int[,] actual = new int[10, 10];
        private string[,] possible = new string[10, 10];
        private bool HintMode;
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
                //txtActivitis shows a warning to the user
                DisplayActivity("Click File->New to start a new" + " game or File->Open to load an existing game", true);
                return;
            }
            Label cellLabel = (Label)sender;
            if (cellLabel.Tag.ToString() == "0")
            {
                //txtActivities shows that is not a valid move. 
                DisplayActivity("Selected cell is not empty", false);
                return;
            }

            int col = int.Parse(cellLabel.Name.Substring(0, 1)); 
            int row = int.Parse(cellLabel.Name.Substring(1, 1));

            //to erase a cell
            if (SelectedNumber == 0)
            {
                if (actual[col, row] == 0)
                    //it means that the cell is empty so dont do anything
                    return;
                else
                {
                    SetCell(col, row, SelectedNumber, 1);
                    DisplayActivity("Number erased at (" + col + "," + row + ")", false);
                }
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

        private void SetCell(int col, int row, int value, int erasable)
        {
            //label for locating
            Control[] lbl = this.Controls.Find(col.ToString() + row.ToString(), true);
            Label cellLabel = (Label)lbl[0];

            //saving the actual cell
            actual[col, row] = value;

            //reset the possible values
            if ((value == 0))
            {
                for (int r = 1; (r <= 9); r++)
                {
                    for (int c = 1; (c <= 9); c++)
                    {
                        if ((actual[c, r] == 0))
                        {
                            possible[c, r] = String.Empty;
                        }
                    }
                }
            }
            else
            {
                possible[col, row] = value.ToString();
            }
            
            //erasing the cell
            if (value == 0)
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
                cellLabel.Text = value.ToString();
                cellLabel.Tag = erasable;
            }
        }

        public void SetToolTip(int col, int row, string possiblevalues)
        {
            Control[] lbl = this.Controls.Find((col.ToString() + row.ToString()), true);
            toolTip1.SetToolTip(((Label)(lbl[0])), possiblevalues);
        }
        private bool IsPuzzleSolved()
        {
            //now checking the row by row 
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

            //checking each column 
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

            //checking each minigrid
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
            //here is the algorthym for the Sudoku valid move
            //bool puzzleSolved = true;
            //each row- if there is number 
            for (int r = 1; r <= 9; r++)
            {
                if (actual[col, r] == SelectedNumber)
                {
                    return false;
                }
            }
            //each coloumn- if there is the number
            for (int c = 1; c <= 9; c++)
            {
                if (actual[c, row] == SelectedNumber)
                {
                    return false;
                }
            }
            //each minigrid
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
                   break;
            }

            StartNewGame();
        }

        private void SaveGameToDisk(bool p)
        {
            if (saveFileName == string.Empty )
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "SDO files (*.sdo)|*.sdo|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = false;
                saveFileDialog1.Title = "Save an Sudoku File";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    saveFileName = saveFileDialog1.FileName;
                }
                else
                {
                    return;
                }
            }


            System.Text.StringBuilder str = new System.Text.StringBuilder();
                for (int row = 1; row <= 9; row++)
                {
                    for (int col = 1; col <= 9; col++)
                    {
                        str.Append(actual[col, row].ToString());
                        if (col == 9)
	                        {
		                       str.Append(Environment.NewLine);
	                        }
                    }
                    
                }

                try
                {
                    bool fileExists = false;
                    fileExists = System.IO.File.Exists(saveFileName);
                    if (fileExists)
                        System.IO.File.Delete(saveFileName); 
                        System.IO.File.WriteAllText(saveFileName, str.ToString(), Encoding.UTF8);
                        toolStripStatusLabel1.Text = "Puzzle saved in " + saveFileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving game. Please try again." + ex);
                }
            


        }

        private void StartNewGame()
        {
            saveFileName = string.Empty;
            txtActivities.Text = string.Empty;
            seconds = 0;
            ClearBoard();
            //GenerateNewPuzzle(1,40);

            GameStarted = true;
            timer1.Enabled = true;
            toolStripStatusLabel1.Text = "New game started";

            toolTip1.RemoveAll();
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

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((Moves.Count == 0))
            {
                return;
            }

            string str = Moves.Pop();
            RedoMoves.Push(str);
            
            SetCell(int.Parse(str), int.Parse(str), 0, 1);
            DisplayActivity(("Value removed at (" + (int.Parse(str) + ")")), false);


        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RedoMoves.Count == 0)
                return;

            string str = RedoMoves.Pop();
            Moves.Push(str);
            
            //not sure if it might work
            SetCell(int.Parse(str), int.Parse(str), int.Parse(str), 1);
            DisplayActivity(("Value reinserted at (" + int.Parse(str)+ ")"), false);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GameStarted)
            {
                DisplayActivity("Game not started yet.", true);
                return;
            }
            SaveGameToDisk(true);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GameStarted)
            {
                DisplayActivity("Game not started yet.", true);
                return;
            }
            SaveGameToDisk(false);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GameStarted)
            {
                DialogResult response = MessageBox.Show("Do you want to save current game?", "Save current game", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (response == DialogResult.Yes)
                {
                    SaveGameToDisk(false);
                }
                else if (response == DialogResult.Cancel)
                {
                    return;
                }
            }
            
            string[] fileContents;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "SDO files (*.sdo)|*.sdo|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = false;


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileContents = System.IO.File.ReadAllLines(openFileDialog1.FileName);
                toolStripStatusLabel1.Text = openFileDialog1.FileName;
                saveFileName = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            
            StartNewGame();

           

            //starting the board
            int counter;
            
            
            for (int row = 1; (row <= 9); row++)
            {
                counter = 0;
                for (int col = 1; (col <= 9); col++)
                {
                  
                    try
                    {
                        if (int.Parse(fileContents[row-1][counter].ToString()) != 0 )
                            
                        {
                            //erasable is 1- but in normal way it should be 0 so user cannot change the numbers.
                            SetCell(col, row, int.Parse(fileContents[row-1][counter].ToString()), 0);
                            
                        }
                    }
                    catch
                    {
                        MessageBox.Show("File does not contain a valid Sudoku text");
                        return;
                    }
                    counter ++;
                    
                }
            }
        }

        private string CalculatePossibleValues(int col,  int row) 
            {
       
            string str;
            if ((possible[col, row] == String.Empty)) {
                str = "123456789";
            }
            else {
                str = possible[col, row];
            }
            int r;
            int c;
            // checking by column 
            for (r = 1; (r <= 9); r++) {
                if ((actual[col, r] != 0)) {
                
                    str = str.Replace(actual[col, r].ToString(), String.Empty);
                }
            }
            // checking by row 
            for (c = 1; (c <= 9); c++) 
            {
                if ((actual[c, row] != 0)) {
               
                    str = str.Replace(actual[c, row].ToString(), String.Empty);
                }
            }
            // check within the minigrid 
            int startC;
            int startR;
            startC = (col - ((col - 1) % 3));
            startR = (row - ((row - 1) % 3));
            for (int rr = startR; (rr <= (startR + 2)); rr++) 
            {
                for (int cc = startC; (cc <= (startC + 2)); cc++) 
                {
                    if ((actual[cc, rr] != 0)) 
                    {
                        str = str.Replace(actual[cc, rr].ToString(), String.Empty);
                    }
                }
            }
            // if there is not possible value, it means that invalid move
            if ((str == String.Empty)) 
            {
            throw new Exception("Invalid Move");
            }
            return str;
      

        }

        public bool CheckColumnsAndRows() 
        {
            bool changes = false;
            for (int row = 1; (row <= 9); row++) 
            {
                for (int col = 1; (col <= 9); col++)
                {
                    if ((actual[col, row] == 0))
                    {
                        try
                        {
                            possible[col, row] = CalculatePossibleValues(col, row);
                        }
                        catch (Exception ex)
                        {
                            DisplayActivity("Invalid placement, please undo move", false);
                            throw new Exception("Invalid Move");
                        }
                        SetToolTip(col, row, possible[col, row]);
                        if ((possible[col, row].Length == 1))
                        {
                            SetCell(col, row, int.Parse(possible[col, row]), 1);
                            actual[col, row] = int.Parse(possible[col, row]);
                            DisplayActivity("Col/Row and Minigrid Elimination", false);
                            DisplayActivity("=========================", false);
                            DisplayActivity(("Inserted value " + (actual[col, row] + (" in " + ("(" + (col + ("," + (row + ")"))))))), false);
                            Application.DoEvents();
                            Moves.Push((col + (row + possible[col, row])));
                            changes = true;
                            if (HintMode)
                            {
                                return true;
                            }
                        }
                        
                    }
                }
                
            }
            return changes;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GameStarted)
            {
                DialogResult response = MessageBox.Show("Do you want to save current game?", "Save current game", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (response == DialogResult.Yes)
                {
                    SaveGameToDisk(false);
                }
                else if (response == DialogResult.Cancel)
                {
                    return;
                }
            }
            Application.Exit();
        }

        private string GenerateNewPuzzle(int level, int score)
        {
            int c;
            int r;
            string str;
            int numberofemptycells;
            for (r = 1; (r <= 9); r++)
            {
                for (c = 1; (c <= 9); c++)
                {
                    actual[c, r] = 0;
                    possible[c, r] = String.Empty;
                }
            }
            ActualStack.Clear();
            PossibleStack.Clear();
            //try
            //{
            //    if (!SolvePuzzle())
            //    {
            //        SolvePuzzleByBruteForce();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return String.Empty;
            //}

            //private int[,] actual_backup = actual.Clone();
            //actualBackup = actual.Clone();
            //Random random = new Random();
            //switch (levelToolStripMenuItem.Tag)
            //{
            //    case levelToolStripMenuItem.:
            //        numberofemptycells = RandomNumber(40, 45);
            //        break;
            //    case 2:
            //        numberofemptycells = RandomNumber(46, 49);
            //        break;
            //    case 3:
            //        numberofemptycells = RandomNumber(50, 53);
            //        break;
            //    case 4:
            //        numberofemptycells = RandomNumber(54, 58);
            //        break;
            //}
            ActualStack.Clear();
            PossibleStack.Clear();
            BruteForceStop = false;
            //CreateEmptyCells(numberofemptycells);
            str = String.Empty;
            for (r = 1; (r <= 9); r++)
            {
                for (c = 1; (c <= 9); c++)
                {
                    actual[c, r].ToString();
                }
            }
            //int tries = 0;
            for (
            ; true;
            )
            //{
            //    totalscore = 0;
            //    try
            //    {
            //        if (!SolvePuzzle())
            //        {
            //            if ((level < 4))
            //            {
            //                VacateAnotherPairOfCells(str);
            //                tries++;
            //            }
            //            else
            //            {
            //                SolvePuzzleByBruteForce();
            //            }
            //            break; 
            //        }
            //        break; 
            //    }
            //    catch (Exception ex)
            //    {
            //        return String.Empty;
            //    }
            //    if ((tries > 50))
            //    {
            //        return String.Empty;
            //    }
            //}
            //score = totalscore;
            return str;
        }

        private int RandomNumber(int p1, int p2)
        {
            throw new NotImplementedException();
        }

        //private void RandomizeThePossibleValues(string str)
        //{
        //    char[,] s;
        //    int i;
        //    int j;
        //    char temp;
        //    Randomize();
        //    s = str.ToCharArray;
        //    for (i = 0; (i <= (str.Length - 1)); i++)
        //    {
        //        j = (int.Parse(((((str.Length - i) + 1) * Rnd()) + i)) % str.Length);
        //        temp = s[i];
        //        s[i] = s[j];
        //        s[j] = temp;
        //    }
        //    str = s;
        //}

    //    private void CreateEmptyCells(int numberofemptycells)
    //    {
    //        //private void CreateEmptyCells(int empty) {
    //        int c;
    //        int r;
    //        string[] emptyCells = new string[numberofemptycells-1];
    //        for (int i = 0; (i <= numberofemptycells / 2); i++) 
    //        { 
    //            bool duplicate = false;   
                
    //            while ((r == 5) && (c > 5))
    //             {
                    
    //                c = RandomNumber(1, 9);
    //                r = RandomNumber(1, 5);
                    
    //            //for (int j = 0; (j <= i); j++) 
    //            //{
    //            //    if (((emptyCells[j] == c.ToString()) + r.ToString())) 
    //            //    {
    //            //        duplicate = true;
    //            //        break;
    //            //    }
    //            //}
    //            if (!duplicate) 
    //            {
    //                emptyCells[i] = (c.ToString() + r.ToString());
    //                actual[c, r] = 0;
    //                possible[c, r] = String.Empty;
    //                emptyCells[(numberofemptycells - (1 - i))] = (((10 - c)).ToString() + ((10 - r)).ToString());
    //                actual[(10 - c), (10 - r)] = 0;
    //                possible[(10 - c), (10 - r)] = String.Empty;
    //            }
    //            }
    //    }
    //}


        private void btnSolvePuzzle_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("It will be show the possible solved sudoku!");
            //return;

            HintMode = false;
            try
            {
                SolvePuzzle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please undo your move", "Invalid Move", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHint_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Possible numbers in each cell will be shown");

            HintMode = true;
            try
            {
                SolvePuzzle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please undo your move", "Invalid Move", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
       
        public bool SolvePuzzle() 
        {
            bool changes;
            bool ExitLoop = false;


            try
            {
                do
                {
                    changes = CheckColumnsAndRows();
                    if ((HintMode && changes) || IsPuzzleSolved())
                    {
                        ExitLoop = true;
                        //SolvePuzzleByBruteForce();
                        break; 
                    }
                } while (!(!changes));
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid Move");
            }

        if (IsPuzzleSolved()) {
            timer1.Enabled = false;
            SystemSounds.Beep.Play();
            toolStripStatusLabel1.Text = "*****Puzzle Solved*****";
            MessageBox.Show("Puzzle solved");
            return true;
        }
        else {
            return false;
        }
    }

        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mediumToolStripMenuItem.Checked = false;
            hardToolStripMenuItem.Checked = false;
            samuraiToolStripMenuItem.Checked = false;
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            easyToolStripMenuItem.Checked = false;
            hardToolStripMenuItem.Checked = false;
            samuraiToolStripMenuItem.Checked = false;
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            easyToolStripMenuItem.Checked = false;
            mediumToolStripMenuItem.Checked = false;
            samuraiToolStripMenuItem.Checked = false;
        }

        private void samuraiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            easyToolStripMenuItem.Checked = false;
            mediumToolStripMenuItem.Checked = false;
            hardToolStripMenuItem.Checked = false;
        }

        //private void SolvePuzzleByBruteForce()
        //{
        //    int col;
        //    int row;
        //    string possibleValues;
        //    //FindCellWithFewestPossibleValues(c, r);

         
        //    int min = 10;
        //    for (int r = 1; (r <= 9); r++)
        //    {
        //        for (int c = 1; (c <= 9); c++)
        //        {
        //            if (((actual[c, r] == 0) && (possible[c, r].Length < min)))
        //            {
        //                min = possible[c, r].Length;
                        
        //                col = c;
        //                row = r;
        //            }
        //        }
        //    }
        //    possibleValues = possible[col, row];

        //    //string possibleValues = possible[col, row];
        //    ActualStack.Push(((int[,])(actual.Clone())));
        //    PossibleStack.Push(((string[,])(possible.Clone())));
        //    for (int i = 0; (i <= (possibleValues.Length - 1)); i++)
        //    {
        //        Moves.Push((col + (row + possibleValues.ToString())));

        //        SetCell(col, row, int.Parse(possibleValues.ToString()), 1);
        //        DisplayActivity("Solve Puzzle By Brute Force", false);
        //        DisplayActivity("===========================", false);
        //        DisplayActivity(("Trying to insert value " + (actual[col, row] + (" in " + ("(" + (col + ("," + (row + ")"))))))), false);

        //        try
        //        {
        //            if (SolvePuzzle())
        //            {
        //                BruteForceStop = true;
        //                return;
        //            }
        //            else
        //            {
        //                SolvePuzzleByBruteForce();
        //                if (BruteForceStop)
        //                    return;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            DisplayActivity("Invalid move; Backtracking...", false);
        //            actual = ActualStack.Pop();
        //            possible = PossibleStack.Pop();
        //        }
        //    }
        //}

        //public void FindCellWithFewestPossibleValues(int col, int row)
        //{
        //    int min = 10;
        //    for (int r = 1; (r <= 9); r++)
        //    {
        //        for (int c = 1; (c <= 9); c++)
        //        {
        //            if (((actual[c, r] == 0) && (possible[c, r].Length < min)))
        //            {
        //                min = possible[c, r].Length;
        //                col = c;
        //                row = r;
        //            }
        //        }
        //    }
        //}

    }
}
