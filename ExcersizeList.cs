using Android.Content;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Java.Util.Concurrent;
using System.Globalization;

namespace Gym_Me
{
    [Serializable]
    public record ExcersizeData
    {
        [Ignore] public required int Id { get; set; } = -1;
        [Name("Name")] public required string Name { get; set; }
        [Name("Description")] public required string Description { get; set; }
        [Name("Muscles")] public required string Muscles { get; set; }
        [Name("VideoLink")] public required string VideoLink { get; set; }
    }
    public class ExcersizeList
    {
        private static ExcersizeList _instance;
        private static readonly object _lock = new object();
        private ExcersizeList() { } // step 1: private constructure ensures that only this class can create itself

        public List<ExcersizeData> excersizes { get; private set; } = new List<ExcersizeData>();

        // singelton design pattern: 
        // its a concept of modles to solve common problems. 
        // singletone ensures that only one instence is in the system 
        
        // getter property that can create the class and return it 
        public static ExcersizeList Instance
        {
            get 
            {
                if (_instance == null)
                {
                    lock (_lock) // to ensure that only one thread can use it at a time
                    {
                        if (_instance == null) // could have changed during the lock 
                        {
                            _instance = new ExcersizeList();
                        }
                    }
                }
                return _instance;
            }
        }

        public void LoadCsvFromAssets(Context context)
        {
            string fileName = "excersize.csv";
            try
            {
                using (var stream = context.Assets.Open(fileName)) // open the file and basic operations
                using (var reader = new StreamReader(stream, true))// stream reader. easier way to read the file
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) // parse stream into data
                {
                    // Automatically maps CSV columns to the GymData class properties
                    var records = csv.GetRecords<ExcersizeData>(); // returns IEnumerable<ExcersizeData>, iterator
                    this.excersizes = new List<ExcersizeData>(records); // Transforms this into a list
                    foreach (var record in records) // for debug
                    {
                        Console.WriteLine( record.Name + record.Description + record.Muscles + record.VideoLink);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
            }
        }
        public ExcersizeData GetExcersizeData(string excersizeName)
        {
            ExcersizeData returnVal = null;
            int cnt = 0;
            foreach (var excersize in excersizes)
            {
                if (excersize.Name == excersizeName)
                {
                    excersize.Id = cnt;
                    returnVal = excersize with { };
                    break;
                }
                cnt++;
            }
            return returnVal;
        }

        public ExcersizeData GetExcersizeData(int excersizeId)
        {
            if (excersizeId > 0)
            {
                return excersizes[excersizeId] with { };
            }
            return null;
        }
    }
}
