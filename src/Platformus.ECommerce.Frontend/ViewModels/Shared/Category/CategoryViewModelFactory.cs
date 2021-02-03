﻿// Copyright © 2020 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Platformus.Core.Frontend.ViewModels;
using Platformus.ECommerce.Data.Entities;

namespace Platformus.ECommerce.Frontend.ViewModels.Shared
{
  public class CategoryViewModelFactory : ViewModelFactoryBase
  {
    public CategoryViewModel Create(Category category)
    {
      return new CategoryViewModel()
      {
        Name = category.Name.GetLocalizationValue(),
        Categories = category.Categories == null ? Array.Empty<CategoryViewModel>() : category.Categories.OrderBy(c => c.Position).Select(
          c => new CategoryViewModelFactory().Create(c)
        ).ToArray()
      };
    }
  }
}