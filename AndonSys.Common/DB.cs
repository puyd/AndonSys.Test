using System;
using System.Data;
using System.Data.Common;

using System.Configuration;
using System.Diagnostics;

/// <summary>
///DB 的摘要说明
/// </summary>
namespace AndonSys.Common
{
    public class DB : IDisposable
    {
        IDbConnection con = null;
        IDbCommand cmd = null;
        IDbTransaction trs = null;

        public DB(string prov, string conStr)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(prov);
            con = factory.CreateConnection();
            con.ConnectionString = conStr;
        }

        public void Open()
        {
            if (cmd != null) Close();

            con.Open();

            cmd = con.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 60 * 1000;
        }

        public void Close()
        {
            if (cmd != null)
            {
                lock (cmd)
                {
                    cmd.Dispose();
                    cmd = null;
                }
            }

            con.Close();
        }

        public DataTable QueryTable(string sql)
        {
            IDataReader r;

            lock (cmd)
            {
                cmd.CommandText = sql;
                r = cmd.ExecuteReader();
            }

            DataTable t = new DataTable();
            t.Load(r);

            r.Close();
            r.Dispose();

            return t;

        }

        public int ExcuteSql(string sql)
        {
            lock (cmd)
            {
                cmd.CommandText = sql;
                return cmd.ExecuteNonQuery();
            }
        }

        public object GetValBySql(string sql)
        {
            IDataReader r;
            lock (cmd)
            {
                cmd.CommandText = sql;
                r = cmd.ExecuteReader();
            }

            object v = null;

            if (r.Read()) v = r[0];

            r.Close();
            r.Dispose();

            return v;
        }

        public void BiginTrans()
        {
            if (trs != null)
            {
                throw new Exception("Transaction exists !");
            }
            trs=con.BeginTransaction();
        }

        public void Commit()
        {
            if (trs == null)
            {
                throw new Exception("Transaction is null !");
            }
            trs.Commit();

            trs.Dispose();
            trs = null;
        }

        public void Rollback()
        {
            if (trs == null)
            {
                throw new Exception("Transaction is null !");
            }
            trs.Rollback();

            trs.Dispose();
            trs = null;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (con.State != ConnectionState.Closed)
            {
                Close();
            }

            con.Dispose();
        }

        #endregion
    }
}
