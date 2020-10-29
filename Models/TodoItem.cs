using System;

namespace Serversideprogrammering.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DoneBefore { get; set; }
        public DateTime Added { get; set; }
        public bool IsDone { get; set; }
        public int UserId { get; set; }
    }
}
