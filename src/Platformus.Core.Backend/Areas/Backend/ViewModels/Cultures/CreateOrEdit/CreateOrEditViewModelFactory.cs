﻿// Copyright © 2020 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Platformus.Core.Data.Entities;

namespace Platformus.Core.Backend.ViewModels.Cultures
{
  public static class CreateOrEditViewModelFactory
  {
    public static CreateOrEditViewModel Create(Culture culture)
    {
      if (culture == null)
        return new CreateOrEditViewModel()
        {
        };

      return new CreateOrEditViewModel()
      {
        Id = culture.Id,
        Name = culture.Name,
        IsFrontendDefault = culture.IsFrontendDefault,
        IsBackendDefault = culture.IsBackendDefault
      };
    }
  }
}