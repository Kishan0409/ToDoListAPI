using ToDoListAPI.Models;

namespace ToDoListAPI.Interface
{
    public interface IToDoList
    {
        List<ToDoItem> GetAllTodosList();
        ToDoItem? GetDetailsById(int id);
        void CreateTodoItem(ToDoItem item);
        string UpdateDetails(int Id , ToDoItem item);
        string Delete(int id);
    }
}
