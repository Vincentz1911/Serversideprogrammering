using System.Collections.Generic;

namespace Serversideprogrammering.Models
{
    public class User
    {
        public User()
        {
            TodoItems = new List<TodoItem>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public List<TodoItem> TodoItems { get; set; }
    }
}
