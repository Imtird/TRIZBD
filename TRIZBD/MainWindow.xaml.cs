using System.Windows;
using TRIZBD.Windows.DataWindows;

namespace TRIZBD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EventsWindow ew = new EventsWindow();
            this.Hide();
            ew.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            EventTypesWindow etw = new EventTypesWindow();
            this.Hide();
            etw.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            LocationsWindow lw = new LocationsWindow();
            this.Hide();
            lw.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OrganizersWindow ow = new OrganizersWindow();
            this.Hide();
            ow.Show();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ParticipantsWindow pw = new ParticipantsWindow();
            this.Hide();
            pw.Show();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            RegistrationsWindow rw = new RegistrationsWindow();
            this.Hide();
            rw.Show();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            FeedbacksWindow fw = new FeedbacksWindow();
            this.Hide();
            fw.Show();
        }
    }
}
