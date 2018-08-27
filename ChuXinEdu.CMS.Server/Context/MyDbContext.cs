

using System;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.Extensions.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ChuXinEdu.CMS.Server.Context
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options)
        : base(options)
        { }
        public static DataTable GetDataTable(string sql, params object[] args)
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


    }
}