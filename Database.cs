// Patch notes - sorting
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;

public class Database : MonoBehaviour
{
	void Start()
	{
		DataTable table = new DataTable();
		table.Columns.Add("FirstColumn", typeof(int));
		table.Columns.Add("SecondColumn", typeof(int));
		table.Columns.Add("ThirdColumn", typeof(int));	
		table.Rows.Add(0, 9,  5);
		table.Rows.Add(1, 2, 80);
		table.Rows.Add(1, 0,  2);
		table.Rows.Add(1, 0,  1);
		table.Rows.Add(0, 9,  7);
		table.Rows.Add(0, 9,  6);
		table.Rows.Add(0, 8,  8);
		table.Rows.Add(1, 4, 12);
		table.Rows.Add(0, 3, 32);
		table.Rows.Add(2, 6,  1);
		table.Rows.Add(0, 9,  2);
		table.Rows.Add(1, 2, 40);
		table.Rows.Add(2, 5, 95);
		table.Rows.Add(1, 8, 21);
		table.Rows.Add(0, 8, 69);
		table.Rows.Add(1, 2, 80);
		table.Rows.Add(0, 7, 70);

		DataView dv = table.DefaultView;
		dv.Sort = "FirstColumn, SecondColumn, ThirdColumn";
		DataTable tablesort = dv.ToTable();
		foreach (DataRow row in tablesort.Rows)
		{
			Debug.Log(row.ItemArray[0].ToString()+"-"+row.ItemArray[1].ToString()+"-"+row.ItemArray[2].ToString());          
		}
	}
}
