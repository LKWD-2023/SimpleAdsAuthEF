using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SimpleAdsAuth.Data
{
    public class SimpleAdDb
    {
        private readonly string _connectionString;

        public SimpleAdDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddSimpleAd(SimpleAd ad)
        {
            using var context = new SimpleAdsAuthDataContext(_connectionString);
            ad.Date = DateTime.Now;
            context.SimpleAds.Add(ad);
            context.SaveChanges();
        }

        public List<SimpleAd> GetAds()
        {
            using var context = new SimpleAdsAuthDataContext(_connectionString);
            return context.SimpleAds.Include(s => s.User)
                .OrderByDescending(a => a.Date)
                .ToList();
        }

        public List<SimpleAd> GetAdsForUser(int userId)
        {
            using var context = new SimpleAdsAuthDataContext(_connectionString);
            return context.SimpleAds.Include(s => s.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Date)
                .ToList();
        }

        public int GetUserIdForAd(int adId)
        {
            using var context = new SimpleAdsAuthDataContext(_connectionString);
            return context.SimpleAds.FirstOrDefault(a => a.Id == adId).UserId;
        }

        public void Delete(int id)
        {

            using var context = new SimpleAdsAuthDataContext(_connectionString);
            context.Database.ExecuteSqlInterpolated($"DELETE FROM SimpleAds WHERE Id = {id}");
        }
    }
}