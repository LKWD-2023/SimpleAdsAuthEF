﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleAdsAuth.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SimpleAdsAuth.Data;

namespace SimpleAdsAuth.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult Index()
        {
            SimpleAdDb db = new SimpleAdDb(_connectionString);
            List<SimpleAd> ads = db.GetAds();
            var currentUserId = GetCurrentUserId();
            var vm = new HomePageViewModel
            {
                Ads = ads.Select(ad => new AdViewModel
                {
                    Ad = ad,
                    CanDelete = currentUserId != null && ad.UserId == currentUserId
                }).ToList()
            };

            return View(vm);
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult NewAd(SimpleAd ad)
        {
            int? userId = GetCurrentUserId();
            ad.UserId = userId.Value;
            SimpleAdDb db = new SimpleAdDb(_connectionString);
            db.AddSimpleAd(ad);

            return Redirect("/");
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            SimpleAdDb db = new SimpleAdDb(_connectionString);

            var userIdForAd = db.GetUserIdForAd(id);

            var currentUserId = GetCurrentUserId().Value;
            if (currentUserId == userIdForAd)
            {
                db.Delete(id);
            }

            return Redirect("/");
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            SimpleAdDb db = new SimpleAdDb(_connectionString);
            var userId = GetCurrentUserId().Value;
            return View(db.GetAdsForUser(userId));
        }

        public IActionResult Test()
        {
            return Json(new SimpleAd
            {
                Id = 10,
                Date = DateTime.Now,
                Description = "My Thing",
                PhoneNumber = "234234234234",
                UserId = 89
            });
        }

        private int? GetCurrentUserId()
        {
            var userDb = new UserDb(_connectionString);
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }
            var user = userDb.GetByEmail(User.Identity.Name);
            if (user == null)
            {
                return null;
            }

            return user.Id;
        }
    }
}
