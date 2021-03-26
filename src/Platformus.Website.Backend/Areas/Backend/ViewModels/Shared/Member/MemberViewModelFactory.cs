﻿// Copyright © 2020 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Platformus.Core.Backend.ViewModels;
using Platformus.Website.Data.Entities;

namespace Platformus.Website.Backend.ViewModels.Shared
{
  public class MemberViewModelFactory : ViewModelFactoryBase
  {
    public MemberViewModel Create(Member member)
    {
      return new MemberViewModel()
      {
        Id = member.Id,
        Name = member.Name,
        Position = member.Position,
        PropertyDataType = member.PropertyDataType == null ? null : new DataTypeViewModelFactory().Create(member.PropertyDataType),
        RelationClass = member.RelationClass == null ? null : new ClassViewModelFactory().Create(member.RelationClass)
      };
    }
  }
}