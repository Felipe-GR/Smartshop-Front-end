using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockInfo.Models;

namespace StockInfo.Repository
{
    public interface IDatabaseRepository
    {
        void DeleteStock(int id);
        void AddStock(string name, string description);

        void UpdateStockInfo(int id, string name, string description);
        Stock GetStockInfo(int itemId);

        List<Stock> GetSearchResults(string name);

        List<Stock> GetStockInfo();
    }
}
