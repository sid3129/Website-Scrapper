using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace parse5
{

    class doormint
    {
        public string title { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string rating { get; set; }
    }

    
    class Program
    {
        public static List<doormint> data =new List<doormint>();
        public static string values_filename;
        public static string csv_filename;
        public static string website;
        public static int ctr = 0;
        public static string responseString = "";
        public static string csv_str_1 = "";
        public static string csv_str_2 = "";
        public static string csv_str_3 = "";
        public static string csv_str_4 = "";
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource == null)
                return null;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                ctr = End + strEnd.Length;
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return null;
            }
        }

        public static void title()
        {

            string temp = "";
                temp = getBetween(responseString,
               "<a onclick=\"_ct('clntnm', 'lspg')",
               "</a>");

            responseString = responseString.Remove(0, ctr);
            string data = getBetween(temp,
            "title='",
            "'");

                csv_str_1 = data;
        }

        public static void phone()
        {

            string temp = "";
            temp = getBetween(responseString,
           "<p class=\"jrcw\"  onclick=\"_ct('clntphn', 'lspg');\"",
           "</p>");

            responseString = responseString.Remove(0, ctr);
            string data = getBetween(temp,
            " href=\"",
            "\">");

            csv_str_2 = data;
        }

        //public static void phone()
        //{   
        //        string data = getBetween(responseString,
        //           "<p class=\"jrcw\"  onclick=\"_ct('clntphn', 'lspg');\" ><span class=\"jrc\"></span><a href=\"",
        //           "\"");
        //        responseString = responseString.Remove(0, ctr);
        //        csv_str_2 = data;

        //}

        public static void address()
        {
                string data = getBetween(responseString,
                   "<span class=\"blckarw\"></span>",
                   "</span>");
                if (data != null)
                data = data.Trim();
                responseString = responseString.Remove(0, ctr);
                csv_str_3 = data;
        }

        public static void ratings()
        {
                string data = getBetween(responseString,
                   "<span class=\"fctrnam\"><b>",
                   "R");
            if(data!=null)
               data = data.Trim();
                responseString = responseString.Remove(0, ctr);
                csv_str_4 = data;
        }

        public static void correct()
        {
            csv_str_1 = csv_str_1.Replace(',', ' ');
            csv_str_2 = csv_str_2.Replace(',', ' ');
            csv_str_3 = csv_str_3.Replace(',', ' ');
            csv_str_4 = csv_str_4.Replace(',', ' ');
        }
       public static void write()
        {
             create_file();

            StreamReader myReader = new StreamReader(values_filename);
            responseString = myReader.ReadToEnd();

            string path = csv_filename;
            using (var w = new StreamWriter(path))
            {
                for (int i = 0; i < 24; i++)
                {
                    parse();
                    if (csv_str_1 == null || csv_str_2 == null || csv_str_3 == null || csv_str_4 == null)
                        break;
                    correct();

                    var line = string.Format("{0},{1},{2},{3}",csv_str_1,csv_str_2,csv_str_3,csv_str_4);
                    
                    {
                    doormint temp=new doormint();
                    
                    temp.title=csv_str_1;
                    temp.phone=csv_str_2;
                    temp.address=csv_str_3;
                    temp.rating=csv_str_4;

                     if(data.Exists(item => item.title==temp.title )==false)
                     data.Add(temp);
                     }
                    w.WriteLine(line);
                    
                }
                w.Flush();               
            }
            }

       
        public static void create_file()
       {
           Console.WriteLine("fetching from " + website);
           WebRequest myWebRequest = WebRequest.Create(website);
           WebResponse myWebResponse = myWebRequest.GetResponse();
           Stream ReceiveStream = myWebResponse.GetResponseStream();
           Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
           StreamReader readStream = new StreamReader(ReceiveStream, encode);
           string strResponse = readStream.ReadToEnd();
           StreamWriter oSw = new StreamWriter(values_filename);
           oSw.WriteLine(strResponse);
           oSw.Close();
           readStream.Close();
           myWebResponse.Close(); 
       }

        public static void final_write()
        {

            string path = "FinalOutput.csv";
            using (var w = new StreamWriter(path))
            {

                foreach (doormint ob in data)
                {
                    var line = string.Format("{0},{1},{2},{3}", ob.title, ob.phone, ob.address, ob.rating);
                    w.WriteLine(line);
                }
                 w.Flush();
             }

        }
  

       public static void parse()
        {
             {
                 title();
                 phone();
                 address();
                 ratings();
            }
        }

        static void Main(string[] args)
        {
            for (int i = 1; i <=20; i++ )
            {
                website = "http://www.justdial.com/Mumbai/Electricians/ct-54768/page-" + i.ToString();
                values_filename = "values" + i.ToString() + ".txt";
                csv_filename = "myOutput" + i.ToString() + ".csv";
                write();
            }

            final_write();
            Console.WriteLine("PROCESS COMPLETED SUCCESSFULLY PRESS ENTER");
            Console.ReadLine();
        }
    }
}
