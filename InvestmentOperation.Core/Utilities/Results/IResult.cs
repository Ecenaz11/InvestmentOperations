using System;
using System.Collections.Generic;
using System.Text;

namespace InvestmentOperation.Core.Utilities.Results
{
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
    }
}
