using System.Collections.Generic;
using System.Data;
using DemoWebsite.ViewModels;
using FreshCommonUtility.Dapper;
using FreshCommonUtility.SqlHelper;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebsite.Controllers
{
    public class CarController : Controller
    {
        //
        // GET: /Home/
        private IDbConnection _connection;

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }
        public ActionResult List()
        {
            IEnumerable<CarViewModel> result;
            using (_connection = SqlConnectionHelper.GetOpenConnection())
            {
                result = _connection.GetList<CarViewModel>();
            }
            return View(result);
        }

        public ActionResult Details(int id)
        {
            CarViewModel result;
            using (_connection = SqlConnectionHelper.GetOpenConnection())
            {
                result = _connection.Get<CarViewModel>(id);
            }
            return View(result);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CarViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (_connection = SqlConnectionHelper.GetOpenConnection())
                {
                    _connection.Insert(model);
                }
                return RedirectToAction("index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            CarViewModel model;
            using (_connection = SqlConnectionHelper.GetOpenConnection())
            {
                model = _connection.Get<CarViewModel>(id);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CarViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (_connection = SqlConnectionHelper.GetOpenConnection())
                {
                    _connection.Update(model);
                }
                return RedirectToAction("index");
            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            using (_connection = SqlConnectionHelper.GetOpenConnection())
            {
                _connection.Delete<CarViewModel>(id);
            }
            return RedirectToAction("index");
        }


    }
}
