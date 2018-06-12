# Visual Studio Compatibility Problems
The project was created using Visual Studio 2017. With VS2017 the structure of some project files has changed, making the projects incompatible with VS2015.
To tackle this problem, we added another branch. However, the project may be incompatible with VS2013 and below.

## VS2017
If you are using VS2017, the project should run out of the box. Just clone the project, open the solution and build the project.
```
git clone https://github.com/tospie/eca2ld.git --recursive
```

## VS2015
When using VS2015, after cloning the project, you need to switch the branch and update the submodules before opening the solution and building the project.
```
git clone https://github.com/tospie/eca2ld.git --recursive
cd eca2ld
git checkout vs2015
git submodule update
```

## Testing the Solution
To check if everything worked set the ExampleServer as startup project, then Start the project. If the program throws a HTTPListenerException "Access is denied", simply change the port of the URL in the Program.cs to a port that your user has access to.

If the console is blank, navigate to http://localhost:12345/entities/e (if you replaced the port number, replace it here, too). Your browser should respond with a turtle representation of an Object.
