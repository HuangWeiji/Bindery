﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace Bindery.Interfaces
{
    public interface IControlBinder<TSource, TControl>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent 
    {
         IControlPropertyBinder<TSource, TControl, TProp> Property<TProp>(Expression<Func<TControl, TProp>> member);
        IControlEventBinder<TSource, TControl, EventArgs> OnEvent(string eventName);
        IControlEventBinder<TSource, TControl, TEventArgs> OnEvent<TEventArgs>(string eventName);
        IControlBinder<TSource, TControl> OnClick(Func<TSource, ICommand> commandMember);
         IControlObservableBinder<TSource, TControl, TArg> On<TArg>(Func<TControl, IObservable<TArg>> observableMember);
    }

}
