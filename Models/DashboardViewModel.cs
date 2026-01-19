namespace LibraryMS.Models
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }      
        public int TotalAuthors { get; set; }    
        public int TotalCategories { get; set; } 
        public int ActiveLoans { get; set; }     
        public int OverdueBooks { get; set; }    
    }
}