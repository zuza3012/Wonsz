using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Wonsz {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        int width = 450, height = 450, step, rows, cols, size_matrix, offsetX = 0, offsetY = 0;
        Cell[,] board;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        bool start = false;
        SnakeDirection snakeDirection;
        int last_segment;
        Level level;
        enum Level {
            easy = 1,
            medium = 2,
            hard = 3
        }


        enum SnakeDirection {
            Up,
            Down,
            Right,
            Left
        }


        public MainWindow() {
            InitializeComponent();
            size_matrix = 20;
            board = new Cell[size_matrix, size_matrix];


            rows = size_matrix;
            cols = size_matrix;

            step = width / rows;
            DrawLines(rows, cols, offsetX, offsetY, step);

            Initialize();

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(200);

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            canvas.Children.Clear();
            DrawLines(rows, cols, 0, 0, step);
            Console.WriteLine("0, 0 {0}", board[0, 0].Free);
            Console.WriteLine("0, 0 {0}", board[0, 0].Food);

            GameLogic();

            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    if (board[i, j].Food == true)
                        DrawRect(i, j, step, offsetX, offsetY, board[i, j].Val);
                    else if (board[i, j].Free == false)
                        DrawRect(i, j, step, offsetX, offsetY, board[i, j].Val);
                }
            }

            //WriteBoard();
        }


        private void DrawRect(int i, int j, int step, int offsetX, int offsetY, int val) {
            // jesli food
            SolidColorBrush color = Brushes.Transparent;
            if (val == -1) // food
                color = Brushes.Orange;
            else if (val > 0) // snake
                color = Brushes.Green;


            Rectangle rect = new Rectangle {
                Width = step - 4,
                Height = step - 4,
                Fill = color
            };

            Canvas.SetLeft(rect, i * step + offsetX + 2);
            Canvas.SetTop(rect, j * step + offsetY + 2);
            canvas.Children.Add(rect);

        }


        private void DrawLines(int rows, int cols, int offsetX, int offsetY, int step) {
            for (int i = 0; i < cols + 1; i++) {    //rysowanie linii pionowych
                Line lineX = new Line {
                    Stroke = Brushes.Black,

                    X1 = i * step + offsetX,
                    Y1 = 0 + offsetY,

                    X2 = i * step + offsetX,
                    Y2 = rows * step + offsetY
                };
                canvas.Children.Add(lineX);
            }

            for (int j = 0; j < rows + 1; j++) {    //rysowanie linii poziomych
                Line lineY = new Line {
                    Stroke = Brushes.Black,

                    X1 = 0 + offsetX,
                    Y1 = j * step + offsetY,

                    X2 = cols * step + offsetX,
                    Y2 = j * step + offsetY
                };
                canvas.Children.Add(lineY);
            }
        }

        private void Initialize() {

            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    board[i, j] = new Cell(); //new Cell(false, true);
                    //DrawRect(i, j, step, offsetX, offsetY, board[i, j].Food, board[i, j].Free);
                }
            }

            //GenerateFood((int)level.easy);
            //Console.WriteLine("Level: {0} with number {1}", level.easy, (int)level.easy);
            board[3, 3].Food = true;
            board[3, 3].Val = -1;// -1 dla jedzenia
            board[5, 5].Free = false;
            //board[8, 5].Head = true;

            board[5, 5].Val = 1; // glowa
            board[6, 5].Val = 2;
            board[7, 5].Val = 3;

            board[7, 5].Free = false;
            board[6, 5].Free = false;
            snakeDirection = SnakeDirection.Left;
            last_segment = 3; // bo patrz wyzej


        }


        private void GenerateFood(int food_count) {
            Random rand = new Random();

            for (int i = 0; i < food_count; i++) {
                int n = rand.Next(0, rows);
                int m = rand.Next(0, cols);

                if (board[n, m].Food != true) {
                    board[n, m].Food = true;
                    board[n, m].Val = -1;
                }
                    
            }

        }

        private void WriteBoard() { // sprawdza czy zajete
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {

                    Console.Write(board[i, j].Val + " ");

                }
                Console.WriteLine();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Down) {
                Debug.WriteLine("Down");
                snakeDirection = SnakeDirection.Down;
            }
            if (e.Key == Key.Up) {
                Debug.WriteLine("Up");
                snakeDirection = SnakeDirection.Up;
            }
            if (e.Key == Key.Right) {
                Debug.WriteLine("Right");
                snakeDirection = SnakeDirection.Right;

            }
            if (e.Key == Key.Left) {
                Debug.WriteLine("Left");
                snakeDirection = SnakeDirection.Left;

            }

        }


        private void Start_Clicked(object sender, RoutedEventArgs e) {
            if (start == false) {
                Initialize();
                start = true;
                dispatcherTimer.Start();
                startBtn.Content = "Stop";
                if (eRbtn.IsChecked == true)
                    level = Level.easy;
                else if (mRbtn.IsChecked == true)
                    level = Level.medium;
                else if (hRbtn.IsChecked == true)
                    level = Level.hard;


            } else {
                canvas.Children.Clear();
                start = false;
                dispatcherTimer.Stop();
                startBtn.Content = "Start";
            }
        }

        private void GameLogic() {
            bool headMoved = false;

            for (int i = 1; i < rows - 1; i++) {
                for (int j = 1; j < cols - 1; j++) {
                    if (board[i, j].Val == last_segment) {
                        board[i, j].Val = 0;
                    } else if (board[i, j].Val > 1) // w tej komorce jste cialo
                        board[i, j].Val++;
                    else if (board[i, j].Val == 1 && !headMoved) { // glowa

                        if (board[i, j].Food == true) {
                            last_segment++;
                            GenerateFood((int)level);
                        }
                            

                        switch (snakeDirection) {
                            case SnakeDirection.Up:
                                // board[i, j - 1].Head = true;
                                if ((j - 1) == 0) {
                                    dispatcherTimer.Stop();
                                    MessageBox.Show("Game Over!");
                                    break;
                                }
                                board[i, j - 1].Free = false;
                                board[i, j - 1].Val = 1;
                                break;
                            case SnakeDirection.Down:
                                // board[i, j + 1].Head = true;
                                if ((j + 1) == cols - 1) {
                                    dispatcherTimer.Stop();
                                    MessageBox.Show("Game Over!");
                                    break;
                                }
                                board[i, j + 1].Free = false;
                                board[i, j + 1].Val = 1;
                                break;
                            case SnakeDirection.Right:
                                //board[i + 1, j].Head = true;
                                Console.WriteLine("1, i+1 {0}, {1}", i, i+1);
                                if((i + 1) == rows - 1) {
                                    dispatcherTimer.Stop();
                                    MessageBox.Show("Game Over!");
                                    break;
                                }

                                board[i + 1, j].Free = false;
                                board[i + 1, j].Val = 1;
                                break;
                            case SnakeDirection.Left:
                                //board[i - 1, j].Head = true;
                                if ((i - 1) == 0) {
                                    dispatcherTimer.Stop();
                                    MessageBox.Show("Game Over!");
                                    break;
                                }


                                board[i - 1, j].Free = false;
                                board[i - 1, j].Val = 1;
                                break;

                        }
                        board[i, j].Val++;
                        headMoved = true;
                        //board[i, j].Free = true;

                    }


                }
            }

        }
    }
}

