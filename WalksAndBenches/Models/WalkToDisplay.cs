using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalksAndBenches.Models
{
    public class WalkToDisplay
    {
        public string Walk { get; set; }
        public string SubmittedBy { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }
    }
}
