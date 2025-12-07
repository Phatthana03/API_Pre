
using System.Data;
using Npgsql;

public class Database
{

    public static string getConnectionString_PGSQL()
    {
        try
        {
            return DB.conStr_PGSQL;
        }
        catch (Exception exs)
        {
            throw new Exception(exs.Message);
        }
    }

    public static DataTable FillDS_PGSQL(string sqlCommand, params NpgsqlParameter[] arrParam)
    {
        var dt = new DataTable();
        using (var con = new NpgsqlConnection(getConnectionString_PGSQL()))
        {
            try
            {

                con.Open();
                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlCommand;
                cmd.CommandTimeout = 300;
                // Handle the parameters 
                if (arrParam != null)
                {
                    foreach (NpgsqlParameter param in arrParam)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
                NpgsqlDataAdapter dataAdepter = new NpgsqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                dataAdepter.Fill(ds);
                dt = ds.Tables[0];
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        return dt;
    }

    public static int ExecuteDB_PGSQL(string sqlCommand, params NpgsqlParameter[] arrParam)
    {
        var ret = 0;
        using (NpgsqlConnection conn = new NpgsqlConnection(getConnectionString_PGSQL()))
        {
            try
            {

                conn.Open();

                NpgsqlCommand dbCommand = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandText = sqlCommand,
                    CommandType = CommandType.Text,
                    CommandTimeout = 300
                };
                // Handle the parameters 
                if (arrParam != null)
                {
                    foreach (NpgsqlParameter param in arrParam)
                    {
                        dbCommand.Parameters.Add(param);
                    }
                }
                ret = dbCommand.ExecuteNonQuery();
                conn.Close();
            }

            catch (Exception e)
            {
                ret = -1;
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        return ret;
    }

}