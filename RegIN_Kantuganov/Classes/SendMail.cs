
using System.Net;
using System.Net.Mail;

namespace RegIN_Kantuganov.Classes
{
    public class SendMail
    {
        public static void SendMessage(string message, string to) {
            var SmtpClient = new SmtpClient("smtp.yandex.ru")
            {
                Port = 587,
                Credentials = new NetworkCredential("kantuganovmarsel@yandex.ru", "cokefxjptpjzakml"),
                EnableSsl = true,
            };
            SmtpClient.Send("kantuganovmarsel@yandex.ru", to, "Проект RegIN", message);
        }
    }
}
