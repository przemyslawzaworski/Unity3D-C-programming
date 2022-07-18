// Edit -> Project Settings -> Player -> Configuration -> Api Compatibility Level -> .NET 4.x
using UnityEngine;
using System.Data.SqlClient;

public class UnitySQL : MonoBehaviour
{
	void Start()
	{
		SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder()
		{
			DataSource = "127.0.0.1,1433",  // localhost, default MS SQL server port
			InitialCatalog = "master", // default database
			UserID = "SA",  // default username
			Password = "password",  // set password
			MultipleActiveResultSets = true
		};
		SqlConnection connection = new SqlConnection(builder.ConnectionString);
		try
		{
			SqlCommand command = connection.CreateCommand();
			connection.Open();
			command.CommandText = "SELECT Name from sys.databases;"; // show all names of available databases
			SqlDataReader reader = command.ExecuteReader();
			do
			{
				while (reader.Read())
				{
					Debug.Log(reader.GetString(0));
				}
			} while (reader.NextResult());
			reader.Close();
			connection.Close();
		}
		catch (System.Exception exception) 
		{
			Debug.Log(exception.Message);
		}
	}
}