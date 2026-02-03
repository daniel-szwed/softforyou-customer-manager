namespace Domain.Repositories
{
    public class PagedResult<T>
    {
        public T[] Result { get; set; }
        public int TotalCount {  get; set; }
    }
}
