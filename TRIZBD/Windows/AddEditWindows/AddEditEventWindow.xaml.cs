using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TRIZBD.Windows.AddEditWindows
{
    public partial class AddEditEventWindow : Window
    {
        private readonly Event _currentEvent;
        private readonly EventsEntities _context;
        private bool _isInitializing = true;

        public AddEditEventWindow(EventsEntities context, Event selectedEvent = null)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            context.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            try
            {
                if (selectedEvent != null)
                {
                    _currentEvent = _context.Event.AsNoTracking().FirstOrDefault(e => e.Id_event == selectedEvent.Id_event);
                    if (_currentEvent == null)
                    {
                        MessageBox.Show("Мероприятие не найдено в базе данных", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        Close();
                        return;
                    }

                    // Создаем новый объект для редактирования
                    _currentEvent = new Event
                    {
                        Id_event = selectedEvent.Id_event,
                        name = selectedEvent.name,
                        description = selectedEvent.description,
                        date = selectedEvent.date,
                        Id_location = selectedEvent.Id_location,
                        Id_event_type = selectedEvent.Id_event_type
                    };

                    TitleText.Text = "Редактировать мероприятие";
                    ActionButton.Content = "Сохранить";
                }
                else
                {
                    _currentEvent = new Event { date = DateTime.Today };
                }

                DataContext = _currentEvent;
                _isInitializing = false;
                LoadComboBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {GetExceptionMessages(ex)}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                LocationComboBox.ItemsSource = _context.Location.AsNoTracking().ToList();
                EventTypeComboBox.ItemsSource = _context.Event_type.AsNoTracking().ToList();

                if (_currentEvent.Id_event > 0)
                {
                    LocationComboBox.SelectedValue = _currentEvent.Id_location;
                    EventTypeComboBox.SelectedValue = _currentEvent.Id_event_type;
                    DatePicker.SelectedDate = _currentEvent.date;
                }
                else
                {
                    DatePicker.SelectedDate = DateTime.Today;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {GetExceptionMessages(ex)}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetExceptionMessages(Exception ex)
        {
            if (ex == null) return string.Empty;

            string message = ex.Message;
            if (ex.InnerException != null)
            {
                message += "\n" + GetExceptionMessages(ex.InnerException);
            }
            return message;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing) return;

            if (DatePicker.SelectedDate > DateTime.Today.AddYears(2))
            {
                MessageBox.Show("Дата мероприятия не может быть более чем на 2 года в будущем!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DatePicker.SelectedDate = DateTime.Today;
            }
        }

        private void DatePicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9) &&
                !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) &&
                e.Key != Key.OemPeriod && e.Key != Key.Delete &&
                e.Key != Key.Back && e.Key != Key.Tab)
            {
                e.Handled = true;
            }
        }
        
        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateData()) return;

                if (_currentEvent.Id_event > 0)
                {
                    // Для существующего мероприятия - загружаем его из базы
                    var existingEvent = _context.Event.Find(_currentEvent.Id_event);
                    if (existingEvent == null)
                    {
                        MessageBox.Show("Мероприятие не найдено в базе данных!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Обновляем свойства существующей сущности
                    existingEvent.name = NameTextBox.Text.Trim();
                    existingEvent.description = DescriptionTextBox.Text.Trim();
                    existingEvent.date = DatePicker.SelectedDate.Value;
                    existingEvent.Id_location = (int)LocationComboBox.SelectedValue;
                    existingEvent.Id_event_type = (int)EventTypeComboBox.SelectedValue;
                }
                else
                {
                    int nextId = _context.Event.Any()? _context.Event.Max(ev => ev.Id_event) + 1 : 1;

                    // Для нового мероприятия - создаем новую сущность
                    var newEvent = new Event
                    {
                        Id_event = nextId,
                        name = NameTextBox.Text.Trim(),
                        description = DescriptionTextBox.Text.Trim(),
                        date = DatePicker.SelectedDate.Value,
                        Id_location = (int)LocationComboBox.SelectedValue,
                        Id_event_type = (int)EventTypeComboBox.SelectedValue
                    };
                    _context.Event.Add(newEvent);
                }

                _context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => $"{x.PropertyName}: {x.ErrorMessage}");

                MessageBox.Show($"Ошибки валидации:\n{string.Join("\n", errorMessages)}",
                    "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных: {GetExceptionMessages(ex)}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неожиданная ошибка:\n{GetExceptionMessages(ex)}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DatePicker_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            string newText = datePicker.Text + e.Text;
            e.Handled = !DateTime.TryParse(newText, out _);
        }

        private bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название мероприятия!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                NameTextBox.Focus();
                return false;
            }

            if (DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату мероприятия!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DatePicker.Focus();
                return false;
            }

            if (LocationComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите локацию!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                LocationComboBox.Focus();
                return false;
            }

            if (EventTypeComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип мероприятия!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                EventTypeComboBox.Focus();
                return false;
            }

            if (DatePicker.SelectedDate < DateTime.Today)
            {
                MessageBox.Show("Дата мероприятия не может быть в прошлом!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DatePicker.Focus();
                return false;
            }

            if (DatePicker.SelectedDate > DateTime.Today.AddYears(2))
            {
                MessageBox.Show("Дата мероприятия не может быть более чем на 2 года в будущем!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DatePicker.Focus();
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Отменяем изменения, если они были
            if (_context.ChangeTracker.HasChanges())
            {
                var result = MessageBox.Show("Отменить изменения?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;
            }

            DialogResult = false;
            Close();
        }
    }
}