namespace WebAppVideoCamersOperzal.Models.Entities
{
    public class Visit
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string UserAgent { get; set; }
        public long DateCreate { get; set; }
    }
}
