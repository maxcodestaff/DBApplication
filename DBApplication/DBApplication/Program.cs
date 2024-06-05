using MySqlConnector;

namespace DBApplication
{
    public interface IDatabaseConnectionManager
    {
        bool OpenConnection();
        bool CloseConnection();
        List<Dictionary<string, object>> ExecuteQuery(string l_query);
    }

    public class MySqlConnectionManager : IDatabaseConnectionManager
    {
        public MySqlConnection connection;
        private string _connectionString;

        public MySqlConnectionManager(string l_connectionString)
        {
            this._connectionString = l_connectionString;
            this.connection = new MySqlConnection(_connectionString);
            Initialize();
        }

        private void Initialize()
        {
            connection = new MySqlConnection(_connectionString);
        }

        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public List<Dictionary<string, object>> ExecuteQuery(string l_query)
        {
            List<Dictionary<string, object>> l_data = new List<Dictionary<string, object>>();
            if (OpenConnection())
            {
                using (MySqlCommand l_command = new MySqlCommand(l_query, connection))
                {
                    using (MySqlDataReader l_dataReader = l_command.ExecuteReader())
                    {
                        while (l_dataReader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < l_dataReader.FieldCount; i++)
                            {
                                row.Add(l_dataReader.GetName(i), l_dataReader.GetValue(i));
                            }
                            l_data.Add(row);
                        }
                    }
                }
                CloseConnection();
            }
            return l_data;
        }
    }

    public class Weapon
    {
        private int Id;
        private string Name;
        private int Damage;
        private string Image;

        public Weapon()
        {
            Name = string.Empty;
            Image = string.Empty;
        }

        #region properties
        public int id
        {
            get { return Id; }
            set { Id = value; }
        }

        public string name
        {
            get { return Name; }
            set { Name = value; }
        }

        public int damage
        {
            get { return Damage; }
            set { Damage = value; }
        }

        public string image
        {
            get { return Image; }
            set { Image = value; }
        }
        #endregion
    }

    public class WeaponsDAO
    {
        private IDatabaseConnectionManager _connectionManager;

        public WeaponsDAO(IDatabaseConnectionManager l_connectionManager)
        {
            _connectionManager = l_connectionManager;
        }

        public List<Weapon> GetAllWeapons()
        {
            List<Weapon> Weapons = new List<Weapon>();
            string l_query = "SELECT * FROM Weapons";
            List<Dictionary<string, object>> result = _connectionManager.ExecuteQuery(l_query);

            foreach (var row in result)
            {
                Weapon weapon = new Weapon
                {
                    id = Convert.ToInt32(row["id"]),
                    name = row["name"]?.ToString() ?? string.Empty,
                    damage = Convert.ToInt32(row["damage"]),
                    image = row["image"]?.ToString() ?? string.Empty,
                };

                Weapons.Add(weapon);
            }
            return Weapons;
        }

        public Weapon? GetWeaponById(int id)
        {
            string query = $"SELECT * FROM Weapons WHERE id = {id}";
            List<Dictionary<string, object>> result = _connectionManager.ExecuteQuery(query);

            if (result.Count > 0)
            {
                Dictionary<string, object> row = result[0];
                Weapon weapon = new Weapon
                {
                    id = Convert.ToInt32(row["id"]),
                    name = row["name"]?.ToString() ?? string.Empty,
                    damage = Convert.ToInt32(row["damage"]),
                    image = row["image"]?.ToString() ?? string.Empty
                };
                return weapon;
            }
            else
            {
                return null;
            }
        }
    }
}