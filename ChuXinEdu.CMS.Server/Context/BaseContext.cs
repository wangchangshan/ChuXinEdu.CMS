using Microsoft.EntityFrameworkCore;
using ChuXinEdu.CMS.Server.Model;
using ChuXinEdu.CMS.Server.ViewModel;
using ChuXinEdu.CMS.Server.Utils;

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

        // for test  ViewModel与Model之间通过automapp 来映射，不需要写在这里
        public virtual DbSet<StudentDescTest> StudentDescTest { get; set; }

        public virtual DbSet<Simplify_StudentCourseList> Simplify_StudentCourseList {get; set;}

        public virtual DbSet<CA_R_PERIOD_STUDENTS> CA_R_PERIOD_STUDENTS {get; set;}

        public virtual DbSet<DIC_R_KEY_VALUE> DIC_R_KEY_VALUE {get; set;}

        public virtual DbSet<STUDENT_R_COURSE_OVERVIEW> STUDENT_R_COURSE_OVERVIEW {get; set;}

        public virtual DbSet<Student> Student { get; set; }

        public virtual DbSet<StudentTemp> StudentTemp { get; set; }

        public virtual DbSet<StudentRecommend> StudentRecommend { get; set; }

        public virtual DbSet<StudentActivity> StudentActivity { get; set; }

        public virtual DbSet<StudentActivityImage> StudentActivityImage { get; set; }

        public virtual DbSet<StudentArtwork> StudentArtwork { get; set; }

        public virtual DbSet<StudentBackFee> StudentBackFee { get; set; }

        public virtual DbSet<StudentCourseArrange> StudentCourseArrange { get; set; }

        public virtual DbSet<StudentCourseList> StudentCourseList { get; set; }

        public virtual DbSet<StudentCourseComment> StudentCourseComment { get; set; }

        public virtual DbSet<StudentCoursePackage> StudentCoursePackage { get; set; }

        public virtual DbSet<StudentStatusHistory> StudentStatusHistory { get; set; }

        public virtual DbSet<SysActivity> SysActivity { get; set; }

        public virtual DbSet<SysCourseArrangeTemplate> SysCourseArrangeTemplate { get; set; }
        
        public virtual DbSet<SysCourseArrangeTemplateDetail> SysCourseArrangeTemplateDetail { get; set; }
        
        public virtual DbSet<SysCoursePackage> SysCoursePackage { get; set; }
        
        public virtual DbSet<SysDictionary> SysDictionary { get; set; }

        public virtual DbSet<SysCodeFactory> SysCodeFactory { get; set; }
        
        public virtual DbSet<SysFinace> SysFinace { get; set; }
        
        public virtual DbSet<SysHoliday> SysHoliday { get; set; }

        public virtual DbSet<SysUser> SysUser { get; set; }

        public virtual DbSet<SysMenu> SysMenu { get; set; }

        public virtual DbSet<SysWxUser> SysWxUser { get; set; }
        
        public virtual DbSet<SysLoginHistory> SysLoginHistory { get; set; }
        
        public virtual DbSet<Teacher> Teacher { get; set; }

        public virtual DbSet<TeacherRole> TeacherRole { get; set; }

        public virtual DbSet<WxPicture> WxPicture { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conn = CustomConfig.GetSetting("MySqlConnection");
            optionsBuilder.UseMySql(conn);
        }
    }
}
