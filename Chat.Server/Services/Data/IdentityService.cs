using Chat.Server.Entities.Models;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Chat.Server.Services.Data
{
    public class IdentityService
    {
        public async Task<bool> writeUserAsync(AppUser user,string path = "AppUsers.json")
        {
            string _user = JsonSerializer.Serialize(user);
            await jsonFileControlAsync(path);

            if((await getUserAsync(path,user.username)) != null)
                return false;
            using (FileStream fileStream = new FileStream(path,FileMode.Open,FileAccess.ReadWrite))
            {
                fileStream.SetLength(fileStream.Length - 1);
                byte[] bytes = Encoding.UTF8.GetBytes("," + _user + "]");
                fileStream.Seek(0, SeekOrigin.End);
                fileStream.Write(bytes, 0,bytes.Length);
            }
            return true;
        }
        public async Task<bool> loginAsync(AppUser user, string path = "AppUsers.json")
        {
            return await chechPasswordAsync(path,user);
        }

        /////////
        private async Task jsonFileControlAsync(string path)
        {
            if (!File.Exists(path))
            {
                AppUser admin = new AppUser()
                {
                    username = "admin",
                    password = "123456Aa."
                };
                string _admin = JsonSerializer.Serialize(admin);
                using (FileStream fs = new FileStream(path,FileMode.Create, FileAccess.Write))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes($"[{_admin}]");
                    await fs.WriteAsync(bytes,0,bytes.Length);
                };
            }
        }
        private async Task<bool> chechPasswordAsync(string path, AppUser user)
        {
            return (await getUsersAsync(path)).Where(a => a.username == user.username && a.password == user.password).Any();
        }
        private async Task<List<AppUser>> getUsersAsync(string path)
        {
            string content = "[]";
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fs.Length];
                await fs.ReadAsync(bytes, 0, bytes.Length);
                content = Encoding.UTF8.GetString(bytes);
            }
            return JsonSerializer.Deserialize<List<AppUser>>(content);
        }
        private async Task<AppUser?> getUserAsync(string path,string username)
        {
            return (await getUsersAsync(path)).Where(a => a.username==username).FirstOrDefault();
        }
    }
}
