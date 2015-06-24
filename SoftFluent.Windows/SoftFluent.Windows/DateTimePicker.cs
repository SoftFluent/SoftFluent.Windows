using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SoftFluent.Windows
{
    public class DateTimePicker : DatePicker
    {
        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register("SelectedDateTime", typeof(DateTime?), typeof(DateTimePicker),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, SelectedTimesChanged));

        public static readonly DependencyProperty TimeIntervalProperty =
            DependencyProperty.Register("TimeInterval", typeof(TimeSpan), typeof(DateTimePicker),
            new FrameworkPropertyMetadata(new TimeSpan(0, 15, 0), FrameworkPropertyMetadataOptions.AffectsRender, TimesChanged));

        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(TimeSpan), typeof(DateTimePicker),
            new FrameworkPropertyMetadata(new TimeSpan(0, 0, 0), FrameworkPropertyMetadataOptions.AffectsRender, TimesChanged));

        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(DateTimePicker),
            new FrameworkPropertyMetadata(new TimeSpan(1, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsRender, TimesChanged));

        private static void SelectedTimesChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            DateTimePicker dtp = (DateTimePicker)source;
            dtp.SelectClosestMatch();
        }

        private static void TimesChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            DateTimePicker dtp = (DateTimePicker)source;
            TimeSpan tsi = dtp.TimeInterval;
            if (tsi <= TimeSpan.Zero)
            {
                tsi = new TimeSpan(0, 15, 0);
            }
            dtp._timeControl.Items.Clear();
            TimeSpan ts = dtp.StartTime;
            do
            {
                dtp._timeControl.Items.Add(ts);
                ts += tsi;
            }
            while (ts < dtp.EndTime);
        }

        private Calendar _calendar;
        private Popup _popup;
        private readonly ListBox _timeControl;
        private TextBox _textbox;
        private bool _initialSetup;
        private bool _handleEvents = true;

        public DateTimePicker()
        {
            _timeControl = new ListBox();
            _timeControl.SelectionChanged += OnTimeControlSelectionChanged;
            _timeControl.ItemStringFormat = "{0:hh\\:mm}";
        }

        public TimeSpan? SelectedTime
        {
            get
            {
                if (!SelectedDateTime.HasValue)
                    return null;

                return SelectedDateTime.Value.TimeOfDay;
            }
            set
            {
                if (SelectedDateTime.HasValue)
                {
                    SelectedDateTime = SelectedDateTime.Value.Date + value;
                    return;
                }
                SelectedDateTime = DateTime.Now.Date + value;
            }
        }

        // it's a DateTime (not a TimeSpan so we can use the exact same bindings as for SelectedDate)
        public DateTime? SelectedDateTime
        {
            get { return (DateTime?)GetValue(SelectedDateTimeProperty); }
            set { SetValue(SelectedDateTimeProperty, value); }
        }

        public TimeSpan StartTime
        {
            get { return (TimeSpan)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        public TimeSpan EndTime
        {
            get { return (TimeSpan)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        public TimeSpan TimeInterval
        {
            get { return (TimeSpan)GetValue(TimeIntervalProperty); }
            set { SetValue(TimeIntervalProperty, value); }
        }

        protected virtual void SelectClosestMatch()
        {
            if (_timeControl.Items.Count == 0 || !SelectedTime.HasValue)
            {
                _timeControl.SelectedIndex = -1;
                if (_timeControl.Items.Count > 0)
                {
                    _timeControl.ScrollIntoView(0);
                }
                return;
            }

            TimeSpan? prev = null;
            foreach (TimeSpan ts in _timeControl.Items)
            {
                if (ts > SelectedTime.Value)
                {
                    if (prev.HasValue)
                    {
                        _handleEvents = false;
                        _timeControl.SelectedItem = prev.Value;
                        _handleEvents = true;
                    }
                    else
                    {
                        // select first
                        _timeControl.SelectedIndex = 0;
                    }

                    _timeControl.ScrollIntoView(_timeControl.SelectedItem);
                    return;
                }
                prev = ts;
            }

            // select last
            _timeControl.SelectedIndex = _timeControl.Items.Count - 1;
            _timeControl.ScrollIntoView(_timeControl.SelectedItem);
        }

        protected virtual void UpdateTextbox()
        {
            if (SelectedTime.HasValue && SelectedDate.HasValue)
            {
                string newText;
                switch (SelectedDateFormat)
                {
                    case DatePickerFormat.Long:
                        newText = SelectedDate.Value.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern);
                        break;

                    default:
                        newText = SelectedDate.Value.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                        break;
                }
                newText += " " + SelectedTime.Value;
                _textbox.Text = newText;
            }
        }

        protected virtual void OnTimeControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_handleEvents)
                return;

            if (_timeControl.SelectedIndex >= 0)
            {
                SelectedTime = (TimeSpan)_timeControl.SelectedItem;
            }
            else
            {
                SelectedTime = null;
            }

            UpdateTextbox();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                switch (e.Key)
                {
                    case Key.Space:
                    case Key.Enter:
                        _popup.IsOpen = false;
                        OnTimeControlSelectionChanged(null, null);
                        break;
                }
            }
        }

        protected override void OnSelectedDateChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectedDateChanged(e);
            if (SelectedDate.HasValue && SelectedDate.Value.TimeOfDay != TimeSpan.Zero)
            {
                SelectedTime = SelectedDate.Value.TimeOfDay;
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            TimesChanged(this, new DependencyPropertyChangedEventArgs());
        }

        protected override void OnCalendarClosed(RoutedEventArgs e)
        {
            base.OnCalendarClosed(e);
            OnTimeControlSelectionChanged(null, null);
        }

        protected override void OnCalendarOpened(RoutedEventArgs e)
        {
            base.OnCalendarOpened(e);
            // small hack when date is 1
            if (SelectedDate.HasValue && (SelectedDate.Value.Year == 1 || SelectedDate.Value.Year == 9999))
            {
                DisplayDate = DateTime.Now;
            }
            _timeControl.Height = _calendar.RenderSize.Height - 6; // I'd like to improve this
            SelectedTimesChanged(this, new DependencyPropertyChangedEventArgs());
            OnTimeControlSelectionChanged(null, null);
        }

        public override void OnApplyTemplate()
        {
            _textbox = GetTemplateChild("PART_TextBox") as TextBox;
            _textbox.TextChanged += (s, e) =>
            {
                if (!_initialSetup)
                {
                    UpdateTextbox();
                    _initialSetup = true;
                }
            };

            base.OnApplyTemplate();
            _popup = GetTemplateChild("PART_Popup") as Popup;
            if (_popup != null)
            {
                _calendar = (Calendar)_popup.Child;
                _popup.Child = null;
                Grid grid = new Grid();
                _popup.Child = grid;

                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                _timeControl.VerticalAlignment = VerticalAlignment.Top;
                _timeControl.Margin = new Thickness(3); // I'd like to improve this too
                Grid.SetColumn(_timeControl, 1);

                grid.Children.Add(_calendar);
                grid.Children.Add(_timeControl);
            }
        }
    }
}
