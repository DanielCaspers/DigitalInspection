using System;
using System.Linq;
using System.Web.Mvc;
using DigitalInspection.Models;
using DigitalInspection.ViewModels;
using DigitalInspection.Services;
using System.Collections.Generic;

namespace DigitalInspection.Controllers
{
	public class WorkOrdersController : Controller
	{
		private static readonly string RESOURCE = "Work order";

		// TODO: Determine how to store WorkOrder -> Checklist relationship for persistence
		private ApplicationDbContext _context;

		public WorkOrdersController()
		{
			_context = new ApplicationDbContext();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_context.Dispose();
		}

		private WorkOrderMasterViewModel GetWorkOrderViewModel()
		{
			WorkOrder wo = new WorkOrder
			{
				Id = "987435543",
				EmployeeId = 4067,
				Status = WorkOrderStatus.NotStarted,
				Date = new DateTime(2017, 5, 30, 11, 15, 0),
				Customer = new Customer
				{
					FirstName = "Dan",
					LastName = "Caspers"
				},
				Vehicle = new Vehicle
				{
					VIN = "MH3RH06YXFK002818",
					Year = 2015,
					Make = "Yamaha",
					Model = "R3",
					Color = "Blue",
					LicensePlate = "293-GBE",
					LicenseState = "IL",
					Transmission = Transmission.Manual,
					EngineDisplacement = 0.3f,
					Mileage = 1222,
				}

			};

			WorkOrder wo2 = new WorkOrder
			{
				Id = "12345465",
				EmployeeId = 4056,
				Status = WorkOrderStatus.Complete,
				Date = new DateTime(2017, 4, 1, 1, 23, 56),
				Customer = new Customer
				{
					FirstName = "Rick",
					LastName = "Murphy"
				},
				Vehicle = new Vehicle
				{
					VIN = "MH3RH06YXFK002817",
					Year = 2016,
					Make = "Kia",
					Model = "Optima",
					Color = "Silver",
					Transmission = Transmission.Auto,
					LicensePlate = "293-GBE",
					LicenseState = "IL",
					EngineDisplacement = 3.9f,
					Mileage = 151500,
				}
			};

			WorkOrder wo3 = new WorkOrder
			{
				Id = "ujiasdfiuhasdf",
				EmployeeId = 4171,
				Status = WorkOrderStatus.NotStarted,
				Date = new DateTime(2017, 4, 1, 13, 26, 56),
				Customer = new Customer
				{
					FirstName = "Steve",
					LastName = "Caspers"
				},
				Vehicle = new Vehicle
				{
					VIN = "MH3RH06YXFK002817",
					Year = 2017,
					Make = "Toyota",
					Model = "Avalon",
					Color = "Crimson",
					Transmission = Transmission.Auto,
					LicensePlate = "672-KUB",
					LicenseState = "MN",
					EngineDisplacement = 3.5f,
					Mileage = 35000,
				}
			};

			List<WorkOrder> orders = new List<WorkOrder>();

			for(int i=0; i < 60; i++)
			{
				if (i % 3 == 0)
				{
					orders.Add(wo);
				}
				else if(i % 3 == 1)
				{
					orders.Add(wo3);
				}
				else
				{
					orders.Add(wo2);
				}
			}

			return new WorkOrderMasterViewModel
			{
				WorkOrders = orders
			};
		}

		// GET: Work Orders page and return response to index.cshtml
		public PartialViewResult Index()
		{
			return PartialView(GetWorkOrderViewModel());
		}

		// GET: _WorkOrderTable partial and return it to _WorkOrderTable.cshtml 
		public PartialViewResult _WorkOrderTable()
		{
			return PartialView(GetWorkOrderViewModel());
		}

	}
}
