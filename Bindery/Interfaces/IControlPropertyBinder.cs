﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using Bindery.Interfaces.Basic;

namespace Bindery.Interfaces
{
    public interface IControlPropertyBinder<TSource, TControl, TControlProp> : IBasicControlPropertyBinder<TSource, TControl, TControlProp>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent
    {
        IControlBinder<TSource, TControl> Get(Expression<Func<TSource, TControlProp>> sourceMember);
        IControlBinder<TSource, TControl> Get<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember, Func<TSourceProp, TControlProp> convertToControlPropertyType);

        IControlBinder<TSource, TControl> Bind(
            Expression<Func<TSource, TControlProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);

        IControlBinder<TSource, TControl> Bind<TSourceProp>(
            Expression<Func<TSource, TSourceProp>> sourceMember,
            Func<TSourceProp, TControlProp> convertToControlPropertyType,
            Func<TControlProp, TSourceProp> convertToSourcePropertyType,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);
    }
}
