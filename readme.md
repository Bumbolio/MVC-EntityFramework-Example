## Getting Started
1. Create a new directory (folder) named `StudentInformation`.
2. Open a console and change the directory to the newly created `StudentInformation` folder.
3. Create a new mvc application by running the following command: `dotnet new mvc`
4. Install EntityFramework package with the following command:  `dotnet add package Microsoft.EntityFrameworkCore`
5. Install the EntityFramework Design package with the following command: `dotnet add package Microsoft.EntityFrameworkCore.Design`
6. Install the EntityFramework SqlServer provider package with the follwing command `dotnet add package Microsoft.EntityFrameworkCore.SqlServer` 
5. Install the Entity Framework dotnet CLI tools with the following command: `dotnet tool install --global dotnet-ef`

## Creating Models
Now lets create a few classes for Student, Course, and Enrollment that will serve as our data models. 

### Student Model
1. Go to the Models directory (folder) and create a `Student.cs` file.
2. Add the following code to `Student.cs`: 
```
using System;
using System.Collections.Generic;

namespace StudentInformation.Models
{
    public class Student
    {
        public Guid ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
```
All models should have a property named `ID`.  Our Entity Framework will automatically use the `ID` property as the primary key (unique identifier) for each entity. In this example we are setting the ID type to `Guid` to ensure that every `ID` is unique.  The `Enrollments` property is a type of `ICollection<Enrollment>`, though the `Enrollment` class doesn't exist yet.  We will create it later.  This `Enrollments` property is referred to as a **Navigation Property**.  Navigation Properties inform Entity Framework how multiple models are related to each other.  Because `Enrollments` is a `ICollection<Enrollment>`, we are telling Entity Framework that the `Student` entity has many `Enrollments`.

### Enrollment Model
1. Go to the Models directory (folder) and create an `Enrollment.cs` file.
2. Add the following code to `Enrollment.cs`
```
using System;

namespace StudentInformation.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
        public Guid ID { get; set; }
        public Guid CourseID { get; set; }
        public Guid StudentID { get; set; }
        public Grade? Grade { get; set; }

        public Course Course { get; set; }
        public Student Student { get; set; }
    }
}
```

An `Enrollment` entity maintains the relationship between the `Student` and `Course` entities.  `Student` and `Course` are considered **Navigation Properties** of the `Enrollment` class.  `StudentID` and `CourseID` are the foreign keys (unique identifiers) for the `Student` and `Course` entities.  **Entity Framework** will automatically map the **Navigation Properties** `Student` and `Course` to the foreign key properties `StudentID` and `CourseID` by convention. Please note that the `Course` model hasn't been created yet, we'll do that one next!

### Course Model
1. Go to the Models directory (folder) and create an `Course.cs` file.
2. Add the following code to `Course.cs`:
```
using System;
using System.Collections.Generic;

namespace StudentInformation.Models
{
    public class Course
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
```
Similar to the `Student` model, the `Course` model has a **Nagivation Property** `Enrollments` since each course can have many enrollments.  Our initial data models are now complete.  

## Creating and Registering the Database Context
We will be using **Entity Framework** as our ORM (Object Relational Mapping) software.  EF will help us generate a database based on the models we described earlier.  

### Create the StudentInformation Database Context
1. Create a new direcrtory (folder) in your project named `Data`.  This folder should be at the same level as your `Controllers` or `Models` folders.  
2. Create a new file in the `Data` folder named `StudentInformationContext.cs`
3. Add the following code to your `StudentInformationContext.cs` file:
```
using StudentInformation.Models;
using Microsoft.EntityFrameworkCore;

namespace StudentInformation.Data
{
    //Creates a new database context named StudentInformationContext
    public class StudentInformationContext : DbContext
    {
        public StudentInformationContext(DbContextOptions<StudentInformationContext> options) : base(options)
        {
        }

        //This is where we register our models as entities
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
    }
}
```
The `StudentInformationContext` context is how our application will interact with the database to create, read, update, and delete `Student`, `Course`, and `Enrollment` entities.   

### Register the StudentInformation Database Context
Now that we created a `StudentInformationContext` Database context, we need to tell our application to use the context. We'll do this by registering our new context in the `ConfigureServices` method in the `Startup.cs` file.  This will make our context available to all our `Controllers` via the magic of `Dependency Injection`.  Sounds scary, but for now all you need to know is that our context will be easily accessible throughout our application. 

1. Find the `Startup.cs` file in the root of your application.  
2. Add the following code inside the `ConfigureServices` method in `Startup.cs`:
```
    services.AddDbContext<StudentInformationContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
```
Be sure to also add necessary `using` statements at the top of your `Startup.cs` file:
```
using Microsoft.EntityFrameworkCore;
using StudentInformation.Data;
```

3. Now we need to define what `DefaultConnection` is. We will define our `DefaultConnection` in the `appsettings.json` file.  In this example, we are using SqlLite to connect to a local SQL file that will live inside our project.  Find the `appsettings.json` file, and add the `ConnectionStrings` section.  The resulting file should look something like this:
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentInformation;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```  

## Entity Framework Code First Migrations
Now that we have our context setup, we need to generate our first **Code First Migration**.  A Code First Migration takes the models that we registered in our `StudentInformationContext` context and applies them as a series of changes to be made on our target database.  Let's generate our first migration. 

* In the terminal, enter the following command:  `dotnet ef migrations add InitialCreate`

In the prior command, `IntialCreate` is the name of our migration. Each migration should have a unique name, and should be descriptive enough that we know what will happen if we run the migration.  Every time we make a change to any of the **Models** such as `Student`, `Course`, or `Enrollment` we will need to generate a new migration so we can apply those changes to our database.  The advantage of Code First Migrations is that Entity Framework takes care of creating all the tables and columns in your database, all you need to do is write the models.  Neat!

* Once the initial migration has been generated, you should see a new directory (folder) named `Migrations`.  Inside this folder you will see a file such as `20200308233652_InitialCreate.cs` which contains all the actions that this migration will apply to the database.  The file name will vary depending on when the migration was generated.  

### Applying pending migrations to your database
Once a migration has been generated, you must apply the migration to the database. To do so run the following command in the terminal:
```
dotnet ef database update
```

## Scaffolding Controllers and Views
It's now possible to scaffold (generate) code based on our `Models` and `StudentInformationContext`.  This helps us save time by generating boilerplate code that we can later modify.  Let's generate a controller and a view for the `Student` model.

### Install Code Generation Tools and Packages
1. Install the `dotnet-aspnet-codegenerator` tool via the following command:
```
dotnet tool install --global dotnet-aspnet-codegenerator
```
2. Add the `Microsoft.VisualStudio.Web.CodeGeneration.Design` package to your project via the following command:
```
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
```

### Scaffold the Student Controller and Views
Now that our tools are installed, run the following command to scaffold the `Student` controller and views:
```
dotnet aspnet-codegenerator controller --model Student --dataContext StudentInformationContext --controllerName StudentController --relativeFolderPath Controllers --useDefaultLayout
```
Once the command has ran, a new `StudentController.cs` file will be available in the `Controllers` directory (folder). There is also a set a views for `Student` in the `Views/Student` folder.

## Wrapping things up
Now that we have a controller and views for our `Student` model, let's make sure we can navigate to our `Student` view by adding it to the navigation menu.

1. Find the `_Layout.cshtml` file under the `Views/Shared` directory (folder).
2. In `_Layout.cshtml`, find the `<nav>` element and add another `nav-item` inside the `<ul>`.  The new `nav-item` should look like this:
```
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-controller="Student" asp-action="Index">Students</a>
</li>
```
The `asp-controller` property tells our MVC application which **Controller** this link should link to, and the `asp-action` property tells our applicication which **Action** on the controller this link should link to.  In essence, this generate a link with an href property of `/Student`.  When we navigate to `/Student` in our browser, our application invokes the `Index` action on our `StudentController`.  If you look in the `StudentController.cs` file, you will see that the `Index` action simply uses the database context to list out all the students in our database.  The view that is rendered is chosen automatically by convetion.  In other words, because our controller is named `StudentController`, and our action is named `Index`, our MVC application will look for a view named `Student` and render the corresponding view named `Index.cshtml`.  It is also possible to specify which view to render in the `StudentController`.

Now that we added `Student` as a navigation item, let's start our application and navigate to the `Student` page.  We should now be able to Create, Read, Update, and Delete students. To start the application run the following command in the terminal:
```
dotnet run
```
This command will show you the URL that your application is running on.  

