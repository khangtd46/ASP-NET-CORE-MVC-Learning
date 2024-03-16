﻿using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Ultilities
{
	public class ValidEmailDomainAttribute : ValidationAttribute
	{
		private readonly string allowedDomain;

		public ValidEmailDomainAttribute(string allowedDomain)
        {
			this.allowedDomain = allowedDomain;
		}
        public override bool IsValid(object? value)
		{
			string[] strings = value.ToString().Split('@');
			return strings[1].ToUpper() == allowedDomain.ToUpper();
		}
	}
}
