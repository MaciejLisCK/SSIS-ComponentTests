using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.Utitlities
{
    public class UpperCaseConverter
    {
        public string Convert(string value)
        {
            var uppercase = value.ToUpper();

            return uppercase;
        }
    }
}
