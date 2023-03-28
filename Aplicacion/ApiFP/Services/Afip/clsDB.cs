using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// Summary description for clsDB
/// </summary>
public class clsDB
{
    public clsDB()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static void InsertNewToken(TokenData tokenData)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AfipWsConnectionString"].ConnectionString.ToString()))
            {
                string strInsert = "INSERT INTO FE_Login (token,[sign],fecha_inicial,fecha_caducidad,servicio,ambiente) values (@token, @sign, @fecha_inicial, @fecha_caducidad, @servicio, @ambiente)";
                SqlCommand cmd = new SqlCommand(strInsert);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                cmd.Parameters.AddWithValue("@token", tokenData.Token);
                cmd.Parameters.AddWithValue("@sign", tokenData.Sign);
                cmd.Parameters.AddWithValue("@fecha_inicial", tokenData.GenerationTime);
                cmd.Parameters.AddWithValue("@fecha_caducidad", tokenData.ExpirationTime);
                cmd.Parameters.AddWithValue("@servicio", tokenData.Service);
                cmd.Parameters.AddWithValue("@ambiente", tokenData.Enviroment);
                connection.Open();
                //cmd.Prepare();                
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        catch (Exception e)
        {
            throw new Exception("InsertNewToken DB Exception:" + e.Message);
        }
    }
    public static void GetToken(ref TokenData tokenData)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AfipWsConnectionString"].ConnectionString.ToString()))
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();

                string strSelect = "SELECT TOP 1 * FROM FE_Login WHERE servicio=@servicio and ambiente=@ambiente ORDER BY 1 DESC";
                SqlCommand cmd = new SqlCommand(strSelect);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                da.SelectCommand = cmd;

                cmd.Parameters.AddWithValue("@servicio", tokenData.Service);
                cmd.Parameters.AddWithValue("@ambiente", tokenData.Enviroment);

                da.Fill(ds);
                //connection.Open();
                //cmd.Prepare();                
                if (ds.Tables[0].Rows.Count > 0)
                {
                    tokenData.GenerationTime = (DateTime)ds.Tables[0].Rows[0]["fecha_inicial"];
                    tokenData.ExpirationTime = (DateTime) ds.Tables[0].Rows[0]["fecha_caducidad"];
                    tokenData.Token = ds.Tables[0].Rows[0]["token"].ToString();
                    tokenData.Sign = ds.Tables[0].Rows[0]["sign"].ToString();
                }
                connection.Close();
            }
        }
        catch (Exception e)
        {
            throw new Exception("GetToken DB Exception:" + e.Message);
        }
    }
}