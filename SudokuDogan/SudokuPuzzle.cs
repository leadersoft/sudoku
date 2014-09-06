using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuDogan
{
    class SudokuPuzzle
    {
        //private int[,] actual = new int[10, 10];
        //private string[,] possible = new string[10, 10];
        //private bool BruteForceStop = false;
        //private Stack<int[,]> ActualStack = new Stack<int[,]>();
        //private Stack<string[,]> PossibleStack = new Stack<string[,]>();

        //private int totalscore;
        //private bool SolvePuzzle(); 
        //private bool CheckColumnsAndRows();
        //private string  CalculatePossibleValues(int col, int row);
        //private bool LookForLoneRangersinMinigrids();
        //private bool LookForLoneRangersinRows(); 
        //private bool LookForLoneRangersinColumns(); 
        //private bool LookForTwinsinMinigrids();
        //private bool LookForTwinsinRows();
        //private bool LookForTwinsinColumns();
        //private bool LookForTripletsinMinigrids();
        //private bool LookForTripletsinRows();
        //private bool LookForTripletsinColumns();
    //    private void FindCellWithFewestPossibleValues(ref int col, ref int row);
    //    private void SolvePuzzleByBruteForce();
    //    private void RandomizeThePossibleValues(ref string str)
    //{
    //    char[,] s;
    //    int i;
    //    int j;
    //    char temp;
    //    Randomize();
    //    s = str.ToCharArray;
    //    for (i = 0; (i
    //                <= (str.Length - 1)); i++)
    //    {
    //        j = (int.Parse(((((str.Length - i)
    //                        + 1)
    //                        * Rnd())
    //                        + i)) % str.Length);
    //            temp = s[i];
    //        s[i] = s[j];
    //        s[j] = temp;
    //    }
    //    str = s;
    //    }
    //    //private void SolvePuzzleByBruteForce() {
    //    //int c;
    //    //int r;
    //    //totalscore += 5;
    //    //FindCellWithFewestPossibleValues(c, r);
    //    //string possibleValues = possible(c, r);
    //    //RandomizeThePossibleValues(possibleValues);
    //    //ActualStack.Push(((int[,])(actual.Clone())));
    //    //PossibleStack.Push(((string[,])(possible.Clone())));
    //    //for (int i = 0; (i
    //    //            <= (possibleValues.Length - 1)); i++)
    //    //{
    //    //    actual[c, r] = int.Parse(possibleValues[i].ToString());
    //    //    try
    //    //    {
    //    //        if (SolvePuzzle())
    //    //        {
    //    //            BruteForceStop = true;
    //    //            return;
    //    //        }
    //    //        else
    //    //        {
    //    //            SolvePuzzleByBruteForce();
    //    //            if (BruteForceStop)
    //    //            {
    //    //                return;
    //    //            }
    //    //            ((Exception)(ex));
    //    //            totalscore += 5;
    //    //            actual = ActualStack.Pop();
    //    //            possible = PossibleStack.Pop();
    //    //        }
    //    //    }
    //    //    temp;
    //    //    str = s;
    //    //}
    //    //}


    //    private string GenerateNewPuzzle(int level, int score) {
    //    int c;
    //    int r;
    //    string str;
    //    int numberofemptycells;
    //    for (r = 1; (r <= 9); r++) {
    //        for (c = 1; (c <= 9); c++) {
    //            actual[c, r] = 0;
    //            possible[c, r] = String.Empty;
    //        }
    //    }
    //    ActualStack.Clear();
    //    PossibleStack.Clear();
    //    try {
    //        if (!SolvePuzzle()) {
    //            SolvePuzzleByBruteForce();
    //        }
    //    }
    //    catch (Exception ex) {
    //        return String.Empty;
    //    }
    //    actual_backup = actual.Clone();
    //    switch (level) {
    //        case 1:
    //            numberofemptycells = RandomNumber(40, 45);
    //            break;
    //        case 2:
    //            numberofemptycells = RandomNumber(46, 49);
    //            break;
    //        case 3:
    //            numberofemptycells = RandomNumber(50, 53);
    //            break;
    //        case 4:
    //            numberofemptycells = RandomNumber(54, 58);
    //            break;
    //    }
    //    ActualStack.Clear();
    //    PossibleStack.Clear();
    //    BruteForceStop = false;
    //    CreateEmptyCells(numberofemptycells);
    //    str = String.Empty;
    //    for (r = 1; (r <= 9); r++) {
    //        for (c = 1; (c <= 9); c++) {
    //            actual(c, r).ToString();
    //        }
    //    }
    //    int tries = 0;
    //    for (
    //    ; true; 
    //    ) {
    //        totalscore = 0;
    //        try {
    //            if (!SolvePuzzle()) {
    //                if ((level < 4)) {
    //                    VacateAnotherPairOfCells(str);
    //                    tries++;
    //                }
    //                else {
    //                    SolvePuzzleByBruteForce();
    //                }
    //                break; //Warning!!! Review that break works as 'Exit Do' as it could be in a nested instruction like switch
    //            }
    //            break; //Warning!!! Review that break works as 'Exit Do' as it could be in a nested instruction like switch
    //        }
    //        catch (Exception ex) {
    //            return String.Empty;
    //        }
    //        if ((tries > 50)) {
    //            return String.Empty;
    //        }
    //    }
    //    score = totalscore;
    //    return str;
    //}

    //    private int RandomNumber(int min, int max) {
    //    return (int((((max - min) + 1) * Rnd())) + min);
    //}

    //    public string GetPuzzle(int level)
    //    {
    //        int score;
    //        string result;
    //        for (
    //        ; (false == false);
    //        )
    //        {
    //            result = GenerateNewPuzzle(level, score);
    //            if ((result != String.Empty))
    //            {
    //                switch (level)
    //                {
    //                    case 1:
    //                        if (((score >= 42)
    //                                    && (score <= 46)))
    //                        {
    //                            break; 
    //                        }
    //                        break;
    //                    case 2:
    //                        if (((score >= 49)
    //                                    && (score <= 53)))
    //                        {
    //                            break; 
    //                        }
    //                        break;
    //                    case 3:
    //                        if (((score >= 56)
    //                                    && (score <= 60)))
    //                        {
    //                            break; 
    //                        }
    //                        break;
    //                    case 4:
    //                        if (((score >= 112)
    //                                    && (score <= 116)))
    //                        {
    //                            break; 
    //                        }
    //                        break;
    //                }
    //            }
    //        }
    //        return result;
    //    }

        
    }

    }

