using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Whois.NET;


namespace DarionMograine
{
    static class Utilities
    {

        static public string Pinger(string hostname)
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(hostname, 1000);
                if (reply != null)
                {
                    string response = ("Status :  " + reply.Status + " \n Time : " + reply.RoundtripTime.ToString() + " \n Address : " + reply.Address);
                    return response;

                }
                else
                {

                    return "Got null as the ping reply. Not sure what to do with it. Sorry.";
                }
            }
            catch
            {
                string response = "Host is not resolvable or there is a timeout.";
                return response;
            }
        }
        static public string getNSLookup(string IPAddress)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = "nslookup.exe";
            psi.Arguments = IPAddress;
            /// here is the key code (these two lines)
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;

            psi.CreateNoWindow = false;
            p.StartInfo = psi;
            p.Start();

            p.WaitForExit();
            /// this is where the output from nslookup will be stored, p.StandardOutput
            System.IO.StreamReader output = p.StandardOutput;

            System.Text.StringBuilder sb = new StringBuilder();
            while (output.Peek() > -1)
            {
                /// foreach outputed line, store it in the StringBuilder and append a new line after it
                sb.Append(output.ReadLine() + Environment.NewLine);
            }
            string result = sb.ToString();
            psi = null; p = null;
            return result;
        }

        static public string getHeaders(string Url)
        {
            Dictionary<string, string> HeaderList = new Dictionary<string, string>();

            WebRequest WebRequestObject = HttpWebRequest.Create(Url);
            WebResponse ResponseObject = WebRequestObject.GetResponse();

            foreach (string HeaderKey in ResponseObject.Headers)
                HeaderList.Add(HeaderKey, ResponseObject.Headers[HeaderKey]);

            ResponseObject.Close();
            string headers = Utilities.ToString(HeaderList, " , ", " | ");
            return headers.ToString();
        }
        private static string ToString(this Dictionary<string, string> source, string keyValueSeparator, string sequenceSeparator)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            var str = new StringBuilder();
            foreach (var keyvaluepair in source)
                str.Append(string.Format("{0}{1}{2}{3}", keyvaluepair.Key, keyValueSeparator, keyvaluepair.Value, sequenceSeparator));
            var retval = str.ToString();
            return retval.Substring(0, retval.Length - sequenceSeparator.Length); //remove last  seq_separator
        }

        static public string Roll()
        {
                Random random = new Random();
                return random.Next(1, 100).ToString();
        }

        static public string MagicBall()
        {
            // A array of authors  
            string[] answers = { "It is certain.", "It is decidedly so.", "Without a doubt.", "Yes - definitely.",
            "You may rely on it.", "As I see it, yes.", "Most likely.", "Outlook good.",
            " Yes.", "Signs point to yes.", "Reply hazy, try again.", "Ask again later.", "Better not tell you now.",
                "Cannot predict now.", "Concentrate and ask again.", "Don't count on it.", "My reply is no.", "My sources say no.",
                "Outlook not so good.", "Very doubtful."};

            // Create a Random object  
            Random rand = new Random();
            // Generate a random index less than the size of the array.  
            int index = rand.Next(answers.Length);
            // Display the result.  
            return answers[index];
        }

        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public static string Telnet(string host, string port)
        {
            int intport;
            using (TcpClient tcpClient = new TcpClient())
            {
                    try
                    {
                        intport = Convert.ToInt32(port);
                    }
                    catch
                    {
                        return "Port should be a numeric value!";
                    }

                var result = tcpClient.BeginConnect(host, intport, null, null);

                result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                if (!tcpClient.Connected)
                {
                    return "Port Closed! :(";
                }

                // we have connected
                tcpClient.EndConnect(result);
                return "Port Open! :)";
            }
        }
        
        public static string Whois(string host)
        {
            var result = WhoisClient.Query(host);
            string output;
            output = result.AddressRange.Begin.ToString() +" "+ result.AddressRange.End.ToString() + "\n"; // "199.71.0.0 - 199.71.0.255"
            output = output + result.OrganizationName.ToString()+ "\n"; // "American Registry for Internet Numbers"
            output = output + (string.Join(" > ", result.RespondedServers)); // "whois.arin.net" 
            return output;
        }

        public static string SslLabs(string host, string port)
        {
            string result;
            try
            {
                //Console.WriteLine(command);
                Process process = new Process();
                process.StartInfo.FileName = "ssllabs.exe";
                process.StartInfo.Arguments = host + " " + port;
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
                return result;
            }
            catch
            {
                result = "You must supply command for this to work.";
                return result;
            }
        }
    }
}
