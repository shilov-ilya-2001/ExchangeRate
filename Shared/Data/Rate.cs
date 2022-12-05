using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public class Rate
    {
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public int Amount { get; set; }
    }
}
