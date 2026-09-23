using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading.Tasks;
using SAE_IHM_Systeme;

namespace SAE_IHM_Interface
{
    public partial class FenetreJeu : Window
    {
        private ParametresJeu _parametresJeu;
        private bool _estIA;
        private int _nbLignes;
        private int _nbColonnes;
        private int _nbJetonsAligner;
        private bool _modeChallenge;
        private int _nbManches;
        private int _difficulteIA;
        private int _tempsJ1;
        private int _tempsJ2;

        private Partie _partie;

        private readonly List<string> _formes = new List<string>
        {
            "Cercle", "CercleCible", "Hexagone"
        };

        private readonly List<Color> _couleurs = new List<Color>
        {
            Colors.Black,  Colors.Red,    Colors.Blue,
            Colors.Green,  Colors.Orange, Colors.Purple,
            Colors.Brown,  Colors.Yellow
        };

        private Color _couleurJ1;
        private Color _couleurJ2;

        public class Case : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            public int Ligne   { get; set; }
            public int Colonne { get; set; }

            public Color ColorJ1 { get; set; } = Colors.Red;
            public Color ColorJ2 { get; set; } = Colors.Blue;

            public string FormeJ1 { get; set; } = "Cercle";
            public string FormeJ2 { get; set; } = "Cercle";

            private int _joueur = 0;
            public int Joueur
            {
                get => _joueur;
                set
                {
                    if (_joueur == value) return;
                    _joueur = value;
                    Notifier(nameof(Joueur));
                    Notifier(nameof(CouleurJeton));
                    Notifier(nameof(CouleurContour));
                    Notifier(nameof(VisibiliteCercle));
                    Notifier(nameof(VisibiliteCible));
                    Notifier(nameof(VisibiliteHexagone));
                    Notifier(nameof(VisibiliteApercu));
                }
            }

            private int _apercu = 0;
            public int Apercu
            {
                get => _apercu;
                set
                {
                    if (_apercu == value) return;
                    _apercu = value;
                    Notifier(nameof(Apercu));
                    Notifier(nameof(CouleurApercu));
                    Notifier(nameof(VisibiliteApercu));
                }
            }

            public Brush CouleurApercu =>
                Apercu == 1 ? new SolidColorBrush(ColorJ1) :
                Apercu == 2 ? new SolidColorBrush(ColorJ2) : Brushes.Transparent;

            public Visibility VisibiliteApercu =>
                (Joueur == 0 && Apercu != 0)
                    ? Visibility.Visible : Visibility.Collapsed;

            private string FormeCourante =>
                Joueur == 1 ? FormeJ1 :
                Joueur == 2 ? FormeJ2 : "";

            public Visibility VisibiliteCercle =>
                (Joueur == 0 || FormeCourante == "Cercle")
                    ? Visibility.Visible : Visibility.Collapsed;
            public Visibility VisibiliteCible =>
                (Joueur != 0 && FormeCourante == "CercleCible")
                    ? Visibility.Visible : Visibility.Collapsed;
            public Visibility VisibiliteHexagone =>
                (Joueur != 0 && FormeCourante == "Hexagone")
                    ? Visibility.Visible : Visibility.Collapsed;

            private bool _gagnante = false;
            public bool Gagnante
            {
                get => _gagnante;
                set
                {
                    if (_gagnante == value) return;
                    _gagnante = value;
                    Notifier(nameof(Gagnante));
                    Notifier(nameof(CouleurContour));
                    Notifier(nameof(EpaisseurContour));
                }
            }

            public Brush CouleurJeton =>
                Joueur == 1 ? JetonBrillant(ColorJ1) :
                Joueur == 2 ? JetonBrillant(ColorJ2) :
                new SolidColorBrush(Color.FromRgb(223, 229, 238));

            private static Brush JetonBrillant(Color c)
            {
                var b = new RadialGradientBrush
                {
                    GradientOrigin = new Point(0.35, 0.30),
                    Center = new Point(0.5, 0.5),
                    RadiusX = 0.75,
                    RadiusY = 0.75
                };
                b.GradientStops.Add(new GradientStop(Eclaircir(c, 0.45), 0.0));
                b.GradientStops.Add(new GradientStop(c, 0.55));
                b.GradientStops.Add(new GradientStop(Assombrir(c, 0.30), 1.0));
                b.Freeze();
                return b;
            }

            private static Color Eclaircir(Color c, double f) => Color.FromRgb(
                (byte)(c.R + (255 - c.R) * f),
                (byte)(c.G + (255 - c.G) * f),
                (byte)(c.B + (255 - c.B) * f));

            private static Color Assombrir(Color c, double f) => Color.FromRgb(
                (byte)(c.R * (1 - f)),
                (byte)(c.G * (1 - f)),
                (byte)(c.B * (1 - f)));

            public Brush CouleurContour =>
                Gagnante     ? Brushes.Gold :
                Joueur == 0  ? Brushes.LightGray : Brushes.Black;

            public double EpaisseurContour => Gagnante ? 4.0 : 1.5;

            private void Notifier(string prop) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private Case[,] _plateau = null!;
        private ObservableCollection<Case> _cases = null!;

        public FenetreJeu(ParametresJeu parametresJeu, bool estIA,
                          int nbLignes, int nbColonnes,
                          int nbJetonsAligner,
                          bool modeChallenge, int nbManches,
                          int difficulteIA = 4,
                          int tempsJ1 = 0, int tempsJ2 = 0)
        {
            InitializeComponent();

            _parametresJeu   = parametresJeu;
            _estIA           = estIA;
            _nbLignes        = nbLignes;
            _nbColonnes      = nbColonnes;
            _nbJetonsAligner = nbJetonsAligner;
            _modeChallenge   = modeChallenge;
            _nbManches       = nbManches;
            _difficulteIA    = difficulteIA;
            _tempsJ1         = tempsJ1;
            _tempsJ2         = tempsJ2;

            _couleurJ1 = _couleurs[_parametresJeu.CouleurIndex1];
            _couleurJ2 = _couleurs[_parametresJeu.CouleurIndex2];

            LabelJ2.Text = estIA ? "IA" : "J2";

            var config = new Configuration
            {
                NbLignes        = nbLignes,
                NbColonnes      = nbColonnes,
                NbAligner       = nbJetonsAligner,
                ModeChallenge   = modeChallenge,
                NbManchesMax    = nbManches,
                ContreIA        = estIA,
                DifficulteIA    = _difficulteIA,
                TempsParCoupJ1  = _tempsJ1,
                TempsParCoupJ2  = _tempsJ2,
                CouleurJ1       = "Rouge",
                CouleurJ2       = "Bleu",
                SymboleJ1       = "R",
                SymboleJ2       = "B",
                NomJ1           = "Joueur 1",
                NomJ2           = estIA ? "IA" : "Joueur 2"
            };

            Joueur j1 = new JoueurHumain(config.CouleurJ1, config.SymboleJ1);
            Joueur j2 = estIA
                ? (Joueur)new JoueurIA(config.CouleurJ2, config.SymboleJ2, profondeur: config.DifficulteIA)
                : new JoueurHumain(config.CouleurJ2, config.SymboleJ2);

            _partie = new Partie(config, j1, j2);
            _partie.OnChangementJoueur += OnChangementJoueur;
            _partie.OnTimerTick        += OnTimerTick;
            _partie.OnFinPartie        += OnFinPartie;
            _partie.OnTourSaute        += OnTourSaute;
            _partie.OnMancheSuivante   += OnMancheSuivante;

            InitialiserPlateau();

            this.Loaded += FenetreJeu_Loaded;
        }

        private void FenetreJeu_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = TrouverEnfantVisuel<UniformGrid>(GrilleJeu);
            if (grid != null)
            {
                grid.Rows    = _nbLignes;
                grid.Columns = _nbColonnes;
            }

            double zoom = _parametresJeu.Taille / 100.0;
            const double cellBase = 64;
            GrilleJeu.Width  = _nbColonnes * cellBase * zoom;
            GrilleJeu.Height = _nbLignes  * cellBase * zoom;

            DessinerJeton(CanvasJ1,
                _couleurs[_parametresJeu.CouleurIndex1],
                _formes[_parametresJeu.FormeIndex1], 30);

            DessinerJeton(CanvasJ2,
                _couleurs[_parametresJeu.CouleurIndex2],
                _formes[_parametresJeu.FormeIndex2], 30);

            _partie.Demarrer();

            MajAffichageChallenge();
        }

        private void MajAffichageChallenge()
        {
            if (!_modeChallenge)
            {
                ManchePill.Visibility = Visibility.Collapsed;
                LabelScoreJ1.Visibility = Visibility.Collapsed;
                LabelScoreJ2.Visibility = Visibility.Collapsed;
                return;
            }

            ManchePill.Visibility = Visibility.Visible;
            LabelManche.Text = $"Manche {_partie.MancheCourante}  ·  BO{_partie.Config.NbManchesMax}";

            LabelScoreJ1.Visibility = Visibility.Visible;
            LabelScoreJ2.Visibility = Visibility.Visible;
            LabelScoreJ1.Text = _partie.ScoreJ1.ToString();
            LabelScoreJ2.Text = _partie.ScoreJ2.ToString();
        }

        private readonly Dictionary<string, BitmapImage> _cacheIcones = new();

        private void AfficherIconeResultat(string? nom)
        {
            if (nom == null) { ImgResultat.Source = null; return; }
            if (!_cacheIcones.TryGetValue(nom, out var img))
            {
                img = new BitmapImage(new Uri($"pack://application:,,,/Assets/{nom}.png"));
                _cacheIcones[nom] = img;
            }
            ImgResultat.Source = img;
        }

        private void InitialiserPlateau()
        {
            _plateau = new Case[_nbLignes, _nbColonnes];
            _cases   = new ObservableCollection<Case>();

            for (int l = 0; l < _nbLignes; l++)
            {
                for (int c = 0; c < _nbColonnes; c++)
                {
                    var cas = new Case
                    {
                        Ligne    = l,
                        Colonne  = c,
                        ColorJ1  = _couleurJ1,
                        ColorJ2  = _couleurJ2,
                        FormeJ1  = _formes[_parametresJeu.FormeIndex1],
                        FormeJ2  = _formes[_parametresJeu.FormeIndex2]
                    };
                    _plateau[l, c] = cas;
                    _cases.Add(cas);
                }
            }

            GrilleJeu.ItemsSource = _cases;
        }

        private void SynchroniserPlateau()
        {
            var pions = _partie.Grille.Pions;

            for (int l = 0; l < _nbLignes; l++)
            {
                for (int c = 0; c < _nbColonnes; c++)
                {
                    var pion = pions[l, c];
                    _plateau[l, c].Joueur =
                        pion == null            ? 0 :
                        pion.Couleur == "Rouge" ? 1 : 2;
                }
            }
        }

        private void Case_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not Case cas) return;
            if (!_partie.EstEnCours()) return;

            if (_partie.JoueurCourant is JoueurIA) return;

            EffacerApercu();

            bool ok = _partie.JouerCoup(cas.Colonne);
            if (!ok) return;

            SynchroniserPlateau();
        }

        private int _apercuLigne = -1;
        private int _apercuColonne = -1;

        private void Case_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Case cas)
                MontrerApercu(cas.Colonne);
        }

        private void GrilleJeu_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            EffacerApercu();
        }

        private void MontrerApercu(int colonne)
        {
            EffacerApercu();

            if (!_partie.EstEnCours() || _partie.JoueurCourant is JoueurIA) return;

            int ligne = _partie.ObtenirHauteurPion(colonne);
            if (ligne < 0) return;

            int joueur = _partie.JoueurCourant.Couleur == "Rouge" ? 1 : 2;
            _plateau[ligne, colonne].Apercu = joueur;
            _apercuLigne = ligne;
            _apercuColonne = colonne;
        }

        private void EffacerApercu()
        {
            if (_apercuLigne >= 0 && _apercuColonne >= 0)
                _plateau[_apercuLigne, _apercuColonne].Apercu = 0;
            _apercuLigne = -1;
            _apercuColonne = -1;
        }

        private bool _iaEnCours = false;

        private void FaireJouerIASiBesoin()
        {
            if (!(_estIA && _partie.EstEnCours()
                         && _partie.JoueurCourant is JoueurIA joueurIA))
                return;
            if (_iaEnCours) return;
            _iaEnCours = true;

            LabelTour.Text = "L'IA réfléchit…";
            LabelTimer.Text = "";

            Grille grilleCopie = new Grille(_partie.Grille);

            Task.Run(() => joueurIA.MeilleurCoup(grilleCopie))
                .ContinueWith(tache =>
                {
                    _iaEnCours = false;

                    if (_partie.EstEnCours() && _partie.JoueurCourant is JoueurIA)
                    {
                        _partie.JouerCoup(tache.Result);
                        SynchroniserPlateau();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnChangementJoueur(string couleur)
        {
            Dispatcher.Invoke(() =>
            {
                string nom = couleur == "Rouge"
                    ? "Joueur 1"
                    : (_estIA ? "IA" : "Joueur 2");
                LabelTour.Text = $"Tour {nom}";

                int tempsAlloue = _partie.TempsAlloueCourant;
                if (tempsAlloue > 0)
                {
                    LabelTimer.Text = $"{tempsAlloue} s";
                    TimerPill.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xF1, 0xF0));
                    LabelTimer.Foreground = new SolidColorBrush(Color.FromRgb(0xD3, 0x2F, 0x2F));
                    TimerPill.Visibility = Visibility.Visible;
                }
                else
                {
                    LabelTimer.Text = "";
                    TimerPill.Visibility = Visibility.Collapsed;
                }

                FaireJouerIASiBesoin();
            });
        }

        private void OnTimerTick(int tempsRestant)
        {
            Dispatcher.Invoke(() =>
            {
                LabelTimer.Text = $"{tempsRestant} s";
                TimerPill.Visibility = Visibility.Visible;
                bool urgent = tempsRestant <= 5;
                TimerPill.Background = new SolidColorBrush(urgent ? Color.FromRgb(0xFF, 0xDE, 0xDA)
                                                                  : Color.FromRgb(0xFF, 0xF1, 0xF0));
                LabelTimer.Foreground = new SolidColorBrush(urgent ? Color.FromRgb(0xB7, 0x1C, 0x1C)
                                                                   : Color.FromRgb(0xD3, 0x2F, 0x2F));
            });
        }

        private void OnFinPartie(EtatPartie etat)
        {
            Dispatcher.Invoke(() =>
            {
                LabelTimer.Text = "";
                TimerPill.Visibility = Visibility.Collapsed;

                foreach (var (ligne, col) in _partie.ObtenirCasesGagnantes())
                    _plateau[ligne, col].Gagnante = true;

                switch (etat)
                {
                    case EtatPartie.VictoireJ1:
                        AfficherIconeResultat("trophy");
                        LabelResultat.Text = "Joueur 1 gagne !";
                        break;
                    case EtatPartie.VictoireJ2:
                        AfficherIconeResultat(_estIA ? "robot" : "trophy");
                        LabelResultat.Text = _estIA ? "L'IA gagne !" : "Joueur 2 gagne !";
                        break;
                    case EtatPartie.Nul:
                        AfficherIconeResultat("egalite");
                        LabelResultat.Text = "Match nul !";
                        break;
                    default:
                        AfficherIconeResultat(null);
                        LabelResultat.Text = "Partie terminée";
                        break;
                }

                if (_modeChallenge)
                    LabelResultat.Text +=
                        $"\nScore final — {_partie.Config.NomJ1} {_partie.ScoreJ1} : " +
                        $"{_partie.ScoreJ2} {_partie.Config.NomJ2}";

                BtnContinuer.Visibility = Visibility.Collapsed;
                BtnRejouer.Visibility   = Visibility.Visible;

                LabelTour.Text = "Partie terminée";
                OverlayResultat.Visibility = Visibility.Visible;
            });
        }

        private void OnTourSaute(string couleur)
        {
            Dispatcher.Invoke(() =>
            {
                string nom = couleur == "Rouge" ? "Joueur 1" : (_estIA ? "IA" : "Joueur 2");
                LabelTimer.Text = "";
                TimerPill.Visibility = Visibility.Collapsed;
                LabelTour.Text = $"Tour de {nom} passé";

                AfficherToast($"Temps écoulé — tour de {nom} passé");
            });
        }

        private DispatcherTimer? _toastTimer;

        private void AfficherToast(string message)
        {
            ToastTexte.Text = message;
            ToastTimeout.Visibility = Visibility.Visible;

            _toastTimer?.Stop();
            _toastTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2.4) };
            _toastTimer.Tick += (s, e) =>
            {
                ToastTimeout.Visibility = Visibility.Collapsed;
                _toastTimer?.Stop();
            };
            _toastTimer.Start();
        }

        private void OnMancheSuivante(int scoreJ1, int scoreJ2)
        {
            Dispatcher.Invoke(() =>
            {
                LabelTimer.Text = "";
                TimerPill.Visibility = Visibility.Collapsed;

                foreach (var (ligne, col) in _partie.ObtenirCasesGagnantes())
                    _plateau[ligne, col].Gagnante = true;

                MajAffichageChallenge();

                string? vainqueurManche =
                    _partie.Etat == EtatPartie.VictoireJ1 ? _partie.Config.NomJ1 :
                    _partie.Etat == EtatPartie.VictoireJ2 ? _partie.Config.NomJ2 :
                    null;

                AfficherIconeResultat("swords");
                LabelResultat.Text =
                    (vainqueurManche != null
                        ? $"{vainqueurManche} remporte la manche !\n"
                        : "Manche nulle !\n") +
                    $"Score — {_partie.Config.NomJ1} {scoreJ1} : {scoreJ2} {_partie.Config.NomJ2}";

                BtnContinuer.Visibility = Visibility.Visible;
                BtnRejouer.Visibility   = Visibility.Collapsed;

                OverlayResultat.Visibility = Visibility.Visible;
            });
        }

        private void Bouton_pause_Click(object sender, RoutedEventArgs e)
        {
            _partie.Pause();
            Overlay.Visibility = Visibility.Visible;
        }

        private void Bouton_reprendre_Click(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Collapsed;
            _partie.Reprendre();
            FaireJouerIASiBesoin();
        }

        private void Bouton_retour_Click(object sender, RoutedEventArgs e)
        {
            _partie.Abandonner();
            this.Owner.Show();
            this.Close();
        }

        private void Bouton_continuer_Click(object sender, RoutedEventArgs e)
        {
            OverlayResultat.Visibility = Visibility.Collapsed;

            foreach (var cas in _cases)
                cas.Gagnante = false;

            _partie.DemarrerProchaineManche();

            SynchroniserPlateau();
            MajAffichageChallenge();
        }

        private void Bouton_rejouer_Click(object sender, RoutedEventArgs e)
        {
            FenetreJeu f = new FenetreJeu(
                _parametresJeu, _estIA,
                _nbLignes, _nbColonnes, _nbJetonsAligner,
                _modeChallenge, _nbManches, _difficulteIA,
                _tempsJ1, _tempsJ2);
            f.Owner = this.Owner;
            f.Show();
            this.Close();
        }

        private void Bouton_quitter_Click(object sender, RoutedEventArgs e)
        {
            _partie.Abandonner();
            this.Owner.Show();
            this.Close();
        }

        private void DessinerJeton(Canvas canvas, Color couleur, string forme, double taille)
        {
            canvas.Children.Clear();
            canvas.Width  = taille;
            canvas.Height = taille;

            double epaisseur   = 2.0;
            Brush  contour     = new SolidColorBrush(couleur);
            Brush  remplissage = new SolidColorBrush(Color.FromArgb(40, couleur.R, couleur.G, couleur.B));

            switch (forme)
            {
                case "Cercle":
                    var cercle = new Ellipse
                    {
                        Width           = taille - epaisseur * 2,
                        Height          = taille - epaisseur * 2,
                        Fill            = remplissage,
                        Stroke          = contour,
                        StrokeThickness = epaisseur
                    };
                    Canvas.SetLeft(cercle, epaisseur);
                    Canvas.SetTop(cercle, epaisseur);
                    canvas.Children.Add(cercle);
                    break;

                case "CercleCible":
                    var ext = new Ellipse
                    {
                        Width           = taille - epaisseur * 2,
                        Height          = taille - epaisseur * 2,
                        Fill            = remplissage,
                        Stroke          = contour,
                        StrokeThickness = epaisseur
                    };
                    Canvas.SetLeft(ext, epaisseur);
                    Canvas.SetTop(ext, epaisseur);
                    canvas.Children.Add(ext);

                    double r2 = (taille - epaisseur * 2) * 0.45;
                    var inner = new Ellipse
                    {
                        Width           = r2,
                        Height          = r2,
                        Fill            = remplissage,
                        Stroke          = contour,
                        StrokeThickness = epaisseur * 0.8
                    };
                    Canvas.SetLeft(inner, (taille - r2) / 2);
                    Canvas.SetTop(inner, (taille - r2) / 2);
                    canvas.Children.Add(inner);
                    break;

                case "Hexagone":
                    var hex = new Polygon
                    {
                        Fill            = remplissage,
                        Stroke          = contour,
                        StrokeThickness = epaisseur
                    };
                    double cx = taille / 2, cy = taille / 2;
                    double r    = taille / 2 - epaisseur;
                    double step = 2 * Math.PI / 6;
                    double startAngle = Math.PI / 2;
                    for (int i = 0; i < 6; i++)
                    {
                        double angle = startAngle + i * step;
                        hex.Points.Add(new Point(cx + r * Math.Cos(angle),
                                                 cy + r * Math.Sin(angle)));
                    }
                    canvas.Children.Add(hex);
                    break;
            }
        }

        private static T? TrouverEnfantVisuel<T>(DependencyObject parent)
            where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T resultat) return resultat;

                var trouve = TrouverEnfantVisuel<T>(child);
                if (trouve != null) return trouve;
            }
            return null;
        }
    }
}
