﻿using Csla.Core;
using Csla.Rules;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Blazor
{
  /// <summary>
  /// Validation message base type
  /// </summary>
	public class CslaValidationMessageBase : ComponentBase, IDisposable
	{

    /// <summary>
    /// Value indicating whether validation is initiated
    /// </summary>
		protected bool _validationInitiated = false;
		private EditContext _previousEditContext;
		private EventHandler<FieldChangedEventArgs> _fieldChangedHandler;
		private EventHandler<ValidationStateChangedEventArgs> _validationStateChangedHandler;

    /// <summary>
    /// Gets or sets the property name
    /// </summary>
		[Parameter] public string PropertyName { get; set; }
    /// <summary>
    /// Gets or sets the wrapper id
    /// </summary>
    [Parameter] public string WrapperId { get; set; } = "wrapper";
    /// <summary>
    /// Gets or sets the wrapper class
    /// </summary>
    [Parameter] public string WrapperClass { get; set; } = "validation-messages";
    /// <summary>
    /// Gets or sets the error class
    /// </summary>
		[Parameter] public string ErrorClass { get; set; } = "text-danger";
    /// <summary>
    /// Gets or sets the warning class
    /// </summary>
		[Parameter] public string WarningClass { get; set; } = "text-warning";
    /// <summary>
    /// Gets or sets the info class
    /// </summary>
		[Parameter] public string InfoClass { get; set; } = "text-info";
    /// <summary>
    /// Gets or sets the error wrapper class
    /// </summary>
    [Parameter] public string ErrorWrapperClass { get; set; } = "error-messages";
    /// <summary>
    /// Gets or sets the warning wrapper class
    /// </summary>
    [Parameter] public string WarningWrapperClass { get; set; } = "warning-messages";
    /// <summary>
    /// Gets or sets the info wrapper class
    /// </summary>
    [Parameter] public string InfoWrapperClass { get; set; } = "information-messages";
    /// <summary>
    /// Gets or sets the current edit context
    /// </summary>
    [CascadingParameter] protected EditContext CurrentEditContext { get; set; }

		#region Event Handlers
		
    /// <summary>
    /// On initialized method
    /// </summary>
		protected override void OnInitialized()
		{
			// Initialise event handler delegates for use in capturing state changes
			_fieldChangedHandler = (sender, eventArgs) => OnFieldChanged(sender, eventArgs);
			_validationStateChangedHandler = (sender, eventArgs) => OnValidationStateChanged(sender, eventArgs);
		}

    /// <summary>
    /// On parameters set method
    /// </summary>
		protected override void OnParametersSet()
		{
			// Check that the required parameters have been provided
			if (CurrentEditContext == null)
			{
				throw new InvalidOperationException(
          string.Format(Csla.Properties.Resources.CascadingEditContextRequiredException,
          nameof(CslaValidationMessages), nameof(EditContext)));

      }

      if (string.IsNullOrWhiteSpace(PropertyName))
			{
				throw new InvalidOperationException(
          string.Format(Csla.Properties.Resources.ParameterRequiredException, 
          nameof(CslaValidationMessages), nameof(PropertyName)));
			}

			// Wire up handling of the state change events we need event
			if (CurrentEditContext != _previousEditContext)
			{
				DetachPreviousEventHandlers();
				CurrentEditContext.OnFieldChanged += _fieldChangedHandler;
				CurrentEditContext.OnValidationStateChanged += _validationStateChangedHandler;
				_previousEditContext = CurrentEditContext;
			}
		}

    /// <summary>
    /// On field changed method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
		protected void OnFieldChanged(object sender, FieldChangedEventArgs eventArgs)
		{
			if (eventArgs.FieldIdentifier.FieldName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase))
			{
				_validationInitiated = true;
				StateHasChanged();
			}
		}

    /// <summary>
    /// On validation state changed method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
		protected void OnValidationStateChanged(object sender, ValidationStateChangedEventArgs eventArgs)
		{
			IEnumerable<string> messages;

			// Retrieve the messages for the property in which we are interested
			messages = CurrentEditContext.GetValidationMessages(CurrentEditContext.Field(PropertyName));

			if (messages.Any())
			{
				// If any messages are present then validation has been run
				// This is a cheat used to identify that the form has been submitted with invalid data
				_validationInitiated = true;
			}

			StateHasChanged();
		}

    /// <summary>
    /// Detatch previous event handlers
    /// </summary>
		protected void DetachPreviousEventHandlers()
		{
			if (_previousEditContext != null)
			{
				// Unhook any event handler made to a different edit context
				_previousEditContext.OnFieldChanged -= _fieldChangedHandler;
				_previousEditContext.OnValidationStateChanged -= _validationStateChangedHandler;
			}
		}

		#endregion

		#region Message Retrieval

    /// <summary>
    /// Get error messages
    /// </summary>
    /// <returns></returns>
		protected IEnumerable<string> GetErrorMessages()
		{
			return GetBrokenRuleMessages(PropertyName, RuleSeverity.Error);
		}

    /// <summary>
    /// Get warning messages
    /// </summary>
    /// <returns></returns>
		protected IEnumerable<string> GetWarningMessages()
		{
			return GetBrokenRuleMessages(PropertyName, RuleSeverity.Warning);
		}

    /// <summary>
    /// Get info messages
    /// </summary>
    /// <returns></returns>
		protected IEnumerable<string> GetInfoMessages()
		{
			return GetBrokenRuleMessages(PropertyName, RuleSeverity.Information);
		}

		private IEnumerable<string> GetBrokenRuleMessages(string propertyName, RuleSeverity severity)
		{
			IList<string> messages = new List<string>();
			ICheckRules objectUnderTest;

			// Attempt to gain access to the underlying CSLA object
			objectUnderTest = CurrentEditContext.Model as ICheckRules;
			if (objectUnderTest == null)
			{
        throw new ArgumentException("Model");
      }

			// Iterate through the broken rules to find the subset we want
			foreach (BrokenRule rule in objectUnderTest.GetBrokenRules())
			{
				// Exclude any broken rules that are not for the property we are interested in
				if (rule.Property.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
				{
					// Exclude any of a severity other than that we want
					if (rule.Severity == severity)
					{
						// Rule meets our criteria, so add its text to the list we are to return
						messages.Add(rule.Description);
					}
				}
			}

			// Return the list of messages that matched the criteria
			return messages;
		}

		#endregion

		#region IDisposable Interface

    /// <summary>
    /// Dispose the object
    /// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

    /// <summary>
    /// Dispose the object
    /// </summary>
    /// <param name="disposing">Disposing</param>
		protected virtual void Dispose(bool disposing)
		{
      if (disposing)
        DetachPreviousEventHandlers();
    }

    #endregion

  }
}
