using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using Roommates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Roommates.Repositories
{
    /// <summary>
    ///  This class is responsible for interacting with Room data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    public class ChoreRepository : BaseRepository
    {
        /// <summary>
        ///  When new ChoreRepository is instantiated, pass the connection string along to the BaseRepository
        /// </summary>
        public ChoreRepository(string connectionString) : base(connectionString) { }
        // ...We'll add some methods shortly...
        /// <summary>
        ///  Get a list of all Chores in the database
        /// </summary>
        public List<Chore> GetAll()
        {
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.
            using (SqlConnection conn = Connection)
            {
                // Note, we must Open() the connection, the "using" block doesn't do that for us.
                conn.Open();
                // We must "use" commands too.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = "SELECT Id, Name FROM Chore";
                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();
                    // A list to hold the chores we retrieve from the database.
                    List<Chore> chores = new List<Chore>();
                    // Read() will return true if there's more data to read
                    while (reader.Read())
                    {
                        // The "ordinal" is the numeric position of the column in the query results.
                        //  For our query, "Id" has an ordinal value of 0 and "Name" is 1.
                        int idColumnPosition = reader.GetOrdinal("Id");
                        // We user the reader's GetXXX methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(idColumnPosition);
                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);
                        // Now let's create a new chore object using the data from the database.
                        Chore chore = new Chore
                        {
                            Id = idValue,
                            Name = nameValue,
                        };
                        // ...and add that chore object to our list.
                        chores.Add(chore);
                    }
                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();
                    // Return the list of rooms who whomever called this method.
                    return chores;
                }
            }
        }
        /// <summary>
        ///  Returns a single chore with the given id.
        /// </summary>
        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    Chore chore = null;
                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        chore = new Chore
                        {
                            Id = id,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };
                    }
                    reader.Close();
                    return chore;
                }
            }
        }
        /// <summary>
        ///  Add a new chore to the database
        ///   NOTE: This method sends data to the database,
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore (Name)
                                         OUTPUT INSERTED.Id
                                         VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    int id = (int)cmd.ExecuteScalar(); //method does two things: First, it executes the SQL command against the database. Then it looks at the first thing that the database sends back(in our case this is just the Id it created for the room) and returns it.
                    chore.Id = id;
                }
            }
        }

        public void Update(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Chore
                                SET Name = @name
                                WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    cmd.Parameters.AddWithValue("@id", chore.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool IsChoreAssigned(int choreId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT COUNT(*) FROM RoommateChore
                                WHERE ChoreId = @choreId";
                    cmd.Parameters.AddWithValue("@choreId", choreId);

                    int count = (int)cmd.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        public void DeleteChore(int id)
        {
            if (IsChoreAssigned(id))
            {
                throw new Exception("Cannot delete chore. It is assigned to one or more roommates.");
            }

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public List<Chore> GetUnassignedChores()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT c.Id, c.Name
                        FROM Chore c
                        LEFT JOIN RoommateChore rc ON c.Id = rc.ChoreId
                        WHERE rc.RoommateId IS NULL";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Chore> unassignedChores = new List<Chore>();
                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumnPosition);
                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);
                        Chore chore = new Chore
                        {
                            Id = idValue,
                            Name = nameValue
                        };
                        unassignedChores.Add(chore);
                    }
                    reader.Close();
                    return unassignedChores;
                }
            }
            // when this method is finished we can look in the database and see the new room.
        }
    }
}