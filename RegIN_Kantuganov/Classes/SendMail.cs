
using System.Net;
using System.Net.Mail;

namespace RegIN_Kantuganov.Classes
{
    public class SendMail
    {
        public static void SendMessage(string message, string to) {
            var SmtpClient = new SmtpClient()
            {
                Port = 587,
                Credentials = new NetworkCredential(),
                EnableSsl = true
            };
            SmtpClient.Send("landaxer@yandex.ru", to, "Проект RegIN", message);
        }
    }
}
