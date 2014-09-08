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

        //drawing the board
        const int cell_Width = 32;
        const int cell_Height = 32;
        const int x_Offset = -20;
        const int y_Offset = 25;
        private Color deffault_backcolor = Color.White;
        private Color fixed_forecolor = Color.Blue;
        private Color fixed_backcolor = Color.LightSteelBlue;
        private Color user_forecolor = Color.Black;
        private Color user_backcolor = Color.LightYellow;
        ///////////////////////////////////////////////////
 
        private int selected_Number;
        private Stack<string> moves;
        private Stack<string> redoMoves;
        private Stack<int[,]> actual_stack = new Stack<int[,]>();
        private Stack<string[,]> possible_stack = new Stack<string[,]>();

        private bool brute_forcestop = false;

        private string saveFileName = string.Empty;
        private int[,] actual = new int[10, 10];
        private string[,] possible = new string[10, 10];
        private bool hint_mode;
        private int seconds = 0;
        //
        private int cPos;
        private int rPos;
        private bool changes;
        private bool game_started = false;

        public Form1()
        {
            InitializeComponent();
            
        }

        public void DrawBoard()
        {
            toolStripButton1.Checked = true;
            selected_Number = 1;
            Point location = new Point();

            for (int row = 1; row <= 9; row++)
            {
                for (int col = 1; col <= 9; col++)
                {
                    location.X = col * (cell_Width + 1) + x_Offset;
                    location.Y = row * (cell_Height + 1) + y_Offset;
                    Label lbl = new Label();

                    var _with1 = lbl;
                    _with1.Name = col.ToString() + row.ToString();
                    _with1.BorderStyle = BorderStyle.Fixed3D;
                    _with1.Location = location;
                    _with1.Width = cell_Width;
                    _with1.Height = cell_Height;
                    _with1.TextAlign = ContentAlignment.MiddleCenter;
                    _with1.BackColor = deffault_backcolor;
                    _with1.Font = new Font(_with1.Font, _with1.Font.Style | FontStyle.Bold);
                    _with1.Tag = "1";
                    lbl.Click += Cell_Click;
                    this.Controls.Add(lbl);
                }
            }
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            if (!game_started)
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
            if (selected_Number == 0)
            {
                if (actual[col, row] == 0)
                    //it means that the cell is empty so dont do anything
                    return;
                else
                {
                    SetCell(col, row, selected_Number, 1);
                    DisplayActivity("Number erased at (" + col + "," + row + ")", false);
                }
            }
            else if (cellLabel.Text == string.Empty)
            {
                if (!IsMoveValid(col, row, selected_Number))
                {
                   DisplayActivity("Invalid move at (" + col + "," + row + ")", false);
                    return;
                }

                SetCell(col, row, selected_Number, 1);
                DisplayActivity("Number placed at (" + col + "," + row + ")", false);
                moves.Push(cellLabel.Name.ToString() + selected_Number);
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
                cellLabel.BackColor = deffault_backcolor;

            }
            else
            {
                if (erasable == 0)
                {
                    cellLabel.BackColor = fixed_backcolor;
                    cellLabel.ForeColor = fixed_forecolor;
                }
                else
                {
                    cellLabel.BackColor = user_backcolor;
                    cellLabel.ForeColor = user_forecolor;
                }
                cellLabel.Text = value.ToString();
                cellLabel.Tag = erasable;
            }
        }

        public void SetToolTip(int col, int row, string possiblevalues)
        {
            //find the cursor and cell
            Control[] lbl = this.Controls.Find((col.ToString() + row.ToString()), true);
            //give the possible values
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
            x1 = 1 * (cell_Width + 1) + x_Offset - 1;
            x2 = 9 * (cell_Width + 1) + x_Offset + cell_Width;
            for (int r = 1; r <= 10; r += 3)
            {
                y1 = r * (cell_Height + 1) + y_Offset - 1;
                y2 = y1;
                e.Graphics.DrawLine(Pens.Black, x1, y1, x2, y2);
            }
            y1 = 1 * (cell_Height + 1) + y_Offset - 1;
            y2 = 9 * (cell_Height + 1) + y_Offset + cell_Height;
            for (int c = 1; c <= 10; c += 3)
            {
                x1 = c * (cell_Width + 1) + x_Offset - 1;
                x2 = x1;
                e.Graphics.DrawLine(Pens.Black, x1, y1, x2, y2);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            selected_Number = 1;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 1" + Environment.NewLine;
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            selected_Number = 2;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 2" + Environment.NewLine;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            selected_Number = 3;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 3" + Environment.NewLine;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            selected_Number = 4;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 4" + Environment.NewLine;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            selected_Number = 5;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 5" + Environment.NewLine;
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            selected_Number = 6;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 6" + Environment.NewLine;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            selected_Number = 7;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 7" + Environment.NewLine;
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            selected_Number = 8;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 8" + Environment.NewLine;
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            selected_Number = 9;
            //txtActivities.Text = string.Empty;
            txtActivities.Text += "Selected number = 9" + Environment.NewLine;
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            selected_Number = 0;
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
            
            //System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            //toolStripStatusLabel1.Text = "Generating new puzzle...";
            //SudokuPuzzle sp = new SudokuPuzzle();
            //string puzzle = String.Empty;
            //if (easyToolStripMenuItem.Checked)
            //{
            //    puzzle = sp.GetNewPuzzle(1);
            //}
            //else if (mediumToolStripMenuItem.Checked)
            //{
            //    puzzle = sp.GetNewPuzzle(2);
            //}
            //else if (hardToolStripMenuItem.Checked)
            //{
            //    puzzle = sp.GetNewPuzzle(3);
            //}
            //else if (samuraiToolStripMenuItem.Checked)
            //{
            //    puzzle = sp.GetNewPuzzle(4);
            //}
            //System.Windows.Forms.Cursor.Current = Cursors.Default;
            
            //StartNewGame();
            //int counter = 0;
            //for (int row = 1; (row <= 9); row++)
            //{
            //    for (int col = 1; (col <= 9); col++)
            //    {
            //        if ((puzzle(counter).ToString() != "0"))
            //        {
            //            SetCell(col, row, int.Parse(puzzle(counter).ToString()), 0);
            //        }
            //        counter++;
            //    }
            //}
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

            game_started = true;
            timer1.Enabled = true;
            toolStripStatusLabel1.Text = "New game started";

            toolTip1.RemoveAll();
        }

        private void ClearBoard()
        {
            moves = new Stack<string>();
            redoMoves = new Stack<string>();
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
            //if this is the first move or not- so do nothing
            if ((moves.Count == 0))
            {
                return;
            }


            string str = moves.Pop();
            redoMoves.Push(str);
            
            SetCell(int.Parse(str[0].ToString()), int.Parse(str[1].ToString()), 0, 1);
            DisplayActivity(("Value removed at (" + (int.Parse(str[0].ToString()) + "," + int.Parse(str[1].ToString()) + ")")), false);


        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (redoMoves.Count == 0)
                return;

            string str = redoMoves.Pop();
            moves.Push(str);
            
            
            SetCell(int.Parse(str[0].ToString()), int.Parse(str[1].ToString()), int.Parse(str[2].ToString()), 1);
            DisplayActivity(("Value reinserted at (" + int.Parse(str[0].ToString())+ "," + int.Parse(str[1].ToString()) + ")"), false);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!game_started)
            {
                DisplayActivity("Game not started yet.", true);
                return;
            }
            SaveGameToDisk(true);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!game_started)
            {
                DisplayActivity("Game not started yet.", true);
                return;
            }
            SaveGameToDisk(false);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (game_started)
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
                        //trying to replace all non numbers to 0
                        fileContents[row - 1] = System.Text.RegularExpressions.Regex.Replace(fileContents[row - 1].ToString(), "[^0-9.]", "0");
                        
                        if (int.Parse(fileContents[row - 1][counter].ToString()) != 0)
                        {
                           //erasable is 1- but in normal way it should be 0 so user cannot change the numbers.
                           SetCell(col, row, int.Parse(fileContents[row - 1][counter].ToString()), 0);
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
            if ((possible[col, row] == String.Empty)) 
            {
                str = "123456789";
            }
            else 
            {
                str = possible[col, row];
            }
            
            int r;
            int c;
            // checking by column 
            for (r = 1; (r <= 9); r++) {
                if ((actual[col, r] != 0)) {
                    //cell is not empty
                    str = str.Replace(actual[col, r].ToString(), String.Empty);
                }
            }
            // checking by row 
            for (c = 1; (c <= 9); c++) 
            {
                if ((actual[c, row] != 0)) 
                {
                    //cell is not empty
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
                        //displaying the possible number
                        SetToolTip(col, row, possible[col, row]);
                        if ((possible[col, row].Length == 1))
                        {
                            SetCell(col, row, int.Parse(possible[col, row]), 1);
                            actual[col, row] = int.Parse(possible[col, row]);
                            DisplayActivity("Col/Row and Minigrid Elimination", false);
                            DisplayActivity("=========================", false);
                            DisplayActivity(("Inserted value " + (actual[col, row] + (" in " + ("(" + (col + ("," + (row + ")"))))))), false);
                            
                            //refresh the application
                            Application.DoEvents();

                            moves.Push((col + (row + possible[col, row])));
                            changes = true;
                            if (hint_mode)
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
            if (game_started)
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
            actual_stack.Clear();
            possible_stack.Clear();
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
            actual_stack.Clear();
            possible_stack.Clear();
            brute_forcestop = false;
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

        private void btnSolvePuzzle_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("It will be show the possible solved sudoku!");
            //return;

            hint_mode = false;
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

            hint_mode = true;
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
            //bool changes;
            bool ExitLoop = false;
            try
            {
                while (!changes)
                {
                    while (!changes)
                    {
                        while (!changes)
                        {
                            while (!changes)
                            {
                                changes = CheckColumnsAndRows();
                                if ((hint_mode && changes) || IsPuzzleSolved())
                                {
                                    ExitLoop = true;
                                    break;
                                }
                            }
                            if (ExitLoop)
                            {
                                break;
                                changes = LookForLoneRangersinMinigrids();
                                if (((hint_mode && changes) || IsPuzzleSolved()))
                                {
                                    ExitLoop = true;
                                    break;
                                }
                            }
                        }


                        if (ExitLoop)
                        {
                            break;
                        }
                        changes = LookForLoneRangersinRows();
                        if (((hint_mode && changes) || IsPuzzleSolved()))
                        {
                            ExitLoop = true;
                            break;
                        }
                    }

                    if (ExitLoop)
                    {
                        break;
                    }
                    changes = LookForLoneRangersinColumns();
                    if (((hint_mode && changes) || IsPuzzleSolved()))
                    {
                        ExitLoop = true;
                        break;
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception("Invalid Move");
            }
                if (IsPuzzleSolved())
                            {
                                timer1.Enabled = false;
                                SystemSounds.Beep.Play();
                                toolStripStatusLabel1.Text = "*****Puzzle Solved*****";
                                MessageBox.Show("Puzzle solved");
                                return true;
                            }
                            else
                            {
                                return false;
                            }
            }
           
        public bool LookForLoneRangersinMinigrids()
        {
            bool changes = false;
            bool NextMiniGrid;
            int occurrence;
            
            for (int n = 1; (n <= 9); n++)
            {
                for (int r = 1; (r <= 9); r = (r + 3))
                {
                    for (int c = 1; (c <= 9); c = (c + 3))
                    {
                        NextMiniGrid = false;
                        occurrence = 0;
                        for (int rr = 0; (rr <= 2); rr++)
                        {
                            for (int cc = 0; (cc <= 2); cc++)
                            {
                                if (((actual[(c + cc), (r + rr)] == 0) && possible[(c + cc), (r + rr)].Contains(n.ToString())))
                                {
                                    occurrence++;
                                    cPos = (c + cc);
                                    rPos = (r + rr);
                                    if ((occurrence > 1))
                                    {
                                        NextMiniGrid = true;
                                        break;
                                    }
                                }
                            }
                            if (NextMiniGrid)
                            {
                                break;
                            }
                        }
                        if (!NextMiniGrid && (occurrence == 1))
                        {
                            //number is there
                            SetCell(cPos, rPos, n, 1);
                            SetToolTip(cPos, rPos, n.ToString());
                            moves.Push((cPos + (rPos + n.ToString())));
                            DisplayActivity("Look for Lone Rangers in Minigrids", false);
                            DisplayActivity("===========================", false);
                            DisplayActivity(("Inserted value " + (n.ToString() + (" in " + ("(" + (cPos + ("," + (rPos + ")"))))))), false);
                            Application.DoEvents();
                            changes = true;
                            if (hint_mode)
                            {
                                return true;
                            }
                            
                        }
                    }
                }
            }
            return changes;
        }

        public bool LookForLoneRangersinRows() 
        { 
            bool changes = false; 
            int occurrence; 
            
                for (int r = 1; (r <= 9); r++) 
                { 
                    for (int n = 1; (n <= 9); n++) 
                    { 
                        occurrence = 0; 
                        for (int c = 1; (c <= 9); c++) 
                        { 
                            if (((actual[c, r] == 0) && possible[c, r].Contains(n.ToString()))) 
                            { occurrence++; 
                                if ((occurrence > 1)) 
                                { 
                                    break; 
                                    cPos = c; 
                                    rPos = r; 
                                } 
                                if ((occurrence == 1)) 
                                {
                                    
                                    SetCell(cPos, rPos, n, 1); 
                                    SetToolTip(cPos, rPos, n.ToString()); 
                                    moves.Push((cPos + (rPos + n.ToString()))); 
                                    DisplayActivity("Look for Lone Rangers in Rows", false); 
                                    DisplayActivity("=========================", false); 
                                    DisplayActivity(("Inserted value " + (n.ToString() + (" in " + ("(" + (cPos + ("," + (rPos + ")"))))))), false); 
                                    Application.DoEvents(); 
                                    changes = true; 
                                    
                                    if (hint_mode) 
                                    { 
                                        return true; 
                                    } 
                                    
                                } 
                            } 
                        } 
                    } 
                }
                return changes; 
        }

        public bool LookForLoneRangersinColumns()
        {
            bool changes = false;
            int occurrence;
            
            // ----check by column----
            for (int c = 1; (c <= 9); c++)
            {
                for (int n = 1; (n <= 9); n++)
                {
                    occurrence = 0;
                    for (int r = 1; (r <= 9); r++)
                    {
                        if (((actual[c, r] == 0) && possible[c, r].Contains(n.ToString())))
                        {
                            occurrence++;
                            // ---if multiple occurrences, not a lone ranger anymore
                            if (occurrence > 1)
                            {
                                break;
                            }
                            cPos = c;
                            rPos = r;
                        }
                    }
                    if ((occurrence == 1))
                    {
                        // --number is confirmed---
                        
                        SetCell(cPos, rPos, n, 1);
                        SetToolTip(cPos, rPos, n.ToString());
                        // ---saves the move into the stack
                        moves.Push((cPos + (rPos + n.ToString())));
                        DisplayActivity("Look for Lone Rangers in Columns", false);
                        DisplayActivity("===========================", false);
                        DisplayActivity(("Inserted value " + (n.ToString() + (" in " + ("(" + (cPos + ("," + (rPos + ")"))))))), false);
                        Application.DoEvents();
                        changes = true;
                        if (hint_mode)
                        {
                            return true;
                        }
                        
                    }
                }
            }
            return changes;
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This program has been created in 2014!" + Environment.NewLine + "Dogan Alkan");
        }

    }
}
