using StudioCore.KCD.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace StudioCore.KCD;

public static class XmlHelper
{
    public static void Example()
    {
        string xmlPath = "rpg_params.xml";
        XDocument doc = XDocument.Load(xmlPath);
        RpgParam db = DeserializeFromXDocument<RpgParam>(doc);

        Console.WriteLine($"Database Name: {db.Name}, Version: {db.RpgParams.Version}");

        foreach (var param in db.RpgParams.RpgParamList)
        {
            Console.WriteLine($"Key: {param.Key}, Value: {param.Value}");
        }
    }

    public static T DeserializeFromFile<T>(string filePath)
    {
        XmlSerializer serializer = new(typeof(T));
        using StreamReader reader = new(filePath);
        return (T)serializer.Deserialize(reader);
    }

    public static T DeserializeFromXDocument<T>(XDocument xdoc)
    {
        XmlSerializer serializer = new(typeof(T));
        using var reader = xdoc.CreateReader();
        return (T)serializer.Deserialize(reader);
    }
}
