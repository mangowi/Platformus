﻿// Copyright © 2021 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

namespace Platformus.Core.Backend
{
  [RestrictChildren("columns", "rows")]
  public class TableTagHelper : TagHelper
  {
    public class Column
    {
      public string Label { get; set; }
      public string SortingPropertyPath { get; set; }

      public Column(string label, string sortingPropertyPath = null)
      {
        this.Label = label;
        this.SortingPropertyPath = sortingPropertyPath;
      }
    }

    public enum SortingDirection
    {
      Ascending,
      Descending
    }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }
    public string Class { get; set; }

    public string Sorting { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
      output.SuppressOutput();

      List<Column> columns = new List<Column>();

      context.Items["Columns"] = columns;

      TagHelperContent content = await output.GetChildContentAsync();

      output.Content.AppendHtml(this.CreateTable(columns, content.GetContent()));
    }

    private TagBuilder CreateTable(IEnumerable<Column> columns, string rows)
    {
      TagBuilder tb = new TagBuilder(TagNames.Table);

      tb.AddCssClass("master-detail__table table" + (string.IsNullOrEmpty(this.Class) ? null : $" {this.Class}"));
      tb.InnerHtml.AppendHtml(this.CreateColGroups(columns));
      tb.InnerHtml.AppendHtml(this.CreateTBody(columns, rows));
      return tb;
    }

    private TagBuilder CreateColGroups(IEnumerable<Column> columns)
    {
      TagBuilder tb = new TagBuilder("colgroup");

      foreach (Column column in columns)
        tb.InnerHtml.AppendHtml(this.CreateColGroup(column));

      return tb;
    }

    private TagBuilder CreateColGroup(Column column)
    {
      TagBuilder tb = new TagBuilder("col");

      tb.TagRenderMode = TagRenderMode.SelfClosing;

      if (this.IsSortedByColumn(column))
        tb.AddCssClass("table__cell--ordered-by");

      return tb;
    }

    private TagBuilder CreateTBody(IEnumerable<Column> columns, string rows)
    {
      TagBuilder tb = new TagBuilder("tbody");

      tb.InnerHtml.AppendHtml(this.CreateRow(columns));

      if (string.IsNullOrWhiteSpace(rows))
        tb.InnerHtml.AppendHtml(this.CreateNoRecordsRow(columns));

      tb.InnerHtml.AppendHtml(rows);
      return tb;
    }

    private TagBuilder CreateNoRecordsRow(IEnumerable<Column> columns)
    {
      TagBuilder tb = new TagBuilder(TagNames.TR);

      tb.AddCssClass("table__row");
      tb.InnerHtml.AppendHtml(this.CreateNoRecordsCell(columns));
      return tb;
    }

    private TagBuilder CreateNoRecordsCell(IEnumerable<Column> columns)
    {
      TagBuilder tb = new TagBuilder(TagNames.TD);

      tb.AddCssClass("table__cell table__cell--no-records");
      tb.MergeAttribute(AttributeNames.ColSpan, columns.Count().ToString());

      IStringLocalizer<TableTagHelper> localizer = this.ViewContext.HttpContext.GetStringLocalizer<TableTagHelper>();

      tb.InnerHtml.AppendHtml(localizer["No records"]);
      return tb;
    }

    private TagBuilder CreateRow(IEnumerable<Column> columns)
    {
      TagBuilder tb = new TagBuilder(TagNames.TR);

      tb.AddCssClass("table__row");

      foreach (Column column in columns)
        tb.InnerHtml.AppendHtml(this.CreateCell(column));

      return tb;
    }

    private TagBuilder CreateCell(Column column)
    {
      TagBuilder tb = new TagBuilder(TagNames.TD);

      tb.AddCssClass("table__cell table__cell--header");

      if (string.IsNullOrEmpty(column.SortingPropertyPath))
        tb.InnerHtml.AppendHtml(column.Label);

      else
      {
        if (this.IsSortedByColumn(column))
        {
          tb.AddCssClass("table__cell--header-ordered-by");
          tb.AddCssClass(this.GetSortingDirection() == SortingDirection.Ascending ? "table__cell--header-ordered-by-asc" : "table__cell--header-ordered-by-desc");
        }

        tb.InnerHtml.AppendHtml(this.CreateSortingLink(column));
      }

      return tb;
    }

    private TagBuilder CreateSortingLink(Column column)
    {
      TagBuilder tb = new TagBuilder(TagNames.A);

      tb.AddCssClass("table__order-by");

      string sorting;

      if (string.Equals(column.SortingPropertyPath, this.GetSortingPropertyPath(), StringComparison.OrdinalIgnoreCase))
      {
        tb.AddCssClass("table__order-by--ordered-by");
        sorting = (this.GetSortingDirection() == SortingDirection.Ascending ? "-" : "%2B") + column.SortingPropertyPath.ToLower();
      }

      else sorting = "%2B" + column.SortingPropertyPath.ToLower();

      tb.MergeAttribute(
        AttributeNames.Href,
        this.ViewContext.HttpContext.Request.CombineUrl(
          new Url.Descriptor(name: "filter", takeFromUrl: true),
          new Url.Descriptor(name: "sorting", value: sorting),
          new Url.Descriptor(name: "offset", takeFromUrl: true),
          new Url.Descriptor(name: "limit", takeFromUrl: true)
        )
      );

      tb.InnerHtml.AppendHtml(column.Label);
      return tb;
    }

    private string GetSortingPropertyPath()
    {
      if (string.IsNullOrEmpty(this.Sorting))
        return null;

      return this.Sorting.Substring(1);
    }

    private SortingDirection GetSortingDirection()
    {
      if (string.IsNullOrEmpty(this.Sorting))
        return SortingDirection.Ascending;

      return this.Sorting[0] == '+' ? SortingDirection.Ascending : SortingDirection.Descending;
    }

    private bool IsSortedByColumn(Column column)
    {
      return !string.IsNullOrEmpty(column.SortingPropertyPath) && string.Equals(column.SortingPropertyPath, this.GetSortingPropertyPath(), StringComparison.OrdinalIgnoreCase);
    }
  }
}