﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace Bindery.Interfaces
{
    public interface IControlObservableConversionBinder<TSource, TControl, TConverted>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent
    {
        IControlBinder<TSource, TControl> Executes(Func<TSource, ICommand> commandMember);
        IControlBinder<TSource, TControl> UpdateSource(Expression<Func<TSource, TConverted>> member);
    }
}