# TestMvcApplication

A repository for ASP.NET MVC patterns and best practices.

## Purpose
Quite some time ago I started writing a book. It was about how to write enterprise applications using ASP.NET MVC and back-end services. However, it didn't focus on teaching the technologies involved. Instead it focused on how to properly configure a solution, how to minimize dependencies and how to build a system that is both configurable and testable. Basically, it covered topics that would have saved me years of trial and error.

I ended up realizing that my book was about how to architect a system and, honestly, that topic is just too big for one book. I also realized that I wasn't as advanced in some topics as I needed to be, such as continuous integration. The most I could say is "you should be doing this". Otherwise, I would have needed to explain how to configure TeamCity or Jenkins step-by-step on a separate server.

My book was centered around developing ASP.NET MVC project, gradually introducing different problems and how to solve them. I am publishing the code that was the result. Even if you think you're an expert MVC developer, I can almost guarantee there is some code contained here that will excite you. Other code can be carried over to any project, MVC or not. Some of this code will seem fairly boilerplate. Other code could very well change your whole approach to software development.

## Points of Interest
To save you time from pouring over hundreds of lines of code, I will try to direct your attention to features that will be helpful to you.

### Service Interfaces
This project contains the interfaces to the data/service layer. Here the interfaces for [Repositories](http://martinfowler.com/eaaCatalog/repository.html) are found. You will also find the [DTO](http://martinfowler.com/eaaCatalog/dataTransferObject.html)s that are passed to/returned by the repositories. This project is also where the interfaces for connecting to remote services, the file system, an SMTP server, etc. would go.

`ServiceException` is used to indicate that an error occurred at the service layer. No other type of exception should be thrown by this layer. Different services can create sub-classes of this exception. This way consumers of this service can assume non-`ServiceException`s are fatal errors and not just the result of a database being down or service being unavailable.

The DTOs are just plain objects. They are intentionally simple. `Customer` represents the values that are expected to be returned from the database. It is comprised on public accessor/mutators and doesn't attempt to hide anything.

The `ICustomerRepository` has methods for interacting with the database. Since this is just an interface, nothing should be exposed regarding whether we're interacting with a database, a service, etc.

There is also a `ISynchronizer` class. It is a strange class because it is an artifact of working with [Entity Framework](http://msdn.microsoft.com/en-us/data/ef.aspx). This class forces Entity Framework to commit any local changes to the database. One option would be to perform synchronization in the `ICustomerRepository` class. However, using `ISynchronizer` allows us to mix calls to multiple repositories without hitting the database every time. At a high level, the `ISynchronizer` class actually breaks the abstraction, making other layers of the system aware that things aren't immediately synchronized to the end-point.

### Policies
This project is extremely important for making an application configurable. This project, along with Ninject, makes it possible to do [aspect-oriented programming (AOP)](http://en.wikipedia.org/wiki/Aspect-oriented_programming). This allows us to program declaratively to do things like database transactions, exception handling, logging and a whole lot more.

`AttributeInterceptor` is an abstract class. It provides a single method for getting custom attributes from a method. Most policies are declared using attributes. For instance, we might indicate that a method should run within a database transaction by doing something like this:

    [Transaction]
    public void CreateAccount(AccountDetail details)
    {
        // do some work
    }

We can configure the system look for methods with this attribute and decorate them so additional code is run before and/or after the method.

A common operation in a layered architecture is to simply capture exceptions from a lower layer and wrap them with a new exception. This is what `ExceptionInterceptor` does. The idea is that the cause of the exception can't be handled yet but we don't want to burden higher layers with handling low-level exception types. Allowing certain types of exceptions to bubble up could actually break our abstraction. `ExceptionInterceptor` will inspect the decorated method for an optional `ErrorMessage` attribute and use it as the new exception's message.

The `SynchronizeInterceptor` and the `TransactionInterceptor` classes have a lot in common. `SynchronizeInterceptor` will tell Entity Framework to synchronize any local changes with the database. `TransactionInterceptor` will start a transaction before executing the method and will commit if it finishes without throwing an exception. It becomes immediately obvious that `ISynchronizer` impacts the design of the code at multiple layers.

### Data Modeling
This project contains an implementation for the `ICustomerRepository` and `ISynchronizer` interfaces.
