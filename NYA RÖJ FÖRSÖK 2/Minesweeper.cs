using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NYA_RÖJ_FÖRSÖK_2
{
    public partial class Minesweeper : Form
    {
        // 'field' and 'buttons' are two private instance variables of the class 'Minesweeper'
        // 'field' is a two-dimensional array of integers that represents the game board.
        // Each cell of the board can either be a bomb (-1) or a number indicating the number of bombs in its neighboring cells
        // 'buttons' is a two-dimensional array of Button objects that represents the game board UI.
        private int[,] field;
        private Button[,] buttons;
        public Minesweeper()
        {
            InitializeComponent();
        }

        // Timer starts at 0 seconds
        private int elapsedTime = 0;

        // init() is a private method that initializes the game board by randomly placing the bombs and updating the neighboring cells with the bomb count.
        private void init(int width, int height, int bomb) 
        {
            field = new int[width, height];
            buttons = new Button[width, height];

            //Randomize bomb place
            Random random= new Random();
            while (bomb > 0)
            {
                int x = random.Next(0, width);
                int y = random.Next(0, height);
                if (field[x, y] == -1) continue;
                field[x, y] = -1;
                for (int dx=-1; dx<=1; dx++)
                {
                    for (int dy= -1; dy<=1; dy++)
                    {
                        if (x + dx < 0) continue;
                        if (y + dy < 0) continue;
                        if (x + dx >= width) continue;
                        if (y + dy >= height) continue;
                        if (field[x + dx, y + dy] != -1)
                            field[x + dx, y + dy]++;
                    }
                }
                bomb--;
            }
        }

        // The 'Form1_Load' event handler is executed when the form is loaded. It starts the timer and calls the 'init()' method to initialize the game board
        // It then creates a 'Button' object for each cell of the board and sets its properties such as 'Font', 'Width', 'Height', and 'Text'
        // Finally, it adds the button to the form's 'Controls' collection and attaches event handlers for MouseDown and MouseClick events
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            // Here is the manager for the field, where the width, height and amount of bombs gets chosen
            // For example, as of now, its a field build by 12 x 8 with 10 total bombs
            init(12,8, 10);
            for (int x = 0; x <field.GetLength(0); x++)
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    Button b = new Button();
                    buttons[x, y] = b;
                    b.Font = new Font("Arial", 40);
                    b.Left = x * 80;
                    b.Top = y * 80;
                    b.Width = 80;
                    b.Height = 80;
                    b.Text = "";
                    Controls.Add(b);
                    b.MouseDown += B_MouseDown;
                    b.MouseClick += B_Click;
                }
        }

        // 'B_MouseDown' is an event handler for the 'MouseDown' event of the buttons
        // If the right mouse button is clicked, it toggles the flag on/off by setting the button's 'Text' property to either a flag emoji or an empty string
        // It then calls 'CheckWin()' method to check if the game is won.
        private void B_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button b = (Button)sender;
                if (b.Text == "")
                {
                    b.Text = "\U0001F6A9";
                    CheckWin();
                }
                else
                {
                    b.Text = "";
                }
            }
        }

        // 'CheckWin()' method checks if all the bombs are flagged by iterating through all the buttons and counting the flagged and unflagged bombs
        // If all the bombs are flagged, it disables all the buttons and shows a message box congratulating the player.
        private void CheckWin()
        {
            int bombCount = 0;
            int flaggedCount = 0;
            foreach (Button button in buttons)
            {
                if (field[button.Left / 80, button.Top / 80] == -1)
                {
                    bombCount++;
                    if (button.Text == "\U0001F6A9")
                    {
                        flaggedCount++;
                    }
                }
            }
            if (flaggedCount == bombCount)
            {
                // Disable all the buttons and show the winner screen
                foreach (Button button in buttons)
                {
                    button.Enabled = false;
                }
                timer1.Stop();
                MessageBox.Show("Congratulations! You won!");
            }
        }

        // 'B_Click' is an event handler for the 'MouseClick' event of the buttons
        // If the left mouse button is clicked, it checks if the clicked button is a bomb, all non-bomb buttons are clicked, or the button has neighboring bombs
        // Depending on the situation, it either shows the bomb, disables all buttons and shows a message box indicating game over, or it recursively reveals the neighboring buttons until it reaches a button that has neighboring bombs.
        private void B_Click(object sender , MouseEventArgs e)
        {
            Button b = (Button)sender;
            int x = b.Left / 80;
            int y = b.Top / 80;

            if (e.Button == MouseButtons.Right)
            {
                if (b.Text == "\U0001F6A9")
                {
                    // Unflag the button
                    b.Text = "";
                }
                else
                {
                    // Flag the button
                    b.Text = "\U0001F6A9";
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                // Check if all non-bomb buttons have been clicked
                bool allNonBombButtonsClicked = true;
                foreach (Button button in buttons)
                {
                    if (field[button.Left / 80, button.Top / 80] != -1)
                    {
                        if (button.Enabled)
                        {
                            allNonBombButtonsClicked = false;
                            break;
                        }
                    }
                }
                if (allNonBombButtonsClicked)
                {
                    // Disable all the buttons and show the winner screen
                    foreach (Button button in buttons)
                    {
                        button.Enabled = false;
                    }
                    timer1.Stop();
                    MessageBox.Show("Congratulations! You won!");
                }
                else if (field[x, y] == -1)
                {
                    // The button clicked is a bomb
                    b.Text = "\U0001F4A3";

                    // Disable all the buttons and show the gameover screen
                    foreach (Button button in buttons)
                    {
                        button.Enabled = false;
                    }
                    timer1.Stop();
                    MessageBox.Show("Game Over");
                }
                else
                {
                    if (field[x, y] == 0)
                    {
                        b.Text = "";
                        cover(x, y);
                    }
                    else
                    {
                        b.Text = "" + field[x, y];
                    }
                    b.Enabled = false;
                }
            }
        }

        // Opens up a empty field when clicked on a empty button
        private void cover(int x, int y)
        {
            Stack<Point> stack=new Stack<Point>();
            stack.Push(new Point(x, y));
            while (stack.Count > 0)
            {
                Point p = stack.Pop();
                if (p.X < 0 || p.Y < 0) continue;
                if (p.X>=field.GetLength(0)
                    || p.Y>=field.GetLength(1)) continue;

                if (!buttons[p.X, p.Y].Enabled) continue;
                
                buttons[p.X, p.Y].Enabled = false;
                if (field[p.Y,p.Y]!=0)
                    buttons[p.X, p.Y].Text = ""+field[p.X, p.Y];

                if (field[p.X, p.Y] != 0) continue;
                stack.Push(new Point(p.X - 1, p.Y));
                stack.Push(new Point(p.X + 1, p.Y));
                stack.Push(new Point(p.X    , p.Y-1));
                stack.Push(new Point(p.X    , p.Y+1));
            }
        }

        // Restart button
        private void button2_Click(object sender, EventArgs e)
        {
            // Restart the application
            Application.Restart();
        }

        // The 'timer1_Tick' event handler is executed every second and updates the elapsed time.
        private void timer1_Tick(object sender, EventArgs e)
        {
            // 'elapsedTime' is an instance variable that keeps track of the elapsed time since the game started.
            elapsedTime++;
            label1.Text = "Elapsed Time: " + elapsedTime.ToString() + " seconds";
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void gameOver()
        {
            // Stop the timer
            timer1.Stop();
        }
    }
}