using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CSV_Data_Loader.Interfaces;
using CSV_Data_Loader.Models;
using CsvHelper;

namespace CSV_Data_Loader.Services
{
    public class CSVLoadService : ICSVLoadService
    {
        private string[] _headers;
        private Dictionary<string, int> _namePosition = new Dictionary<string, int>();
        private MyCompanyContext _context;

        public CSVLoadService(MyCompanyContext context)
        {
            _context = context;
        }

        public Type MatchEntity(StreamReader reader)
        {
            string header = reader.ReadLine();

            if (header == null)
            {
                return null;
            }

            _headers = header.Split(',');

            var type = typeof(IEntityBase);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(t => type.IsAssignableFrom(t));
            foreach (Type t in types)
            {
                var properties = t.GetProperties();
                if (_headers.Length > properties.Count())
                {
                    continue;
                }

                var propertyNames = properties.Select(p => p.Name).ToArray();

                if (propertyNames.Intersect(_headers).Any())
                {
                    return t;
                }
            }
            return null;
        }

        public void FindMappingPosition(Type type)
        {
            var properties = type.GetProperties();
            var propertyNames = properties.Select(p => p.Name).ToArray();

            foreach (var name in propertyNames)
            {
                for (var i = 0; i < _headers.Length; i++)
                {
                    if (_headers[i] == name)
                    {
                        _namePosition.Add(name, i);
                        break;
                    }
                }
            }

        }

        public void ProcessData(CsvReader reader, Type type)
        {
            var instance = Activator.CreateInstance(type);
            var properties = type.GetProperties();
            for (var i = 0; i < properties.Length; i++)
            {
                if(!_headers.Contains(properties[i].Name))
                {
                    continue;
                }

                var typeProperty = properties[i].PropertyType.Name;
                switch (typeProperty)
                {
                    case "Int32":
                        properties[i].SetValue(instance,
                                        int.Parse(reader[_namePosition[properties[i].Name]]));
                        break;
                    case "Decimal":
                        properties[i].SetValue(instance,
                                        decimal.Parse(reader[_namePosition[properties[i].Name]]));
                        break;
                    default:
                        properties[i].SetValue(instance, reader[_namePosition[properties[i].Name]]);
                        break;

                }
            }

            switch(type.Name)
            {
                case "Person":
                    _context.Add< Person>(instance as Person);
                    break;
                case "Product":
                    _context.Add(instance as Product);
                    break;
                default:
                    break;
            }
            _context.SaveChanges();
        }
    }
}
