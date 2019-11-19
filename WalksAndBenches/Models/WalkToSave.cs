using Microsoft.WindowsAzure.Storage.Table;

namespace WalksAndBenches.Models
{
    public class WalkToSave : TableEntity
    {
        public WalkToSave()
        {

        }

        public WalkToSave(string submitterName, string walkName)
        {
            PartitionKey = submitterName;
            SubmitterName = submitterName;
            RowKey = walkName;
            WalkName = walkName;
        }

        public string Description { get; set; }
        public string Location { get; set; }
        public string SubmitterName { get; set; }
        public string WalkName { get; set; }
        public string Url { get; set; }
    }
}
