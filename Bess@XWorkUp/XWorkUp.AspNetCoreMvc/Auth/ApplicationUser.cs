using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace XWorkUp.AspNetCoreMvc.Auth
{
	// Add profile data for application users by adding properties to the ApplicationUser class
	public class ApplicationUser : IdentityUser
	{
		public DateTime BirthDate { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		//public Claim Claims { get; internal set; }
	}
}
