using System;

namespace LibraryMS.Models
{
    public class OverdueInfo
    {
        public int RecordId { get; set; } 
        public string BookTitle { get; set; } 
        public string MemberEmail { get; set; } 
        public DateTime DueDate { get; set; } 
        public int OverdueDays { get; set; } 
    }
}