#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Milan.JsonStore
{
  public abstract class DomainEntity : IDomainEntity, IDataErrorInfo
  {
    public DomainEntity()
    {
      Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    [JsonProperty]
    public bool IsReadonly
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    private readonly Dictionary<string, object> _PropertyValues = new Dictionary<string, object>();

    public event PropertyChangedEventHandler PropertyChanged;

    protected void Set<T>(T value, [CallerMemberName] string name = "")
    {
      if (_PropertyValues.ContainsKey(name))
      {
        var oldValue = _PropertyValues[name];

        if (oldValue != null &&
            oldValue.Equals(value))
        {
          return;
        }
      }
      else
      {
        _PropertyValues.Add(name, value);
      }
      _PropertyValues[name] = value;
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(name));
      }
    }

    protected T Get<T>([CallerMemberName] string name = "")
    {
      if (_PropertyValues.ContainsKey(name))
      {
        return (T) _PropertyValues[name];
      }
      else
      {
        return default(T);
      }
    }

    protected void RaisePropertyChanged(Expression<Func<object>> prop)
    {
      var propertyChanged = PropertyChanged;
      if (propertyChanged != null)
      {
        var expr = prop.Body as UnaryExpression;
        var member = (expr.Operand as MemberExpression).Member;
        propertyChanged(this, new PropertyChangedEventArgs(member.Name));
      }
    }

    public virtual int CompareTo(object obj)
    {
      return obj != this
               ? 1
               : 0;
    }

    private string _error;

    public string this[string propertyName]
    {
      get
      {
        var propertyValue = GetType()
          .GetProperty(propertyName)
          .GetValue(this, null);
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(this, null, null)
                      {
                        MemberName = propertyName
                      };
        Validator.TryValidateProperty(propertyValue, context, validationResults);
        if (validationResults.Count > 0)
        {
          _error = validationResults.FirstOrDefault(x => x.MemberNames.FirstOrDefault() == propertyName)
                                     .ErrorMessage;
          return Error;
        }
        else
        {
          _error = String.Empty;
          return Error;
        }
      }
    }

    public string Error
    {
      get { return _error; }
    }

  }
}