﻿using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Data.Notification
{
	[CLSCompliant(false)]
	[LiquidType("Email", "FirstName", "LastName")]
	public class RegistrationNotification : Core.Notification.Notification
	{
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}
