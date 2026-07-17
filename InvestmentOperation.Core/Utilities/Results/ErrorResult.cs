using System;
using System.Collections.Generic;
using System.Text;

namespace InvestmentOperation.Core.Utilities.Results
{
    public class ErrorResult : Result
    {
        public ErrorResult() : base(false, string.Empty)
        {
        }

        public ErrorResult(string message) : base (false, message)
        {
        }
    }
}
