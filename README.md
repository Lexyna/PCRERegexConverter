# PCRERegexConverter

To compile the project, run the following from within the projects root folder:
```
 csc src\* src\Automaton\* src\Token\* 
 ```

 To run the Project, enter the following command at the root of the 'Entry.exe' file:
 ```
 Entry.exe "[regex]"
 ``` 

 where ```[regex]``` is your *regular* PCRE string.

 Example input:
 ```
 Entry.exe "[abc](a|de?)|(f{3,5}g+)|r*+"
 ```