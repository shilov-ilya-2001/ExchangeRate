using ExchangeRateClient.Model;
using ExchangeRateClient.Services;
using Microsoft.VisualBasic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using Shared.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExchangeRateClient.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Constants

        private const int DateForLastYears = 5;

        #endregion

        #region Fields

        private string? _selectedCurrency;
        private DateTime _startDate;
        private DateTime _endDate;

        private RelayCommand? _validateStartDateCommand;
        private RelayCommand? _validateEndDateCommand;
        private RelayCommand? _getCurrencyDataCommand;

        private ApiService _apiService;

        #endregion

        #region Properties

        public ObservableCollection<string> Currencies { get; set; }
        public string? SelectedCurrency
        {
            get { return _selectedCurrency; }
            set
            {
                _selectedCurrency = value;
                OnPropertyChanged("SelectedCurrency");
            }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }

        public PlotModel MyModel { get; set; }

        public RelayCommand ValidateStartDateCommand
        {
            get
            {
                return _validateStartDateCommand ??
                    (_validateStartDateCommand = new RelayCommand(obj =>
                    {
                        DateTime? date = (DateTime?)obj;
                        if (date.HasValue)
                        {
                            if (!IsDateForLastYears(date.Value, DateForLastYears) || date > DateTime.Today)
                            {
                                StartDate = DateTime.Today;
                            }
                        }
                    }));
            }
        }

        public RelayCommand ValidateEndDateCommand
        {
            get
            {
                return _validateEndDateCommand ??
                    (_validateEndDateCommand = new RelayCommand(obj =>
                    {
                        DateTime? date = (DateTime?)obj;
                        if (date.HasValue)
                        {
                            if (!IsDateForLastYears(date.Value, DateForLastYears) || date < StartDate || date > DateTime.Today)
                            {
                                EndDate = DateTime.Today;
                            }
                        }
                    }));
            }
        }

        public RelayCommand GetCurrencyDataCommand
        {
            get
            {
                return _getCurrencyDataCommand ??
                    (_getCurrencyDataCommand = new RelayCommand(async obj =>
                    {
                        string? selectedCurrency = (string?)obj;
                        if (!string.IsNullOrEmpty(selectedCurrency))
                        {
                            List<Rate> rates = await _apiService.GetCurrencyRates(SelectedCurrency, StartDate, EndDate);
                            if (rates.Count > 0)
                            {
                                DisplayRateModel(rates);
                            }
                        }
                    }));
            }
        }

        #endregion

        #region Events

        

        #endregion

        #region Constructors

        public MainViewModel()
        {
            Currencies = new ObservableCollection<string> { "USD", "EUR", "RUB" };
            SelectedCurrency = Currencies[1];
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;

            _apiService = new ApiService();

            this.MyModel = new PlotModel { Title = "Анализ курса валют" };
        }

        #endregion

        #region Methods

        private bool IsDateForLastYears(DateTime date, int years)
        {
            return date >= DateTime.Today.AddYears(-years + 1);
        }

        private void DisplayRateModel(List<Rate> rates)
        {
            MyModel.Series.Clear();
            MyModel.Axes.Clear();

            LineSeries lineSeries = new LineSeries();
            lineSeries.Color = OxyColor.FromRgb(0, 0, 255);
            lineSeries.MarkerFill = OxyColor.FromRgb(255, 0, 0);
            lineSeries.MarkerType = MarkerType.Circle;

            for (int i = 0; i < rates.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(rates[i].Date), rates[i].Value));
            }

            MyModel.Axes.Add(new DateTimeAxis 
            { 
                Position = AxisPosition.Bottom, 
                StringFormat = "dd.MM.yyyy",
            });

            MyModel.Series.Add(lineSeries);

            MyModel.InvalidatePlot(true);
        }


        #endregion
    }
}
