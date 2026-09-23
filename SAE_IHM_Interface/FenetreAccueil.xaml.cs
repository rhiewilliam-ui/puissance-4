using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SAE_IHM_Systeme;

namespace SAE_IHM_Interface
{
    public partial class FenetreAccueil : Window
    {
        private ParametresJeu _parametresJeu = new ParametresJeu();
        public FenetreAccueil()
        {
            InitializeComponent();
        }

        private void Bouton_Jouer_Click(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Visible;
        }

        private void Bouton_Parametres_Click(object sender, RoutedEventArgs e)
        {
            FenetreParametresJeu f = new FenetreParametresJeu(_parametresJeu);
            f.Owner = this;
            f.Show();
            this.Hide();
        }

        private void Bouton_Replay_Click(object sender, RoutedEventArgs e)
        {
            FenetreReplay f = new FenetreReplay();
            f.Show();
        }

        private void FermerOverlay_Click(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Collapsed;
        }

        private void Bouton_IA_Click(object sender, RoutedEventArgs e)
        {
            FenetreIA f = new FenetreIA(_parametresJeu);
            f.Owner = this;
            f.Show();
            this.Hide();
        }

        private void Bouton_Humain_Click(object sender, RoutedEventArgs e)
        {
            FenetreHumain f = new FenetreHumain(_parametresJeu);
            f.Owner = this;
            f.Show();
            this.Hide();
        }
    }
}
