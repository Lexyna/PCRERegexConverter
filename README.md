# PCRERegexConverter

To compile the project, run the following from within the projects root folder:
```
 dotnet build
 ```

 To run the Project, enter the following command at the root directory:
 ```
 dotnet run "[regex]"
 ``` 

 where ```[regex]``` is your *regular* PCRE string.

 Example input:
 ```
 Entry.exe "[abc](a|de?)|(f{3,5}g+)|r*+"
 ```
 Example output:
 ```
 Simplified regex:
((a|b|c)(a|(d(e|))))|(fff(f|)(f|)gg*)|(r*(?!r))
 ```