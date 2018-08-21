
using Microsoft.EntityFrameworkCore;
using MySql.Data;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.Context
{
    public class DictionaryContext : DbContext
    {
        public DictionaryContext() : base()
        {

        }

        public DictionaryContext(DbContextOptions<DictionaryContext> options) : base(options)
        {

        }

        public virtual DbSet<SysDictionary> sysDictionary { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=chuxin;user=cswang;password=123456a?;port=3306;sslmode=none;");
            //optionsBuilder.UseMySQL("server=localhost;userid=cswang;pwd=123456a?;port=3306;database=chuxin;sslmode=none;");
        }
    }
}
