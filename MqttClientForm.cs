using MQTTnet;
using MQTTnet.Client;

namespace MqttClientWpz
{
    public partial class Form1 : Form
    {
        private MqttClient mqttClient;
        private const string mqttClientId = "WPZ_Client";
        public Form1()
        {
            InitializeComponent();        
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            mqttClient = new MqttFactory().CreateMqttClient() as MqttClient;
            mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceived;
            mqttClient.ConnectedAsync += MqttClient_Connected;
            mqttClient.DisconnectedAsync += MqttClient_Disconnected;
            timer1.Start();
        }
        private Task MqttClient_ApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            string receiveMsg 
                = "msg:" 
                + arg.ClientId
                + " " 
                + arg.ApplicationMessage.ConvertPayloadToString()
                + " " 
                + DateTime.Now.ToString();
            //label5.Text = receiveMsg;
            MessageBox.Show(receiveMsg);
            return Task.CompletedTask;
        }
        private Task MqttClient_Disconnected(MqttClientDisconnectedEventArgs arg)
        {
            MessageBox.Show("Disconnect ok");
            return Task.CompletedTask;
        }
        private Task MqttClient_Connected(MqttClientConnectedEventArgs arg)
        {
            MessageBox.Show("connect ok");
            return Task.CompletedTask;
        }
        /// <summary>
        /// 连接MQTT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string IP = textBox3.Text;
            int port = Convert.ToInt32(textBox5.Text);
            try
            {
                var bu = new MqttClientOptionsBuilder().WithTcpServer(IP, port).WithClientId(mqttClientId);
                mqttClient?.ConnectAsync(bu.Build());
            }
            catch (Exception ex)
            {
                Invoke((new Action(() =>
                {
                    MessageBox.Show($"连接到MQTT服务器失败！" + Environment.NewLine + ex.Message + Environment.NewLine);
                })));
            }
        }
        /// <summary>
        /// 中断MQTT连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            mqttClient?.DisconnectAsync();
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            string topic = textBox1.Text;
            string payload = textBox2.Text;
            mqttClient?.PublishStringAsync(topic, payload);
            //byte[] payLoadByte = System.Text.Encoding.Default.GetBytes(payload);
            //var msgOption = new MqttApplicationMessageBuilder()
            //    .WithTopic(topic)
            //    .WithPayload(payLoadByte)
            //    .Build();
            //mqttClient?.PublishAsync(msgOption);
        }
        /// <summary>
        /// 订阅主题消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            string topic = textBox4.Text;
            var subOption = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();
            mqttClient?.SubscribeAsync(subOption);
        }
        /// <summary>
        /// 解除订阅主题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            string topic = textBox4.Text;
            var removeSubOption = new MqttClientUnsubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();
            mqttClient?.UnsubscribeAsync(removeSubOption);
        }
        /// <summary>
        /// 显示系统时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            label5.Text = System.DateTime.Now.ToString();
        }
    }
}