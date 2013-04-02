# TestMvcApplication

A repository for ASP.NET MVC patterns and best practices.

## Purpose
Quite some time ago I started writing a book. It was about how to write enterprise applications using ASP.NET MVC and back-end services. However, it didn't focus on teaching the technologies involved. Instead it focused on how to properly configure a solution, how to minimize dependencies and how to build a system that is both configurable and testable. Basically, it covered topics that would have saved me years of trial and error when I was new to .NET development.

I eventually realized that my book was about how to architect a system and, honestly, that topic is just too big for one book. I also realized that I wasn't qualified to write about some topics, such as continuous integration. The most I could say is "you should be doing this". In the case of continuous integration I would have needed to explain how to configure [TeamCity](http://www.jetbrains.com/teamcity/) or [Jenkins](http://jenkins-ci.org/) step-by-step on a separate server. I simply lacked a high-enough perspective on these topics as a whole.

My book was centered around developing an ASP.NET MVC project, gradually introducing different problems and ways to solve them. I am publishing the code that resulted from months of research. Even if you think you're an expert MVC developer, I can almost guarantee there is some code contained here that will interest you. A lot of best practices can be carried over to any project, MVC or not. Some of this code will seem fairly boilerplate. Other code could very well change your whole approach to software development.

## Points of Interest
To save you time from pouring over hundreds of lines of code, I will try to direct your attention to features that will be useful for you.

### Service Interfaces
This project contains the interfaces to the data/service layer. Here the interfaces for [Repositories](http://martinfowler.com/eaaCatalog/repository.html) are found. You will also find the [DTO](http://martinfowler.com/eaaCatalog/dataTransferObject.html)s that are passed to/returned by the repositories. This project is also where the interfaces for connecting to remote services, the file system, an SMTP server, etc. would go.

`ServiceException` is used to indicate that an error occurred at the service layer. No other type of exception should be thrown by this layer. Different services can create sub-classes of this exception. This way consumers of this service can assume non-`ServiceException`s are fatal errors and not just the result of a database being down or service being unavailable. This exception handling policy is related to [checked vs unchecked exceptions](http://stackoverflow.com/questions/6115896/java-checked-vs-unchecked-exception-explanation). Proper exception handling is one of those topics that most developers ignore because it is such a complex subject.

The DTOs are just plain objects. They are intentionally simple. `Customer` represents the values that are expected to be returned from the database. It is comprised of public accessor/mutators and doesn't attempt to hide anything. Sometimes it may be useful to implement `ICloneable` to support copying DTOs.

The `ICustomerRepository` has methods for interacting with the database. Since this is just an interface, nothing should be exposed regarding whether we're interacting with a database, a service, etc. In this solution, I am creating a repository for each database table. In a different situation, I might create a repository for a particular feature.

### Policies
This project is extremely important for making an application configurable. This project, along with Ninject, makes it possible to do [aspect-oriented programming (AOP)](http://en.wikipedia.org/wiki/Aspect-oriented_programming). This allows us to program declaratively to do things like database transactions, exception handling, logging and a whole lot more.

`AttributeInterceptor` is an abstract class. It provides a single method for getting custom attributes from a method. Most policies are declared using attributes. For instance, we might indicate that a method should run within a database transaction by doing something like this:

    [Transaction]
    public void CreateAccount(AccountDetail details)
    {
        // do some work
    }

We can configure the system to look for methods with this attribute and decorate them so additional code is run before and/or after the method. In this case, we would create an instance of `TransactionScope` to create a database transaction.

`TransactionInterceptor` will start a transaction before executing the method and will commit it if it finishes without throwing an exception. The `Transaction` attribute can be created with additional arguments for configuring the transaction.

A common operation in a layered architecture is to simply capture exceptions from a lower layer and wrap them with a new exception. This is what `ExceptionInterceptor` does. The idea is that the cause of the exception can't be handled yet but we don't want to burden higher layers with handling low-level exception types. Allowing certain types of exceptions to bubble up could actually break our abstraction. `ExceptionInterceptor` will inspect the decorated method for an optional `ErrorMessage` attribute and use it as the new exception's message.

Other common operations, such as logging can be performed in a declarative way. Just defining some arbitrary classes won't accomplish anything. Take a look at the `TestMvcApplication/App_Start/NinjectWebCommon.cs` class - it will show how these interceptors are hooked up.

### Data Modeling
This project contains an implementation for the `ICustomerRepository` interface. This solution uses Entity Framework with SQL Server to store customer data. When building large systems, it is usually important to hide which underlying data access tools you are using. The Repository pattern achieves this for us nicely. If we decided to move our data layer to a separate service (maybe for load balancing or to be closer to the database), the repository interface will shield most of the code from the change.

In order to implement the `ICustomerRepository` interface, `CustomerRepository` accepts an instance of `EntitySet`. `EntitySet` is a thin wrapper around a `DbContext` class, a member of the Entity Framework. This class is configured to allow us to use raw DTOs or [POCO](http://msdn.microsoft.com/en-us/library/vstudio/dd456853.aspx)s. Normally, Entity Framework will auto-generate our data objects for us based on the entity model; however, these classes have a lot of additional logic for managing their state internally which in larger systems can lead to EF concerns bleeding into other layers.

The `EntitySet` class provides access to a collection (`IQueryable`) of customers. It also provides raw access to the underlying database connection (a `SqlConnection` in this case) for occasions when you need to circumvent EF. `EntitySet` implements `IDisposable`, meaning that it manages the lifetime of the connection to SQL Server. It provides a `SaveChanges` method for persisting any local changes to the database. This class is also where any convenience methods for the data layer would be found. Take a look at the `GetEntities` method for an example - a method that makes it possible to map hand-written SQL results to entities in our model.

Although it is not used in this solution, the `ConnectionManager` is a useful class. It will open a closed connection and automatically close it when it is disposed. If the connection is already open, it simply does nothing. This is useful when working with EF because EF opens and closes connections between operations (but thanks to connection pooling this has no overhead). This class makes sure the connection is always open for hand-written SQL.

I make it really easy to escape the confines of Entity Framework. It is tempting to do everything in terms of EF but that can lead to problems. EF provides its own list of convenience methods for calling stored procedures and even its own dialect of SQL for fine-grain control. However, your system may need to interact with an existing data layer expecting a database connection. Or you may have team members who prefer working directly with ADO.NET. In my experience, almost all of my code could be written using EF and only one out of a hundred SQL needed me to work around it.

Designing a repository can be tricky. Like most classes, creating the wrong interface can leak assumptions about how the repository is implemented. If you inspect the `Add`, `Update` and `Remove` methods, you'll notice that I call `SaveChanges`. Another option would have been to move the responsibility of calling `SaveChanges` into a separate class (call it `Synchronizer`). This way you could make lots of different changes and then force EF to synchronize with the database all at one time. However, that approach has a weakness - if we move away from EF to, say, a web service, clients will assume nothing will happen until we call `SaveChanges` on the `Synchronizer`. This means our web service would have to be stateful, which would prevent us from using a pure RESTful interface. As you can see, even little decisions can impact how our code can evolve in the future.

Along the same lines, look at the `Update` method. It takes two objects - the original object and the updated object. Coming from a background where I worked on many different data layers, this method signature is the most universal. In some systems, I only passed the modified object, assuming that the primary key is unmodified. However, this approach doesn't work in some systems using natural keys. In dynamic languages, the modified object can be a dictionary just containing the properties that changed. In this system, EF requires us to first retrieve the object to update, modify it and then save the changes. This means I will receive a copy of the original. How you implement `Update` will depend on your environment. You may choose to simply pass the primary key to identify the record in the database. You may choose to just pass the updated values. It really depends on your situation. But remember if you ever move your implementation to a different tier, you shouldn't need to modify every piece of code doing an update.

### MvcUtilities
This project contains classes that are used by the ASP.NET MVC project. Most of these classes are convenience classes, to save you from repeating the same code in your controllers. Typically, these classes would be in the ASP.NET MVC project, instead of separated out like this. However, keeping them in a separate project will make it easy to reuse these utility classes in another solution. It also seems to help cut down on unanticipated dependencies.

In the `MvcUtilities/Binders` folder, you'll find different model binders. These are classes that will automate converting query strings, form data, etc. into data objects. My advice is to avoid manually extracting values and use model binding as much as possible. Not only does this cut down on the lines of code, it makes it easier to follow ASP.NET MVC error handling conventions.

The `CustomFieldNameModelBinder` class allows you to decorate properties with one or more `FieldName` attributes. The default model binder follows very specific naming conventions to map field names to properties. However, your front-end developers may have naming conventions that you must adhere to. `CustomFieldNameModelBinder` will search for the corresponding property based on the `FieldName` attribute. Look at the `TestMvcApplication/App_Start/BindingConfig.cs` file to see how this is configured with ASP.NET MVC.

The `ModelBinderBuilder` class is an even more flexible class. In the `TestMvcApplication/App_Start/BindingConfig.cs` file, you can actually build a model binder on-the-fly specifying how to map between fields and properties. This may be a better approach if you don't want to dirty your view models with lots of attributes. This also comes in handy when your front-end team doesn't have a consistent naming convention (the horror!). Unfortunately, ASP.NET MVC does not provide a way to specify which model binder to use on an action-per-action basis. However, you can easily create a sub-class of `IModelBinder` that simply delegates to a model binder built using `ModelBinderBuilder`.

In the `MvcUtilities/ActionResults` folder, the `JsonWithHttpStatusCodeResult` demonstrates extending the `JsonResult` class to send a different HTTP status code. Writing this class is ridiculously easy, but people just repeat the same code throughout their code anyway.

This project also has a general-purpose exception class `CodedException`. `CodedException` holds an HTTP status code. This is used in the MVC project to send meaningful status codes to the UI. This is especially useful when working with JSON and RESTful interfaces. The exceptions that are caught by the UI should inherit from `CodedException`.

In the `MvcUtilities/FilterAttributes` folder you'll find some simple filters. `HttpStatusCodeErrorHandlerAttribute` will respond to exceptions by returning an HTTP status code. This saves you from needing to put `try/catch` blocks in every controller method, only to return an `HttpStatusCodeResult`. The `RedirectOnErrorAttribute` is similar in that it will catch exceptions and send the user to an error screen. If you were writing a client-heavy front-end with JSON end-points, you'll probably use `HttpStatusCodeErrorHandlerAttribute`. If you're following a more classic approach, `RedirectOnErrorAttribute` will do the trick. Both of these attributes will capture instances of `CodedException` and extract the HTTP status code and error message to return.

### Adapters
This project represents the layer between the views and the domain/data layer. This code would usually just exist in the ASP.NET MVC project because its main responsibility is converting raw HTTP values (query strings, form data, session variables, etc.) into representations that the domain layer can understand. It is also responsible for converting objects from the domain layer into a representation that the UI can understand (a View Model). In some cases, this code interacts directly with the data layer. In this project, there is no domain layer and so all interactions are with the data layer directly. Because of the translation from and to UI elements and domain elements, I called this project Adapters.

I have broken this code out into its own project because I found that it was difficult to avoid creating interdependencies whenever I started implementing this code in the MVC project. I managed to create dependencies to the `HttpContext` accidentally more often than I thought I would. If you have two or more MVC projects, say for different devices (desktop, mobile, etc.) you can put the reusable code here. Most projects won't call for a separate project and this code would normally appear as part of the MVC project. Although, I would advise taking steps to reduce the size of you controllers.

The `CustomerData` class is an example of a view model. It uses [Data Annotations](http://weblogs.asp.net/scottgu/archive/2010/01/15/asp-net-mvc-2-model-validation.aspx) so that ASP.NET MVC can automate some of the control rendering and validation. You'll find that I prefer to avoid automatic client-side validation and do it manually using jQuery Validate. However, I prefer automatic server-side validation for certain types of validation. This class also uses the `FieldName` attribute. The important thing to notice about this class is that it is a dumb DTO again. It's even dumber than `Customer` because it replaces the `Guid` with a `string`. When the view receives a view model, it shouldn't need to do any additional processing, except some basic formatting maybe.

A common mistake I see other developers making is reusing data layer DTOs as view models. In simple applications, there is a one-to-one mapping between DTOs and view models. Because of that, most developers think it is wasteful to duplicate the DTO. This is perfectly fine in some cases, except it can lead to problems further along in the project. Eventually a screen will require some additional information or it will compute a value from the DTO's properties. Instead of creating a raw view model, some developers play tricks like creating view models containing DTOs and/or will perform computations in the view. I recommend keeping DTOs out of your view models and avoiding deep nesting or computation of any kind. Look at Fowler's treatment on [Presentaion Model](http://martinfowler.com/eaaDev/PresentationModel.html)s to get a basic overview of this topic.

The `CustomerAdapter` class is the result of extracting out code from the MVC controller and putting it into its own isolated class. There's also an interface `ICustomerAdapter` which is needed to apply policies using Ninject. Whether I'd normally create this interface depends on my unit testing strategy.

The `CustomerAdapter` class is dependent on `CustomerRepository`. Ninject will pass the repository to the adapter when instantiating it. Most of the methods in the adapter are just pass-throughs to the repository. What you might not realize is that policies are used to add additional code to these methods at runtime. Investigate the `TestMvcApplication/App_Start/NinjectWebCommon.cs` file to see how these policies are configured.

The `CustomerMapper` class has two methods for converting between the data objects and the view models. These types of conversions are extremely common. The [AutoMapper](https://github.com/AutoMapper/AutoMapper) project is aimed at automating some of these mappings, but I usually just do it manually. Code for mapping between representations usually finds its way into mapper classes. `PrimitiveMapper` has methods for converting between representations of GUIDs and date/times. In large projects, the primitive mapper can become quite large and the names of the conversion methods can be obscure. However, creating a primitive mapper is a great way to ensure consistency in the way primitives are formatted/parsed.

The `CustomerAdapter` class is dependent on the `ICustomerMapper` interface. The constructor actually creates the mapper using `new` in the constructor. Some developers would be tempted to pass the mapper to the constructor. However, any unit tests would then need to mock out the interface and most of the time the mock object would just perform a mapping - essentially repeating the production logic. This can be useful if you want to verify that the mapper was indeed called, but most of the time this just seems a bit ridiculous. For this reason, I provide a property for setting the mapper instance, but I create a `CustomerMapper` by default.

One thing to keep in mind is that some view models are extremely complex. While they should be relatively straight-forward for the UI to render, they may still consist of many levels of nested objects. Here things can get a little fuzzy because a considerable amount of business logic can be involved when building view models. Whose responsibility is it to build the view models? Here, I would say that the Adapters classes are responsible for building the view models. Its job then is to probe the data/business layer to get the information it needs to build it. This level of cooperation can be unnerving to some people, but it is far more testable and keeps you domain logic independent of your UI. It makes sense to mock out complex mappers that have a lot of their own dependencies and business logic.

Another option to building large view models is to use ASP.NET MVC's `Html.Action` and `Html.RenderAction` to build complex screens out of smaller parts. This is a great solution for decomposing large screens into little, reusable chunks. The only problem is that you can end up grabbing the same data multiple times in some cases.

### TestMvcApplication
The actual MVC project is a treasure trove of useful utilities. I wanted to provide an example of how to build an ASP.NET MVC application following the "classic" model and another following a more client-driven approach. The classic model is what you will read about in books and on the Internet about ASP.NET MVC. The more client-driven approach involves using JavaScript and RESTful services to avoid redirecting the user between screens.

The term classic isn't meant to be derogatory. Most web-based business applications can be implemented using the classic approach. It's for web applications that don't need the "wow" effect and consist mostly of form entry. The classic approach is also easier because the tools are built with it in mind. To be clear, ASP.NET MVC also provides tools for doing some fancy client-side processing, but it isn't quite as flexible as grappling with raw JavaScript. The classic code can be executed by navigating to the `Classic` controller/URL (which is the home page).

The more client-driven approach relies heavily on JavaScript libraries to perform actions on the client's behalf without redirecting them to a dozen different screens. Instead, it uses [Knockout.js](http://knockoutjs.com/) to perform data binding and [Twitter Bootstrap](http://twitter.github.com/bootstrap/) to control the layout and display modals. Interactions with the server are coordinated using [jQuery's AJAX](http://api.jquery.com/jQuery.ajax/) capabilities. I was liberal with my use of JavaScript in this code because I wanted to demonstrate that realistic tasks can be performed with JavaScript without needing to buy third-party control libraries. The Knockout.js code can be executed by navigating to the `Knockout` controller/URL.

The reality is that most projects will combine these techniques. For instance, it might make sense to provide client-heavy UIs for related actions (for instance, CRUD oeprations) and have separate screens for each database table. Tools like CSS3 and [jQuery transition effects](http://jqueryui.com/effect/) can aid in replacing jarring screen loads with smooth transitions. Because both approaches have their place in modern web applications you should spend time looking at both the classic and the Knockout.js versions of this application.

#### Controllers Should Be Small
Regardless of the examples from your favorite ASP.NET MVC book, your controller methods should be [quite small](http://www.viddler.com/v/b568679c). Most examples I've found do the following in their controllers: perform validation, query repositories, manipulate the session, build view models, do exception handling and much, much more. If your controller methods get much larger than 10 lines of code, you might consider changing your approach. Your controller also shouldn't be littered with a dozen little, private helper methods either. This suggests you are performing business logic in your controllers, which is not where it belongs.

Earlier I mentioned that creating a separate Adapters-like project may be overkill. However, that doesn't necessarily mean you should shove all that code into your controller. The more time you spend inside your controller, the more likely you'll build brittle software. There's nothing wrong with creating a new folder in your MVC project to hold additional code. If you do decide to combine your code with a controller, MVC's [Action Filters](http://msdn.microsoft.com/en-us/library/dd410209.aspx) and custom model binders can keep your controllers fit. I'd still recommend moving view model building off into its own code somewhere.

#### Know Your ActionResults
MVC uses the really generic `ActionResult` class to represent the results of a request. There are a lot of built-in `ActionResult`s. I see a lot of developers essentially rewriting existing `ActionResult` code in their controllers. For instance `HttpStatusCodeResult` is a simple `ActionResult` for returning an HTTP status code. However, I see people setting `Response.StatusCode` all the time (which makes the controller hard to test). I also see actions returning `string` whenever JSON is being returned. The `JsonResult` is much better equipped to do this.

With MVC 4, I haven't needed to write many `ActionResult`s. Almost everything I could ever want is already there.

#### Action Filters Should Make You Happy
If you think writing Policy classes using Ninject is just too bizarre, you'll be glad to know there is something built-in to ASP.NET MVC for achieving pretty much the same thing... but it just works on controllers. ASP.NET MVC was built with enough insight to support AOP out of the gate, via action filters. The cool thing is you can get away with sub-classing an existing filter, so you don't even need to write that much code.

There are a ton of existing filters. You should definitely know about: the `Authorize` filter; the `HttpPost`, `HttpPut`, `HttpDelete`, etc. filters; the `HandlerError` filter and the `OutputCache` filter.

The usefulness of action filter attributes is only limited to your imagination. Here is a list of tasks I implemented using filter attributes:

 * I validated Captchas.
 * I redirected the user to a change password screen when their passwords expired.
 * I required some screens to be HTTPS-only unless working locally.
 * I authorized users against system-managed permissions.
 * I made it possible for user-support to access customer information.

Basically, any time more than one action involved the same code, I moved it into a filter attribute. You can apply as many filter attributes to one action as you want, so go crazy.

#### Bundling
If you have high load, you will want to minify your JavaScript and CSS. In the past, switching between development and release versions of these files was a real pain. With MVC 4, bundling is built-in to make this task simple. There is a decent [introduction to bundling](http://www.asp.net/mvc/tutorials/mvc-4/bundling-and-minification) on Microsoft's website. Take a look at the `TestMvcApplication/App_start/BundleConfig.cs` file, the `TestMvcApplication/global.asax` file and the `TestMvcApplication/Views/Shared/_Layout.cshtml` file to see this in action.

#### Gluing Everything Together with Ninject
There is a considerable amount of magic involved in how Ninject configures this system. Manually connecting all the different pieces by hand would be a nightmare. Fortunately, installing Ninject's MVC extension via NuGet automates most of this process. Don't worry if looking at the `TestMvcApplication/App_Start/NinjectWebCommon.cs` file hurts your brain - it hurts mine too! The only important thing in this file is the `TestModule` class. In this class we specify how to build our dependencies.

First we specify that we want to create a `ContextManager`. The `ContextManager` provides access to our `HttpContext`, which contains request, response, user, profile, session and cache data. It also provides access to any configuration settings. In this example, I also provide convenience methods for building URLs.

Typically, I would break out `ContextManager`'s responsibilities by creating separate interfaces that it can then be implemented - a case of multiple interface inheritance. In this example, the only code Ninject needs to create a `ContextManager` is the line: `Bind<ContextManager>().ToSelf().InRequestScope();`. This line says call the default constructor whenever a `ContextManager` is first requested and reuse it for the rest of the request. If we wanted to break it out into interfaces, we'd have to tell Ninject to reuse the original `ContextManager` instance: `Bind<IConfigurationManager>().To<ContextManager>();`. Ninject is pretty smart and will reuse the same instance.

The `EntitySet` class is also scoped to a single request. This is important for making sure database connections are not left open. The `EntitySet` creation is interesting because we need to pass the connection string configuration setting, which comes from the `ContextManager` we just configured.

The `CustomerRepository` binding applies a policy for converting raw `Exception`s to `ServiceException`s. We then apply two policies to the `CustomerAdapter`. Ninject allows us to control the order than policies are executed, which is demonstated here. The order that policies take place can be really important. For the same reason, MVC's action filters support a similar feature.

#### Using Partial Views
When I first started building MVC applications, I repeated myself a lot. It took me a while to start using partial views. Once I did, I became a lot more productive *and* I suddenly found it a lot easier to switch between the classic approach and the rich-client approach.

Here's one example. Most applications have screens that allow you to create and update a record. Most of the time, these screens are almost identical. Typically, the only differences are the form's action URL, what fields are populated and maybe there's a hidden field for the primary key. Duplicating the HTML, JavaScript and the CSS for these pages would be crazy. Take a look at the `TestMvcApplication/Views/Classic/_CustomerDetails.cshtml` file and the corresponding `Create.cshtml` and `Edit.cshtml` files. You'll see I created my own extension method for optionally setting the value attributes of the controls.

As I mentioned earlier, take a hard look at the `Html.Partial`, `Html.RenderPartial`, `Html.Action` and the `Html.RenderAction` extension methods. They're extremely useful. They give you the flexibility of WebForm's User Controls without all the complexity.

#### HTML Helpers Aren't Always Your Friend
Some people might call me crazy, but I tend to limit my use of the `Html` extension methods. Some good examples of `Html` extensions methods are `BeginForm`, `EditorFor` or any of the other dozen `*For` methods. Personally, my experience is that these are big time savers in the beginning and slowly become a maintenance issue over time.

Some of the benefits include:

 * consistent HTML generation
 * consistent <input> naming convention
 * usually less typing
 * the use of DataAnnotation attributes to determine which tag to use
 * some limited type safety
 * automatic handling of unchecked check boxes and disabled controls
 * less need to know HTML

Some of the negatives are:

 * the generated HTML may violate existing coding conventions
 * the automatically generated HTML may cause unanticipated layout issues
 * it is harder for non-developer types to read and maintain
 * anonymous types for route data and CSS styles become eye sores
 * it can make it harder to switch to a more client-driven application

You'll find that I still rely on the `Url` extensions and many of the `Html` extensions. This save me from hard-coding URLs in a lot of places. I just don't like entire tags to be created for me. Embrace the tags.

#### Eliminate Hard-Coding with T4MVC
I am using a project called [T4MVC](http://t4mvc.codeplex.com/). At the start of a project, you will find yourself wanting to rename/move controllers and actions. But methods like `Url.Action` take strings. It is extremely difficult to go through all your views and controllers and update these strings without missing one or two. T4MVC will actually generate constants and extension methods excepting `ActionResult`s. When you change the names of your controllers and actions, you simply rerun the code generation tool and you'll receive compiler errors everywhere the old name is listed. It's not an ideal refactoring but it is much more safe.

Just a fair warning, I ran into issues using the T4MVC project. Specifically, I created another route and somehow this broke the T4MVC code. The trick was to move all other custom routes after the default route.

## Please Contribute
I'd like to continue improving this project. If you have any awesome code snippets, contribute!
