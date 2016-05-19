using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DeviceClient m_clt;
        DispatcherTimer m_t;
        ObservableCollection<string> m_col = new ObservableCollection<string>();
        public MainPage()
        {
            this.InitializeComponent();
            Setup();
        }

        public async void Setup()
        {
            m_t = new DispatcherTimer();
            m_t.Tick += timer_Ticks;
            lstEvents.ItemsSource = m_col;
         }
        string[] deviceNames = new string[] { "PC0", "PC1", "PC2", "PC3", "PC4", "PC5", "PC6", "PC7", "PC8", "PC9" };
        private async void timer_Ticks(object sender, object e)
        {
            Random rnd = new Random();
            if (m_clt == null) return;
            if (chkMsg1.IsChecked == true)
            {
                MMsg1 msg1 = new MMsg1();
                msg1.DeviceName = deviceNames[rnd.Next(deviceNames.Length)];
                msg1.MsgType = "M1";
                msg1.MyProperty1 = rnd.Next();
                await sendMsgObj(msg1);
            }
            if (chkMsg2.IsChecked == true)
            {
                MMsg2 msg2 = new MMsg2();
                msg2.DeviceName = deviceNames[rnd.Next(deviceNames.Length)];
                msg2.MsgType = "M2";
                msg2.MyProperty2 = "AB" + Char.ConvertFromUtf32(Char.ConvertToUtf32("A",0) + rnd.Next(24));
                await sendMsgObj(msg2);
            }

            if (chkError1.IsChecked==true)
            {
                MError err = new MError();
                err.DeviceName = deviceNames[rnd.Next(deviceNames.Length)];
                err.MsgType = "E";
                await sendMsgObj(err);
            }
            if (chkError2.IsChecked == true)
            {
                await sendMsg("ABC"); //Trivia
            }
            DateTime now = DateTime.Now;
            TimeSpan ts = new TimeSpan(now.Year, now.Minute, now.Second, now.Millisecond);
            if (chkMAll.IsChecked == true)
            {
                MAll mall = new MAll();
                mall.MsgType = "ALL";
                mall.DeviceName = deviceNames[rnd.Next(deviceNames.Length)];
                mall.Light = 1000 * Math.Cos(ts.TotalMilliseconds / 175) * Math.Sin(ts.TotalMilliseconds / 360) + rnd.Next(30);
                mall.Potentiometer1 = 1000 * Math.Cos(ts.TotalMilliseconds / 275) * Math.Sin(ts.TotalMilliseconds / 560) + rnd.Next(30);
                mall.Potentiometer2 = 1000 * Math.Cos(ts.TotalMilliseconds / 60) * Math.Sin(ts.TotalMilliseconds / 120) + rnd.Next(30);
                mall.Pressure = (float)(1000 * Math.Cos(ts.TotalMilliseconds / 180) * Math.Sin(ts.TotalMilliseconds / 110) + rnd.Next(30));
                mall.Temperature = (float)(1000 * Math.Cos(ts.TotalMilliseconds / 180) * Math.Sin(ts.TotalMilliseconds / 110) + rnd.Next(30));
                mall.ADC3 = rnd.Next(1000);
                mall.ADC4 = rnd.Next(1000);
                mall.ADC5 = rnd.Next(1000);
                mall.ADC6 = rnd.Next(1000);
                mall.ADC7 = rnd.Next(1000);
                mall.Altitude = (float)(1000 * Math.Cos(ts.TotalMilliseconds / 480) * Math.Sin(ts.TotalMilliseconds / 2000) + rnd.Next(30));
                mall.ColorName = "ABC";
                mall.ColorRaw = new ColorData() { Blue = (ushort)rnd.Next(255), Clear = (ushort)rnd.Next(255), Green = (ushort)rnd.Next(255), Red = (ushort)rnd.Next(255) };
                mall.ColorRgb = new RgbData() { Blue = (ushort)rnd.Next(255), Green = (ushort)rnd.Next(255), Red = (ushort)rnd.Next(255) };
                await sendMsgObj(mall);
            }

            if (chkMSPI.IsChecked == true)
            {
                MSPI mspi = new MSPI();
                mspi.MsgType = "SPI";
                mspi.DeviceName = deviceNames[rnd.Next(deviceNames.Length)];
                mspi.Light = 1000 * Math.Cos(ts.TotalMilliseconds / 175) * Math.Sin(ts.TotalMilliseconds / 360) + rnd.Next(30);
                mspi.Potentiometer1 = 1000 * Math.Cos(ts.TotalMilliseconds / 275) * Math.Sin(ts.TotalMilliseconds / 560) + rnd.Next(30);
                mspi.Potentiometer2 = 1000 * Math.Cos(ts.TotalMilliseconds / 60) * Math.Sin(ts.TotalMilliseconds / 120) + rnd.Next(30);
                await sendMsgObj(mspi);
            }


        }

        private async Task sendMsgObj(object data)
        {
            var messageString = JsonConvert.SerializeObject(data);
            await sendMsg(messageString);
        }

        private async Task sendMsg(string messageString)
        {
            var message = new Message(Encoding.UTF8.GetBytes(messageString));
            await m_clt.SendEventAsync(message);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
        }

        private void btnConnectSubscribe_Click(object sender, RoutedEventArgs e)
        {
            btnConnectSubscribe.IsEnabled = false;
            tgSend.IsEnabled = true;
            m_clt =DeviceClient.CreateFromConnectionString(txtConnectionString.Text,TransportType.Http1);
            ReceiveDataFromAzure(); //Loop, last command 

        }
        public async Task ReceiveDataFromAzure()
        {

            Message receivedMessage;
            string messageData;

            while (true)
            {
                receivedMessage = await m_clt.ReceiveAsync();

                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    //Wykonanie polecenia
                    if (messageData.Length >= 1)
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            m_col.Add(messageData);
                        });
                    }
                    //await m_deviceClient.RejectAsync(receivedMessage);
                    //await m_deviceClient.AbandonAsync(receivedMessage); - odrzuca, ale komunikat wraca
                    //Potwierdzenie wykonania
                    await m_clt.CompleteAsync(receivedMessage); //potwierdza odebranie
                }
            }
        }

        private void tgSend_Toggled(object sender, RoutedEventArgs e)
        {
            if (tgSend.IsOn)
            {
                m_t.Interval = TimeSpan.FromMilliseconds(int.Parse(txtDelay.Text)); //Not too often!
                m_t.Start();
            }
            else
            {
                m_t.Stop();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            m_col.Clear();
        }

        private void txtDelay_TextChanged(object sender, TextChangedEventArgs e)
        {
            int d;
            if (int.TryParse(txtDelay.Text,out d))
                m_t.Interval = TimeSpan.FromMilliseconds(d);
        }
    }
}
