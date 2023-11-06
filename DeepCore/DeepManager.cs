using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using UnitODB.Data.SQLite; 
using System.IO;
using System.Data;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Shared;

namespace DeepCore
{
    public class DeepManager
    {
        public static DeepManager Instance { get; } = new DeepManager();

        public string Database { get; set; }
        private string _conn;

        public IList<DeepMatch> Ranklist { get; set; }
        public string Wallpaper { get; set; }
        public bool AutoRanking { get; set; } 

        private UnitOfWork unit;
        public UnitOfWork Unit
        {
            get
            {
                if (this.unit == null)
                {
                    this.unit = this.GetContainer();
                }

                return this.unit;
            }
        }
        private DeepManager()
        {
            this.Ranklist = new List<DeepMatch>();
            
            this.AutoRanking = true; 
        }

        public UnitOfWork GetContainer()
        {  
            var connStr = this.GetConnStr(this.Database);

            var context = new SQLiteOdbContext(connStr);

            context.Depth = 2;

            return new UnitOfWork(context);
        }

        public string GetConnStr(string file)
        {
            if (string.IsNullOrEmpty(this._conn))
            {
                this._conn = ConfigurationManager.AppSettings["Connection"].ToString();
            }

            return string.Format(this._conn, file);
        }

        public void Save(DeepLap car)
        { 
            int id = this.Unit.Context.ExecuteStore(car);

            car.Id = id;
        } 

        public void Dispose()
        {
            if (this.unit != null)
            {
                this.unit.Dispose();
                this.unit = null;
            }
        }

        public void Init()
        { 
            using (var ct = this.GetContainer())
            {
                ct.AutoCommit = false;

                ct.Create<DeepLap>();
                 
                ct.Commit();
            }
        }

        public void Save(object obj, string file)
        {
            FileInfo fi = new FileInfo(file);

            string json = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);

            using (StreamWriter writer = fi.CreateText())
            {
                writer.Write(json);                
            }
        }

        public T Load<T>(string file) where T : class
        {
            FileInfo fi = new FileInfo(file);

            using (StreamReader reader = fi.OpenText())
            {
                var json = reader.ReadToEnd();
 
                return json.Deserialize<T>();                
            }
        }
    }
}
