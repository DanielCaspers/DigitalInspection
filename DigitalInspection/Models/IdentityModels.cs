﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using DigitalInspection.Models.Inspections;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DigitalInspection.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : IdentityUser
	{
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}
	}

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public DbSet<InspectionImage> InspectionImages { get; set; }
		public DbSet<Inspection> Inspections { get; set; }
		public DbSet<InspectionItem> InspectionItems { get; set; }
		public DbSet<InspectionMeasurement> InspectionMeasurements { get; set; }
		public DbSet<CannedResponse> CannedResponses { get; set; }
		public DbSet<Measurement> Measurements { get; set; }
		public DbSet<Checklist> Checklists { get; set; }
		public DbSet<ChecklistItem> ChecklistItems { get; set; }
		public DbSet<Tag> Tags { get; set; }

		public ApplicationDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		// https://stackoverflow.com/a/28950804/2831961
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			// base.OnModelCreating(modelBuilder);

			// Needed to ensure subclasses share the same table
			modelBuilder.Entity<ApplicationUser>()
				.ToTable("AspNetUsers");

			// modelBuilder.Ignore<ApplicationUser>();
			modelBuilder.Ignore<IdentityUserLogin>();
			modelBuilder.Ignore<IdentityRole>();
			modelBuilder.Ignore<IdentityUserRole>();
			modelBuilder.Ignore<IdentityUserClaim>();


		}
	}
}