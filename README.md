Bindery
=======
Bindery aims to support fluent MVVM binding definition for WinForms applications.

Projects
--------
* **Bindery:** 
  * Contains the static `Create` factory class 
  * Dependent on the `Rx-Linq` package
* **Bindery.Commands:** 
  * Contains `CommandBase` and `EnablableCommandBase` (abstract base implementations of ICommand)
  * Dependent on the `Rx-Linq` package

Assumptions
-----------
* A view model is a binding source, an object of any type. Full binding functionality requires a view model to properly implement `System.ComponentModel.INotifyPropertyChanged`.
* A target is a binding target, an object of any type. A target only supports a limited set of binding functionality.
* A control is a target that implements `System.Windows.Forms.IBindableComponent`. A control supports the full range of binding functionality.
* A command is an object that implements `System.Windows.Input.ICommand`.

Code Examples
-------------
### Binding
##### Create a root binder for the view model
<pre><code>var binder = Create.Binder(viewModel);</code></pre>

##### Create a root binder and set the default subscription scheduler to schedule actions on the form's thread
<pre><code>var binder = Create.Binder(viewModel, new ControlScheduler(form));</code></pre>

##### Dispose of a binder
Diposing of a binder removes all bindings and disposes of all subcriptions created by the binder.
<pre><code>binder.Dispose();</code></pre>

##### Register external disposables with the binder
This can be used to tie the lifetime of other objects to the binder's lifetime.
<pre><code>binder.RegisterDisposable(disposableViewModel, disposableCommand);</code></pre>

##### Bind a view model property to a control property
<pre><code>// Two-way binding
binder.Control(textBox).Property(c => c.Text).Bind(vm => vm.Name); 
// One-way binding from source to control
binder.Control(form).Property(c => c.UseWaitCursor).Get(vm => vm.IsBusy); 
// One-way binding from control to source
binder.Control(textBox).Property(c => c.Text).Set(vm => vm.Name); 
</code></pre>

##### Bind an integer view model property to a string control property
<pre><code>binder.Control(textBox).Property(c => c.Text).Get(vm => vm.Age, Convert.ToString);
</code></pre>

##### Bind a button's `Click` event to a command
This also "binds" the control's `Enabled` property to the command's `CanExecute` method.
<pre><code>ICommand command = new CommandImplementation(viewModel);
binder.Control(button).OnClick(command);
</code></pre>

##### Bind a form's `MouseMove` event to a command
<pre><code>ICommand command = new CommandImplementation(viewModel);
binder.Control(form).OnEvent&lt;MouseEventArgs&gt;("MouseMove")
  .Transform(o => o.Where(e => e.Button==MouseButtons.Left).Select(e => new {e.X, e.Y})) 
  // Mouse coords are passed to command.Execute()
  .Execute(command);
</code></pre>

##### Bind a form's `MouseMove` event arguments to a view model property
<pre><code>binder.Control(form).OnEvent&lt;MouseEventArgs&gt;("MouseMove")
  .Transform(o => o.Select(e => new MyCoord{X = e.X, Y = e.Y}))
  .Set(vm => vm.CurrentMouseCoords);
</code></pre>

##### Bind to a non-control target object
Non-control targets support a limited set of binding options. Two-way binding and one-way binding from target to source are not supported.
<pre><code>binder.Target(target).Property(t => t.Status).Get(vm => vm.Status);</code></pre>

### Observable subscriptions

##### Trigger an action when a view model property changes
<pre><code>binder.OnPropertyChanged(vm => vm.ErrorMessage).Subscribe(msg => DisplayErrorDialog(msg));</code></pre>

##### Subscribe to a button's `Click` event to close the form
<pre><code>binder.Control(cancelButton).OnClick().Subscribe(e => form.Close());</code></pre>

##### Subscribe to a form's `Closed` event to dispose of the binder
<pre><code>binder.Control(form).OnEvent("Closed").Subscribe(e => binder.Dispose())</code></pre>

##### Create an observable subscription to execute a command
<pre><code>binder.Observe(viewModel.Observable).Execute(command);</code></pre>

##### Overriding the default scheduler to execute the command immediately on each observed object
<pre><code>binder.Observe(viewModel.Observable).ObserveOn(Scheduler.Immediate).Execute(command);</code></pre>

##### Subscribe to an observable with full subscription syntax support
<pre><code>binder.Observe(viewModel.Observable).Subscribe(
  ctx=>ctx.OnNext(oVal => OnNextAction(oVal))
       .OnError(ex => HandleException(ex))
       .OnComplete(() => OnCompleteAction()));
</code></pre>

### Event to observable conversion
<pre><code>IObservable&lt;string&gt; mouseMoveButtons =
  Create.ObservableFor(form).Event&lt;MouseEventArgs&gt;("MouseMove")
       .Select(e => Convert.ToString(e.Button));
</code></pre>

### Commands

##### CommandBase
`CommandBase` is an abstract implementation of `System.Windows.Input.ICommand` designed to be useful for MVVM command implementation.

* `Execute()` is abstract and must be implemented by an inheriting class.
* `CanExecute()` is implemented to return `true` but can be overridden by an inheriting class.
* The `CanExecuteChanged` event has a protected `OnCanExecuteChanged()` invocator method.
* A new `ObserveCanExecuteChanges()` method creates an `IObservable<bool>` that publishes the value of `CanExecute()` each time it changes, assuming the `CanExecuteChanged` event is triggered properly.

##### EnablableCommandBase
`EnablableCommandBase` is an abstract implementation of `CommandBase` that exposes an `Enabled` property. The `Enabled` property:

* Controls the return value of CanExecute()
* Raises the `CanExecuteChanged` event when its value changes
