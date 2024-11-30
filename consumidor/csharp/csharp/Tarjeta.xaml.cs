using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace csharp
{
    /// <summary>
    /// Lógica de interacción para Tarjeta.xaml
    /// </summary>
    public partial class Tarjeta : UserControl
    {
        public Tarjeta(Color fondo, String mensaje)
        {
            InitializeComponent();
            Contenedor.BorderBrush = new SolidColorBrush(fondo);
            Contenedor.Background = new SolidColorBrush(fondo);
            Texto.Content = mensaje;
            Texto.Foreground = new SolidColorBrush(ContrastColor(fondo));
        }

        private Color ContrastColor(Color color)
        {
            // Método para calcular el color de contraste
            int d = 0;

            // Obtener el brillo del color
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

            if (luminance > 0.5)
                d = 0; // color oscuro
            else
                d = 255; // color claro

            return Color.FromRgb((byte)d, (byte)d, (byte)d);
        }
    }
}
