using System.Diagnostics;
using MailKit;
using MailKit.Net.Imap;


using var client = new ImapClient();
client.Connected += (_, _) => Console.WriteLine("Connected!"); 

await client.ConnectAsync("imap.gmail.com",993, true);

await client.AuthenticateAsync("seimur313@gmail.com", "cdtrkyczltjpiskd");

var inboxFolder = client.Inbox;



while (true)
{
    await inboxFolder.OpenAsync(FolderAccess.ReadOnly);
    var message = inboxFolder.GetMessage(inboxFolder.Count - 1);
    var textBody = message.TextBody;
    if (textBody.Contains("shutdown"))
    {
        var split = textBody.Split(' ');
        var seconds = 0;
        for (var i = 0; i < split.Length - 1; i++)
        {
            switch (split[i])
            {
                case "/h":
                    seconds += Convert.ToInt32(split[i + 1]) * 3600;
                    break;
                case "/m":
                    seconds += Convert.ToInt32(split[i + 1]) * 60;
                    break;
                case "/s":
                    seconds += Convert.ToInt32(split[i + 1]);
                    break;
            }
        }
        var process = Process.Start("shutdown", $"/s /t {seconds} /c \"Shuting down your pc in {seconds} seconds\"");
        await process.WaitForExitAsync();
    }
}