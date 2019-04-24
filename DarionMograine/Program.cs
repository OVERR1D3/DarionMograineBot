using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Diagnostics;

namespace DarionMograine
{
    class Program
    {
        //static ITelegramBotClient botClient;
        static void Main(string[] args)
        {
            ///DEBUG

            //AI may be some time in the future
            // neuro Neural = new neuro();
            //Neural.Brain();

            ///DEBUGEOF

            // Bot initialization
            int owner_user_id = 00000000000; //YOURUSERIDGOESHERE
            var botClient = new TelegramBotClient("YOURBOTTOKENGOESHERE");
            try
            {
            var me = botClient.GetMeAsync().Result;
                Console.WriteLine($"Tormented souls! I am user {me.Id} and my name is {me.FirstName}.");
            }
            catch
            {
                Console.WriteLine("Mortal! My soul cannot connect to the telegram server.");
                Console.ReadLine();
                return;
            }



            // EOF Bot Initialization

            //Accepting Messages
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
            //EOF Accepting Messages

            // Parsing Messages
            async void Bot_OnMessage(object sender, MessageEventArgs e)
            {
                if (e.Message.Text != null && e.Message.Text.ToLower() == "/start")
                {
                    Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      text: "Send /help for command list\n"
                    );
                    return;
                }

                if (e.Message.Text != null && e.Message.Text.ToLower() == "/help")
                {
                    Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      replyToMessageId: e.Message.MessageId,
                      text: "/stop - stops the bot.\n" +
                      "/help - shows this list of commands.\n" +
                      "/ping [host] - pings host.\n" +
                      "/nslookup [host] - tries to resolve the host's IP.\n" +
                      "/whois [ip] - queries IANA whois database.\n" +
                      "/telnet [host] [port] - checks if the port is open.\n" +
                      "/ssllabs [host] [port] - provides SSL inspection of the host.\n" +
                      "/cmd [command] - execute windows shell command. Restricted to be used only by the owner of the bot.\n" +
                      "/getheaders [host] - gets http headers and cookies from the web host.\n" +
                      "/roll - generates random number between 1-100.\n" +
                      "/password [optional length] - generates random password. Default is 16 chars.\n" +
                      "/magicball [question] - Performs and act of deviation by answering 'Yes' or 'No' to your question.\n" +
                      "/userid - shows your Telegram user ID.\n" +
                      "/chatid - shows your Telegram (current) chat ID."
                    );
                    return;
                }

                if (e.Message.Text != null && e.Message.Text.ToLower() == "/roll")
                {
                    string rolled_number = Utilities.Roll();

                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      replyToMessageId: e.Message.MessageId,
                      text: rolled_number
                    );
                    return;
                }

                if (e.Message.Text != null && e.Message.Text.ToLower() == "/userid")
                {
                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      replyToMessageId: e.Message.MessageId,
                     text: e.Message.From.ToString()
                    );
                    return;
                }

                if (e.Message.Text != null && e.Message.Text.ToLower() == "/chatid")
                {
                    string rolled_number = Utilities.Roll();

                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      replyToMessageId: e.Message.MessageId,
                     text: e.Message.Chat.Id.ToString()
                    );
                    return;
                }

                if (e.Message.Text != null && e.Message.Text.ToLower().Contains("/ping"))
                {
                    string pingresult;
                    try
                    {
                        string host = e.Message.Text.Split(' ')[1];
                        pingresult = Utilities.Pinger(host);
                    }
                    catch
                    {
                        pingresult = "You must supply hostname for the ping to work";
                    }
                    
                    Console.WriteLine($"{pingresult} {e.Message.Chat.Id}.");
                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      replyToMessageId: e.Message.MessageId,
                      text: pingresult
                    );
                    return;
                }

                if (e.Message.Text != null && e.Message.Text.ToLower().Contains("/nslookup"))
                {
                    string nslookupresult;
                    try
                    {
                        string host = e.Message.Text.Split(' ')[1];
                        nslookupresult = Utilities.getNSLookup(host);
                    }
                    catch
                    {
                        nslookupresult = "You must supply hostname for the nslookup to work";
                    }

                    Console.WriteLine($"{nslookupresult} {e.Message.Chat.Id}.");
                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      replyToMessageId: e.Message.MessageId,
                      text: nslookupresult
                    );
                    return;
                }

                if (e.Message.Text != null && e.Message.Text.ToLower().Contains("/cmd")) 
                {
                    if (e.Message.From.Id == owner_user_id)
                    {
                        string command;
                        string result;
                        try
                        {
                            int i = e.Message.Text.IndexOf(" ") + 1;
                            command = e.Message.Text.Substring(i);
                            if (command.ToLower().Contains("powershell"))
                            {
                                command = "";
                                result = "Powershell execution is prohibited!";
                                await botClient.SendTextMessageAsync(
                                chatId: e.Message.Chat,
                                replyToMessageId: e.Message.MessageId,
                                text: result
                                );
                                return;
                            }
                            //Console.WriteLine(command);
                            Process process = new Process();
                            process.StartInfo.FileName = "cmd.exe";
                            process.StartInfo.Arguments = "/C " + command;
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;
                            process.Start();
                            string output = process.StandardOutput.ReadToEnd();
                            Console.WriteLine(output);
                            string err = process.StandardError.ReadToEnd();
                            Console.WriteLine(err);
                            process.WaitForExit();
                            result = output + err;
                        }
                        catch
                        {
                            result = "You must supply command for this to work.";
                        }

                        try
                        { 
                        await botClient.SendTextMessageAsync(
                          chatId: e.Message.Chat,
                          replyToMessageId: e.Message.MessageId,
                          text: result
                        );
                        return;
                        }
                        catch
                        {

                            await botClient.SendTextMessageAsync(
                             chatId: e.Message.Chat,
                            replyToMessageId: e.Message.MessageId,
                            text: "message is too long, try to run command with the shorter output."
                                );
                            return;
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                         chatId: e.Message.Chat,
                         replyToMessageId: e.Message.MessageId,
                         text: "You are not authorized to run this command."
                       );
                        return;
                    }
                    
                }

                if (e.Message.Text != null && e.Message.Text.ToLower().Contains("/powershell"))
                {
                    if (e.Message.From.Id == owner_user_id)
                    {
                        string command;
                        string result;
                        try
                        {
                            int i = e.Message.Text.IndexOf(" ") + 1;
                            command = e.Message.Text.Substring(i);

                            //Console.WriteLine(command);
                            Process process = new Process();
                            process.StartInfo.FileName = "powershell.exe";
                            process.StartInfo.Arguments = "-Command " + command;
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;
                            process.Start();
                            string output = process.StandardOutput.ReadToEnd();
                            Console.WriteLine(output);
                            string err = process.StandardError.ReadToEnd();
                            Console.WriteLine(err);
                            process.WaitForExit();
                            result = output + err;
                        }
                        catch
                        {
                            result = "You must supply command for this to work.";
                        }

                        try
                        {
                            await botClient.SendTextMessageAsync(
                              chatId: e.Message.Chat,
                              replyToMessageId: e.Message.MessageId,
                              text: result
                            );
                            return;
                        }
                        catch
                        {

                            await botClient.SendTextMessageAsync(
                             chatId: e.Message.Chat,
                            replyToMessageId: e.Message.MessageId,
                            text: "message is too long, try to run command with the shorter output."
                                );
                            return;
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                         chatId: e.Message.Chat,
                         replyToMessageId: e.Message.MessageId,
                         text: "You are not authorized to run this command."
                       );
                        return;
                    }

                }

                if (e.Message.Text != null && e.Message.Text.ToLower().Contains("/whois"))
                {
                    string whoisresult;
                    try
                    {
                        string host = e.Message.Text.Split(' ')[1];
                        whoisresult = Utilities.Whois(host);
                    }
                    catch
                    {
                        whoisresult = "You must supply IP address for whois. Use nslookup to find one.";
                    }

                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      replyToMessageId: e.Message.MessageId,
                      text: whoisresult
                    );
                    return;
                }

                if (e.Message.Text != null && e.Message.Text.ToLower().Contains("/telnet"))
                {
                    string telnetresult;
                    string host;
                    string port;
                    try
                    {
                        host = e.Message.Text.Split(' ')[1];
                       //Console.WriteLine(host);

                        try
                        {
                            port = e.Message.Text.Split(' ')[2];
                            //Console.WriteLine(port);
                            telnetresult = Utilities.Telnet(host, port);
                        }
                        catch
                        {
                            telnetresult = "You must supply port for the nslookup to work";
                        }
                    }
                    catch
                    {
                        telnetresult = "You must supply hostname for the telnet to work";
                    }


                    

                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      replyToMessageId: e.Message.MessageId,
                      text: telnetresult
                    );
                    return;
                }

                if (e.Message.Text != null && e.Message.Text.ToLower().Contains("/ssllabs"))
                {
                    string telnetresult;
                    string host;
                    string port;
                    try
                    {
                        host = e.Message.Text.Split(' ')[1];
                        //Console.WriteLine(host);

                        try
                        {
                            port = e.Message.Text.Split(' ')[2];
                            //Console.WriteLine(port);
                            await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            replyToMessageId: e.Message.MessageId,
                            text: "Inspecting the host, expect the result within next 60 seconds"
                            );
                            telnetresult = Utilities.SslLabs(host, port);
                        }
                        catch
                        {
                            telnetresult = "You must supply port for the SSL Labs to work";
                        }
                    }
                    catch
                    {
                        telnetresult = "You must supply hostname for the SSL Labs to work";
                    }

                    try
                    {
                        await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        replyToMessageId: e.Message.MessageId,
                        text: telnetresult
                        );
                        return;
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(
                          chatId: e.Message.Chat,
                          replyToMessageId: e.Message.MessageId,
                          text: "SSL Labs result recieved in Unexpected format"
                        );
                        return;
                    }
                }

                if (e.Message.Text != null && e.Message.Text.ToLower().Contains("/getheaders"))
                {
                    string headersresult = "";
                    try
                    {
                        //string host = e.Message.Text.Split(' ')[1];
                        int i = e.Message.Text.IndexOf(" ") + 1;
                        string str = e.Message.Text.Substring(i);
                        headersresult = Utilities.getHeaders(str);
                    }
                    catch
                    {
                       headersresult = "You must supply http:// or https:// url for the getheaders to work. Remember about the protocol prefix!";
                    }

                    Console.WriteLine($"Requested headers {e.Message.Chat.Id}.");
                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      replyToMessageId: e.Message.MessageId,
                      text: headersresult
                    );
                    return;
                }

                if (e.Message.Text != null && e.Message.Text.ToLower().Contains("/password"))
                {
                    string password_generated = "";
                    int pwd_legth = 16;
                    try
                    {
                        //string host = e.Message.Text.Split(' ')[1];
                        int i = e.Message.Text.IndexOf(" ") + 1;
                        string str = e.Message.Text.Substring(i);
                        try
                        {
                            pwd_legth = Convert.ToInt32(str);
                        }
                        catch
                        {
                            pwd_legth = 16;
                        }
                        password_generated = Utilities.CreatePassword(pwd_legth);
                    }
                    catch
                    {
                        password_generated = Utilities.CreatePassword(pwd_legth);
                    }

                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      replyToMessageId: e.Message.MessageId,
                      text: password_generated
                    );
                    return;
                }

                if (e.Message.Text != null && e.Message.Text.ToLower().Contains("/magicball"))
                {
                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      replyToMessageId: e.Message.MessageId,
                      text: Utilities.MagicBall()
                    );
                    return;
                }



                //Default Message
                if (e.Message.Text != null && e.Message.Text.ToLower() != "/start")
                {
                    Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      replyToMessageId: e.Message.MessageId,
                      text: "Unrecognized command:\n" + e.Message.Text
                    );
                    return;
                }
            }
            // EOF Parsing Messages
        }
    }
}
