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


### TODO - Finishing highlights.
