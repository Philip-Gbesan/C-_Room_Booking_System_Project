using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.Json;

namespace C_Project
{
    public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity, new()
    {
        private readonly List<T> _items = new List<T>();
        private readonly string _filePath;

        public InMemoryRepository()
        {
            // File name based on entity type (Room -> rooms.json)
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, typeof(T).Name.ToLower() + "s.json");

            LoadFromFile();
        }

        public void Add(T entity)
        {
            if (_items.Any())
                entity.Id = _items.Max(i => i.Id) + 1;
            else
                entity.Id = 1;

            _items.Add(entity);
            SaveToFile();
        }

        public void Delete(int id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                _items.Remove(item);
                SaveToFile();
            }
        }

        public IEnumerable<T> GetAll()
        {
            return _items;
        }

        public T? GetById(int id)
        {
            return _items.FirstOrDefault(i => i.Id == id);
        }

        public void Update(T entity)
        {
            var index = _items.FindIndex(i => i.Id == entity.Id);
            if (index != -1)
            {
                // Replace in place
                _items[index] = entity;
                SaveToFile();
            }
        }


        private void SaveToFile()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_filePath, JsonSerializer.Serialize(_items, options));

            // Generic feedback message
            Console.WriteLine($"[✔] {typeof(T).Name} repository updated.");
        }

        public void ClearAll()
        {
            _items.Clear();
            SaveToFile(); // overwrite JSON with an empty list
            Console.WriteLine($"[{typeof(T).Name}] repository cleared.");
        }


        private void LoadFromFile()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var data = JsonSerializer.Deserialize<List<T>>(json);
                if (data != null)
                {
                    _items.AddRange(data);
                }
            }
        }
    }
}
