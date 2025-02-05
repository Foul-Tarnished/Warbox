using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudioCore.KCD;

public class KCDText
{
    public List<Row> Rows { get; set; } = new List<Row>();

    public KCDText(XDocument doc) 
    {
        foreach (var rowElement in doc.Descendants("Row"))
        {
            var row = new Row();
            foreach (var cell in rowElement.Elements("Cell"))
            {
                row.Cells.Add(cell.Value);
            }

            Rows.Add(row);
        }
    }

    public XDocument ExportXML()
    {
        XElement tableElement = new XElement("Table");
        foreach (var row in Rows)
        {
            XElement rowElement = new XElement("Row");
            foreach (var cell in row.Cells)
            {
                rowElement.Add(new XElement("Cell", cell));
            }
            tableElement.Add(rowElement);
        }
        return new XDocument(tableElement);
    }

    public void AddRow(params string[] cells)
    {
        Rows.Add(new Row(cells));
    }
    public class Row
    {
        public List<string> Cells { get; set; } = new List<string>();

        public Row() { }

        public Row(params string[] cells)
        {
            Cells.AddRange(cells);
        }

        public Row DeepCopy()
        {
            return (Row)this.MemberwiseClone();
        }

        public Row NewCopy()
        {
            var newRow = new Row();
            foreach(var cell in Cells)
            {
                newRow.Cells.Add(cell);
            }

            return newRow;
        }
    }
}

