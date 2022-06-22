using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System;


public class FishData
{
    public string Name { get; set; }
    public float Size { get; set; }
    public string Explanation { get; set; }
    public int Price { get; set; }

    public int MaxHP { get; set; }
    public int RecoveryRate { get; set; }
    public float Resistration { get; set; }
}

public static class FishingData
{
    private static DataSet Stage;

    private static DataTable upStream;
    public static DataTable UpStream { 
        get
        {
            if (upStream == null)
            {
                upStream = new DataTable("upStream");

                DataColumn column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Name";
                column.ReadOnly = true;
                column.Unique = true;
                upStream.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Single");
                column.ColumnName = "Probability";
                column.ReadOnly = true;
                column.Unique = false;
                upStream.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Single");
                column.ColumnName = "Bite 1";
                column.ReadOnly = true;
                column.Unique = false;
                upStream.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Single");
                column.ColumnName = "Bite 2";
                column.ReadOnly = true;
                column.Unique = false;
                upStream.Columns.Add(column);


                DataRow row;
                var dataSet = CSVParser.Read("DataTable/UpStream");
                foreach (var data in dataSet)
                {
                    row = upStream.NewRow();
                    row["Name"] = data.Key.ToString();
                    row["Bite 1"] = float.Parse(data.Value["Bite 1"].ToString().Replace('%', ' ')) / 100f;
                    row["Bite 2"] = float.Parse(data.Value["Bite 2"].ToString().Replace('%', ' ')) / 100f;
                    upStream.Rows.Add(row);
                }

                return upStream;
            }
            else
                return upStream;
        } 
    }

    private static DataTable fishDataTable;
    public static DataTable FishDataTable
    {
        get
        {
            if (fishDataTable == null)
            {
                fishDataTable = new DataTable("Fish");
                DataColumn column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Name";
                column.ReadOnly = true;
                column.Unique = true;
                fishDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Single");
                column.ColumnName = "MinSize";
                column.ReadOnly = true;
                column.Unique = false;
                fishDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Single");
                column.ColumnName = "MaxSize";
                column.ReadOnly = true;
                column.Unique = false;
                fishDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Explanation";
                column.ReadOnly = true;
                column.Unique = false;
                fishDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Int32");
                column.ColumnName = "Price";
                column.ReadOnly = true;
                column.Unique = false;
                fishDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Int32");
                column.ColumnName = "MaxHP";
                column.ReadOnly = true;
                column.Unique = false;
                fishDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Single");
                column.ColumnName = "RecoveryRate";
                column.ReadOnly = true;
                column.Unique = false;
                fishDataTable.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Single");
                column.ColumnName = "Registration";
                column.ReadOnly = true;
                column.Unique = false;
                fishDataTable.Columns.Add(column);

                DataRow row;
                var dataSet = CSVParser.Read("DataTable/FishDataTable");
                foreach (var data in dataSet)
                {
                    row = fishDataTable.NewRow();
                    row["Name"] = data.Key.ToString();
                    row["MinSize"] = data.Value["MinSize"];
                    row["MaxSize"] = data.Value["MaxSize"];
                    row["Explanation"] = data.Value["Explanation"];
                    row["Price"] = data.Value["Price"];
                    row["MaxHP"] = data.Value["MaxHP"];
                    row["RecoveryRate"] = data.Value["RecoveryRate"];
                    row["Registration"] = data.Value["Registration"];
                    fishDataTable.Rows.Add(row);
                }

                return fishDataTable;
            }
            else
                return fishDataTable;
        }
    }
}