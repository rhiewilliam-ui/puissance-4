
using System;
using System.Collections.Generic;

namespace SAE_IHM_Systeme
{

    public class Pion
    {

        private string couleur;
        private string forme;

        public Pion(string couleur, string forme)
        {
            this.couleur = couleur;
            this.forme = forme;
        }

        public string Couleur
        {
            get { return couleur; }
            set { couleur = value; }
        }

        public string Forme
        {
            get { return forme; }
            set { forme = value; }
        }

        public override string ToString()
        {
            return couleur;
        }
    }

    public class Grille
    {

        private int nbColonnes;
        private int nbLignes;
        private int nbAligner;
        private Pion?[,] pions;

        public Grille(int nblignes, int nbcolonnes, int nbAligner)
        {
            this.nbLignes = nblignes;
            this.nbColonnes = nbcolonnes;
            this.nbAligner = nbAligner;
            this.pions = new Pion?[nbLignes, nbColonnes];
        }

        public Grille(Grille autre)
        {
            this.nbLignes = autre.nbLignes;
            this.nbColonnes = autre.nbColonnes;
            this.nbAligner = autre.nbAligner;
            this.pions = new Pion?[nbLignes, nbColonnes];

            for (int ligne = 0; ligne < nbLignes; ligne++)
            {
                for (int col = 0; col < nbColonnes; col++)
                {
                    if (autre.pions[ligne, col] != null)
                    {
                        this.pions[ligne, col] = new Pion(
                            autre.pions[ligne, col]!.Couleur,
                            autre.pions[ligne, col]!.Forme
                        );
                    }
                }
            }
        }

        public int NbLignes { get { return nbLignes; } }

        public int NbColonnes { get { return nbColonnes; } }

        public int NbAligner { get { return nbAligner; } }

        public Pion?[,] Pions { get { return pions; } }

        public bool PoserPion(int colonne, Pion pion)
        {
            if (!ColonneJouable(colonne)) return false;

            for (int ligne = nbLignes - 1; ligne >= 0; ligne--)
            {
                if (pions[ligne, colonne] == null)
                {
                    pions[ligne, colonne] = pion;
                    return true;
                }
            }
            return false;
        }

        public bool ColonneJouable(int colonne)
        {
            if (colonne < 0 || colonne >= nbColonnes) return false;
            return pions[0, colonne] == null;
        }

        public bool GrillePleine()
        {
            for (int col = 0; col < nbColonnes; col++)
            {
                if (ColonneJouable(col)) return false;
            }
            return true;
        }

        public bool Victoire(string couleur)
        {
            int[,] directions = new int[,]
            {
                { 0, 1 },
                { 1, 0 },
                { 1, 1 },
                { 1, -1 }
            };

            for (int l = 0; l < nbLignes; l++)
            {
                for (int c = 0; c < nbColonnes; c++)
                {
                    for (int d = 0; d < 4; d++)
                    {
                        int dl = directions[d, 0];
                        int dc = directions[d, 1];
                        int count = 0;

                        for (int k = 0; k < nbAligner; k++)
                        {
                            int nl = l + k * dl;
                            int nc = c + k * dc;

                            if (nl >= 0 && nl < nbLignes &&
                                nc >= 0 && nc < nbColonnes &&
                                pions[nl, nc] != null &&
                                pions[nl, nc]!.Couleur == couleur)
                            {
                                count++;
                            }
                        }

                        if (count == nbAligner) return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            string result = "";
            for (int l = 0; l < nbLignes; l++)
            {
                for (int c = 0; c < nbColonnes; c++)
                {
                    if (pions[l, c] == null)
                        result += "[ ]";
                    else
                        result += $"[{pions[l, c]!.Forme}]";
                }
                result += "\n";
            }
            return result;
        }
    }

    public abstract class Joueur
    {

        private string couleur;
        private string symbole;

        public Joueur(string couleur, string symbole)
        {
            this.couleur = couleur;
            this.symbole = symbole;
        }

        public string Couleur
        {
            get { return couleur; }
            set { couleur = value; }
        }

        public string Symbole
        {
            get { return symbole; }
            set { symbole = value; }
        }

        public abstract int ChoisirColonne(Grille grille);
    }

    public class JoueurHumain : Joueur
    {

        public JoueurHumain(string couleur, string symbole)
            : base(couleur, symbole)
        {
        }

        public override int ChoisirColonne(Grille grille)
        {
            int colonne = -1;
            bool saisieValide = false;

            while (!saisieValide)
            {
                Console.Write($"joueur{Couleur}, choisissez une colonne (0-{grille.NbColonnes - 1}): ");
                string? saisie = Console.ReadLine();

                if (!int.TryParse(saisie, out colonne))
                {
                    Console.WriteLine("Erreur : veuillez entrer un nombre entier .");
                    continue;
                }

                if (colonne < 0 || colonne >= grille.NbColonnes)
                {
                    Console.WriteLine($"Erreur : la colonne doit être entre 0 et {grille.NbColonnes - 1}.");
                    continue;
                }

                if (!grille.ColonneJouable(colonne))
                {
                    Console.WriteLine("Erreur : cette colonne est pleine.");
                    continue;
                }

                saisieValide = true;
            }

            return colonne;
        }
    }

    public class Noeud
    {

        private Grille grille;
        private int valeurHeuristique;
        private Noeud? parent;
        private List<Noeud> enfants;
        private int colonne;

        public Noeud(Grille grille, Noeud? parent = null, int colonne = -1)
        {
            this.grille = new Grille(grille);
            this.valeurHeuristique = 0;
            this.parent = parent;
            this.enfants = new List<Noeud>();
            this.colonne = colonne;
        }

        public Grille Grille { get { return grille; } }

        public int ValeurHeuristique
        {
            get { return valeurHeuristique; }
            set { valeurHeuristique = value; }
        }

        public Noeud? Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public List<Noeud> Enfants { get { return enfants; } }

        public int Colonne { get { return colonne; } }

        public void AjouterEnfant(Noeud enfant)
        {
            enfants.Add(enfant);
        }
    }

    public class Heuristique
    {

        private double CentreColi(int nbColonnes)
        {
            return (nbColonnes - 1) / 2.0;
        }

        public int EvaluationPosition(Grille grille, string couleurIA, string couleurAdversaire)
        {
            int scoreIA = CalculerScore(grille, couleurIA, couleurAdversaire);
            int scoreAdversaire = CalculerScore(grille, couleurAdversaire, couleurIA);
            return scoreIA - scoreAdversaire;
        }

        private int CalculerScore(Grille grille, string couleur, string couleurAdverse)
        {
            int score = 0;
            int N = grille.NbAligner;
            int L = grille.NbLignes;
            int C = grille.NbColonnes;

            score += BonusPositionnel(grille, couleur);

            int[,] directions = new int[,]
            {
                { 0,  1 },
                { 1,  0 },
                { 1,  1 },
                { 1, -1 }
            };

            for (int l = 0; l < L; l++)
            {
                for (int c = 0; c < C; c++)
                {
                    for (int d = 0; d < 4; d++)
                    {
                        int dl = directions[d, 0];
                        int dc = directions[d, 1];

                        int finL = l + (N - 1) * dl;
                        int finC = c + (N - 1) * dc;
                        if (finL < 0 || finL >= L || finC < 0 || finC >= C)
                            continue;

                        score += EvaluerFenetre(grille, l, c, dl, dc, couleur, couleurAdverse);
                    }
                }
            }

            score += ScoreDoubleMenace(grille, couleur, couleurAdverse);

            return score;
        }

        private int EvaluerFenetre(Grille grille, int l, int c, int dl, int dc,
                                    string couleur, string couleurAdverse)
        {
            int N = grille.NbAligner;
            int L = grille.NbLignes;
            int nbAlli = 0;
            int nbVide = 0;
            int nbAdverse = 0;
            int hauteurCaseVide = -1;

            for (int k = 0; k < N; k++)
            {
                int nl = l + k * dl;
                int nc = c + k * dc;

                if (grille.Pions[nl, nc] == null)
                {
                    nbVide++;
                    hauteurCaseVide = nl;
                }
                else if (grille.Pions[nl, nc]!.Couleur == couleur)
                {
                    nbAlli++;
                }
                else
                {
                    nbAdverse++;
                }
            }

            if (nbAlli > 0 && nbAdverse > 0) return 0;

            int score = 0;

            if (nbAlli == N)
            {
                score = (int)Math.Pow(N, 3) * 100;
            }
            else if (nbAlli == N - 1 && nbVide == 1)
            {
                int scoreBase = (int)Math.Pow(N, 2) * 10;
                double coeffParite = 1.0 + ((L - hauteurCaseVide) / (double)L);
                score = (int)(scoreBase * coeffParite);
            }
            else if (nbAlli == N - 2 && nbVide == 2)
            {
                score = N * 1;
            }

            return score;
        }

        private int BonusPositionnel(Grille grille, string couleur)
        {
            int N = grille.NbAligner;
            int C = grille.NbColonnes;
            double centreColi = CentreColi(C);
            int bonusMax = N * 3;
            double facteur = (double)N / C;
            int score = 0;

            for (int l = 0; l < grille.NbLignes; l++)
            {
                for (int c = 0; c < C; c++)
                {
                    if (grille.Pions[l, c] != null &&
                        grille.Pions[l, c]!.Couleur == couleur)
                    {
                        double distance = Math.Abs(c - centreColi);
                        score += (int)(bonusMax - (distance * facteur));
                    }
                }
            }

            return score;
        }

        private int ScoreDoubleMenace(Grille grille, string couleur, string couleurAdverse)
        {
            int N = grille.NbAligner;
            int L = grille.NbLignes;
            int C = grille.NbColonnes;
            int nbMenaces = 0;

            int[,] directions = new int[,]
            {
                { 0,  1 }, { 1,  0 }, { 1,  1 }, { 1, -1 }
            };

            for (int l = 0; l < L; l++)
            {
                for (int c = 0; c < C; c++)
                {
                    for (int d = 0; d < 4; d++)
                    {
                        int dl = directions[d, 0];
                        int dc = directions[d, 1];

                        int finL = l + (N - 1) * dl;
                        int finC = c + (N - 1) * dc;
                        if (finL < 0 || finL >= L || finC < 0 || finC >= C)
                            continue;

                        int nbAlli = 0, nbVide = 0, nbAdverse = 0;

                        for (int k = 0; k < N; k++)
                        {
                            int nl = l + k * dl;
                            int nc = c + k * dc;

                            if (grille.Pions[nl, nc] == null)
                                nbVide++;
                            else if (grille.Pions[nl, nc]!.Couleur == couleur)
                                nbAlli++;
                            else
                                nbAdverse++;
                        }

                        if (nbAlli == N - 1 && nbVide == 1 && nbAdverse == 0)
                            nbMenaces++;
                    }
                }
            }

            if (nbMenaces >= 2)
                return (int)Math.Pow(N, 3) * 90;

            return 0;
        }

        public int ScoreCoupCadeau(Grille grille, int colonne,
                                    string couleurIA, string couleurAdversaire)
        {
            int N = grille.NbAligner;

            Grille copie = new Grille(grille);
            Pion pion = new Pion(couleurIA, "");
            copie.PoserPion(colonne, pion);

            Grille copie2 = new Grille(copie);
            Pion pionAdverse = new Pion(couleurAdversaire, "");
            copie2.PoserPion(colonne, pionAdverse);

            if (copie2.Victoire(couleurAdversaire))
                return -(int)Math.Pow(N, 3) * 90;

            return 0;
        }
    }

    public class AlgoMinMax
    {

        private int profondeur;
        private Heuristique heuristique;
        private string couleurIA;
        private string couleurAdversaire;

        public AlgoMinMax(int profondeur, string couleurIA, string couleurAdversaire)
        {
            this.profondeur = profondeur;
            this.couleurIA = couleurIA;
            this.couleurAdversaire = couleurAdversaire;
            this.heuristique = new Heuristique();
        }

        public int Profondeur
        {
            get { return profondeur; }
            set
            {
                if (value > 0 && value <= 15)
                    profondeur = value;
                else
                    Console.WriteLine("Profondeur invalide");
            }
        }

        private readonly Random alea = new Random();

        public int MeilleurCoup(Grille grille)
        {
            bool facile = profondeur <= 2;
            if (facile)
            {
                double probaAleatoire = profondeur <= 1 ? 0.70 : 0.45;
                if (alea.NextDouble() < probaAleatoire)
                    return ColonneAleatoire(grille);
            }

            int meilleurScore = int.MinValue;
            int meilleurColonne = 0;

            for (int i = 0; i < grille.NbColonnes; i++)
            {
                if (grille.ColonneJouable(i)) { meilleurColonne = i; break; }
            }

            for (int col = 0; col < grille.NbColonnes; col++)
            {
                if (!grille.ColonneJouable(col)) continue;

                if (!facile)
                {
                    Grille copieTest = new Grille(grille);
                    copieTest.PoserPion(col, new Pion(couleurIA, ""));
                    if (copieTest.Victoire(couleurIA))
                        return col;

                    Grille copieAdverse = new Grille(grille);
                    copieAdverse.PoserPion(col, new Pion(couleurAdversaire, ""));
                    if (copieAdverse.Victoire(couleurAdversaire))
                        meilleurColonne = col;
                }

                int malus = facile ? 0
                    : heuristique.ScoreCoupCadeau(grille, col, couleurIA, couleurAdversaire);

                Noeud noeud = CreerFeuille(grille, col, couleurIA);

                int profExploration = facile ? 0 : profondeur - 1;

                int score = Minimax(noeud, profExploration, int.MinValue, int.MaxValue, false) + malus;

                if (score > meilleurScore)
                {
                    meilleurScore = score;
                    meilleurColonne = col;
                }
            }

            return meilleurColonne;
        }

        private int ColonneAleatoire(Grille grille)
        {
            List<int> jouables = new List<int>();
            for (int c = 0; c < grille.NbColonnes; c++)
                if (grille.ColonneJouable(c)) jouables.Add(c);
            return jouables.Count == 0 ? 0 : jouables[alea.Next(jouables.Count)];
        }

        private int Minimax(Noeud noeud, int profondeurCourante,
                            int alpha, int beta, bool estMax)
        {
            Grille grille = noeud.Grille;

            if (grille.Victoire(couleurIA))
                return (int)Math.Pow(grille.NbAligner, 3) * 100;

            if (grille.Victoire(couleurAdversaire))
                return -(int)Math.Pow(grille.NbAligner, 3) * 100;

            if (grille.GrillePleine())
                return 0;

            if (profondeurCourante == 0)
                return AttribuerValeur(noeud);

            if (estMax)
            {
                int meilleurScore = int.MinValue;

                for (int col = 0; col < grille.NbColonnes; col++)
                {
                    if (!grille.ColonneJouable(col)) continue;

                    Noeud enfant = CreerFeuille(grille, col, couleurIA);
                    noeud.AjouterEnfant(enfant);

                    int score = Minimax(enfant, profondeurCourante - 1,
                                       alpha, beta, false);

                    meilleurScore = Math.Max(meilleurScore, score);
                    alpha = Math.Max(alpha, meilleurScore);

                    if (beta <= alpha) break;
                }

                return meilleurScore;
            }
            else
            {
                int meilleurScore = int.MaxValue;

                for (int col = 0; col < grille.NbColonnes; col++)
                {
                    if (!grille.ColonneJouable(col)) continue;

                    Noeud enfant = CreerFeuille(grille, col, couleurAdversaire);
                    noeud.AjouterEnfant(enfant);

                    int score = Minimax(enfant, profondeurCourante - 1,
                                       alpha, beta, true);

                    meilleurScore = Math.Min(meilleurScore, score);
                    beta = Math.Min(beta, meilleurScore);

                    if (beta <= alpha) break;
                }

                return meilleurScore;
            }
        }

        private Noeud CreerFeuille(Grille grille, int colonne, string couleur)
        {
            Grille copie = new Grille(grille);
            copie.PoserPion(colonne, new Pion(couleur, ""));
            return new Noeud(copie);
        }

        private int AttribuerValeur(Noeud noeud)
        {
            int score = heuristique.EvaluationPosition(
                noeud.Grille, couleurIA, couleurAdversaire);
            noeud.ValeurHeuristique = score;
            return score;
        }
    }

    public class JoueurIA : Joueur
    {

        private AlgoMinMax algo;

        public JoueurIA(string couleur, string symbole, int profondeur)
            : base(couleur, symbole)
        {
            this.algo = new AlgoMinMax(profondeur, couleur, ObtenirCouleurAdversaire(couleur));
        }

        public AlgoMinMax Algo { get { return algo; } }

        public int Profondeur
        {
            get { return algo.Profondeur; }
            set { algo.Profondeur = value; }
        }

        private string ObtenirCouleurAdversaire(string couleur)
        {
            return couleur == "Rouge" ? "Bleu" : "Rouge";
        }

        public override int ChoisirColonne(Grille grille)
        {
            Console.WriteLine($"L'IA ({Couleur}) réfléchit...");
            int colonne = MeilleurCoup(grille);
            Console.WriteLine($"L'IA ({Couleur}) joue en colonne {colonne}");
            return colonne;
        }

        public int MeilleurCoup(Grille grille)
        {
            return algo.MeilleurCoup(grille);
        }
    }

    public class Configuration
    {

        public int NbLignes { get; set; } = 6;

        public int NbColonnes { get; set; } = 7;

        public int NbAligner { get; set; } = 4;

        public bool ContreIA { get; set; } = true;

        public int DifficulteIA { get; set; } = 4;

        public int TempsParCoupJ1 { get; set; } = 0;
        public int TempsParCoupJ2 { get; set; } = 0;

        public bool ModeChallenge { get; set; } = false;

        public int NbManchesMax { get; set; } = 3;

        public string CouleurJ1 { get; set; } = "Rouge";

        public string CouleurJ2 { get; set; } = "Bleu";

        public string SymboleJ1 { get; set; } = "R";

        public string SymboleJ2 { get; set; } = "B";

        public string NomJ1 { get; set; } = "Joueur 1";

        public string NomJ2 { get; set; } = "Joueur 2";

        public string FormeJetons { get; set; } = "Rond";

        public string TypeMatch { get; set; } = "Solo";

        public int? NumTournoi { get; set; } = null;
    }

    public enum EtatPartie
    {
        NonDemarree,
        EnCours,
        Pause,
        VictoireJ1,
        VictoireJ2,
        Nul,
        Abandonnee
    }

    public class Partie
    {

        private Grille grille;
        private Joueur joueur1;
        private Joueur joueur2;
        private Joueur joueurCourant;
        private Configuration config;
        private EtatPartie etat;

        private int scoreJ1;
        private int scoreJ2;
        private int mancheCourante;
        private bool joueur1ACommenceCetteManche;

        private System.Timers.Timer? timer;
        private int tempsRestant;

        public event Action<int>? OnTimerTick;

        public event Action<string>? OnTourSaute;

        public event Action<EtatPartie>? OnFinPartie;

        public event Action<int, int>? OnMancheSuivante;

        public event Action<string>? OnChangementJoueur;

        public Partie(Configuration config, Joueur joueur1, Joueur joueur2)
        {
            this.config = config;
            this.joueur1 = joueur1;
            this.joueur2 = joueur2;
            this.grille = new Grille(config.NbLignes, config.NbColonnes, config.NbAligner);
            this.joueurCourant = joueur1;
            this.etat = EtatPartie.NonDemarree;
            this.scoreJ1 = 0;
            this.scoreJ2 = 0;
            this.mancheCourante = 1;
            this.joueur1ACommenceCetteManche = true;
        }

        public Grille Grille => grille;

        public Joueur JoueurCourant => joueurCourant;

        public EtatPartie Etat => etat;

        public int ScoreJ1 => scoreJ1;

        public int ScoreJ2 => scoreJ2;

        public int MancheCourante => mancheCourante;

        public int TempsRestant => tempsRestant;

        public int TempsAlloueCourant =>
            joueurCourant is JoueurIA ? 0 :
            (joueurCourant == joueur1 ? config.TempsParCoupJ1 : config.TempsParCoupJ2);

        public Configuration Config => config;

        public Joueur? Gagnant
        {
            get
            {
                if (etat == EtatPartie.VictoireJ1) return joueur1;
                if (etat == EtatPartie.VictoireJ2) return joueur2;
                return null;
            }
        }

        public void Demarrer()
        {
            etat = EtatPartie.EnCours;
            DemarrerTimerSiHumain();
            OnChangementJoueur?.Invoke(joueurCourant.Couleur);
        }

        public void Pause()
        {
            if (etat != EtatPartie.EnCours) return;
            etat = EtatPartie.Pause;
            ArretTimer();
        }

        public void Reprendre()
        {
            if (etat != EtatPartie.Pause) return;
            etat = EtatPartie.EnCours;
            DemarrerTimerSiHumain();
        }

        public void Abandonner()
        {
            etat = EtatPartie.Abandonnee;
            ArretTimer();
        }

        public bool JouerCoup(int colonne)
        {
            if (etat != EtatPartie.EnCours) return false;
            if (!grille.ColonneJouable(colonne)) return false;

            ArretTimer();

            Pion pion = new Pion(joueurCourant.Couleur, joueurCourant.Symbole);
            grille.PoserPion(colonne, pion);

            if (VerifierFinPartie()) return true;

            ChangerJoueur();
            return true;
        }

        private void SauterTour()
        {
            if (etat != EtatPartie.EnCours) return;

            OnTourSaute?.Invoke(joueurCourant.Couleur);

            if (grille.GrillePleine())
            {
                etat = EtatPartie.Nul;
                GererFinPartie();
                return;
            }

            ChangerJoueur();
        }

        private void ChangerJoueur()
        {
            joueurCourant = joueurCourant == joueur1 ? joueur2 : joueur1;
            OnChangementJoueur?.Invoke(joueurCourant.Couleur);
            DemarrerTimerSiHumain();
        }

        private bool VerifierFinPartie()
        {
            if (grille.Victoire(joueurCourant.Couleur))
            {
                if (joueurCourant == joueur1)
                {
                    scoreJ1++;
                    etat = EtatPartie.VictoireJ1;
                }
                else
                {
                    scoreJ2++;
                    etat = EtatPartie.VictoireJ2;
                }

                GererFinPartie();
                return true;
            }

            if (grille.GrillePleine())
            {
                etat = EtatPartie.Nul;
                GererFinPartie();
                return true;
            }

            return false;
        }

        private void GererFinPartie()
        {
            ArretTimer();

            if (config.ModeChallenge && !BOGagne())
            {
                OnMancheSuivante?.Invoke(scoreJ1, scoreJ2);
                return;
            }

            OnFinPartie?.Invoke(etat);
        }

        public bool ColonneJouable(int colonne)
        {
            return grille.ColonneJouable(colonne);
        }

        public int ObtenirHauteurPion(int colonne)
        {
            if (!grille.ColonneJouable(colonne)) return -1;

            for (int l = grille.NbLignes - 1; l >= 0; l--)
            {
                if (grille.Pions[l, colonne] == null)
                    return l;
            }
            return -1;
        }

        public List<(int ligne, int colonne)> ObtenirCasesGagnantes()
        {
            var cases = new List<(int, int)>();
            if (Gagnant == null) return cases;

            int[,] directions = new int[,]
            {
                { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 }
            };

            for (int l = 0; l < grille.NbLignes; l++)
            {
                for (int c = 0; c < grille.NbColonnes; c++)
                {
                    for (int d = 0; d < 4; d++)
                    {
                        int dl = directions[d, 0];
                        int dc = directions[d, 1];
                        var sequence = new List<(int, int)>();

                        for (int k = 0; k < grille.NbAligner; k++)
                        {
                            int nl = l + k * dl;
                            int nc = c + k * dc;

                            if (nl < 0 || nl >= grille.NbLignes ||
                                nc < 0 || nc >= grille.NbColonnes) break;

                            if (grille.Pions[nl, nc]?.Couleur == Gagnant.Couleur)
                                sequence.Add((nl, nc));
                            else
                                break;
                        }

                        if (sequence.Count == grille.NbAligner)
                            return sequence;
                    }
                }
            }

            return cases;
        }

        private bool BOGagne()
        {
            int manchesNecessaires = (config.NbManchesMax / 2) + 1;
            return scoreJ1 >= manchesNecessaires || scoreJ2 >= manchesNecessaires;
        }

        public void DemarrerProchaineManche()
        {
            mancheCourante++;
            joueur1ACommenceCetteManche = !joueur1ACommenceCetteManche;
            joueurCourant = joueur1ACommenceCetteManche ? joueur1 : joueur2;

            grille = new Grille(config.NbLignes, config.NbColonnes, config.NbAligner);
            etat = EtatPartie.EnCours;

            OnChangementJoueur?.Invoke(joueurCourant.Couleur);
            DemarrerTimerSiHumain();
        }

        public string ObtenirScoreBO()
        {
            return $"{config.NomJ1} : {scoreJ1} | {config.NomJ2} : {scoreJ2}";
        }

        private void DemarrerTimerSiHumain()
        {
            if (joueurCourant is JoueurIA) return;

            int tempsAlloue = (joueurCourant == joueur1)
                ? config.TempsParCoupJ1
                : config.TempsParCoupJ2;
            if (tempsAlloue <= 0) return;

            ArretTimer();

            tempsRestant = tempsAlloue;

            timer = new System.Timers.Timer(1000);
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) =>
            {
                tempsRestant--;
                OnTimerTick?.Invoke(tempsRestant);

                if (tempsRestant <= 0)
                {
                    ArretTimer();
                    SauterTour();
                }
                else
                {
                    timer?.Start();
                }
            };
            timer.Start();
        }

        private void ArretTimer()
        {
            timer?.Stop();
            timer?.Dispose();
            timer = null;
        }

        public bool EstTerminee()
        {
            return etat == EtatPartie.VictoireJ1 ||
                   etat == EtatPartie.VictoireJ2 ||
                   etat == EtatPartie.Nul ||
                   etat == EtatPartie.Abandonnee;
        }

        public bool EstEnPause() => etat == EtatPartie.Pause;

        public bool EstEnCours() => etat == EtatPartie.EnCours;
    }
}
