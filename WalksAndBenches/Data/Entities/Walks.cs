using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalksAndBenches.Data.Entities
{
    public class Walks
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string SubmitterName { get; set; }
        public string WalkName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
    }
}
