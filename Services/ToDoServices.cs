using System.Data.SqlClient;
using ToDoListAPI.Interface;
using ToDoListAPI.Models;

namespace ToDoListAPI.Services
{
    public class ToDoServices : IToDoList
    {

        private static IConfiguration _IConfiguration;
        private string _connectionString;


        public ToDoServices(IConfiguration configuration)
        {

            _connectionString = configuration.GetConnectionString("DefaultConnection");


        }
        private readonly List<ToDoItem> _todoitem = new();


        public List<ToDoItem> GetAllTodosList()
        {
            var todoList = new List<ToDoItem>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM TodoItems";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                todoList.Add(new ToDoItem
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Status = Convert.ToBoolean(reader["Status"])
                                });
                            }
                        }
                    }
                }

                return todoList;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");

            }

        }


        public ToDoItem GetDetailsById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID must be greater than zero.", nameof(id));
            }
            ToDoItem todo = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM TodoItems WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                todo = new ToDoItem
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Status = Convert.ToBoolean(reader["Status"])
                                };
                            }
                        }
                    }
                }
                if (todo == null)
                {
                    throw new KeyNotFoundException($"TODO item with ID {id} was not found.");
                }
                return todo;

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred {id}: {ex.Message}");
            }

        }

        public void CreateTodoItem(ToDoItem todo)
        {
            if (string.IsNullOrWhiteSpace(todo.Title))
            {
                throw new ArgumentException("Title cannot be empty or null.", nameof(todo.Title));
            }
            try
            {
                if (CheckTodoItemExist(todo.Title))
                {
                    throw new ArgumentException("A TODO item with the same title already exists.");
                }
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO TodoItems (Title, Description, Status) VALUES (@Title, @Description, @Status)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Title", todo.Title);
                        command.Parameters.AddWithValue("@Description", todo.Description);
                        command.Parameters.AddWithValue("@Status", todo.Status);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"An error occurred while adding a new TODO item: {ex.Message}");
            }

        }
        public bool CheckTodoItemExist(string title)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM TodoItems WHERE Title = @Title";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", title);
                    int count = (int)command.ExecuteScalar();
                    return count > 0; 
                }
            }
        }
        public string UpdateDetails(int Id,ToDoItem todo)
        {
            if (todo == null)
            {
                throw new ArgumentNullException(nameof(todo), "Todo item cannot be null.");
            }

            if (string.IsNullOrEmpty(todo.Title))
            {
                throw new ArgumentException("Title is required.", nameof(todo.Title));
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE TodoItems SET Title = @Title, Description = @Description, Status = @Status WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        command.Parameters.AddWithValue("@Title", todo.Title);
                        command.Parameters.AddWithValue("@Description", todo.Description);
                        command.Parameters.AddWithValue("@Status", todo.Status);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new KeyNotFoundException($"Todo item with Id {todo.Id} was not found.");
                        }
                    }

                }
                return "Successfully updated the TODO item.";

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the TODO item.", ex);
            }

        }

        public string Delete(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id must be greater than zero.", nameof(id));
            }
            try
            {
                bool exists = false;
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(1) FROM TodoItems WHERE Id = @Id";

                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Id", id);
                        exists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;
                    }
                }

                if (!exists)
                {
                    return $"Todo item with Id {id} does not exist.";
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM TodoItems WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
                return "Successfully deleted the TODO item.";
            }
            catch (Exception ex)
            {

                throw new Exception("An error occurred while deleting the TODO item.", ex);
            }

        }
    }
}
