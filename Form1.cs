using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace RuneChan
{
    public partial class Form1 : Form
    {
        public class Rune
        {
            public List<object> autoModifiedSelections { get; set; }
            public bool current { get; set; }
            public int id { get; set; }
            public bool isActive { get; set; }
            public bool isDeletable { get; set; }
            public bool isEditable { get; set; }
            public bool isValid { get; set; }
            public long lastModified { get; set; }
            public string name { get; set; }
            public int order { get; set; }
            public int primaryStyleId { get; set; }
            public List<int> selectedPerkIds { get; set; }
            public int subStyleId { get; set; }
        }
        public string port;
        public string b64;
        public int xamp;
        public string runa;
        public string acid;
        public List<Rune> runesAram;
        public List<Rune> runesSr;
        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public void Auth()
        {
            wmic();
            Thread.Sleep(1000);
            string sampo = File.ReadAllText("temp.txt");
            Regex pattern1 = new Regex(@"remoting-auth-token=(.*?)""");
            Match match1 = pattern1.Match(sampo);
            Regex pattern2 = new Regex(@"--app-port=(.*?)""");
            Match match2 = pattern2.Match(sampo);            
            port = match2.Value.Substring(11,5);
            string token = match1.Value.Substring(20).Replace("\"","");
            b64 = EncodeTo64("riot:" + token);
            File.Delete("temp.txt");
        }
        public void wmic() {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C WMIC path win32_process get Caption,Commandline | find \"--remoting-auth-token=\" > temp.txt";
            process.StartInfo = startInfo;
            process.Start();
        }
        public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        public void LoadJson()
        {
            var webRequesta = WebRequest.Create(@"https://pastebin.com/raw/5WBtRahe");
            using (var response = webRequesta.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var strContent = reader.ReadToEnd();
                runesAram = JsonConvert.DeserializeObject<List<Rune>>(strContent);
            }
            var webRequestb = WebRequest.Create(@"https://pastebin.com/raw/dR5vE4Ba");
            using (var response = webRequestb.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var strContent = reader.ReadToEnd();
                runesSr = JsonConvert.DeserializeObject<List<Rune>>(strContent);
            }
            var webRequestc = WebRequest.Create(@"https://pastebin.com/raw/kcycDGYk");
            using (var response = webRequestc.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                var strContent = reader.ReadToEnd();
                label2.Text = strContent + "\n";
            }

        }
        public string BuscaA(int campeao)
        {
            foreach (var item in runesAram)
            {
                if (item.id == campeao)
                {
                    runa = JsonConvert.SerializeObject(item);
                }
            }
            return runa.ToString();
        }
        public string BuscaS(int campeao)
        {
            foreach (var item in runesSr)
            {
                if (item.id == campeao)
                {
                    runa = JsonConvert.SerializeObject(item);
                }
            }
            return runa.ToString();
        }
        public void Post()
        {
            string uri = "https://127.0.0.1:" + port + "/lol-perks/v1/pages";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Headers.Add("Authorization", "Basic " + b64);
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(runa);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }
        public string Get(string api)
        {
            string uri = "https://127.0.0.1:" + port + api;       
            string result;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Headers.Add("Authorization", "Basic " + b64);
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }
        public void Del()
        {
            string uri = "https://127.0.0.1:" + port + "/lol-perks/v1/pages";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Headers.Add("Authorization", "Basic " + b64);
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "DELETE";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(510);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }
        public Form1()
        {
            InitializeComponent();
            LoadJson();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (snder, cert, chain, error) => true;
            Auth();
            try {
                dynamic stuff = JsonConvert.DeserializeObject(Get("/lol-summoner/v1/current-summoner"));
                acid = stuff.accountId;
            }    
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Application.Exit();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            dynamic lobby = JsonConvert.DeserializeObject(Get("/lol-lobby-team-builder/champ-select/v1/session"));
            foreach (dynamic player in lobby.myTeam)
            {
                if (player.summonerId == acid)
                {
                    xamp = player.championId;
                    break;
                }
            }
            BuscaA(xamp);
            Del();
            Post();
            this.WindowState = FormWindowState.Minimized;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            
        }
        private void button3_Click(object sender, EventArgs e)
        {

        }
        private void button4_Click(object sender, EventArgs e)
        {

            //button4.Enabled = false;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            BuscaA(887);
            Post();
            this.WindowState = FormWindowState.Minimized;
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            BuscaA(666);
            Post();
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            dynamic lobby = JsonConvert.DeserializeObject(Get("/lol-lobby-team-builder/champ-select/v1/session"));
            foreach (dynamic player in lobby.myTeam)
            {
                if (player.summonerId == acid)
                {
                    xamp = player.championId;
                    break;
                }
            }
            BuscaS(xamp);
            Del();
            Post();
            this.WindowState = FormWindowState.Minimized;
        }

        private void button3_Click_2(object sender, EventArgs e)
        {

        }
    }
}
