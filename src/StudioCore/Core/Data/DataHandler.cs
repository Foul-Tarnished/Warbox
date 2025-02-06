using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using static Assimp.Metadata;

namespace StudioCore.Core.Data;

public class DataHandler
{
    private bool SetupData = false;

    public Dictionary<DataStatus, XDocument> Localization = new Dictionary<DataStatus, XDocument>();
    public Dictionary<DataStatus, XDocument> Tables = new Dictionary<DataStatus, XDocument>();
    public Dictionary<DataStatus, XDocument> Scripts = new Dictionary<DataStatus, XDocument>();

    public DataHandler() { }

    public void OnGui()
    {
        if (!SetupData && Warbox.DataRoot != "" && Warbox.ProjectDataRoot != "")
        {
            SetupData = true;

            Tables = SetupDataFromPak("Data", "Tables");
            Scripts = SetupDataFromPak("Data", "Scripts");
            Localization = SetupDataFromPak("Localization", "English_xml");
        }
    }

    public Dictionary<DataStatus, XDocument> SetupDataFromPak(string folderName, string pakName)
    {
        if (Warbox.DataRoot == "")
            return new Dictionary<DataStatus, XDocument>();

        var dataDir = $"{Warbox.DataRoot}\\{folderName}\\{pakName}.pak";
        var projectDir = $"{Warbox.ProjectDataRoot}\\{folderName}\\";

        var baseData = ReadXmlFromZip(dataDir);
        var projectData = ReadXmlFromDirectory(projectDir);
        var finalData = new Dictionary<DataStatus, XDocument>();

        // Replace entries with project data if present
        if (projectData.Count > 0)
        {
            foreach (var bEntry in baseData)
            {
                var bDataStatus = bEntry.Key;
                var hasProjectVersion = false;

                foreach (var pEntry in projectData)
                {
                    var pDataStatus = pEntry.Key;

                    // Is match for existing file, override
                    if (bDataStatus.Name == pDataStatus.Name || pDataStatus.Name.Contains(bDataStatus.Name) && pDataStatus.Name.Contains("__"))
                    {
                        hasProjectVersion = true;

                        pEntry.Key.IsProjectData = true;
                        if(finalData.ContainsKey(pEntry.Key))
                        {
                            finalData[pEntry.Key] = pEntry.Value;
                        }
                        else
                        {
                            if(pDataStatus.Name.Contains(bDataStatus.Name) && 
                                pDataStatus.Name.Contains("__"))
                            {
                                finalData.Add(pDataStatus, pEntry.Value);
                            }
                        }
                    }
                    // Is unique to project, new file
                    else
                    {
                        if (!finalData.ContainsKey(pEntry.Key))
                        {
                            finalData.Add(pDataStatus, pEntry.Value);
                        }
                    }
                }

                // Is not affected by project, vanilla
                if(!hasProjectVersion)
                {
                    finalData.Add(bDataStatus, bEntry.Value);
                }
            }
        }
        else
        {
            finalData = baseData;
        }

        return finalData;
    }

    private Dictionary<DataStatus, XDocument> ReadXmlFromZip(string zipPath)
    {
        var xmlFiles = new Dictionary<DataStatus, XDocument>();

        using (FileStream zipStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
        using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    using (Stream entryStream = entry.Open())
                    {
                        try
                        {
                            string xmlContent;
                            Encoding encoding = DetectEncoding(entryStream, out xmlContent);

                            using (StringReader stringReader = new StringReader(xmlContent))
                            {
                                XDocument xmlDoc = XDocument.Load(stringReader);

                                var name = Path.GetFileNameWithoutExtension(entry.FullName);
                                var dataStatus = new DataStatus(name, entry.FullName);

                                xmlFiles[dataStatus] = xmlDoc;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading {entry.FullName}: {ex.Message}");
                        }
                    }
                }
            }
        }

        return xmlFiles;
    }

    public Dictionary<DataStatus, XDocument> ReadXmlFromDirectory(string directoryPath)
    {
        var xmlFiles = new Dictionary<DataStatus, XDocument>();

        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"Directory '{directoryPath}' does not exist.");
            return xmlFiles;
        }

        foreach (string filePath in Directory.GetFiles(directoryPath, "*.xml", SearchOption.AllDirectories))
        {
            try
            {
                string xmlContent;
                Encoding encoding = DetectEncoding(filePath, out xmlContent);

                using (StringReader stringReader = new StringReader(xmlContent))
                {
                    XDocument xmlDoc = XDocument.Load(stringReader);

                    var name = Path.GetFileNameWithoutExtension(filePath);
                    var dataStatus = new DataStatus(name, filePath);
                    xmlFiles[dataStatus] = xmlDoc;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading {filePath}: {ex.Message}");
            }
        }

        return xmlFiles;
    }

    private Encoding DetectEncoding(Stream stream, out string xmlContent)
    {
        using (StreamReader reader = new StreamReader(stream, Encoding.Default, detectEncodingFromByteOrderMarks: true))
        {
            xmlContent = reader.ReadToEnd();

            var match = System.Text.RegularExpressions.Regex.Match(xmlContent, @"<\?xml\s+.*?encoding=['""](.+?)['""]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
            {
                try
                {
                    return Encoding.GetEncoding(match.Groups[1].Value);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"Warning: Unsupported encoding '{match.Groups[1].Value}', defaulting to UTF-8.");
                }
            }

            return Encoding.UTF8; 
        }
    }

    private Encoding DetectEncoding(string filePath, out string xmlContent)
    {
        using (StreamReader reader = new StreamReader(filePath, Encoding.Default, detectEncodingFromByteOrderMarks: true))
        {
            xmlContent = reader.ReadToEnd();

            // Check for encoding declaration inside XML
            var match = System.Text.RegularExpressions.Regex.Match(xmlContent, @"<\?xml\s+.*?encoding=['""](.+?)['""]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
            {
                try
                {
                    return Encoding.GetEncoding(match.Groups[1].Value);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"Warning: Unsupported encoding '{match.Groups[1].Value}' in {filePath}, defaulting to UTF-8.");
                }
            }

            return Encoding.UTF8; // Default to UTF-8 if encoding is not specified
        }
    }

    public static void ZipXmlFiles(string sourceDirectory, string zipFilePath)
    {
        if (!Directory.Exists(sourceDirectory))
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");

        string[] xmlFiles = Directory.GetFiles(sourceDirectory, "*.xml", SearchOption.AllDirectories);

        if (xmlFiles.Length == 0)
            throw new Exception("No XML files found in the directory.");

        using (FileStream zipToCreate = new FileStream(zipFilePath, FileMode.Create))
        using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
        {
            foreach (string file in xmlFiles)
            {
                string relativePath = Path.GetRelativePath(sourceDirectory, file);

                archive.CreateEntryFromFile(file, relativePath);
            }
        }
    }
}
