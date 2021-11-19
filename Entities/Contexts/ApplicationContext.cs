using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Chat.Entities.Contexts
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Chat> Chats { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<Chatter> Chatters { get; set; }

        public DbSet<Block> Blocks { get; set; }

        public DbSet<BlockedUser> BlockedUsers { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<GroupMessage> GroupMessages { get; set; }

        public DbSet<GroupMessageReadStatus> GroupMessageReadStatus { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();

            base.OnConfiguring(optionsBuilder);
        }
    }
}
