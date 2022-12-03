using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateClient.ViewModel
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            this.Model = new PlotModel { Title = "Example Plot" };
            this.Model.Series.Add(new FunctionSeries(Math.Sin, 0, 10, 0.01, "sin(x)"));
        }

        public PlotModel Model { get; private set; }
    }
}
