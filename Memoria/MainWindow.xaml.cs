using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Memoria
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int gridSize = 4;
        private Button firstClicked, secondClicked;
        private DispatcherTimer flipBackTimer;
        private DispatcherTimer gameTimer;
        private int elapsedTime;
        private int score;
        private List<string> cardSymbols;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            CreateCardSymbols();
            SetupGrid();
            SetupTimers();
            StartNewGame();
        }

        private void CreateCardSymbols()
        {
            cardSymbols = new List<string>() { "🌟", "🌞", "💕", "🌼", "🌴", "🐟", "💜", "🎉" };
            cardSymbols.AddRange(cardSymbols); // Duplicate for pairs
        }

        private void SetupGrid()
        {
            GameGrid.Rows = gridSize;
            GameGrid.Columns = gridSize;
            RandomizeCards();
        }

        private void RandomizeCards()
        {
            GameGrid.Children.Clear();
            foreach (var card in cardSymbols.OrderBy(x => Guid.NewGuid()))
            {
                var btn = new Button
                {
                    Content = "?",
                    FontSize = 32,
                    BorderBrush = new SolidColorBrush(Colors.Gray),
                    Background = new SolidColorBrush(Colors.LightSkyBlue),
                    Padding = new Thickness(0),
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(5),
                    Cursor = System.Windows.Input.Cursors.Hand
                };
                btn.Click += Card_Click;
                GameGrid.Children.Add(btn);
            }
        }

        private void SetupTimers()
        {
            flipBackTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            flipBackTimer.Tick += FlipBackTimer_Tick;
            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            gameTimer.Tick += GameTimer_Tick;
        }

        private void StartNewGame()
        {
            elapsedTime = 0;
            score = 0;
            ScoreLabel.Content = $"Score: {score}";
            TimerLabel.Content = $"Time: {elapsedTime}s";
            RandomizeCards();
            gameTimer.Start();
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            if (firstClicked != null && secondClicked != null)
                return;

            Button clickedButton = sender as Button;

            if (clickedButton == null || clickedButton.Content.ToString() != "?")
                return;

            string symbol = cardSymbols[GameGrid.Children.IndexOf(clickedButton)];
            clickedButton.Content = symbol;

            if (firstClicked == null)
            {
                firstClicked = clickedButton;
            }
            else
            {
                secondClicked = clickedButton;
                if (firstClicked.Content.ToString() == secondClicked.Content.ToString())
                {
                    firstClicked.IsEnabled = false;
                    secondClicked.IsEnabled = false;
                    firstClicked = null;
                    secondClicked = null;
                    score++;
                    ScoreLabel.Content = $"Score: {score}";
                    CheckForGameEnd();
                }
                else
                {
                    flipBackTimer.Start();
                }
            }
        }

        private void FlipBackTimer_Tick(object sender, EventArgs e)
        {
            flipBackTimer.Stop();
            firstClicked.Content = "?";
            secondClicked.Content = "?";
            firstClicked = null;
            secondClicked = null;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            elapsedTime++;
            TimerLabel.Content = $"Time: {elapsedTime}s";
        }

        private void CheckForGameEnd()
        {
            if (GameGrid.Children.OfType<Button>().All(b => b.IsEnabled == false))
            {
                gameTimer.Stop();
                MessageBox.Show($"Congratulations! You completed the game in {elapsedTime} seconds.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                StartNewGame();
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }
    }
}