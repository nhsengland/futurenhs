namespace MVCForumAutomation
{
    public class TestDefaults
    {
        public Role StandardMembers { get; } = new Role("Standard Members");
        public Group ExampleGroup { get; } = new Group("Example Group");
        public string AdminUsername { get; } = "admin";
        public string AdminPassword { get; } = "password";
    }
}