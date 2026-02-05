using System.Collections.Generic;

namespace LibraryMS.Models
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }      
        public int TotalAuthors { get; set; }    
        public int TotalCategories { get; set; } 
        public int ActiveLoans { get; set; }     
        public int OverdueBooks { get; set; }    

        public List<TransactionInfo> RecentTransactions { get; set; }
    }

    public class TransactionInfo
    {
        public int Id { get; set; }
        public string MemberName { get; set; }
        public string BookTitle { get; set; }
        public System.DateTime LoanDate { get; set; }
        public System.DateTime DueDate { get; set; }
        public string Status { get; set; } 
    }
}