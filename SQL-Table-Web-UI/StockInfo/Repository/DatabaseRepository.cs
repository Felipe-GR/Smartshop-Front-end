using StockInfo.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StockInfo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;

namespace StockInfo.Repository
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly MainDbContext mainDbContext;
        //private readonly IHubContext<CustomersHub> signalContext;
        string connectionString = "";


        public DatabaseRepository(MainDbContext mainDbContext, IConfiguration configuration)
        {
            this.mainDbContext = mainDbContext;
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }


        public List<Stock> GetSearchResults(string name)
        {
            //This method will display all results within the SQL table that correlate with the given name variable/
            //If there is no name give, then by default all results within the Stocks table will be displayed.


            //SQL command text to selects all fields from Stocks table where the name parameter has a close
            //match to any of the column fields.


            string commandtext = "SELECT * FROM Stock WHERE Id LIKE @param OR ProductName LIKE @param OR ProductPrice LIKE @param";

            //Use %  on each side of name variable to work with SQL LIKE command.

            name = "%" + name + "%";

            //initialise new list instance of Stock model. 

            var results = new List<Stock>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                //Establish SQL connection and dependencies.

                conn.Open();

                SqlDependency.Start(connectionString);

                SqlCommand cmd = new SqlCommand(commandtext, conn);
                SqlDependency dependency = new SqlDependency(cmd);

                //Add the name variable to the commandText query.

                cmd.Parameters.AddWithValue("@param", name);

                //Execute reader
                var reader = cmd.ExecuteReader();

                //Read all rows until null
                while (reader.Read())
                {

                    var result = new Stock
                    {
                        //Assign class objects with the fields in search results table.

                        ItemId = (int)reader["ItemId"],
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString()
                    };

                    //Add class objects to the final search results list.

                    results.Add(result);
                }
            }
            //return search results

            return results;
        }

       

        public Stock GetStockInfo(int itemId)
        {
            //This method will get the relevant info for an item in the SQL table, with the given ItemId primary key. 
            //This method will be called when the edit modal partial view. 

            string commandtext = string.Format("SELECT * FROM Stock where Id = {0} ", itemId);

            //initialise new list instance of Stock model. 

            var result = new Stock();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //Establish SQL connection and dependencies.

                conn.Open();

                SqlDependency.Start(connectionString);

                SqlCommand cmd = new SqlCommand(commandtext, conn);
                SqlDependency dependency = new SqlDependency(cmd);

                //Execute reader

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result = new Stock
                    {
                        //Assign class objects with the fields in search results table.

                        ItemId = (int)reader["ItemId"],
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString()
                    };

                    //Add class objects to the final search results list.


                    //return search results

                    return result;
                }
            }

            return result;
        }

        public void DeleteStock(int id)
        {
            string commandText = "DELETE FROM Stock WHERE Id = @id";


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlDependency.Start(connectionString);
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteNonQuery();
            }

        }

        public void UpdateStockInfo(int id, string name, string description)
        {
            string commandText = "UPDATE Stock SET ProductName = @name ,  ProductDescription = @description WHERE Id = @itemId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlDependency.Start(connectionString);
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.Parameters.AddWithValue("@itemId", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@description", description);

                var reader = cmd.ExecuteNonQuery();
            }
        }

        public void AddStock(string name, string description)
        {
            string commandText = "INSERT INTO Stock VALUES (@name, @description)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlDependency.Start(connectionString);
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@description", description);

                var reader = cmd.ExecuteNonQuery();
            }



          
        }

       


        public List<Stock> GetStockInfo()
        {
            string commandtext = "SELECT * FROM Stock";  // Can Please change the table name ok
            var results = new List<Stock>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //establish Sql Connection 
                conn.Open();

                SqlDependency.Start(connectionString);

                SqlCommand cmd = new SqlCommand(commandtext, conn);
                SqlDependency dependency = new SqlDependency(cmd);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var result = new Stock
                    {
                        ItemId = (int)reader["ItemId"],
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString()
                    };

                    results.Add(result);
                }
            }
            return results;
        }


    }
}