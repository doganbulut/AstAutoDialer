// Type: DataHelper
// Assembly: AstAutoDialer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B1750608-7C33-45D3-A128-4749BC17D73C
// Assembly location: I:\TLCSilme\AstDialer\AstAutoDialer.exe

using AstAutoDialer.Properties;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Data;
using System.Data.SqlClient;

public class DataHelper
{
  private DataSet ds;
  private SqlDataReader dataReader;

  private string StrCnn()
  {
    return Settings.Default.SQLConnectionString;
  }

  public bool SetPrmArrayByPrm(SqlParameter[] prmArray, short prmOrdinal, string prmName, SqlDbType prmType, string prmValue, bool outValue)
  {
    try
    {
      if ((int) prmOrdinal >= prmArray.Length)
        return false;
      if (!outValue)
      {
        prmArray[(int) prmOrdinal] = new SqlParameter(prmName, prmType);
        prmArray[(int) prmOrdinal].Value = (object) prmValue;
        return true;
      }
      else
      {
        prmArray[(int) prmOrdinal] = new SqlParameter(prmName, prmType);
        prmArray[(int) prmOrdinal].Size = int.Parse(prmValue);
        prmArray[(int) prmOrdinal].Value = (object) prmValue;
        prmArray[(int) prmOrdinal].Direction = ParameterDirection.Output;
        return true;
      }
    }
    catch
    {
      return false;
    }
  }

  public bool SetPrmArrayByPrm(SqlParameter[] prmArray, short prmOrdinal, string prmName, SqlDbType prmType, int intSize, object prmValue, bool outValue)
  {
    try
    {
      if ((int) prmOrdinal >= prmArray.Length)
        return false;
      if (!outValue)
      {
        prmArray[(int) prmOrdinal] = new SqlParameter(prmName, prmType);
        prmArray[(int) prmOrdinal].Size = intSize;
        prmArray[(int) prmOrdinal].Value = prmValue;
        return true;
      }
      else
      {
        prmArray[(int) prmOrdinal] = new SqlParameter(prmName, prmType);
        prmArray[(int) prmOrdinal].Value = prmValue;
        prmArray[(int) prmOrdinal].Size = intSize;
        prmArray[(int) prmOrdinal].Direction = ParameterDirection.Output;
        return true;
      }
    }
    catch
    {
      return false;
    }
  }

  public DataTable GetDataTable(SqlParameter[] prmArray, string spName)
  {
    try
    {
      this.ds = SqlHelper.ExecuteDataset(this.StrCnn(), CommandType.StoredProcedure, spName, prmArray);
      return this.ds.Tables[0];
    }
    catch
    {
      return (DataTable) null;
    }
  }

  public DataTable GetDataTable(CommandType cmdType, string spName)
  {
    try
    {
      this.ds = SqlHelper.ExecuteDataset(this.StrCnn(), cmdType, spName);
      return this.ds.Tables[0];
    }
    catch
    {
      return (DataTable) null;
    }
  }

  public DataSet GetDataSet(SqlParameter[] prmArray, string spName)
  {
    try
    {
      this.ds = SqlHelper.ExecuteDataset(this.StrCnn(), CommandType.StoredProcedure, spName, prmArray);
      return this.ds;
    }
    catch
    {
      return (DataSet) null;
    }
  }

  public DataSet GetDataSet(CommandType cmdType, string strCmd)
  {
    try
    {
      this.ds = SqlHelper.ExecuteDataset(this.StrCnn(), cmdType, strCmd);
      return this.ds;
    }
    catch
    {
      return (DataSet) null;
    }
  }

  public bool ExecuteScalar(string spName)
  {
    try
    {
      SqlHelper.ExecuteScalar(this.StrCnn(), CommandType.StoredProcedure, spName);
      return true;
    }
    catch
    {
      return false;
    }
  }

  public string ExecuteScalar(SqlParameter[] prmArray, string spName)
  {
    try
    {
      return SqlHelper.ExecuteScalar(this.StrCnn(), CommandType.StoredProcedure, spName, prmArray).ToString();
    }
    catch
    {
      return (string) null;
    }
  }

  public bool ExecuteCommand(string spName)
  {
    try
    {
      SqlHelper.ExecuteNonQuery(this.StrCnn(), CommandType.StoredProcedure, spName);
      return true;
    }
    catch
    {
      return false;
    }
  }

  public bool ExecuteCommand(string spName, SqlParameter[] prmArray)
  {
    try
    {
      SqlHelper.ExecuteNonQuery(this.StrCnn(), CommandType.StoredProcedure, spName, prmArray);
      return true;
    }
    catch (Exception ex)
    {
      return false;
    }
  }

  public SqlDataReader GetDataReader(string spName)
  {
    try
    {
      this.dataReader = SqlHelper.ExecuteReader(this.StrCnn(), CommandType.StoredProcedure, spName);
      return this.dataReader;
    }
    catch
    {
      return (SqlDataReader) null;
    }
  }

  public SqlDataReader GetDataReader(SqlParameter[] prmArray, string spName)
  {
    try
    {
      this.dataReader = SqlHelper.ExecuteReader(this.StrCnn(), CommandType.StoredProcedure, spName, prmArray);
      return this.dataReader;
    }
    catch
    {
      return (SqlDataReader) null;
    }
  }

  public object GetObject(string spName)
  {
    try
    {
      return SqlHelper.ExecuteScalar(this.StrCnn(), CommandType.StoredProcedure, spName);
    }
    catch
    {
      return (object) null;
    }
  }
}
