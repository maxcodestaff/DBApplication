using DBApplication;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class WeaponService
    {
        private readonly WeaponsDAO _weaponsDAO;
        private readonly ILogger<WeaponService> _logger;

        public WeaponService(WeaponsDAO weaponsDAO, ILogger<WeaponService> logger)
        {
            _weaponsDAO = weaponsDAO;
            _logger = logger;
        }

        public List<Weapon> GetAllWeapons()
        {
            return _weaponsDAO.GetAllWeapons();
        }

        public Weapon GetWeaponById(int id)
        {
            var weapon = _weaponsDAO.GetWeaponById(id);
            if (weapon == null)
            {
                throw new InvalidOperationException($"Weapon with id {id} does not exist");
            }
            return weapon;
        }

        public string GetAllWeaponsJson()
        {
            var weapons = GetAllWeapons();
            var json = JsonUtility.ToJson(weapons);
            _logger.LogInformation($"GetAllWeaponsJson: {json}");
            return json;
        }

        public string GetWeaponByIdJson(int id)
        {
            var weapon = GetWeaponById(id);
            var json = JsonUtility.ToJson(weapon);
            _logger.LogInformation($"GetWeaponByIdJson (id: {id}): {json}");
            return json;
        }
    }

    public static class JsonUtility
    {
        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }
}