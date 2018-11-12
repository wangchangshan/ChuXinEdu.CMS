

using System;
using System.Data;
using System.Data.Common;
using ChuXinEdu.CMS.Server.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace ChuXinEdu.CMS.Server.Context
{
    public class ADOContext : DbContext
    {
        public ADOContext() : base()
        {

        }
        public ADOContext(DbContextOptions options)
        : base(options)
        { }
        public static DataTable GetDataTable(string sql, params object[] args)
        {
            try
            {
                using (ADOContext adoContext = new ADOContext())
                {
                    DbProviderFactory factory = DbProviderFactories.GetFactory(adoContext.Database.GetDbConnection());
                    using (var cmd = CreateCommand(factory, args))
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = adoContext.Database.GetDbConnection();
                        using (var adapter = factory.CreateDataAdapter())
                        {
                            adapter.SelectCommand = cmd;
                            var dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conn = CustomConfig.GetSetting("MySqlConnection");
            optionsBuilder.UseMySql(conn);
        }
    }
}