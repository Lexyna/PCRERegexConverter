# PCRERegexConverter

To compile the project, run the following from within the projects root folder:
```
 dotnet build
 ```

To run the Project, navigate to the folder containing the RegExConverter.dll and run with `RegExConverter.dll -r "[regex]"`

 where ```[regex]``` is your *regular* PCRE string.

Additionally, the following options can be enabled:

- `-v`, `--verbose`: Verbose output for Lexer and SimplifyParser
- `--show_nfa`: Shows the created nfa
- `--show_afa`: Shows the created afa
- `--show_verbose_nfa`: Shows the verbose nfa after afa transformation
- `--parse_words`: List of words which will be tested against the created nfa  


 Example input:

`RegExConverter.dll -r "[abc](a|de?)|(f{3,5}g+)|r*+"`

 Example output:
 ```
Regex: [abc](a|de?)|(f{3,5}g+)|r*+
Simplified Regex: ((a|b|c)(a|(d(e|()))))|(fff(f|())(f|())gg*)|(r*(?!r))
 ```

 Example Input 2:

 `RegExConverter.dll -r "ab*c" --parse_words "ac" "abc" "abbc"`

 Example Output 2:

 ```
 Regex: ab*c
Simplified Regex: (ab*c)
---------------------------------- Running words ----------------------------------

Regex Accepts "ac": True

Regex Accepts "abc": True

Regex Accepts "abbc": True
 ```