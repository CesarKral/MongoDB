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
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

//https://docs.mongodb.com/getting-started/csharp/update/
//http://mongodb.github.io/mongo-csharp-driver/2.4/reference/driver/crud/linq/

namespace MongoDB
{
    class Person
    {
        [BsonElement("_id")]
        public ObjectId Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("age")]
        public int Age { get; set; }
        [BsonElement("car")]
        public string Car { get; set; }
        [BsonElement("country")]
        public string Country { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }
    }
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            var document = new BsonDocument();
            document.Add("name", this.tbName.Text);
            int age;
            bool success = int.TryParse(this.tbAge.Text, out age);
            document.Add("age", age);
            document.Add("car", this.tbCar.Text);
            document.Add("country", this.tbCountry.Text);
            document.Add("email", this.tbEmail.Text);
            document.Add("timestamp", DateTime.Now);

            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("test");
            //await db.CreateCollectionAsync("people");
            var col = db.GetCollection<BsonDocument>("people");
            if (success != false)
            {
                await col.InsertOneAsync(document);
                this.tbName.Text = "";
                this.tbAge.Text = "";
                this.tbCar.Text = "";
                this.tbCountry.Text = "";
                this.tbEmail.Text = "";
            }
            
            
        }

        private async void btnFind_Click(object sender, RoutedEventArgs e)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("test");
            bool check = Convert.ToBoolean(this.cbLinq.IsChecked);

            if (check)
            {
                var col = db.GetCollection<Person>("people");
                var queryable = col.AsQueryable();
                var query = from p in queryable
                            where p.Age < 40 && p.Country == "Spain"
                            select p.Name;
                
                var names = new List<String>();
                foreach (var i in query)
                {                 
                    names.Add(i);
                }
                this.lbNames.ItemsSource = names;


            }
            else
            {
                var col = db.GetCollection<BsonDocument>("people");
                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Lt("age", 40) & builder.Eq("country", "Spain");
                var doc = await col.Find(filter).ToListAsync();
                var names = new List<String>();
                foreach (var i in doc)
                {
                    var x = i["name"];
                    names.Add(x.ToString());
                }
                this.lbNames.ItemsSource = names;
                }          
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("test");
            var col = db.GetCollection<BsonDocument>("people");
            var filter = Builders<BsonDocument>.Filter.Eq("name", this.tbName.Text);
            var update = Builders<BsonDocument>.Update.Set("car", this.tbCar.Text).CurrentDate("timestamp");      
            var doc = await col.UpdateManyAsync(filter, update);
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("test");
            var col = db.GetCollection<BsonDocument>("people");
            var filter = Builders<BsonDocument>.Filter.Eq("name", "Eva");
            var result = await col.DeleteManyAsync(filter);
        }
    }
}
