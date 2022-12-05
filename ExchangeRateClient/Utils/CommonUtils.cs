using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExchangeRateClient.Utils
{
    public static class CommonUtils
    {
        public static void ErrorMessageBoxShow(string messageText)
        {
            MessageBox.Show(messageText, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
