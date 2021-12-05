namespace FutureNHS.Infrastructure.Models.GroupPages
{
    public class GroupPage<T>
    {
        public GroupHeader PageHeader {get;set;}
       
        public T PageBody { get; set; }
    }
}
