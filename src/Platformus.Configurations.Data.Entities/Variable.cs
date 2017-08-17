﻿// Copyright © 2015 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ExtCore.Data.Entities.Abstractions;

namespace Platformus.Configurations.Data.Entities
{
  /// <summary>
  /// Represents a configuration variable. The variables are used to store the configuration values.
  /// </summary>
  public class Variable : IEntity
  {
    public int Id { get; set; }
    public int ConfigurationId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public int? Position { get; set; }

    public virtual Configuration Configuration { get; set; }
  }
}