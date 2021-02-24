using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mqtt_to_PI
{
    class PITagSearchResults
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Links
        {
            public string Self { get; set; }
            public string DataServer { get; set; }
            public string Attributes { get; set; }
            public string InterpolatedData { get; set; }
            public string RecordedData { get; set; }
            public string PlotData { get; set; }
            public string SummaryData { get; set; }
            public string Value { get; set; }
            public string EndValue { get; set; }
        }

        public class Root
        {
            public string WebId { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public string Descriptor { get; set; }
            public string PointClass { get; set; }
            public string PointType { get; set; }
            public string DigitalSetName { get; set; }
            public string EngineeringUnits { get; set; }
            public double Span { get; set; }
            public double Zero { get; set; }
            public bool Step { get; set; }
            public bool Future { get; set; }
            public int DisplayDigits { get; set; }
            public Links Links { get; set; }
        }



        //public class Links
        //{
        //    public string Next { get; set; }
        //    public string First { get; set; }
        //    public string Last { get; set; }
        //}

        //public class MatchedField
        //{
        //    public string Field { get; set; }
        //}

        //public class Links2
        //{
        //    public string Self { get; set; }
        //}

        //public class Item
        //{
        //    public string Name { get; set; }
        //    public string Description { get; set; }
        //    public List<MatchedField> MatchedFields { get; set; }
        //    public string ItemType { get; set; }
        //    public List<object> AFCategories { get; set; }
        //    public string UniqueID { get; set; }
        //    public string WebId { get; set; }
        //    public string DataType { get; set; }
        //    public Links2 Links { get; set; }
        //    public double Score { get; set; }
        //}

        //public class RootObject
        //{
        //    public int TotalHits { get; set; }
        //    public Links Links { get; set; }
        //    public List<object> Errors { get; set; }
        //    public List<Item> Items { get; set; }
        //}

    }
}
