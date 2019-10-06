using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalksAndBenches.Models
{
    public class WalkToDisplay
    {
        public string WalkName { get; set; }
        public string SubmittedBy { get; set; }
        public string Description { get; set; }
        public Uri StorageUrl { get; set; }
    }
}
