﻿// Copyright © 2015 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using ExtCore.Data.Entities.Abstractions;

namespace Platformus.Domain.Data.Entities
{
  /// <summary>
  /// Represents a data type. The data types are used to specify, how the object property should be displayed and edited.
  /// </summary>
  public class DataType : IEntity
  {
    public int Id { get; set; }
    public string StorageDataType { get; set; }
    public string JavaScriptEditorClassName { get; set; }
    public string Name { get; set; }
    public int? Position { get; set; }

    public virtual ICollection<Member> Members { get; set; }
    public virtual ICollection<DataTypeParameter> DataTypeParameters { get; set; }
  }
}