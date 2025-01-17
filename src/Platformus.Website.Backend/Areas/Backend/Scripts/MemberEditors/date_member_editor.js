﻿// Copyright © 2020 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

(function (platformus) {
  platformus.memberEditors = platformus.memberEditors || [];
  platformus.memberEditors.date = {};
  platformus.memberEditors.date.create = function (container, member) {
    createField(member).appendTo(container);
  };

  function createField(member) {
    var field = $("<div>").addClass("date-editor").addClass("form__field").addClass("field");

    platformus.controls.label.create({ text: member.name }).appendTo(field);
    createTextBox(member).appendTo(field);
    return field;
  }

  function createTextBox(member) {
    var value = null;

    if (member.property.dateTimeValue) {
      value = moment(member.property.dateTimeValue);

      if (!value.isValid()) {
        value = moment();
      }

      value = value.format(platformus.globalization.getDateFormat());
    }

    return platformus.controls.textBox.create(
      {
        identity: platformus.memberEditors.base.getIdentity(member),
        value: value,
        validation: {
          isRequired: platformus.memberEditors.base.getIsRequiredDataTypeParameterValue(member),
          maxLength: platformus.memberEditors.base.getMaxLengthDataTypeParameterValue(member)
        }
      }
    ).attr("data-type", "date");
  }
})(window.platformus = window.platformus || {});