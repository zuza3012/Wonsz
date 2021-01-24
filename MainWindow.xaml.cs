using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wonsz {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        int width = 450, height = 450, step, rows, cols, size_matrix, offsetX = 0, offsetY = 0, last_segment, speed = 200;
        Cell[,] board;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        //bool start = false;
        SnakeDirection snakeDirection;
        Point headPosition;
        Level level;
        Random rand = new Random();
        double p = 0.01, step1 = 0;
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

        static bool start;
        public static bool getStart {
            set { start = value; }
            get { return start; }
        }

        static bool clicked;
        public static bool getClicked {
            set { clicked = value; }
            get { return clicked; }
        }

        public MainWindow() {
            InitializeComponent();
            size_matrix = 20;
            board = new Cell[size_matrix, size_matrix];
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("Resources/grass.jpg", UriKind.Relative));
            canvas.Background = ib;


            rows = size_matrix;
            cols = size_matrix;
            step = width / rows;
            step1 = ((double)width / (double)rows);
            
            //DrawLines(rows, cols, offsetX, offsetY, step1);

            Initialize();



        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            canvas.Children.Clear();
            //DrawLines(rows, cols, 0, 0, step1);

            GameLogic();

            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    if (board[i, j].Food == true)
                        DrawRect(i, j, step1, offsetX, offsetY, board[i, j].Val);
                    else if (board[i, j].Free == false)
                        DrawRect(i, j, step1, offsetX, offsetY, board[i, j].Val);
                }
            }

        }


        private void DrawRect(int i, int j, double step, int offsetX, int offsetY, int val) {
            // jesli food
            SolidColorBrush color = Brushes.Transparent;
            if (val == -1) // food
                color = Brushes.Orange;
            else if (val > 0) // snake
                color = Brushes.Blue;
            else if (val == -2) // eggs
                color = Brushes.Pink;

            
            Rectangle rect = new Rectangle {
                Width = step,
                Height = step,
                Fill = color
            };

            Ellipse ellipse = new Ellipse {
                Width = step,
                Height = step,
                Stroke = Brushes.Transparent,
                StrokeThickness = 2,
                Fill = Brushes.Brown
            };


            if (val == -2) {
                rect.Fill = new ImageBrush {
                    ImageSource = new BitmapImage(new Uri("Resources/egg.png", UriKind.Relative))
                };
            } else if (val == 1) {

                if (snakeDirection == SnakeDirection.Right) {
                    rect.Fill = new ImageBrush {
                        ImageSource = new BitmapImage(new Uri("Resources/hright.png", UriKind.Relative))
                    };
                } else if (snakeDirection == SnakeDirection.Left) {
                    rect.Fill = new ImageBrush {
                        ImageSource = new BitmapImage(new Uri("Resources/hleft.png", UriKind.Relative))
                    };
                } else if (snakeDirection == SnakeDirection.Up) {
                    rect.Fill = new ImageBrush {
                        ImageSource = new BitmapImage(new Uri("Resources/hup.png", UriKind.Relative))
                    };
                } else {
                    rect.Fill = new ImageBrush {
                        ImageSource = new BitmapImage(new Uri("Resources/hdown.png", UriKind.Relative))
                    };
                }
            }


            if (val != -2) {
                Canvas.SetLeft(rect, i * step + offsetX + 1);
                Canvas.SetTop(rect, j * step + offsetY + 1);
                canvas.Children.Add(rect);
            } else {
                Canvas.SetLeft(ellipse, i * step + offsetX + 1);
                Canvas.SetTop(ellipse, j * step + offsetY + 1);
                canvas.Children.Add(ellipse);
            }

        }


        private void DrawLines(int rows, int cols, int offsetX, int offsetY, double step) {
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
                    board[i, j].Val = 0;
                    //DrawRect(i, j, step, offsetX, offsetY, board[i, j].Food, board[i, j].Free);
                }
            }

            //GenerateFood((int)level.easy);
            //Console.WriteLine("Level: {0} with number {1}", level.easy, (int)level.easy);
            board[3, 3].Food = true;
            board[3, 3].Val = -1;// -1 dla jedzenia

            //board[8, 5].Head = true;

            headPosition = new Point(2, 0);
            board[2, 0].Val = 1; // glowa
            board[1, 0].Val = 2;
            board[0, 0].Val = 3;

            board[2, 0].Free = false;
            board[1, 0].Free = false;
            board[0, 0].Free = false;
            snakeDirection = SnakeDirection.Right;
            last_segment = 3; // bo patrz wyzej


        }


        private void GenerateFood(int food_count) {

            for (int i = 0; i < food_count; i++) {
                int n = rand.Next(0, rows);
                int m = rand.Next(0, cols);

                if (board[n, m].Food != true) {
                    board[n, m].Food = true;
                    board[n, m].Val = -1;
                }

            }

        }

        private void MakeEggs(int last_segment) {

            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    if (board[i, j].Val == last_segment && board[i, j].Free == true) {
                        board[i, j].Free = false;
                        board[i, j].Val = -2; // eggs
                    }
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
                if (snakeDirection != SnakeDirection.Up)
                    snakeDirection = SnakeDirection.Down;
                getClicked = true;
            }
            if (e.Key == Key.Up) {
                Debug.WriteLine("Up");
                if (snakeDirection != SnakeDirection.Down)
                    snakeDirection = SnakeDirection.Up;
                getClicked = true;
            }
            if (e.Key == Key.Right) {
                Debug.WriteLine("Right");
                if (snakeDirection != SnakeDirection.Left)
                    snakeDirection = SnakeDirection.Right;
                getClicked = true;
            }
            if (e.Key == Key.Left) {
                Debug.WriteLine("Left");
                if (snakeDirection != SnakeDirection.Right)
                    snakeDirection = SnakeDirection.Left;
                getClicked = true;
            }


        }


        private void Start_Clicked(object sender, RoutedEventArgs e) {
            Thread soundt = new Thread(Sound);
            if (getStart == false) {
                Initialize();
                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += dispatcherTimer_Tick;


                soundt.Start();

                getStart = true;
                dispatcherTimer.Start();
                startBtn.Content = "Stop";
                if (eRbtn.IsChecked == true) {
                    level = Level.easy;
                    speed = 60;
                    p = 0.01;
                } else if (mRbtn.IsChecked == true) {
                    level = Level.medium;
                    speed = 70;
                    p = 0.03;
                } else if (hRbtn.IsChecked == true) {
                    level = Level.hard;
                    speed = 50;
                    p = 0.05;
                }

                dispatcherTimer.Interval = TimeSpan.FromMilliseconds(speed);
            } else {
                //canvas.Children.Clear();
                getStart = false;
                dispatcherTimer.Stop();
                soundt.Abort();
                startBtn.Content = "Start";
            }
        }

        static void Sound() {
            try {
                while (getStart == true) {

                    if (getClicked == true) {

                        string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
                        string FileName = string.Format("{0}Resources\\blow1.wav", System.IO.Path.GetFullPath(System.IO.Path.Combine(RunningPath, @"..\..\")));                     
                        SoundPlayer player = new SoundPlayer(FileName);
                        player.Load();
                        player.Play();
                        getClicked = false;
                    }
                }
            }catch(Exception ex) {
                Console.WriteLine("Exception: {0}", ex.Message);
            }
           
        }
        private void GameLogic() {

            Point nextMove = new Point(0, 0);

            switch (snakeDirection) {

                case SnakeDirection.Up:
                    nextMove = new Point(headPosition.X, headPosition.Y - 1);
                    break;
                case SnakeDirection.Down:
                    nextMove = new Point(headPosition.X, headPosition.Y + 1);
                    break;
                case SnakeDirection.Right:
                    nextMove = new Point(headPosition.X + 1, headPosition.Y);
                    break;
                case SnakeDirection.Left:
                    nextMove = new Point(headPosition.X - 1, headPosition.Y);
                    break;
                default:
                    throw new Exception("This is not possible to have no direction!");

            }
            if (nextMove.X < 0 || nextMove.X > (cols - 1) || nextMove.Y < 0 || nextMove.Y > (rows - 1)) {
                Console.WriteLine("czek");
                MessageBox.Show("Game Over!");
                dispatcherTimer.Stop();
                return;
            }

            if (board[(int)nextMove.X, (int)nextMove.Y].Food == true) {
                last_segment++;
                GenerateFood((int)level);


            }
            if (board[(int)nextMove.X, (int)nextMove.Y].Val > 0 || board[(int)nextMove.X, (int)nextMove.Y].Val == -2) {
                Console.WriteLine("SNAAAKE");
                dispatcherTimer.Stop();
                MessageBox.Show("Game Over!");

            }

            board[(int)nextMove.X, (int)nextMove.Y].Free = false;
            board[(int)nextMove.X, (int)nextMove.Y].Val = 1;




            board[(int)headPosition.X, (int)headPosition.Y].Val++;
            headPosition = nextMove;



            for (int i = 0; i < rows; i++) {

                for (int j = 0; j < cols; j++) {

                    if (board[i, j].Val == last_segment) {

                        board[i, j].Val = 0;
                        if (rand.NextDouble() < p) {
                            MakeEggs(last_segment);
                            board[i, j].Val = -2;
                        }

                    } else if (board[i, j].Val > 1) // w tej komorce jste cialo
                        board[i, j].Val++;



                }
            }

        }
    }
}

