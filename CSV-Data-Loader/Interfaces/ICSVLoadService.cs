using System;
using System.IO;
using CsvHelper;

namespace CSV_Data_Loader.Interfaces
{
    public interface ICSVLoadService
    {
        Type MatchEntity(StreamReader reader);
        void FindMappingPosition(Type type);
        void ProcessData(CsvReader reader, Type type);
    }
}
