﻿// Copyright © 2020 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Platformus.Website.Data.Entities;

namespace Platformus.Website.Backend.ViewModels.DataTypeParameters
{
  public static class CreateOrEditViewModelFactory
  {
    public static CreateOrEditViewModel Create(DataTypeParameter dataTypeParameter)
    {
      if (dataTypeParameter == null)
        return new CreateOrEditViewModel()
        {
        };

      return new CreateOrEditViewModel()
      {
        Id = dataTypeParameter.Id,
        JavaScriptEditorClassName = dataTypeParameter.JavaScriptEditorClassName,
        Code = dataTypeParameter.Code,
        Name = dataTypeParameter.Name
      };
    }
  }
}