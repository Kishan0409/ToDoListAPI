using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoListAPI.Helper;
using ToDoListAPI.Interface;
using ToDoListAPI.Models;
using ToDoListAPI.Services;

namespace ToDoListAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoListAPIController : ControllerBase
    {
        private IConfiguration _IConfiguration;
        private readonly IToDoList _ITodoList;
        private readonly ToDoHelper _toDoHelper;

        public TodoListAPIController(IToDoList Itodolist, IConfiguration configuration)
        {
            _IConfiguration = configuration;
            _ITodoList = Itodolist;
            _toDoHelper = new ToDoHelper(configuration);

        }

        /// <summary>
        /// Authenticates a user and generates a JWT token upon successful login.
        /// </summary>
        /// <param name="request">The login request containing the username and password.</param>
        /// <returns>An IActionResult containing the generated token if authentication is successful; otherwise, an Unauthorized result.</returns>
        [HttpPost("Login")]
        public IActionResult Login(LoginRequest request)
        {
            if (request.Username == "testuser" && request.Password == "1234")
            {

                var token = _toDoHelper.GenerateToken(request.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }
        /// <summary>
        /// Creates a new TODO item and stores it in the database.
        /// </summary>
        /// <param name="todo">The TODO item to be created, containing the title, description, and status.</param>

        [Authorize]

        [HttpPost("CreateTodoItem")]
        public IActionResult CreateTodoItem(ToDoItem todo)
        {
            try
            {
                _ITodoList.CreateTodoItem(todo);
                return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize]
        /// <summary>
        /// Retrieves all TODO items from the database.
        /// </summary>
        /// <returns>Returns a list of TODO items or a 404 Not Found response if no items are found.</returns>
        [HttpGet("GetAllTodos")]
        public IActionResult GetAllTodos()
        {
            var todos = _ITodoList.GetAllTodosList();
            if (todos.Count == 0)
            {
                return NotFound("No TODO items found.");
            }
            return Ok(todos);
        }


        [Authorize]
        /// <summary>
        /// Retrieves a TODO item by its ID.
        /// </summary>
        /// <param name="id">The ID of the TODO item to retrieve.</param>
        [HttpGet("GetTodoById")]
        public IActionResult GetTodoById(int id)
        {
            try
            {
                var todo = _ITodoList.GetDetailsById(id);
                return Ok(todo);  
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);  
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);  
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");  
            }
        }

        [Authorize]
        /// <summary>
        /// Updates an existing TODO item by its ID.
        /// </summary>
        /// <param name="id">The ID of the TODO item to update.</param>
        /// <param name="updatedTodo">The updated TODO item details.</param>
        [HttpPut("UpdateTodoItem")]
        public IActionResult UpdateTodoItem(int id, [FromBody] ToDoItem updatedTodo)
        {
            
               
            try
            {
                var response  = _ITodoList.UpdateDetails(id,updatedTodo);
                return Ok(response); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }


        [Authorize]
        /// <summary>
        /// Deletes a TODO item by its ID.
        /// </summary>
        /// <param name="id">The ID of the TODO item to delete.</param>
        [HttpDelete("DeleteTodoItem")]
        public IActionResult DeleteTodoItem(int id)
        {
            try
            {
                var response  = _ITodoList.Delete(id);
                return Ok(response); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }

        }
    }
}
