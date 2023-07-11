using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using SudokuGeneration;


namespace WPF_SudokuProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private DateTime startTime;
        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += UpdateTimer;
        }

        private void UpdateTimer(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = DateTime.Now - startTime;
            string formattedTime = $"{elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
            TimerLabel.Content = formattedTime;
        }

        private void RulesButton_Click(object sender, RoutedEventArgs e)
        {
            string rules = @"Як грати в Судоку. Правила і методи вирішення
            Судоку - логічна гра-головоломка з числами. Для її вирішення потрібно сконцентрувати увагу, а також задіяти логічне мислення. Складність судоку залежить від кількості заповнених клітин на початку і від методів, які потрібно застосовувати для її вирішення. Регулярні гри в Судоку покращують пам'ять, ясність розуму і уповільнюють старіння клітин мозку.
            
            Правила гри
            Ігрове поле являє собою квадрат розміром 9×9, розділений на менші квадрати зі стороною в 3 клітини. Мета гри - заповнити порожні клітки цифрами від 1 до 9 так, щоб в кожному рядку, в кожному стовпці і в кожному малому квадраті 3×3 кожна цифра зустрічалася лише один раз.";

            MessageBox.Show(rules, "Правила гри Судоку", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("До побачення", "Повідомлення", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuGrid.Visibility = Visibility.Collapsed;
            GameGrid.Visibility = Visibility.Visible;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuGrid.Visibility = Visibility.Visible;
            GameGrid.Visibility = Visibility.Collapsed;
        }

        private async void EasyLevelButton_Click(object sender, RoutedEventArgs e)
        {
            GameGrid.Visibility = Visibility.Collapsed;
            SudokuGame.Visibility = Visibility.Visible;
            startTime = DateTime.Now;
            timer.Start();
            SudokuGeneratorStrategy strategyEasy = new SudokuGenerator(1);
            Sudoku sudokuEasy = new Sudoku(strategyEasy, SudokuGrid);
            while (true)
            {
                if (sudokuEasy.Win())
                {
                    EndGame();
                    break;
                }
                await Task.Delay(100); 
            }
        }

        private async void HardLevelButton_Click(object sender, RoutedEventArgs e)
        {
            GameGrid.Visibility = Visibility.Collapsed;
            SudokuGame.Visibility = Visibility.Visible;
            startTime = DateTime.Now;
            timer.Start();
            SudokuGeneratorStrategy strategyHard = new SudokuGenerator(2);
            Sudoku sudokuHard = new Sudoku(strategyHard, SudokuGrid);
            while (true)
            {
                if (sudokuHard.Win())
                {
                    EndGame();
                    break;
                }
                await Task.Delay(100);
            }
        }
        private void EndGame()
        {
            timer.Stop();
            TimeSpan elapsed = DateTime.Now - startTime;
            string message = $"Ви пройшли судоку, ви геній! Затрачений час: {elapsed:mm\\:ss}";
            MessageBox.Show(message, "Свято", MessageBoxButton.OK, MessageBoxImage.Information);
            SudokuGame.Visibility = Visibility.Collapsed;
            MainMenuGrid.Visibility = Visibility.Visible;
        }

        private void ExitToMainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            SudokuGame.Visibility = Visibility.Collapsed;
            MainMenuGrid.Visibility = Visibility.Visible;
            timer.Stop();
            
        }

        

    }
}
