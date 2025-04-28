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
        private ExcersizeList() { }

        public List<ExcersizeData> excersizes { get; private set; } = new List<ExcersizeData>();

        public static ExcersizeList Instance
        {
            get 
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
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
                using (var stream = context.Assets.Open(fileName))
                //using (var reader = new StreamReader(stream, Encoding.Unicode))
                using (var reader = new StreamReader(stream, true))
                //using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // Automatically maps CSV columns to the GymData class properties
                    var records = csv.GetRecords<ExcersizeData>();
                    this.excersizes = new List<ExcersizeData>(records);
                    foreach (var record in records)
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
