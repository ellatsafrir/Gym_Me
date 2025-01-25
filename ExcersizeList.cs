﻿using Android.Content;
using CsvHelper;
using System.Globalization;

namespace Gym_Me
{
    public class GymData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Muscles { get; set; } 
        public string VideoLink { get; set; }
        public override string ToString()
        {
            return $"Name: {Name}, Desc: {Description}, Muscles: {Muscles}, Link: {VideoLink}";
        }
    }
    public class ExcersizeList
    {
        public List<GymData> excersizes { get; set; }

        public ExcersizeList()
        {
            this.excersizes = new List<GymData>();
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
                    var records = csv.GetRecords<GymData>();
                    this.excersizes = new List<GymData>(records);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
            }
        }
    }
}
