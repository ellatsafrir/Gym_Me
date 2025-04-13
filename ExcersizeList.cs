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

        //public ExcersizeData(string name, string desc, string muscles, string videoLink) 
        //{
        //    this.Name = name;
        //    this.Description = desc;
        //    this.Muscles = muscles;
        //    this.VideoLink = videoLink;
        //}

        //public ExcersizeData(ExcersizeData val)
        //{
        //    //this.Name = (String)val.Name.Clone();
        //    //this.Description = (String)val.Description.Clone();
        //    //this.Muscles = (String)val.Muscles.Clone();
        //    //this.VideoLink = (String)val.VideoLink.Clone();
        //    this.Name = val.Name;
        //    this.Description = val.Description;
        //    this.Muscles = val.Muscles;
        //    this.VideoLink = val.VideoLink;
        //}
        [Name("Name")] public required string Name { get; set; }
        [Name("Description")] public required string Description { get; set; }
        [Name("Muscles")] public required string Muscles { get; set; }
        [Name("VideoLink")] public required string VideoLink { get; set; }
        //public override string ToString()
        //{
        //    return $"Name: {Name}, Desc: {Description}, Muscles: {Muscles}, Link: {VideoLink}";
        //}
    }
    public class ExcersizeList
    {
        public List<ExcersizeData> excersizes { get; set; }

        public ExcersizeList()
        {
            this.excersizes = new List<ExcersizeData>();
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
            foreach (var excersize in excersizes)
            {
                if (excersize.Name == excersizeName)
                {
                    returnVal = excersize with { };
                    break;
                }
            }
            return returnVal;
        }
    }
}
