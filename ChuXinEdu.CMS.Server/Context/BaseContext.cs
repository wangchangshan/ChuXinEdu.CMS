
using System;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.Extensions.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
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

        public static DataTable GetDataTable(string sql, params object[] args)
		{
			try
			{
				using (SystemContext sysContext = new SystemContext())
				{
					try
					{
						DbProviderFactory factory = DbProviderFactories.GetFactory(sysContext.Database.GetDbConnection());
						using (var cmd = CreateCommand(factory, args))
						{
							cmd.CommandText = sql;
							cmd.CommandType = CommandType.Text;
							cmd.Connection = sysContext.Database.GetDbConnection();
							using (var adapter = factory.CreateDataAdapter())
							{
								adapter.SelectCommand = cmd;
								var dt = new DataTable();
								adapter.Fill(dt);
								return dt;
							}
						}
					}
					catch (Exception ex)
					{
						throw new Exception($"Error occurred during SQL query execution {sql}", ex);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Error occurred during SQL query execution {sql}", ex);
			}
		}

        private static DbCommand CreateCommand(DbProviderFactory factory, params object[] args)
		{
			var cmd = factory.CreateCommand();
			// Construct SQL parameters
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] is string && i <= (args.Length - 1))
				{
					MySqlParameter parm = new MySqlParameter
					{
						ParameterName = "@" + (i + 1),
						Value = args[i]
					};
					//(string)args[i];
					cmd.Parameters.Add(parm);
				}
				else if (args[i] is MySqlParameter)
				{
					cmd.Parameters.Add((MySqlParameter)args[i]);
				}
				else throw new ArgumentException("Invalid number or type of arguments supplied");
			}
			return cmd;
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
