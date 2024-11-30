using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Newtonsoft.Json;

namespace csharp
{
    public partial class MainWindow : Window
    {
        private IConnection connection;
        private ISession session;
        private IMessageConsumer consumer;
        private IDestination destination;

        public MainWindow()
        {
            InitializeComponent();
            
            
        }

        private void InitializeActiveMQConsumer(string tipo, string nombre)
        {
            try
            {
                // Crear una conexión al broker ActiveMQ
                IConnectionFactory factory = new ConnectionFactory("activemq:tcp://localhost:61616");
                connection = factory.CreateConnection();
                connection.Start();

                // Crear una sesión
                session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
                // Crear un consumidor para la cola especificada
                if (tipo.ToLower() == "queue")
                    destination = session.GetQueue(nombre);
                else
                    destination = session.GetTopic(nombre);
                consumer = session.CreateConsumer(destination);
                Desconectar.IsEnabled = true;
                Conectar.IsEnabled = false;
                // Configurar el evento de recepción de mensajes
                consumer.Listener += new MessageListener(OnMessageReceived);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar el consumidor de ActiveMQ: {ex.Message}");
            }
        }
        private List<Color> colores = new List<Color> { Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Purple, Colors.Orange, Colors.Pink, Colors.Brown, Colors.Black, Colors.White };
        private Random random = new Random();
        private void OnMessageReceived(Apache.NMS.IMessage message)
        {
            if (message is ITextMessage textMessage)
            {
                string text = textMessage.Text;
                // Procesar el mensaje recibido
                dynamic objeto = JsonConvert.DeserializeObject(text);
                Dispatcher.Invoke(() => {
                    int posicion = random.Next(colores.Count);
                    Color color = colores[posicion];
                    string mensaje = $"El contenido del mensaje es: {objeto["message"]} (de {destination})";
                    Tarjeta tarjeta = new Tarjeta(color, mensaje);
                    Tabla.Children.Add(tarjeta);
                });
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            // Limpiar recursos
            DesconectarDeActiveMQ();
        }

        private void Conectar_Click(object sender, RoutedEventArgs e)
        {
            string tipo = TipoTxtbox.Text;
            string nombre = NombreTxtbox.Text;
            if (tipo == "" || nombre == "")
            {
                MessageBox.Show("Por favor, ingrese un tipo y un nombre de suscriptor");
                return;
            }
            InitializeActiveMQConsumer(tipo, nombre);
            
        }

        private void Desconectar_Click(object sender, RoutedEventArgs e)
        {
            DesconectarDeActiveMQ();
            Conectar.IsEnabled = true;
            Desconectar.IsEnabled = false;
        }
        private void DesconectarDeActiveMQ()
        {
            try
            {
                consumer?.Close();
                session?.Close();
                connection?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al desconectar de ActiveMQ: {ex.Message}");
            }
        }
    }
}
