
using Microsoft.EntityFrameworkCore;
using MySql.Data;
using ChuXinEdu.CMS.Server.Model;

namespace ChuXinEdu.CMS.Server.Context
{
    public class BaseContext : DbContext
    {
        public BaseContext() : base()
        {

        }
        public BaseContext(DbContextOptions<BaseContext> options) : base(options)
        {

        }

        public virtual DbSet<Student> Student { get; set; }

        public virtual DbSet<StudentActivity> StudentActivity { get; set; }

        public virtual DbSet<StudentActivityImage> StudentActivityImage { get; set; }

        public virtual DbSet<StudentArtwork> StudentArtwork { get; set; }

        public virtual DbSet<StudentBackFee> StudentBackFee { get; set; }

        public virtual DbSet<StudentCourseArrange> StudentCourseArrange { get; set; }

        public virtual DbSet<StudentCourseList> StudentCourseList { get; set; }

        public virtual DbSet<StudentCoursePackage> StudentCoursePackage { get; set; }

        public virtual DbSet<StudentStatusHistory> StudentStatusHistory { get; set; }

        public virtual DbSet<SysActivity> SysActivity { get; set; }

        public virtual DbSet<SysCourseArrangeTemplate> SysCourseArrangeTemplate { get; set; }
        
        public virtual DbSet<SysCourseArrangeTemplateDetail> SysCourseArrangeTemplateDetail { get; set; }
        
        public virtual DbSet<SysCoursePackage> SysCoursePackage { get; set; }
        
        public virtual DbSet<SysDictionary> SysDictionary { get; set; }
        
        public virtual DbSet<SysFinace> SysFinace { get; set; }
        
        public virtual DbSet<SysUser> SysUser { get; set; }
        
        public virtual DbSet<Teacher> Teacher { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost;database=chuxin;user=cswang;password=123456a?;port=3306;sslmode=none;");
        }
    }
}
