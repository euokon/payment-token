using FBN_Card_Payment.DataObjects;
using FBN_Card_Payment.Helper;
using FBN_Card_Payment.Persistence;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FBN_Card_Payment.Repository
{
    public class OtpDataRepository : IOtpDataRepository
    {
        private readonly string _connectionString;
        public OtpDataRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<OtpItem> GetOtpValue(string otpId)
        {
            OtpItem OtpItem = new OtpItem();
            await using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                OracleCommand cmd = new OracleCommand
                {
                    CommandText = "fbn_cd_otp.get_otp_by_user",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };
                cmd.Parameters.Add("p_username", otpId);
                cmd.Parameters.Add("cur_config", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                await connection.OpenAsync();
                try
                {
                    var rdr = await cmd.ExecuteReaderAsync();
                    if (await rdr.ReadAsync())
                    {
                        OtpItem = new OtpItem()
                        {
                            OtpId = rdr["otp_id"].ToString(),
                            Username = rdr["username"].ToString(),
                            OtpValue = rdr["otp"].ToString(),
                            OtpStatus = Convert.ToChar(rdr["otp_status"].ToString()),
                            DateCreated = Convert.ToDateTime(rdr["date_created"].ToString()),
                            ExpirationDate = Convert.ToDateTime(rdr["expiration_date"].ToString()),
                            DateActivated = Convert.ToDateTime(rdr["date_activated"].ToString().ToNullableDateTime()),
                            DateDeactivated = Convert.ToDateTime(rdr["date_deactivated"].ToString().ToNullableDateTime())
                        };
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return OtpItem;
        }

        public async Task<ActionResponse> AddOtpData(OtpItem otpItem)
        {
            var response = new ActionResponse();
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                OracleCommand cmd = new OracleCommand
                {
                    CommandText = "fbn_cd_otp.insert_otp_data",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };
                cmd.Parameters.Add("p_otp_id", otpItem.OtpId);
                cmd.Parameters.Add("p_username", otpItem.Username);
                cmd.Parameters.Add("p_otp", otpItem.OtpValue);

                cmd.BindByName = true;
                await connection.OpenAsync();
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    response.Status = true;
                }
                catch (Exception ex)
                {
                    response.Status = false;
                    response.Message = ex.Message;
                }
            }
            return response;
        }

        public async Task<ActionResponse> ActivateOtp(string otpId)
        {
            var response = new ActionResponse();
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                OracleCommand cmd = new OracleCommand
                {
                    CommandText = "fbn_cd_otp.activate_otp",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };
                cmd.Parameters.Add("p_otp_id", otpId);

                cmd.BindByName = true;
                await connection.OpenAsync();
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    response.Status = true;
                }
                catch (Exception ex)
                {
                    response.Status = false;
                    response.Message = ex.Message;
                }
            }
            return response;
        }

        public async Task<ActionResponse> DeactivateOtp(string otpId)
        {
            var response = new ActionResponse();
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                OracleCommand cmd = new OracleCommand
                {
                    CommandText = "fbn_cd_otp.deactivate_otp",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };
                cmd.Parameters.Add("p_otp_id", otpId);

                cmd.BindByName = true;
                await connection.OpenAsync();
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    response.Status = true;
                }
                catch (Exception ex)
                {
                    response.Status = false;
                    response.Message = ex.Message;
                }
            }
            return response;
        }



    }
}
