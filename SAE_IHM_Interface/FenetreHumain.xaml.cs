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
using System.Windows.Shapes;
using SAE_IHM_Systeme;

namespace SAE_IHM_Interface
{
    public partial class FenetreHumain : Window
    {
        private ParametresJeu _parametresJeu;
        public FenetreHumain(ParametresJeu parametresJeu)
        {
            InitializeComponent();
            _parametresJeu = parametresJeu;
        }

        private void Bouton_Valider_Click(object sender, RoutedEventArgs e)
         {
             int tempsJ1 = OptionsTemps.Secondes((int)SliderTempsJ1.Value);
             int tempsJ2 = OptionsTemps.Secondes((int)SliderTempsJ2.Value);
             FenetreParametresPartie f = new FenetreParametresPartie(
                 _parametresJeu, estIA: false,
                 tempsJ1: tempsJ1, tempsJ2: tempsJ2);
             f.Owner = this;
             f.Show();
             this.Hide();
         }

        private void SliderTempsJ1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LabelTempsJ1 == null)
                return;

            LabelTempsJ1.Text = OptionsTemps.FormatDepuisIndex((int)SliderTempsJ1.Value);
        }

        private void SliderTempsJ2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LabelTempsJ2 == null)
                return;

            LabelTempsJ2.Text = OptionsTemps.FormatDepuisIndex((int)SliderTempsJ2.Value);
        }

        private void Bouton_moins_tempsJ1_Click(object sender, RoutedEventArgs e)
        {
            if (SliderTempsJ1.Value > SliderTempsJ1.Minimum)
                SliderTempsJ1.Value--;
        }

        private void Bouton_plus_tempsJ1_Click(object sender, RoutedEventArgs e)
        {
            if (SliderTempsJ1.Value < SliderTempsJ1.Maximum)
                SliderTempsJ1.Value++;
        }

        private void Bouton_moins_tempsJ2_Click(object sender, RoutedEventArgs e)
        {
            if (SliderTempsJ2.Value > SliderTempsJ2.Minimum)
                SliderTempsJ2.Value--;
        }

        private void Bouton_plus_tempsJ2_Click(object sender, RoutedEventArgs e)
        {
            if (SliderTempsJ2.Value < SliderTempsJ2.Maximum)
                SliderTempsJ2.Value++;
        }

        private void Bouton_retour_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }
    }
}
