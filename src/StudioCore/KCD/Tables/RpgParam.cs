using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace StudioCore.KCD.Tables;

[XmlRoot("database", Namespace = "", IsNullable = false)]
public class Database
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlElement("rpg_params")]
    public RpgParams RpgParams { get; set; }
}

public class RpgParams
{
    [XmlAttribute("version")]
    public int Version { get; set; }

    [XmlElement("rpg_param")]
    public List<RpgParam> RpgParamList { get; set; } = new();
}

public class RpgParam
{
    [XmlAttribute("rpg_param_key")]
    public string Key { get; set; }

    [XmlAttribute("rpg_param_value")]
    public string Value { get; set; }
}

