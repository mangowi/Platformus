﻿// Copyright © 2020 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Magicalizer.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Platformus.Core.Data.Entities;
using Platformus.Core.Filters;

namespace Platformus
{
  public abstract class ModelStateTempDataTransferAttribute : ActionFilterAttribute
  {
    protected static readonly string Key = typeof(ModelStateTempDataTransferAttribute).FullName;
  }

  public class ExportModelStateToTempDataAttribute : ModelStateTempDataTransferAttribute
  {
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      Core.Controllers.ControllerBase controller = filterContext.Controller as Core.Controllers.ControllerBase;

      if (controller != null && !controller.ViewData.ModelState.IsValid)
      {
        if ((filterContext.Result is RedirectResult) || (filterContext.Result is RedirectToRouteResult))
        {
          IEnumerable<ModelStateWrapper> modelStateWrappers = controller.ViewData.ModelState.Select(
            ms => new ModelStateWrapper() {
              Key = ms.Key,
              Value = ms.Value.AttemptedValue,
              ValidationState = ms.Value.ValidationState
              //Errors = ms.Value.Errors.Select(e => e.ErrorMessage)
            }
          );

          ModelState modelState = this.CreateModelStateWithValue(
            filterContext, this.SerializeModelStateWrappers(modelStateWrappers)
          );

          controller.TempData[Key] = modelState.Id;
        }
      }

      base.OnActionExecuted(filterContext);
    }

    private ModelState CreateModelStateWithValue(ActionExecutedContext filterContext, string value)
    {
      ModelState modelState = new ModelState();

      modelState.Id = Guid.NewGuid();
      modelState.Value = value;
      modelState.Created = DateTime.Now;

      IStorage storage = filterContext.HttpContext.GetStorage();

      storage.GetRepository<Guid, ModelState, ModelStateFilter>().Create(modelState);
      storage.Save();
      return modelState;
    }

    private string SerializeModelStateWrappers(IEnumerable<ModelStateWrapper> modelStateWrappers)
    {
      return JsonConvert.SerializeObject(modelStateWrappers, Formatting.None, new JsonSerializerSettings() { ContractResolver = new NoCultureInfoResolver() });
    }
  }

  public class ImportModelStateFromTempDataAttribute : ModelStateTempDataTransferAttribute
  {
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      Core.Controllers.ControllerBase controller = filterContext.Controller as Core.Controllers.ControllerBase;

      if (controller != null)
      {
        if (controller.TempData.ContainsKey(Key))
        {
          Guid modelStateId = (Guid)controller.TempData[Key];
          ModelState modelState = filterContext.HttpContext.GetStorage().GetRepository<Guid, ModelState, ModelStateFilter>().GetByIdAsync(modelStateId).Result;
          IEnumerable<ModelStateWrapper> modelStateWrappers = this.DeserializeModelStateWrappers(modelState.Value);

          if (modelStateWrappers != null)
          {
            if (filterContext.Result is ViewResult)
            {
              foreach (ModelStateWrapper modelStateWrapper in modelStateWrappers)
              {
                controller.ViewData.ModelState.SetModelValue(modelStateWrapper.Key, modelStateWrapper.Value, modelStateWrapper.Value);
                controller.ViewData.ModelState[modelStateWrapper.Key].ValidationState = modelStateWrapper.ValidationState;

                //if (modelStateWrapper.ValidationState == ModelValidationState.Invalid)
                //  foreach (string error in modelStateWrapper.Errors)
                //    controller.ViewData.ModelState[modelStateWrapper.Key].Errors.Add(new ModelError(error));
              }
            }

            else controller.TempData.Remove(Key);
          }
        }
      }

      base.OnActionExecuted(filterContext);
    }

    private IEnumerable<ModelStateWrapper> DeserializeModelStateWrappers(string value)
    {
      return JsonConvert.DeserializeObject<IEnumerable<ModelStateWrapper>>(value, new JsonSerializerSettings() { Error = DeserializationErrorHandler });
    }

    private void DeserializationErrorHandler(object sender, ErrorEventArgs errorArgs)
    {
      errorArgs.ErrorContext.Handled = true;
    }
  }

  public class ModelStateWrapper
  {
    public string Key { get; set; }
    public string Value { get; set; }
    public ModelValidationState ValidationState { get; set; }
    //public IEnumerable<string> Errors { get; set; }
  }

  public class NoCultureInfoResolver : DefaultContractResolver
  {
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
      IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

      return properties.Where(p => p.PropertyType != typeof(CultureInfo)).ToList();
    }
  }
}