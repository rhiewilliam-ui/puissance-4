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
    public partial class FenetreIA : Window
    {
        private ParametresJeu _parametresJeu;
        public FenetreIA(ParametresJeu parametresJeu)
        {
            InitializeComponent();
            _parametresJeu = parametresJeu;
        }
        private void SliderDifficulte_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LabelDifficulte == null)
                return;

            int niveau = (int)SliderDifficulte.Value;
            LabelDifficulte.Text = $"Niveau {niveau} — {NomDifficulte(niveau)}";
        }

        private static string NomDifficulte(int niveau)
        {
            if (niveau <= 2) return "Facile";
            if (niveau <= 4) return "Normal";
            if (niveau <= 6) return "Difficile";
            return "Expert";
        }

        private void Bouton_moins_difficulte_Click(object sender, RoutedEventArgs e)
        {
            if (SliderDifficulte.Value > SliderDifficulte.Minimum)
                SliderDifficulte.Value--;
        }

        private void Bouton_plus_difficulte_Click(object sender, RoutedEventArgs e)
        {
            if (SliderDifficulte.Value < SliderDifficulte.Maximum)
                SliderDifficulte.Value++;
        }
        private void SliderTemps_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LabelTemps == null)
                return;

            LabelTemps.Text = OptionsTemps.FormatDepuisIndex((int)SliderTemps.Value);
        }

        private void Bouton_moins_temps_Click(object sender, RoutedEventArgs e)
        {
            if (SliderTemps.Value > SliderTemps.Minimum)
                SliderTemps.Value--;
        }

        private void Bouton_plus_temps_Click(object sender, RoutedEventArgs e)
        {
            if (SliderTemps.Value < SliderTemps.Maximum)
                SliderTemps.Value++;
        }

        private void ButtonValider_Click(object sender, RoutedEventArgs e)
        {
            int difficulteIA = (int)SliderDifficulte.Value;
            int tempsJ1 = OptionsTemps.Secondes((int)SliderTemps.Value);
            FenetreParametresPartie f = new FenetreParametresPartie(
                _parametresJeu, estIA: true, difficulteIA: difficulteIA,
                tempsJ1: tempsJ1, tempsJ2: 0);
            f.Owner = this;
            f.Show();
            this.Hide();
        }

        private void Bouton_retour_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }
    }
}
