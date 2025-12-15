using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace extract_slip_api.Models
{
    public class ResultData
    {
        public dynamic? result { get; set; }
        public int status { get; set; }
        public string? message { get; set; }
    }
}