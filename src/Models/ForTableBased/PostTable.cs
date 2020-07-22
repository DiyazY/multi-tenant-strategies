namespace Models.ForTableBased
{
    public class PostTable
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public virtual BlogTable Blog { get; set; }
    }
}
