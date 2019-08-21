<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HablasCuanto.aspx.cs" Inherits="HablasCuanto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     
</head>
<body>
    <form id="form1" runat="server">
        <div>
            
            Hablas Cuanto v0 - <a href="https://github.com/matheusgrijo">https://github.com/matheusgrijo</a><br />
            <br />
            Apis utilizadas:<br />
            CryptoWatch: <a href="https://api.cryptowat.ch/exchanges">https://api.cryptowat.ch/exchanges</a><br />
            Exchange Rates: <a href="https://exchangeratesapi.io/">https://exchangeratesapi.io/</a><br />
            <br />
            Esse é um estudo provando com as exchanges e os mercados, permitem a arbitragem cambial, no qual você compra em uma exchange X, vende na Y e espera o mercado inverter para retornar com lucro, ou você pode retornar com prejuízo desde que ele seja menor que o lucro obtido na primeira operação.<br />
            Você pode adicionar mais exchanges abaixo de acordo com a sua necessidade!<br />
            <br />
            
            <asp:TextBox ID="txtExchanges" runat="server" Height="118px" TextMode="MultiLine" Width="857px">https://api.cryptowat.ch/markets/bithumb/btckrw/ohlc?periods=1800&amp;after=1564617600;KRW;BITHUMB
https://api.cryptowat.ch/markets/kraken/btceur/ohlc?periods=1800&amp;after=1564617600;EUR;KRAKEN
https://api.cryptowat.ch/markets/okcoin/btcusd/ohlc?periods=1800&amp;after=1564617600;USD;OKCOIN
https://api.cryptowat.ch/markets/bitflyer/btcjpy/ohlc?periods=1800&amp;after=1564617600;JPY;BITFLYER</asp:TextBox>
             <br />
            <asp:Button ID="Button1" runat="server" Text="Calculate" OnClick="Button1_Click" />

             <div id="curve_chart" style="width: 100%; height: 100%"></div>
            
        </div>
    </form>
</body>
</html>
