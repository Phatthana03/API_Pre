using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

public class ClsSQLDT
{ 
    
    public static Preregitor_Response Preregitor(Preregitor_Request data)
    {
        Preregitor_Response response = new Preregitor_Response();
        if (data.password != data.confirm_password)
        {
            return new Preregitor_Response {status = "Error", message = "รหัสผ่านไม่ตรงกัน" };
        }

        string HashedPassword = BCrypt.Net.BCrypt.HashPassword(data.password);

        data.password = HashedPassword;

        string SQL = @"SELECT username, password
                       FROM users
                       WHERE username = @username AND password = @password
                       ORDER BY id ASC";

        NpgsqlParameter[] param =
        {
            new NpgsqlParameter("@username", data.username),
            new NpgsqlParameter("@password", data.password)
        };

        DataTable dt = Database.FillDS_PGSQL(SQL, param);
        if(dt.Rows.Count == 0)
        {
            string ins_users = @"INSERT INTO users (username, password)
                                 VALUES (@username, @password)";

            NpgsqlParameter[] user_param =
            {
                new NpgsqlParameter("@username", data.username),
                new NpgsqlParameter("@password", data.password)
            };

            var res = Database.ExecuteDB_PGSQL(ins_users, user_param);
            if (res > 0)
            {

                return new Preregitor_Response {status = "Success", message = "ลงทะเบียนสำเร็จ" };
            }
        }

        return new Preregitor_Response { status = "Error", message = "ผู้ใช้นี้มีอยู่แล้ว" };
    }

    public static Login_Response Login_JWT(Login_Request data)
    {
        Login_Response response = new Login_Response();

        bool validate = false;
        string pass = string.Empty;
        string JWTtoken = string.Empty;

        //string HashedPassword = BCrypt.Net.BCrypt.HashPassword(data.password);

        //data.password = HashedPassword;

        string SQL = @"SELECT username, password
                       FROM users
                       WHERE username = @username";

        NpgsqlParameter[] param =
        {
            new NpgsqlParameter("@username", data.username),
            new NpgsqlParameter("@password", data.password)
        };

        DataTable dt = Database.FillDS_PGSQL(SQL, param);
        if (dt.Rows.Count > 0)
        {
            pass = dt.Rows[0]["password"].ToString();
            validate = BCrypt.Net.BCrypt.Verify(data.password, pass);
            if (validate)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("238e70c389e5ab7b1063bc2c1c65e39b"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(issuer: "API",audience: null,expires: DateTime.Now.AddHours(3),signingCredentials: credentials);

                JWTtoken = new JwtSecurityTokenHandler().WriteToken(token);

                return new Login_Response { status = "Success", message = data.username, token = JWTtoken };
            }

            return new Login_Response { status = "Error", message = "รหัสผ่านไม่ถูกต้อง" };

        }

        return new Login_Response {status = "Error", message = "ไม่พบผู้ใช้นี้" };

    }

    public static string GenerateJwtToken(string username)
    {
        var key = Encoding.UTF8.GetBytes(Config.config["Jwt:Key"]);
        var issuer = Config.config["Jwt:Issuer"];

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: issuer,
            claims: new[]
            {
                new Claim(ClaimTypes.Name, username)
            },
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}
