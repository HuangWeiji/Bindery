using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Bindery.Extensions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class SourcePropertyBinder<TSource, TProp> : ISourcePropertyBinder<TSource, TProp>
    {
        private readonly SourceBinder<TSource> _sourceBinder;
        private readonly IObservable<TProp> _observable;

        public SourcePropertyBinder(SourceBinder<TSource> sourceBinder, Expression<Func<TSource, TProp>> member)
        {
            _sourceBinder = sourceBinder;
            _observable = CreateObservable(member);
        }

        public ISourceBinder<TSource> OnChanged(Action<TProp> action)
        {
            var subscription = AsObservable().Subscribe(action);
            _sourceBinder.AddSubscription(subscription);
            return _sourceBinder;
        }

        public IObservable<TProp> AsObservable()
        {
            return _observable;
        }

        private IObservable<TProp> CreateObservable(Expression<Func<TSource, TProp>> member)
        {
            var source = _sourceBinder.Source;
            var notifyPropertyChanged = source as INotifyPropertyChanged;
            if (notifyPropertyChanged==null)
                throw new NotSupportedException();
            return notifyPropertyChanged.CreatePropertyChangedObservable()
                .Where(args => args.PropertyName == member.GetAccessorName())
                .Select(x => member.Compile()(source));
        }
    }
}