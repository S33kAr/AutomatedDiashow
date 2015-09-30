using System.IO;
using System.Xml.Serialization;

namespace AutomatedDiashow
{
    public class Config
    {
        public static Config Load(string filename = "config.xml")
        {
            if (File.Exists(filename))
            {
                using (var configFile = File.OpenRead(filename))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Config));
                    Config config = ser.Deserialize(configFile) as Config;
                    return config;
                }
            }
            return new Config();
        }

        private Config()
        {

        }

        public string ImagesFolder { get; set; }
        public int ImageDuration { get; set; }
        public int ImagesCount { get; set; }

        public int MinimumImageCycle { get; set; }

        public string CommercialsFolder { get; set; }
        public int CommercialsDuration { get; set; }
        public int CommercialsCount { get; set; }

        public string FileExtension { get; set; }
    }
}
