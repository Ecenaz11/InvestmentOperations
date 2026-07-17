using System;
using System.Collections.Generic;
using System.Text;

namespace InvestmentOperation.Core.Utilities.Results
{
    public class SuccessResult : Result
    {
        public SuccessResult() : base (true,string.Empty)
        {
        }

        public SuccessResult(string message) : base (true, message)
        {
        }
    }
}
