﻿// Copyright © 2020 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ExtCore.Events;
using Microsoft.AspNetCore.Http;
using Platformus.Website.Data.Entities;

namespace Platformus.Website.Events
{
  public interface IClassEditedEventHandler : IEventHandler<HttpContext, Class>
  {
  }
}