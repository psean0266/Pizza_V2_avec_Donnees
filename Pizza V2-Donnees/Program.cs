using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks.Dataflow;

namespace projet_pizza
{

    class PizzaPersonalisee : pizza
    {
        static int NumeroPizzaPersonnalisee = 0;
        public PizzaPersonalisee() : base("Personnalisée ", 5f, false, null)
        {

            NumeroPizzaPersonnalisee++;
            nom = "Personnalisée" + NumeroPizzaPersonnalisee;
            ingredients = new List<string>();

            while (true)
            {
                Console.Write("Rentrez un ingrédient pour la pizza personnalisée " + NumeroPizzaPersonnalisee + " (ENTER pour terminer): ");
                var ingredient = Console.ReadLine();


                if (string.IsNullOrWhiteSpace(ingredient))
                {
                    break;
                }
                else if (!ingredients.Contains(ingredient))
                {
                    ingredients.Add(ingredient);
                }
                else
                {
                    Console.WriteLine("ERREUR: L'ingredient " + ingredient + " existe déjà la dans liste");
                }

                Console.WriteLine(string.Join(", ", ingredients));
                Console.WriteLine();
            }

            Console.ReadLine();
            prix = 5 + ingredients.Count * 1.5f;
        }

    }

    class pizza
    {
        public string nom { get; protected set; }
        public float prix { get; protected set; }
        public bool vegetarienne { get; private set; }
        public List<string> ingredients { get; protected set; }

        public pizza(string nom, float prix, bool vegetarienne, List<string> ingredients)
        {
            this.nom = nom;
            this.prix = prix;
            this.vegetarienne = vegetarienne;
            this.ingredients = ingredients;

        }

        public void Afficher()
        {

            string nomAfficher = FormatPremiereLettreMajuscules(nom);

            string badgeVegetarienne = vegetarienne ? " (V)" : " ";  // Si c'est vrai (V) sinon "" ? - : sinon


            var ingredientsAfficher = ingredients.Select(i => FormatPremiereLettreMajuscules(i)).ToList();

            Console.WriteLine(" " + nomAfficher + badgeVegetarienne + " - " + prix + "$");
            Console.WriteLine(string.Join(", ", ingredientsAfficher)); // placer des virgules entre les element de la liste
            Console.WriteLine();

        }

        private static string FormatPremiereLettreMajuscules(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            string nomMinuscules = s.ToLower();
            string nomMajiscules = s.ToUpper();

            string resultat = nomMajiscules[0] + nomMinuscules[1..];

            return resultat;
        }

        public bool ContientIngredient(string ingredient)
        {
            return ingredients.Where(i => i.ToLower().Contains("tomate")).ToList().Count > 0;
        }

    }

    class Program
    {

        static List<pizza> GetPizzaFromCode()
        {
            var pizzas = new List<pizza>
            {
                new pizza("fromages", 11.5f, true, new List<string>{"Fromage de chèvre", "Canta","Mozarella"}),
                new pizza("indienne", 10.5f, false, new List<string>{"Poulet", "Oignon","Mozarella"} ),
                new pizza("mexicaine", 13f, false, new List<string>{"Boeuf", "tomate","Mozarella"," persil"}),
                new pizza("Margherita", 8f, true, new List<string>{"sauce tomate","Mozarella"}),
                new pizza("calzone", 12f, false,new List<string>{"kebab", "Oignon","Mozarella"}),
                new pizza("complète", 9.5f, false,new List<string>{"saumon", "Oignon","Mozarella","tomates"})

            };
            return pizzas;
        }

        static List<pizza> GetPizzaFromFile(string path, string filename)
        {
            var pathAndFile = Path.Combine(path, filename); 
            string JsonFile = null;
            try
            {
                JsonFile = File.ReadAllText(pathAndFile);
            }
            catch
            {
                Console.WriteLine("Erreur de lecture du fichier" + filename);
                return null;
            }


            List<pizza> pizzas = new List<pizza>();
            try
            {
                pizzas = JsonConvert.DeserializeObject<List<pizza>>(JsonFile);
            }
            catch
            {
                Console.WriteLine(" Erreur : Les données ne sont pas valides ");
                return null;
            }

            return pizzas;
        }


        static List<pizza> GetPizzaFromUrl(string url)
        {
            var WebClient = new WebClient();
            string JsonFile = "";

            try
            {
                JsonFile = WebClient.DownloadString(url);
            }
            catch (WebException ex) 
            {
                if (ex.Response != null)
                {
                    var statutCode = ((HttpWebResponse)ex.Response).StatusCode;

                    if(statutCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("ERREUR RESEAU: Non trouvé");
                    }
                    else
                    {
                        Console.WriteLine($"ERREUR RESEAU: {statutCode}");
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            
            }
            List<pizza> pizzas = new List<pizza>();

            try
            {
                pizzas = JsonConvert.DeserializeObject<List<pizza>>(JsonFile);
            }
            catch
            {
                Console.WriteLine(" Erreur : Les données ne sont pas valides ");
                return null;
            }

            return pizzas;

        }

        static void GenerateJsonFile(List<pizza>pizzas, string path, string filename)
        {
            var pathAndFile = Path.Combine(path, filename);
            string json = JsonConvert.SerializeObject(pizzas);
            Console.WriteLine(json);

            File.WriteAllText(pathAndFile, json);

            Console.WriteLine("Fichier généré");
        }

        static void Main(string[] args)
        {
            var path = "Fichier Json";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

           // var filename = "Pizzas.json";
            string url = "https://codeavecjonathan.com/res/pizzasss2.json";

            //    var pizzas = GetPizzaFromFile(path, filename);
            //    var pizzas = GetPizzaFromCode();
            //    GenerateJsonFile(pizzas, path, filename);
            var pizzas = GetPizzaFromUrl(url);

            if (pizzas != null)
            {
                foreach (var pizza in pizzas)
                {
                    pizza.Afficher();
                }
            }
 

           
           



           

            

        }
    }
}