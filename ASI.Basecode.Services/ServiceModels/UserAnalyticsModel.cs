namespace ASI.Basecode.Services.ServiceModels
{
    public class UserAnalyticsModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int TotalBorrowings { get; set; }
        public int ActiveBorrowings { get; set; }
        public int OverdueBorrowings { get; set; }
    }
}