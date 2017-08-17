﻿// Copyright © 2015 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ExtCore.Data.Entities.Abstractions;

namespace Platformus.Domain.Data.Entities
{
  /// <summary>
  /// Represents a tab. The tabs are used to group the members.
  /// </summary>
  public class Tab : IEntity
  {
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string Name { get; set; }
    public int? Position { get; set; }

    public virtual Class Class { get; set; }
  }
}