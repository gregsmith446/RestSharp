using Newtonsoft.Json;
using RestSharp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace RestSharper
{
    class Program
    {
        static void Main(string[] args)
        {
            // start point
            var client = new RestClient("https://jsonplaceholder.typicode.com/");

            // navigate to https://jsonplaceholder.typicode.com/users
            var request = new RestRequest("/users", Method.GET);

            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content; // requested raw content available as string

            // Console.Write(content);

            // convert the string to an array of objects
            var dataAsJsonObject = JsonConvert.DeserializeObject(content);
            // could also convert this using fromObject

            // Console.Write(dataAsJsonObject);

            // parse the content into a jArray
            JArray dataAsSeparateObjects = JArray.Parse(content);

            var dbConnString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Parsing;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            using (SqlConnection connection = new SqlConnection(dbConnString))
            {
                connection.Open();

                // convert the JArray of objects into separate JSON objects
                foreach (var user in dataAsSeparateObjects)
                {
                    // Console.Write(item);
                    // SQL Client Documentation on writing SQL commands in C#
                    // Values uses placeholders in the initial SqlCommand in said format "@example"
                    SqlCommand insertStatement = new SqlCommand("INSERT into [User] (Id, name, username, email) VALUES (@id, @name, @username, @email)", connection);
                    insertStatement.Parameters.AddWithValue("@id", user["id"].ToObject<int>());
                    insertStatement.Parameters.AddWithValue("@name", user["name"].ToString());
                    insertStatement.Parameters.AddWithValue("@username", user["username"].ToString());
                    insertStatement.Parameters.AddWithValue("@email", user["email"].ToString());

                    // execute these above queries
                    insertStatement.ExecuteNonQuery();
                }

                Console.WriteLine("Database Updated");
                connection.Close();
            } 
        }
    }
}
