using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class HablasCuanto : System.Web.UI.Page
{

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }

    public static Double DatetimeToUnix(DateTime date)
    {
        return (date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
    public string HttpGet(string url)
    {
        String r = "";
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
        httpWebRequest.Method = "GET";
        var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        var responseStream = httpWebResponse.GetResponseStream();
        if (responseStream != null)
        {
            var streamReader = new StreamReader(responseStream);
            r = streamReader.ReadToEnd();
        }
        if (responseStream != null) responseStream.Close();
        return r;
    }


    //https://api.cryptowat.ch/markets/coinbase-pro/btcusd/ohlc?periods=1800&after=1564617600
    //https://api.exchangeratesapi.io/history?start_at=2019-08-01&end_at=2021-09-01&base=USD

    private decimal searchUSDValue(String json, DateTime date, String fiat)
    {
        try
        {
            Newtonsoft.Json.Linq.JContainer jsonFiat = (Newtonsoft.Json.Linq.JContainer)JsonConvert.DeserializeObject(json);
            return decimal.Parse(jsonFiat["rates"][date.ToString("yyyy-MM-dd")][fiat].ToString());
        }
        catch
        {
            Newtonsoft.Json.Linq.JContainer jsonFiat = (Newtonsoft.Json.Linq.JContainer)JsonConvert.DeserializeObject(json);
            return decimal.Parse(jsonFiat["rates"][DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")][fiat].ToString());
        }
    }

    private string[] getRawData(String url, String jsonFiat, String fiat)
    {
        String json = HttpGet(url);



        Newtonsoft.Json.Linq.JContainer jResult = (Newtonsoft.Json.Linq.JContainer)JsonConvert.DeserializeObject(json);

        int x = 0;
        foreach (var item in jResult["result"]["1800"])
            x++;


        string[] array = new string[x];
        x = 0;

        foreach (var item in jResult["result"]["1800"])
        {
            if (fiat == "USD")
                array[x] = Math.Round(decimal.Parse(item[1].ToString()), 2).ToString().Replace(",", ".");
            else
                array[x] = Math.Round((decimal.Parse(item[1].ToString()) / searchUSDValue(jsonFiat, UnixTimeStampToDateTime(double.Parse(item[0].ToString())), fiat)), 2).ToString().Replace(",", ".");

            x++;
        }


        return array;
    }

    private string[] getRawDataBase()
    {
        String json = HttpGet("https://api.cryptowat.ch/markets/kraken/btceur/ohlc?periods=1800&after=1564617600");
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        Newtonsoft.Json.Linq.JContainer jResult = (Newtonsoft.Json.Linq.JContainer)JsonConvert.DeserializeObject(json);

        int x = 0;

        foreach (var item in jResult["result"]["1800"])
            x++;


        string[] array = new string[x];
        x = 0;
        foreach (var item in jResult["result"]["1800"])
        {
            array[x] = UnixTimeStampToDateTime(double.Parse(item[0].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
            x++;
        }


        return array;
    }

    private void calculateHablasCuanto(String[] exchanges)
    {
        String jsonFiat = HttpGet("https://api.exchangeratesapi.io/history?start_at=2019-08-01&end_at=2021-09-01&base=USD");


        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        String[] baseData = getRawDataBase();

        System.Collections.ArrayList list = new System.Collections.ArrayList();
        System.Collections.ArrayList listName = new System.Collections.ArrayList();

        for (int i = 0; i < exchanges.Length; i++)
            list.Add(getRawData(exchanges[i].Split(';')[0], jsonFiat, exchanges[i].Split(';')[1]));

        for (int i = 0; i < exchanges.Length; i++)
            listName.Add(exchanges[i].Split(';')[2]);


        String header = "['DATA',";
        for (int i = 0; i < listName.Count; i++)
            header += "'" + listName[i].ToString() + "',";
        header = header.Substring(0, header.Length - 1) + "],";
        

        string data = "";
        for (int i = 0; i < baseData.Length - 30; i++)
        {
            data += "['" + baseData[i] + "',";
            for (int x = 0; x < list.Count; x++)
            {
                data += (list[x] as string[])[i].ToString() + ",";
            }
            data = data.Substring(0, data.Length - 1);
            data += "],";
        }

        data = data.Substring(0, data.Length - 1);




        Response.Write("<script type='text/javascript' src='https://www.gstatic.com/charts/loader.js'></script><script type='text/javascript'>" +
      "google.charts.load('current', { 'packages':['corechart']" +
"});" +
 "     google.charts.setOnLoadCallback(drawChart);" +

  "    function drawChart()" +
"{" +
 "   var data = google.visualization.arrayToDataTable([" +

header + data +
       " ]);" +

    "var options = {" +
     "     title: 'Arbitrage Exchanges'," +
      "    curveType: 'function'," +
       "   legend: { position: 'bottom' }" +
"};" +

"var chart = new google.visualization.LineChart(document.getElementById('curve_chart'));" +

"chart.draw(data, options);" +
 "     } " +
  "  </script>");



    }


    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        calculateHablasCuanto(System.Text.RegularExpressions.Regex.Split(txtExchanges.Text, Environment.NewLine));
    }
}
