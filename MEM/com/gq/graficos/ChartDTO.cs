using System.Collections.Generic;

namespace MEM.com.gq.graficos
{

    public class ChartCollectionDto
    {
        public List<ChartDto> Charts { get; set; } = new List<ChartDto>();
        public Dictionary<string, string> LabelsX { get; set; } = new Dictionary<string, string>();
        public int Intervalos { get; set; }
        public double YmaxValue { get; set; }
        public double YminValue { get; set; }
    }

    public class ChartDto
    {
        public const string TYPE_AREA = "area";
        public const string TYPE_LINEA = "line";
        public const string TYPE_BAR = "bar";

        public string key { get; set; }
        public object y { get; set; }
        public string type { get; set; }
        public int yAxis { get; set; }
        public bool? area { get; set; }
        public int? strokeWidth { get; set; }
        public string classed { get; set; }
        public string color { get; set; }
        public int order { get; set; } = 0;
        public bool bar { get; set; }
        

        public List<ChartValuesDto> values { get; set; } = new List<ChartValuesDto>();
    }

    public class ChartValuesDto
    {
        public object x { get; set; }
        public object y { get; set; }
        public string label { get; set; }
        public string key { get; set; }
    }
}

