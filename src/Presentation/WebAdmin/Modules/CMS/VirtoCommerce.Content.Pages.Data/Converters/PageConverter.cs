﻿using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Content.Pages.Data.Models;

namespace VirtoCommerce.Content.Pages.Data.Converters
{
	public static class PageConverter
	{
		public static ShortPageInfo ToShortModel(this Page page)
		{
			var retVal = new ShortPageInfo();

			retVal.Name = Path.GetFileNameWithoutExtension(page.Path);
			retVal.LastModified = page.ModifiedDate ?? page.CreatedDate;

			return retVal;
		}

		public static ShortPageInfo ToShortModel(this RepositoryContent repositoryContent, DateTime modifiedDate)
		{
			var retVal = new ShortPageInfo();

			retVal.Name = Path.GetFileNameWithoutExtension(repositoryContent.Name);
			retVal.LastModified = modifiedDate;

			return retVal;
		}

		public static Page ToPageModel(this RepositoryContent repositoryContent)
		{
			var retVal = new Page();

			retVal.Name = Path.GetFileNameWithoutExtension(repositoryContent.Name);
			retVal.Content = repositoryContent.Content;

			return retVal;
		}
	}
}