# TestMvcApplication

A repository for ASP.NET MVC patterns and best practices.

## Purpose
Quite some time ago I started writing a book. It was about how to write enterprise applications using ASP.NET MVC and back-end services. However, it didn't focus on teaching the technologies involved. Instead it focused on how to properly configure a solution, how to minimize dependencies and how to build a system that is both configurable and testable. Basically, it covered topics that would have saved me years of trial and error when I was new to .NET development.

I ended up realizing that my book was about how to architect a system and, honestly, that topic is just too big for one book. I also realized that I wasn't as advanced in some topics as I needed to be, such as continuous integration. The most I could say is "you should be doing this". In the case of continuous integration I would have needed to explain how to configure TeamCity or Jenkins step-by-step on a separate server.

My book was centered around developing ASP.NET MVC project, gradually introducing different problems and ways to solve them. I am publishing the code that resulted from months of research. Even if you think you're an expert MVC developer, I can almost guarantee there is some code contained here that will interest you. A lot of best practices can be carried over to any project, MVC or not. Some of this code will seem fairly boilerplate. Other code could very well change your whole approach to software development.

## Points of Interest
To save you time from pouring over hundreds of lines of code, I will try to direct your attention to features that will be useful for you.

### Service Interfaces
This project contains the interfaces to the data/service layer. Here the interfaces for [Repositories](http://martinfowler.com/eaaCatalog/repository.html) are found. You will also find the [DTO](http://martinfowler.com/eaaCatalog/dataTransferObject.html)s that are passed to/returned by the repositories. This project is also where the interfaces for connecting to remote services, the file system, an SMTP server, etc. would go.

`ServiceException` is used to indicate that an error occurred at the service layer. No other type of exception should be thrown by this layer. Different services can create sub-classes of this exception. This way consumers of this service can assume non-`ServiceException`s are fatal errors and not just the result of a database being down or service being unavailable. This exception handling policy is related to [checked vs unchecked exceptions](http://stackoverflow.com/questions/6115896/java-checked-vs-unchecked-exception-explanation).

The DTOs are just plain objects. They are intentionally simple. `Customer` represents the values that are expected to be returned from the database. It is comprised of public accessor/mutators and doesn't attempt to hide anything. Sometimes it may be useful to implement `ICloneable` to support copying DTOs.

The `ICustomerRepository` has methods for interacting with the database. Since this is just an interface, nothing should be exposed regarding whether we're interacting with a database, a service, etc. In this solution, I am creating a repository for each table. In a different situation, I might create a repository for a particular feature.

### Policies
This project is extremely important for making an application configurable. This project, along with Ninject, makes it possible to do [aspect-oriented programming (AOP)](http://en.wikipedia.org/wiki/Aspect-oriented_programming). This allows us to program declaratively to do things like manage database transactions, exception handling, logging and a whole lot more.

`AttributeInterceptor` is an abstract class. It provides a single method for getting custom attributes from a method. Most policies are declared using attributes. For instance, we might indicate that a method should run within a database transaction by doing something like this:

    [Transaction]
    public void CreateAccount(AccountDetail details)
    {
        // do some work
    }

We can configure the system to look for methods with this attribute and decorate them so additional code is run before and/or after the method. In this case, we would create an instance of `TransactionScope` to create a database transaction.

A common operation in a layered architecture is to simply capture exceptions from a lower layer and wrap them with a new exception. This is what `ExceptionInterceptor` does. The idea is that the cause of the exception can't be handled yet but we don't want to burden higher layers with handling low-level exception types. Allowing certain types of exceptions to bubble up could actually break our abstraction. `ExceptionInterceptor` will inspect the decorated method for an optional `ErrorMessage` attribute and use it as the new exception's message.

`TransactionInterceptor` will start a transaction before executing the method and will commit if it finishes without throwing an exception.

Other common operations, such as logging can be performed in a declarative way. Just defining some arbitrary classes won't accomplish anything. Take a look at the `NinjectWebCommon` class in the TestMvcApplication - it will explain how these interceptors are hooked up.

### Data Modeling
This project contains an implementation for the `ICustomerRepository` interface. This solution uses Entity Framework with SQL Server to store customer data. When building large systems, it is usually important to hide which underlying data access tools you are using. The Repository pattern achieves this for us nicely. If we decided to move our data layer to a separate service (maybe for load balancing or to be closer to the database), the repository interface will shield most of the code from the change.

In order to implement the `ICustomerRepository` interface, `CustomerRepository` accepts an instance of `EntitySet`. `EntitySet` is a thin wrapper around a `DbContext` class, a member of the Entity Framework. This class is configured to allow us to use raw DTOs or [POCO](http://msdn.microsoft.com/en-us/library/vstudio/dd456853(v=vs.100).aspx)s. Normally, Entity Framework will auto-generate our data objects for us based on the entity model; however, these classes have a lot of additional logic for managing their state internally which in larger systems can lead to EF concerns bleeding into other layers.

The `EntitySet` class provides access to a collection (`IQueryable`) of customers. It also provides raw access to the underlying database connection (a `SqlConnection` in this case) for occasions when you need to circumvent EF. This class is `IDisposable` meaning that it manages the lifetime of the connection to SQL Server. It provides a `SaveChanges` methods for persisting any local changes to the database. This class is also where any convenience methods for the data layer would be placed. Take a look at the `GetEntities` method for an example - a method that makes it possible to map hand-written SQL results to entities in our model.

Although it is not used in this solution, the `ConnectionManager` is a useful class. It will open a closed connection and automatically close it when it is disposed. If the connection is already open, it simply does nothing. This is useful when working with EF because EF opens and closes connections between operations (but thanks to connection pooling this has no overhead). This class makes sure the connection is always open for hand-written SQL.

I make it really easy to escape the confines of Entity Framework. It is tempting to do everything in terms of EF and that can lead to problems. EF provides its own list of convenience methods for calling stored procedures and even its own dialect of SQL for fine-grain control. However, your system may need to interact with an existing data layer expecting a database connection. Or you may have team members who prefer working directly with ADO.NET. In my experience, almost all of my code could be writting using EF and only one out of a hundred SQL needed me to work around it.

Designing a repository can be tricky. Like most classes, creating the wrong interface can leak assumptions about how the repository is implemented. If you inspect the `Add`, `Update` and `Remove` methods, you'll notice that I call `SaveChanges`. Another option would have been to move the responsibility of calling `SaveChanges` into a separate class (call it `Synchronizer`). This way you could make lots of different changes and then force EF to synchronize with the database all at one time. However, that approach has a weakness - if we move away from EF to, say, a web service, clients will assume nothing will happen until we call `SaveChanges` on `Synchronizer`. This means our web service would have to be stateful, which would prevent us from using a pure RESTful interface. As you can see, even little decisions can impact how our code can evolve in the future.

Along the same lines, look at the `Update` method. It takes two objects - the original object and the updated object. Coming from a background where I worked on many different data layers, this method signature is the most universal. In some systems, I only passed the modified object, assuming that the primary key is unmodified. However, this approach doesn't work in some systems using natural keys. In dynamic languages, the modified object can be a dictionary just containing the properties that changed. In this system, EF requires us to first retrieve the object to update, modify it and then save the changes. This means I will receive a copy of the original. How you implement `Update` will depend on your environment. You may choose to simply pass the primary key to identify the record in the database. You may choose to just pass the updated values. It really depends on your situation. But remember if you ever move your implementation to a different tier, you shouldn't need to modify every piece of coding doing an update.

### Adapters
This project represents the layer between the views and the domain/data layer. This code would usually just exist in the ASP.NET MVC project because its main responsibility is converting raw HTTP values (query strings, form data, session variables, etc.) into representations that the domain layer can understand. It is also responsible for converting objects from the domain layer into representation that the UI can understand (a View Model). In some cases, this code interacts directly with the data layer. In this project, there is no domain layer and so all interactions are with the data layer directly. Because of the translation from and to UI elements and domain elements, I called this project Adapters.

I have broken this code out into its own project because I found that it was difficult to avoid dependencies whenever I started implementing this code in the MVC project. I managed to create dependencies to the `HttpContext` accidentally more often than I thought I would. If you have two or more MVC projects, say for different devices (desktop, mobile, etc.) you can put the reusable code here. Most projects won't call for a separate project and this code would normally appear as part of the MVC project. Although, I would advise taking steps to reduce the size of you controllers.

The `CustomerData` class is an example of a view model. It uses [Data Annotations](http://weblogs.asp.net/scottgu/archive/2010/01/15/asp-net-mvc-2-model-validation.aspx) so that ASP.NET MVC can automate some of the control rendering and validation. You'll find that I prefer to avoid automatic validation and do it manually using jQuery Validate. This class also uses an attribute called `FieldName` which I will explain later. The important thing to notice about this class is that it is a dumn DTO again. It's even dumber than `Customer` because it replaces the `Guid` with a `string`. When the view receives a view model, it shouldn't need to do any additional processing, except basic formatting, maybe.

In the Binders folder, you'll find different model binders. These are classes that will automate converting query strings, form data, etc. into data objects. My advice is to avoid manually extracting values and use model binding as much as possible. Not only does this cut down on the lines of code, it makes it easier to following ASP.NET MVC error handling conventions.

The `CustomFieldNameModelBinder` allows you to decorate properties with one or more `FieldName` attributes. The default model binder follows very specific naming conventions to map field names to properties. However, your front-end developers may have naming conventions that you must adhere to. `CustomFieldNameModelBinder` will search for the corresponding property based on the `FieldName` attribute. Look at the `TestMvcApplication/App_Start/BindingConfig.cs` file to see how this is configured with ASP.NET MVC.

The `ModelBinderBuilder` class is an even more flexible class. In the `global.asax` file, you can actually build a model binder on-the-fly specifying how to map between fields and properties. This may be a better approach if you don't want to dirty your view models with lots of attributes. This also comes in handy when your front-end team doesn't have a consistent naming scheme (the horror!). Unfortunately, ASP.NET MVC does not provide a way to specify which model binder to use on a action-per-action basis. However, you can easily create a sub-class of `IModelBinder` that simply delegates to a generated model binder.

This project also has a general-purpose exception class `AdapterException`. Similar to `ServiceException`, this should be the only type of exception to leave this layer. Specifically, caught `ServiceException`s should be wrapped with `AdapterException`s. If there was a domain layer, its exceptions would be wrapped as well. This would be a good use for the `ExceptionInterceptor` class in the Policies project. `AdapterException` holds and HTTP status code. This is used in the MVC project to send meaningful status codes to the UI. This is especially useful when working with JSON and RESTful interfaces.

The `CustomerAdapter` class is the result is extracting out code from the MVC controller and putting it into its own isolated class. There's also an interface `ICustomerAdapter` which is needed to apply policies using Ninject. Whether I'd normally create this interface depends on my unit testing strategy.

The `CustomerAdapter` has the `CustomerRepository` as a dependency. Ninject will pass the repository to the adapter when instantiating it. Most of the methods in the adapter are just pass-throughs to the repository. What you might not realize is that policies are used to add additional code to these methods at runtime. This also explains why these methods are marked `virtual`. Investigate the `TestMvcApplication/NinjectWebCommon.cs` file to see how these policies are configured.

The `CustomerAdapter` class also has two methods for converting between the data objects and the view models. These types of conversions are extremely common. The [AutoMapper](https://github.com/AutoMapper/AutoMapper) project is aimed at automating some of these mappings, but I usually just do it manually. Code for mapping between representations usually finds its way into mapper classes. `CustomerAdapter` also has methods for converting between representations of GUIDs and date/times. In large projects, code for parsing primitives usually finds its way into a utility class.

One thing to keep in mind is that some view models are extremely complex. While they should be relatively straight-forward for the UI to render, they may still consist of many levels. Here things can get a little fuzzy because a considerable amount of business logic can be involved when building view models. Whose responsibility is it build the view models? Here, I would say that the adapter is still responsible for building the view model. Its job then is to probe the business layer to get the information it needs to build it. This level of cooperation can be unnerving to some people, but it is far more testable and keeps you domain logic independent of your UI.

Another option to building large view models is to use ASP.NET MVC's `Html.Action` and `Html.RenderAction` to build complex screens out of smaller parts. This is a great solution for decomposing large screens into little, reusable chunks. The only problem is that you can end up grabbing the same data multiple times in some cases.

### TODO - Finishing highlights.
