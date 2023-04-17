namespace ProiectLicenta.Email
{
    public interface IEmailSender
    {
        Task Send(string email, string subject, string body);
    }
}
