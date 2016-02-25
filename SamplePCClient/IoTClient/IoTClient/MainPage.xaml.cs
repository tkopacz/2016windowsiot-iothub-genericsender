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

        private async void timer_Ticks(object sender, object e)
        {
            Random rnd = new Random();
            if (m_clt == null) return;
            if (chkMsg1.IsChecked == true)
            {
                MMsg1 msg1 = new MMsg1();
                msg1.DeviceName = "PC";
                msg1.MsgType = "M1";
                msg1.MyProperty1 = rnd.Next();
                await sendMsgObj(msg1);
            }
            if (chkMsg2.IsChecked == true)
            {
                MMsg2 msg2 = new MMsg2();
                msg2.DeviceName = "PC";
                msg2.MsgType = "M2";
                msg2.MyProperty2 = "AB" + Char.ConvertFromUtf32(Char.ConvertToUtf32("A",0) + rnd.Next(24));
                await sendMsgObj(msg2);
            }

            if (chkError1.IsChecked==true)
            {
                MError err = new MError();
                err.DeviceName = "PC";
                err.MsgType = "E";
                await sendMsgObj(err);
            }
            if (chkError1.IsChecked == true)
            {
                await sendMsg("ABC"); //Trivia
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
