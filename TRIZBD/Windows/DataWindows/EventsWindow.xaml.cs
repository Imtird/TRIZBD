using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using TRIZBD.Windows.AddEditWindows;

namespace TRIZBD.Windows.DataWindows
{
    public partial class EventsWindow : Window
    {
        private readonly EventsEntities _context;

        public EventsWindow()
        {
            InitializeComponent();
            _context = new EventsEntities();
            _context.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            LoadEvents();
        }

        private void LoadEvents()
        {
            try
            {
                EventsDataGrid.ItemsSource = _context.Event
                    .Include(e => e.Location)
                    .Include(e => e.Event_type)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new AddEditEventWindow(_context);
            if (addEditWindow.ShowDialog() == true)
            {
                LoadEvents();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = EventsDataGrid.SelectedItem as Event;
            if (selectedEvent == null)
            {
                MessageBox.Show("Выберите мероприятие для редактирования!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var eventToEdit = _context.Event.Find(selectedEvent.Id_event);
            if (eventToEdit == null)
            {
                MessageBox.Show("Выбранное мероприятие не найдено в базе данных!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var addEditWindow = new AddEditEventWindow(_context, eventToEdit);
            if (addEditWindow.ShowDialog() == true)
            {
                LoadEvents();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEvent = EventsDataGrid.SelectedItem as Event;
            if (selectedEvent == null)
            {
                MessageBox.Show("Выберите мероприятие для удаления!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Вы уверены, что хотите удалить выбранное мероприятие?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var eventToDelete = _context.Event.Find(selectedEvent.Id_event);
                if (eventToDelete == null)
                {
                    MessageBox.Show("Мероприятие не найдено в базе данных!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _context.Event.Remove(eventToDelete);
                _context.SaveChanges();
                LoadEvents();

                MessageBox.Show("Мероприятие успешно удалено!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Реализация методов меню
        private void ParticipantsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var participantsWindow = new ParticipantsWindow();
                participantsWindow.Owner = this;
                participantsWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна участников: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OrganizersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var organizersWindow = new OrganizersWindow();
                organizersWindow.Owner = this;
                organizersWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна организаторов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegistrationsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var registrationsWindow = new RegistrationsWindow();
                registrationsWindow.Owner = this;
                registrationsWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна регистраций: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReviewsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var feedbacksWindow = new FeedbacksWindow();
                feedbacksWindow.Owner = this;
                feedbacksWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна отзывов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти из программы?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        // Остальные методы меню
        private void LocationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var locationsWindow = new LocationsWindow();
                locationsWindow.Owner = this;
                locationsWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна локаций: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EventTypeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var eventTypesWindow = new EventTypesWindow();
                eventTypesWindow.Owner = this;
                eventTypesWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна типов мероприятий: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}